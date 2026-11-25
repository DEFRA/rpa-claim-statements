using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using static RPA.ClaimStatements.Web.Tests.Pages.OperatorPageHost;

namespace RPA.ClaimStatements.Web.Tests.Pages
{
    [TestFixture, NonParallelizable]
    [Category("RazorPagesHttp")]
    public class OperatorPageHttpTests
    {
        private OperatorPageHost host;

        [SetUp]
        public void Setup()
        {
            host = new OperatorPageHost();
            host.Settings.Add(new Configuration { ConfigurationID = Guid.NewGuid(), Setting = "Maximum Batch Size", Value = "10" });
            host.Years.Add(new SchemeYear { SchemeYearID = Guid.NewGuid(), SchemeYearNumber = 2026, Active = true });
        }

        [TearDown]
        public void Cleanup() => host.Dispose();

        [TestCase("/", "Claim Statement Generator Web Portal")]
        [TestCase("/Configuration", "Generation Configuration")]
        [TestCase("/Configuration/SchemeYear", "Generation Active Scheme Years")]
        [TestCase("/Suppression", "Existing Suppressions")]
        [TestCase("/Suppression/_Existing", "No current suppressions.")]
        [TestCase("/Error", "Claim Statement Generation and Validation Errors")]
        [TestCase("/Log", "Claim Statement Generation Log")]
        public async Task Legacy_Get_Routes_Render_Real_Pages_And_Partials(string path, string expected)
        {
            using var response = await host.Client.GetAsync(path);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            string body = await response.Content.ReadAsStringAsync();
            Assert.That(body, Does.Contain(expected));
            Assert.That(body.Contains("<html"), Is.EqualTo(!path.Contains("/_")));
            if (path == "/")
            {
                Assert.That(body, Does.Contain("action=\"/Home/Trigger\""));
                Assert.That(body, Does.Contain("action=\"/Account/Logout\""));
                Assert.That(body, Does.Contain("name=\"StatementTypes\""));
                Assert.That(body, Does.Contain("name=\"maximumBatchSize\""));
            }
        }

        public static IEnumerable<TestCaseData> DeniedOperations()
        {
            foreach (var operation in new[]
            {
                ("GET", "/Home/Index"), ("POST", "/Home/Trigger"), ("GET", "/Home/Sample"),
                ("GET", "/Configuration/Index"), ("POST", "/Configuration/Index"),
                ("GET", "/Configuration/SchemeYear"), ("POST", "/Configuration/SchemeYear"),
                ("GET", "/Suppression/Index"), ("GET", "/Suppression/_Existing"),
                ("GET", "/Suppression/_Create"), ("GET", "/Suppression/_CreateSuppression"),
                ("POST", "/Suppression/_CreateSuppression"), ("GET", "/Suppression/_BulkSuppression"),
                ("POST", "/Suppression/_BulkSuppression"), ("GET", "/Suppression/RemoveSuppression"),
                ("GET", "/Suppression/Download"), ("GET", "/Error/Index"),
                ("GET", "/Error/Download"), ("POST", "/Error/ResolveError"),
                ("GET", "/Log/Index"), ("POST", "/Log/Download"), ("GET", "/Import/Import")
            })
                yield return new TestCaseData(operation.Item1, operation.Item2);
        }

        [TestCaseSource(nameof(DeniedOperations))]
        public async Task Authenticated_Without_Permissions_Is_Forbidden_Before_Business_Execution(string method, string path)
        {
            host.Roles("Unrecognised.Role");
            using var response = await Send(method, path);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            NoBusinessCalls();
        }

