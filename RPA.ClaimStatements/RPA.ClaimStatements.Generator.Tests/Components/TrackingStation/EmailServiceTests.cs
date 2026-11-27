using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Moq;
using NUnit.Framework;
using RPA.ClaimStatements.Generator.Components.TrackingStation;
using RPA.ClaimStatements.Generator.Services;

namespace RPA.ClaimStatements.Generator.Tests.Components.TrackingStation
{
    [TestFixture]
    public class EmailServiceTests
    {
        private static MailSettings Settings(bool enabled) => MailSettings.Parse(key =>
            key == MailSettings.EnabledVariable ? enabled.ToString() : "11111111-2222-3333-4444-555555555555");

        [Test]
        public void Send_Disabled_DoesNotReadMailboxAcquireTokenOrCallHttp()
        {
            var configuration = new Mock<IConfigurationService>(MockBehavior.Strict);
            var credential = new TestCredential();
            using var handler = new TestHandler(_ => throw new AssertionException("Unexpected HTTP request"));
            using var client = new HttpClient(handler);
            using var service = new EmailService(configuration.Object, Settings(false), credential, client);
            service.Send("<p>Not sent</p>");
            Assert.That(credential.Calls, Is.Zero);
            configuration.VerifyNoOtherCalls();
        }

        [Test]
        public void Send_Enabled_SubmitsUnchangedHtmlWithMailboxAndBearerToken()
        {
            const string mailbox = "reports+test@synthetic.invalid";
            const string html = "<p>Amount &amp; \"quoted\"</p>";
            var credential = new TestCredential();
            int calls = 0;
            using var handler = new TestHandler(request =>
            {
                calls++;
                Assert.That(request.Method, Is.EqualTo(HttpMethod.Post));
                Assert.That(request.RequestUri.AbsoluteUri, Is.EqualTo(
                    "https://graph.microsoft.com/v1.0/users/" + Uri.EscapeDataString(mailbox) + "/sendMail"));
                Assert.That(request.Headers.Authorization.Scheme, Is.EqualTo("Bearer"));
                Assert.That(request.Headers.Authorization.Parameter, Is.EqualTo("test-placeholder-not-issued"));
                using var payload = JsonDocument.Parse(request.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                var message = payload.RootElement.GetProperty("message");
                Assert.That(message.GetProperty("subject").GetString(), Is.EqualTo("Claim Statement Generation Report"));
                Assert.That(message.GetProperty("body").GetProperty("contentType").GetString(), Is.EqualTo("HTML"));
                Assert.That(message.GetProperty("body").GetProperty("content").GetString(), Is.EqualTo(html));
                Assert.That(message.GetProperty("toRecipients")[0].GetProperty("emailAddress").GetProperty("address").GetString(), Is.EqualTo(mailbox));
                Assert.That(payload.RootElement.GetProperty("saveToSentItems").GetBoolean(), Is.False);
                return new HttpResponseMessage(HttpStatusCode.Accepted);
            });
            using var client = new HttpClient(handler);
            using var service = new EmailService(Configuration(mailbox), Settings(true), credential, client);
            service.Send(html);
            Assert.That(calls, Is.EqualTo(1));
            Assert.That(credential.Scopes, Is.EqualTo(new[] { "https://graph.microsoft.com/.default" }));
        }

        [Test]
        public void Send_GraphRefuses_ThrowsWithStatusCode()
        {
            using var handler = new TestHandler(_ => new HttpResponseMessage(HttpStatusCode.Forbidden));
            using var client = new HttpClient(handler);
            using var service = new EmailService(Configuration("reports@synthetic.invalid"), Settings(true), new TestCredential(), client);
            var error = Assert.Throws<InvalidOperationException>(() => service.Send("<p>Report</p>"));
            Assert.That(error.Message, Does.Contain("403"));
        }

        [Test]
        public void Send_BlankMailbox_FailsBeforeAcquiringTokenOrCallingHttp()
        {
            var credential = new TestCredential();
            using var handler = new TestHandler(_ => throw new AssertionException("Unexpected HTTP request"));
            using var client = new HttpClient(handler);
            using var service = new EmailService(Configuration(" "), Settings(true), credential, client);
            var error = Assert.Throws<InvalidOperationException>(() => service.Send("<p>Report</p>"));
            Assert.That(error.Message, Does.Contain("Email Account"));
            Assert.That(credential.Calls, Is.Zero);
        }

        private static IConfigurationService Configuration(string mailbox)
        {
            var configuration = new Mock<IConfigurationService>();
            configuration.Setup(service => service.GetValue("Email Account")).Returns(mailbox);
            return configuration.Object;
        }

        private sealed class TestHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> send;
            public TestHandler(Func<HttpRequestMessage, HttpResponseMessage> send) => this.send = send;
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
                Task.FromResult(send(request));
        }

        private sealed class TestCredential : TokenCredential
        {
            public int Calls { get; private set; }
            public string[] Scopes { get; private set; }
            public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken) =>
                GetTokenAsync(requestContext, cancellationToken).GetAwaiter().GetResult();
            public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
            {
                Calls++;
                Scopes = requestContext.Scopes;
                return new ValueTask<AccessToken>(new AccessToken("test-placeholder-not-issued", DateTimeOffset.UtcNow.AddMinutes(5)));
            }
        }
    }
}
