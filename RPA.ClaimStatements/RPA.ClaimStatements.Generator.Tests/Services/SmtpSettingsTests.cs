using NUnit.Framework;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;

namespace RPA.ClaimStatements.Generator.Tests.Services
{
    [TestFixture]
    public class SmtpSettingsTests
    {
        private static SmtpSettings Parse(params (string Name, string Value)[] variables)
        {
            var values = new Dictionary<string, string>();
            foreach (var (name, value) in variables)
                values[name] = value;
            return SmtpSettings.Parse(name => values.TryGetValue(name, out var value) ? value : null);
        }

        [Test]
        public void Parse_HostOnly_UsesPlainPort25WithoutCredentials()
        {
            var settings = Parse((SmtpSettings.HostVariable, "smtp.synthetic.invalid"));
            Assert.That(settings.Host, Is.EqualTo("smtp.synthetic.invalid"));
            Assert.That(settings.Port, Is.EqualTo(25));
            Assert.That(settings.EnableSsl, Is.False);
            Assert.That(settings.UserName, Is.Null);

            using (var client = settings.CreateClient())
            {
                Assert.That(client.Host, Is.EqualTo("smtp.synthetic.invalid"));
                Assert.That(client.Credentials, Is.Null);
            }
        }

        [Test]
        public void Parse_AllSettings_AppliesThemToTheClient()
        {
            var settings = Parse(
                (SmtpSettings.HostVariable, "smtp.synthetic.invalid"),
                (SmtpSettings.PortVariable, "587"),
                (SmtpSettings.EnableSslVariable, "true"),
                (SmtpSettings.UserNameVariable, "synthetic-user"),
                (SmtpSettings.PasswordVariable, "synthetic-password"));

            using (var client = settings.CreateClient())
            {
                Assert.That(client.Port, Is.EqualTo(587));
                Assert.That(client.EnableSsl, Is.True);
                var credential = client.Credentials.GetCredential(client.Host, client.Port, "basic");
                Assert.That(credential.UserName, Is.EqualTo("synthetic-user"));
                Assert.That(credential.Password, Is.EqualTo("synthetic-password"));
            }
        }

        [TestCase(null)]
        [TestCase("  ")]
        public void Parse_HostMissingOrBlank_ThrowsNamingTheVariable(string host)
        {
            var error = Assert.Throws<InvalidOperationException>(() => Parse((SmtpSettings.HostVariable, host)));
            Assert.That(error.Message, Does.Contain(SmtpSettings.HostVariable));
        }

        [TestCase("65536")]
        [TestCase("smtp")]
        public void Parse_InvalidPort_Throws(string port)
        {
            var error = Assert.Throws<InvalidOperationException>(() => Parse(
                (SmtpSettings.HostVariable, "smtp.synthetic.invalid"), (SmtpSettings.PortVariable, port)));
            Assert.That(error.Message, Does.Contain(SmtpSettings.PortVariable));
        }

        [Test]
        public void Parse_InvalidEnableSsl_Throws()
        {
            var error = Assert.Throws<InvalidOperationException>(() => Parse(
                (SmtpSettings.HostVariable, "smtp.synthetic.invalid"), (SmtpSettings.EnableSslVariable, "maybe")));
            Assert.That(error.Message, Does.Contain(SmtpSettings.EnableSslVariable));
        }

        [Test]
        public void Parse_CredentialsWithoutTls_ThrowsWithoutDisclosingThePassword()
        {
            var error = Assert.Throws<InvalidOperationException>(() => Parse(
                (SmtpSettings.HostVariable, "smtp.synthetic.invalid"),
                (SmtpSettings.UserNameVariable, "synthetic-user"),
                (SmtpSettings.PasswordVariable, "synthetic-secret")));
            Assert.That(error.Message, Does.Contain(SmtpSettings.EnableSslVariable));
            Assert.That(error.ToString(), Does.Not.Contain("synthetic-secret"));
        }
    }
}