        [TestCase("/Home/Trigger", View)]
        [TestCase("/Home/Sample", Trigger)]
        [TestCase("/Configuration/Index", View)]
        [TestCase("/Configuration/SchemeYear", View)]
        [TestCase("/Import/Import", View)]
        [TestCase("/Suppression/_Create", View)]
        [TestCase("/Suppression/_Create", Manage)]
        [TestCase("/Suppression/_CreateSuppression", View)]
        [TestCase("/Suppression/_CreateSuppression", Manage)]
        [TestCase("/Suppression/_BulkSuppression", View)]
        [TestCase("/Suppression/RemoveSuppression", View)]
        [TestCase("/Suppression/Download", View)]
        [TestCase("/Suppression/Download", Download)]
        [TestCase("/Log/Download", View)]
        [TestCase("/Log/Download", Download)]
        [TestCase("/Error/Download", View)]
        [TestCase("/Error/Download", Download)]
        [TestCase("/Error/ResolveError", View)]
        [TestCase("/Error/ResolveError", Exceptions)]
        public async Task Operation_Permissions_Are_Enforced_At_Page_Level_Not_Handler_Attributes(string path, string roles)
        {
            host.Roles(roles);
            using var response = await Send(path.EndsWith("Trigger") || path.EndsWith("ResolveError") ? "POST" : "GET", path);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            NoBusinessCalls();
        }

        [TestCase("/")]
        [TestCase("/Account/Logout")]
        [TestCase("/Scripts/jquery-1.10.2.js")]
        [TestCase("/fonts/glyphicons-halflings-regular.woff")]
        public async Task Anonymous_Requests_Including_Static_Files_Are_Challenged(string path)
        {
            host.Roles(null);
            using var response = await host.Client.GetAsync(path);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            NoBusinessCalls();
        }

