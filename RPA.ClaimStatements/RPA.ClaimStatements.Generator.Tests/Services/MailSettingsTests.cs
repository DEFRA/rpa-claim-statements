using System;
using NUnit.Framework;
using RPA.ClaimStatements.Generator.Services;

namespace RPA.ClaimStatements.Generator.Tests.Services
{
    [TestFixture]
    public class MailSettingsTests
    {
        [Test]
        public void Parse_MissingSwitch_DisablesMailWithoutOtherSettings()
        {
            Assert.That(MailSettings.Parse(_ => null).Enabled, Is.False);
        }

        [Test]
        public void Parse_EnabledWithoutIdentity_ReportsConfigurationError()
        {
            var error = Assert.Throws<InvalidOperationException>(() =>
                MailSettings.Parse(key => key == MailSettings.EnabledVariable ? "true" : null));
            Assert.That(error.Message, Does.Contain(MailSettings.ManagedIdentityClientIdVariable));
        }
    }
}
