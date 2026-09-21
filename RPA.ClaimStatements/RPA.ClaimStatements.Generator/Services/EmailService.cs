using RPA.ClaimStatements.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Services
{
    public class EmailService : IEmailService, IDisposable
    {
        ClaimStatementsContext db;
        IConfigurationService configurationService;

        public EmailService()
        {
            this.db = new ClaimStatementsContext();
            this.configurationService = new ConfigurationService(db);
        }

        public EmailService(ClaimStatementsContext context)
        {
            this.db = context;
            this.configurationService = new ConfigurationService(context);
        }

        public EmailService(ClaimStatementsContext context, IConfigurationService configurationService)
        {
            this.db = context;
            this.configurationService = configurationService;
        }

        public void Send(string content)
        {
            string email = configurationService.GetValue("Email Account");

            MailMessage message = new MailMessage
            {
                Subject = "Claim Statement Generation Report",
                IsBodyHtml = true,
                Body = content
            };

            message.From = new MailAddress(email);
            message.To.Add(new MailAddress(email));

            SmtpClient client = new SmtpClient();
            client.Send(message);
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    db.Dispose();
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