        [TestCase("/Home/Trigger")]
        [TestCase("/Account/Logout")]
        [TestCase("/Error/ResolveError")]
        public async Task PostOnly_Routes_Reject_Get_And_Put(string path)
        {
            foreach (string method in new[] { "GET", "PUT" })
            {
                using var response = await Send(method, path);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed), method);
            }
            NoBusinessCalls();
        }

        [TestCase("/Home/Trigger")]
        [TestCase("/Account/Logout")]
        public async Task Trigger_And_Logout_Reject_Missing_And_Invalid_Antiforgery(string path)
        {
            foreach (var form in new[] { new Dictionary<string, string>(),
                new Dictionary<string, string> { ["__RequestVerificationToken"] = "invalid" } })
            {
                using var response = await host.Client.PostAsync(path, new FormUrlEncodedContent(form));
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            }
            NoBusinessCalls();
        }

        [TestCase(10)]
        [TestCase(0)]
        [TestCase(-1)]
        public async Task Trigger_Binds_Real_Form_With_Antiforgery_And_Executes_Synchronously(int maximum)
        {
            var form = await Antiforgery();
            form["StatementTypes"] = "XB";
            form["maximumBatchSize"] = maximum.ToString();
            using var response = await host.Client.PostAsync("/Home/Trigger", new FormUrlEncodedContent(form));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
            host.Plant.Verify(p => p.Activate("XB", maximum), maximum > 0 ? Times.Once() : Times.Never());
            UpdateCookies(response);
            using var page = await host.Client.GetAsync(response.Headers.Location);
            Assert.That(await page.Content.ReadAsStringAsync(), Does.Contain("Generation complete"));
        }

        [Test]
        public async Task Trigger_Defaults_Are_Preserved()
        {
            using var response = await host.Client.PostAsync("/Home/Trigger", new FormUrlEncodedContent(await Antiforgery()));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
            host.Plant.Verify(p => p.Activate("ALL", 10), Times.Once);
        }

        [Test]
        public async Task Logout_Binds_Form_And_Signs_Out_Both_Schemes_With_Local_Redirect()
        {
            var form = await Antiforgery();
            using var response = await host.Client.PostAsync("/Account/Logout", new FormUrlEncodedContent(form));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
            Assert.That(response.Headers.GetValues("X-Test-Signed-Out"), Is.EquivalentTo(new[] { "Cookies", "OpenIdConnect" }));
            Assert.That(response.Headers.Location.ToString(), Does.StartWith("/"));
            Assert.That(response.Headers.Location.ToString(), Does.Not.StartWith("//"));
        }

        [Test]
        public async Task Logout_Requires_Authentication_But_No_Application_Permission()
        {
            var form = await Antiforgery();
            host.Roles("Unrecognised.Role");
            using var response = await host.Client.PostAsync("/Account/Logout", new FormUrlEncodedContent(form));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
        }

        [TestCase(Trigger)]
        [TestCase(View)]
        [TestCase(Manage)]
        [TestCase(Exceptions)]
        [TestCase(Manual)]
        public async Task Home_Preserves_Each_Of_The_Five_Legacy_Entry_Permissions(string role)
        {
            host.Roles(role);
            using var response = await host.Client.GetAsync("/");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Configuration_Forms_Render_Original_Collection_Field_Names()
        {
            using var configuration = await host.Client.GetAsync("/Configuration/Index");
            string body = await configuration.Content.ReadAsStringAsync();
            Assert.That(body, Does.Contain("name=\"[0].Value\""));
            Assert.That(body, Does.Contain("name=\"[0].ConfigurationID\""));
            using var years = await host.Client.GetAsync("/Configuration/SchemeYear");
            body = await years.Content.ReadAsStringAsync();
            Assert.That(body, Does.Contain("name=\"[0].Active\""));
            Assert.That(body, Does.Contain("name=\"[0].SchemeYearID\""));
        }

        [TestCase("")]
        [TestCase("model")]
        public async Task Configuration_Post_Binds_Legacy_Unprefixed_And_Prefixed_Collections(string prefix)
        {
            Guid id = Guid.NewGuid();
            using var response = await host.Client.PostAsync("/Configuration/Index", Form(
                (prefix + "[0].ConfigurationID", id.ToString()), (prefix + "[0].Setting", "Maximum Batch Size"),
                (prefix + "[0].Value", "25")));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
            host.Context.Verify(c => c.SetModified(It.Is<object>(o => o is Configuration &&
                ((Configuration)o).ConfigurationID == id && ((Configuration)o).Setting == "Maximum Batch Size" &&
                ((Configuration)o).Value == "25")), Times.Once);
            host.Context.Verify(c => c.SaveChanges(), Times.Once);
        }

        [TestCase("")]
        [TestCase("model")]
        public async Task SchemeYear_Post_Binds_Legacy_Checkbox_Collection(string prefix)
        {
            Guid id = Guid.NewGuid();
            using var response = await host.Client.PostAsync("/Configuration/SchemeYear", Form(
                (prefix + "[0].SchemeYearID", id.ToString()), (prefix + "[0].SchemeYearNumber", "2026"),
                (prefix + "[0].Active", "true")));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
            host.Context.Verify(c => c.SetModified(It.Is<object>(o => o is SchemeYear &&
                ((SchemeYear)o).SchemeYearID == id && ((SchemeYear)o).SchemeYearNumber == 2026 &&
                ((SchemeYear)o).Active)), Times.Once);
            host.Context.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Test]
        public async Task Single_Suppression_Uses_Unprefixed_Form_Validation_And_PascalCase_Json()
        {
            Guid id = Guid.NewGuid();
            using var valid = await host.Client.PostAsync("/Suppression/_CreateSuppression",
                Form(("SuppressionID", id.ToString()), ("FRN", "1000000001"), ("SchemeYear", "2026")));
            Assert.That(valid.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await valid.Content.ReadAsStringAsync(), Is.EqualTo("{\"Ok\":true}"));
            Assert.That(host.Added.Single().SuppressionID, Is.EqualTo(id));
            Assert.That(host.Added.Single().SuppressionStart, Is.Not.EqualTo(default(DateTime)));
            host.Context.Verify(c => c.SaveChanges(), Times.Once);

            using var invalid = await host.Client.PostAsync("/Suppression/_CreateSuppression",
                Form(("FRN", "invalid"), ("SchemeYear", "2014")));
            Assert.That(invalid.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await invalid.Content.ReadAsStringAsync(), Is.EqualTo("{\"Ok\":false}"));
            Assert.That(host.Added.Count, Is.EqualTo(1));
            host.Context.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Test]
        public async Task Single_Suppression_Preserves_Existing_Ended_Row_Guard()
        {
            host.Suppressions.Add(new Suppression(1000000001, 2026) { SuppressionEnd = DateTime.Now });
            using var response = await host.Client.PostAsync("/Suppression/_CreateSuppression",
                Form(("FRN", "1000000001"), ("SchemeYear", "2026")));
            Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("{\"Ok\":true}"));
            Assert.That(host.Added, Is.Empty);
            host.Context.Verify(c => c.SaveChanges(), Times.Never);
        }

        [Test]
        public async Task Bulk_Upload_Binds_Multipart_Names_And_Calls_Existing_Service()
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent("Remove"), "BulkDirections");
            content.Add(new StringContent("2026"), "SchemeYears");
            content.Add(new ByteArrayContent(new byte[] { 1, 2, 3 }), "bulkSource", "synthetic.xlsx");
            using var response = await host.Client.PostAsync("/Suppression/_BulkSuppression", content);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
            host.Bulk.Verify(s => s.BulkUpdate("Remove", "2026",
                It.Is<Microsoft.AspNetCore.Http.IFormFile>(f => f.Name == "bulkSource" &&
                    f.FileName == "synthetic.xlsx" && f.Length == 3)), Times.Once);
        }

        [Test]
        public async Task Bulk_Partial_Renders_Existing_Selection_Values()
        {
            using var response = await host.Client.GetAsync("/Suppression/_BulkSuppression");
            string body = await response.Content.ReadAsStringAsync();
            Assert.That(body, Does.Contain("name=\"BulkDirections\""));
            Assert.That(body, Does.Contain("value=\"Add\""));
            Assert.That(body, Does.Contain("value=\"Remove\""));
            Assert.That(body, Does.Contain("name=\"SchemeYears\""));
            Assert.That(body, Does.Contain("name=\"bulkSource\""));
        }

        [TestCase("GET")]
        [TestCase("POST")]
        public async Task Remove_Suppression_Preserves_Unrestricted_Verb_And_Path_Id(string method)
        {
            var suppression = new Suppression(1000000001, 2026);
            host.Suppressions.Add(suppression);
            using var response = await Send(method, "/Suppression/RemoveSuppression/" + suppression.SuppressionID);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await response.Content.ReadAsStringAsync(), Is.Empty);
            Assert.That(suppression.SuppressionEnd, Is.Not.Null);
            host.Context.Verify(c => c.SetModified(suppression), Times.Once);
            host.Context.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Test]
        public async Task ResolveError_Post_Binds_Id_And_Saves_Without_New_Antiforgery_Requirement()
        {
            var error = new Error { ErrorID = Guid.NewGuid(), Resolved = false };
            host.Errors.Add(error);
            using var response = await host.Client.PostAsync("/Error/ResolveError", Form(("id", error.ErrorID.ToString())));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await response.Content.ReadAsStringAsync(), Is.Empty);
            Assert.That(error.Resolved, Is.True);
            host.Context.Verify(c => c.SaveChanges(), Times.Once);
        }

        [TestCase("GET")]
        [TestCase("POST")]
        public async Task Import_Uses_Existing_Synchronous_Station(string method)
        {
            using var response = await Send(method, "/Import/Import");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
            host.Station.Verify(s => s.Activate(), Times.Once);
        }

        [TestCase("BPS", "synthetic-BPS")]
        [TestCase("XB", "synthetic-XB")]
        public async Task Sample_Download_Preserves_Selection_Mime_And_Name(string type, string bytes)
        {
            using var response = await host.Client.GetAsync("/Home/Sample?statementType=" + type);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content.Headers.ContentType.ToString(), Is.EqualTo("application/octet-stream"));
            Assert.That(response.Content.Headers.ContentDisposition.FileName.Trim('"'), Is.EqualTo("Sample Claim Statement - " + type + ".docx"));
            Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo(bytes));
            host.Sample.Verify(s => s.Sample(), type == "BPS" ? Times.Once() : Times.Never());
            host.SampleXB.Verify(s => s.Sample(), type == "XB" ? Times.Once() : Times.Never());
        }

        [TestCase("/Suppression/Download", "Suppressions.csv", false)]
        [TestCase("/Error/Download", "Errors.csv", true)]
        [TestCase("/Log/Download?count=1", "Log.csv", false)]
        public async Task Csv_Downloads_Preserve_Names_Mime_Bom_Order_Count_And_British_Culture(string path, string name, bool bom)
        {
            var date = new DateTime(2026, 11, 9, 14, 15, 0);
            host.Suppressions.Add(new Suppression(1000000001, 2026) { SuppressionStart = date });
            host.Errors.Add(new Error { FRN = 1000000001, SchemeYear = 2026, ErrorDate = date, ErrorMessage = "synthetic" });
            host.Logs.Add(new Log { FRN = 1000000001, ClaimProduced = date, SchemeYear = 2026, PDFFile = "synthetic" });
            host.Logs.Add(new Log { FRN = 1000000002, ClaimProduced = date.AddDays(-1), SchemeYear = 2026, PDFFile = "older" });
            host.Client.DefaultRequestHeaders.Add("Accept-Language", "en-US");
            using var response = await host.Client.GetAsync(path);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content.Headers.ContentDisposition.ToString(), Is.EqualTo("attachment; filename=" + name));
            Assert.That(response.Content.Headers.ContentType.ToString(), Is.EqualTo("application/text"));
            byte[] data = await response.Content.ReadAsByteArrayAsync();
            Assert.That(data.Take(3).SequenceEqual(Encoding.UTF8.GetPreamble()), Is.EqualTo(bom));
            string body = Encoding.UTF8.GetString(data);
            Assert.That(body, Does.Contain("09/11/2026 14:15:00"));
            Assert.That(body, Does.Contain("1000000001"));
            if (name == "Log.csv") Assert.That(body, Does.Not.Contain("1000000002"));
        }

        [Test]
        public async Task Log_Download_Post_Binds_Count()
        {
            host.Logs.Add(new Log { FRN = 1000000001, ClaimProduced = DateTime.Now, PDFFile = "synthetic" });
            using var response = await host.Client.PostAsync("/Log/Download", Form(("count", "0")));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await response.Content.ReadAsStringAsync(), Does.Not.Contain("1000000001"));
        }

        [TestCase("/Suppression/_Existing")]
        [TestCase("/Log/Index")]
        [TestCase("/Error/Index")]
        public async Task Paging_Query_And_Generated_Links_Preserve_Legacy_Numeric_Page(string path)
        {
            for (int i = 0; i < 3; i++)
            {
                host.Suppressions.Add(new Suppression(1000000001 + i, 2026) { SuppressionStart = DateTime.Today.AddDays(i) });
                host.Logs.Add(new Log { FRN = 1000000001 + i, ClaimProduced = DateTime.Today.AddDays(i), PDFFile = "synthetic" });
                host.Errors.Add(new Error { FRN = 1000000001 + i, ErrorDate = DateTime.Today.AddDays(i), ErrorMessage = "synthetic" });
            }
            using var first = await host.Client.GetAsync(path + "?pageSize=1");
            Assert.That(first.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            string body = await first.Content.ReadAsStringAsync();
            var link = Regex.Match(body, "href=\"([^\"]*[?&](?:amp;)?page=2[^\"]*)\"");
            Assert.That(link.Success, Is.True, "Generated pager link must retain the numeric page query.");
            using var response = await host.Client.GetAsync(WebUtility.HtmlDecode(link.Groups[1].Value));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await response.Content.ReadAsStringAsync(), Does.Contain("Showing 2 to 2 of 3"));
        }

        [TestCase("/Log/Index")]
        [TestCase("/Error/Index")]
        [TestCase("/Suppression/_Existing")]
        public async Task Search_And_Paging_Post_Forms_Still_Bind(string path)
        {
            for (int i = 0; i < 3; i++)
            {
                host.Suppressions.Add(new Suppression(1000000001, 2026) { SuppressionStart = DateTime.Today.AddDays(i) });
                host.Logs.Add(new Log { FRN = 1000000001, ClaimProduced = DateTime.Today.AddDays(i), PDFFile = "synthetic" });
                host.Errors.Add(new Error { FRN = 1000000001, ErrorDate = DateTime.Today.AddDays(i), ErrorMessage = "synthetic" });
            }
            using var response = await host.Client.PostAsync(path, Form(("searchString", " 1000000001 "), ("page", "2"), ("pageSize", "1")));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await response.Content.ReadAsStringAsync(), Does.Contain("Showing 2 to 2 of 3"));
        }

        [Test]
        public async Task Remote_Handler_Exception_Uses_Existing_Generic_500_Page_And_Does_Not_Leak()
        {
            host.Station.Setup(s => s.Activate()).Throws(new InvalidOperationException("private synthetic detail"));
            host.Client.DefaultRequestHeaders.Add("X-Test-Remote", "true");
            using var response = await host.Client.GetAsync("/Import/Import");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
            string body = await response.Content.ReadAsStringAsync();
            Assert.That(body, Does.Contain("Application Error"));
            Assert.That(body, Does.Not.Contain("private synthetic detail"));
        }

        [Test]
        public void Local_Handler_Exception_Remains_Unhandled()
        {
            host.Station.Setup(s => s.Activate()).Throws(new InvalidOperationException("local synthetic detail"));
            var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await host.Client.GetAsync("/Import/Import"));
            Assert.That(exception.Message, Is.EqualTo("local synthetic detail"));
        }

        [Test]
        public void Bulk_Exception_Preserves_Wrapper_And_Inner_Exception()
        {
            var inner = new InvalidOperationException("synthetic bulk failure");
            host.Bulk.Setup(s => s.BulkUpdate(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<Microsoft.AspNetCore.Http.IFormFile>())).Throws(inner);
            var exception = Assert.ThrowsAsync<Exception>(async () =>
                await host.Client.PostAsync("/Suppression/_BulkSuppression", Form(("BulkDirections", "Add"), ("SchemeYears", "2026"))));
            Assert.That(exception.Message, Does.StartWith("Unable to bulk update suppressions."));
            Assert.That(exception.InnerException, Is.SameAs(inner));
        }

        [Test]
        public async Task View_Only_Operator_Does_Not_Render_Management_Controls()
        {
            host.Roles(View);
            using var home = await host.Client.GetAsync("/");
            string body = await home.Content.ReadAsStringAsync();
            Assert.That(body, Does.Not.Contain("generation-request"));
            Assert.That(body, Does.Contain("Download Sample Statement"));
            using var suppressions = await host.Client.GetAsync("/Suppression");
            Assert.That(await suppressions.Content.ReadAsStringAsync(), Does.Not.Contain("Create Suppression"));
        }

        private async Task<Dictionary<string, string>> Antiforgery()
        {
            using var response = await host.Client.GetAsync("/");
            string body = await response.Content.ReadAsStringAsync();
            var match = Regex.Match(body, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
            Assert.That(match.Success, Is.True, "Real rendered form must contain the antiforgery field.");
            // TestServer's HttpClient deliberately has no CookieContainer.
            UpdateCookies(response);
            return new Dictionary<string, string> { ["__RequestVerificationToken"] = WebUtility.HtmlDecode(match.Groups[1].Value) };
        }

        private void UpdateCookies(HttpResponseMessage response)
        {
            if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                var previous = host.Client.DefaultRequestHeaders.TryGetValues("Cookie", out var old)
                    ? string.Join("; ", old) + "; " : "";
                host.Client.DefaultRequestHeaders.Remove("Cookie");
                host.Client.DefaultRequestHeaders.Add("Cookie", previous + string.Join("; ", cookies.Select(c => c.Split(';')[0])));
            }
        }

        private static FormUrlEncodedContent Form(params (string Name, string Value)[] pairs) =>
            new FormUrlEncodedContent(pairs.Select(p => new KeyValuePair<string, string>(p.Name, p.Value)));

        private async Task<HttpResponseMessage> Send(string method, string path)
        {
            using var request = new HttpRequestMessage(new HttpMethod(method), path);
            if (method == "POST") request.Content = Form();
            return await host.Client.SendAsync(request);
        }

        private void NoBusinessCalls()
        {
            Assert.That(host.Context.Invocations, Is.Empty);
            Assert.That(host.Plant.Invocations, Is.Empty);
            Assert.That(host.Station.Invocations, Is.Empty);
            Assert.That(host.Bulk.Invocations, Is.Empty);
            Assert.That(host.Sample.Invocations, Is.Empty);
            Assert.That(host.SampleXB.Invocations, Is.Empty);
            Assert.That(host.Configuration.Invocations, Is.Empty);
        }
    }
}
