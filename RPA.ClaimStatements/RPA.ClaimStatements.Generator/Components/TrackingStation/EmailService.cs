using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;

namespace RPA.ClaimStatements.Generator.Components.TrackingStation
{
    public class EmailService : IEmailService, IDisposable
    {
        private static readonly HttpClient SharedClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };

        ClaimStatementsContext db;
        IConfigurationService configurationService;
        private readonly MailSettings settings;
        private readonly TokenCredential credential;
        private readonly HttpClient client;

        public EmailService() : this(new ClaimStatementsContext()) { }

        public EmailService(ClaimStatementsContext context) : this(context, new ConfigurationService(context)) { }

        public EmailService(ClaimStatementsContext context, IConfigurationService configurationService)
            : this(configurationService, null, null, SharedClient)
        {
            this.db = context;
        }

        // Lets tests supply the settings, token and HTTP handler; production reads them from the environment.
        internal EmailService(IConfigurationService configurationService, MailSettings settings,
            TokenCredential credential, HttpClient client)
        {
            this.configurationService = configurationService;
            this.settings = settings;
            this.credential = credential;
            this.client = client;
        }

        public void Send(string content)
        {
            var mail = settings ?? MailSettings.FromEnvironment();
            if (!mail.Enabled)
            {
                Console.WriteLine("Tracking report mail is disabled (Mail__Enabled is not true).");
                return;
            }

            string email = configurationService.GetValue("Email Account");
            if (string.IsNullOrWhiteSpace(email))
                throw new InvalidOperationException("The Email Account setting must contain the Exchange Online mailbox when mail is enabled.");

            SendAsync(credential ?? mail.CreateCredential(), email.Trim(), content).GetAwaiter().GetResult();
        }

        private async Task SendAsync(TokenCredential tokenCredential, string mailbox, string content)
        {
            var token = await tokenCredential.GetTokenAsync(
                new TokenRequestContext(new[] { "https://graph.microsoft.com/.default" }), CancellationToken.None).ConfigureAwait(false);
            using var request = new HttpRequestMessage(HttpMethod.Post,
                "https://graph.microsoft.com/v1.0/users/" + Uri.EscapeDataString(mailbox) + "/sendMail");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            var payload = new
            {
                message = new
                {
                    subject = "Claim Statement Generation Report",
                    body = new { contentType = "HTML", content },
                    toRecipients = new[] { new { emailAddress = new { address = mailbox } } }
                },
                saveToSentItems = false
            };
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            using var response = await client.SendAsync(request).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.Accepted)
                throw new InvalidOperationException($"Graph tracking report submission failed (HTTP {(int)response.StatusCode}).");
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    db?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
