using System;
using System.Globalization;
using System.Net;
using System.Net.Mail;

namespace RPA.ClaimStatements.Generator.Services
{
    // SMTP delivery for the tracking report, configured entirely through environment variables so each
    // environment supplies its own relay (credentials come from the platform's secret store).
    public sealed class SmtpSettings
    {
        public const string HostVariable = "Smtp__Host";
        public const string PortVariable = "Smtp__Port";
        public const string EnableSslVariable = "Smtp__EnableSsl";
        public const string UserNameVariable = "Smtp__UserName";
        public const string PasswordVariable = "Smtp__Password";

        public string Host { get; private set; }
        public int Port { get; private set; }
        public bool EnableSsl { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }

        public static SmtpSettings FromEnvironment()
        {
            return Parse(Environment.GetEnvironmentVariable);
        }

        public static SmtpSettings Parse(Func<string, string> getVariable)
        {
            string host = getVariable(HostVariable);
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new InvalidOperationException(HostVariable + " is not set or is blank. Supply the SMTP server host name.");
            }

            int port = 25;
            string portValue = getVariable(PortVariable);
            if (!string.IsNullOrWhiteSpace(portValue)
                && (!int.TryParse(portValue, NumberStyles.None, CultureInfo.InvariantCulture, out port) || port < 1 || port > 65535))
            {
                throw new InvalidOperationException(PortVariable + " must be a port number between 1 and 65535.");
            }

            bool enableSsl = false;
            string sslValue = getVariable(EnableSslVariable);
            if (!string.IsNullOrWhiteSpace(sslValue) && !bool.TryParse(sslValue, out enableSsl))
            {
                throw new InvalidOperationException(EnableSslVariable + " must be true or false.");
            }

            string userName = getVariable(UserNameVariable);
            if (!string.IsNullOrEmpty(userName) && !enableSsl)
            {
                throw new InvalidOperationException(UserNameVariable + " requires " + EnableSslVariable + "=true so credentials are not sent unencrypted.");
            }

            return new SmtpSettings
            {
                Host = host.Trim(),
                Port = port,
                EnableSsl = enableSsl,
                UserName = userName,
                Password = getVariable(PasswordVariable)
            };
        }

        public SmtpClient CreateClient()
        {
            var client = new SmtpClient(Host, Port) { EnableSsl = EnableSsl };

            if (!string.IsNullOrEmpty(UserName))
            {
                client.Credentials = new NetworkCredential(UserName, Password);
            }

            return client;
        }
    }
}
