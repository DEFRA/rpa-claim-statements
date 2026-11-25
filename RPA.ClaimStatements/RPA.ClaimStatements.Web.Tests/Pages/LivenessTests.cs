using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;

namespace RPA.ClaimStatements.Web.Tests.Pages
{
    [TestFixture, NonParallelizable]
    public class LivenessTests
    {
        [TestCase("GET", "Alive")]
        [TestCase("HEAD", "")]
        public async Task Anonymous_Exact_Liveness_Is_Process_Only(string method, string body)
        {
            using var host = new OperatorPageHost();
            host.Roles(null);
            using var response = await host.Client.SendAsync(new HttpRequestMessage(new HttpMethod(method), "/health/live"));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo(body));
            Assert.That(response.Headers.CacheControl.NoStore, Is.True);
            host.Context.VerifyNoOtherCalls();
            host.Plant.VerifyNoOtherCalls();
            host.Station.VerifyNoOtherCalls();
        }

        [TestCase("POST", "/health/live")]
        [TestCase("GET", "/health/live/child")]
        [TestCase("GET", "/HEALTH/live")]
        public async Task Liveness_Exception_Does_Not_Expose_Other_Paths_Or_Methods(string method, string path)
        {
            using var host = new OperatorPageHost();
            host.Roles(null);
            using var response = await host.Client.SendAsync(new HttpRequestMessage(new HttpMethod(method), path));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }
    }
}
