using Azure.Identity;
using NUnit.Framework;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;

namespace RPA.ClaimStatements.Generator.Tests.Services
{
    [TestFixture]
    public class TelemetrySettingsTests
    {
        private const string Connection = "InstrumentationKey=33333333-3333-3333-3333-333333333333;IngestionEndpoint=https://unused.invalid/";
        private const string Identity = "33333333-3333-3333-3333-333333333333";

        private static TelemetrySettings Resolve(params (string Key, string Value)[] settings)
        {
            var values = new Dictionary<string, string>();
            foreach (var (key, value) in settings)
                values[key] = value;
            return TelemetrySettings.Resolve(key => values.TryGetValue(key, out var value) ? value : null);
        }

        [Test]
        public void Resolve_NothingConfigured_IsDisabledWithoutCredential()
        {
            var settings = Resolve();

            Assert.That(settings.Enabled, Is.False);
            Assert.That(settings.CreateCredential(), Is.Null);
        }

        [Test]
        public void Resolve_BlankConnectionString_IsDisabled()
        {
            Assert.That(Resolve((TelemetrySettings.ConnectionStringKey, "   ")).Enabled, Is.False);
        }

        [Test]
        public void Resolve_AzureVariableWinsOverAlternativeKey()
        {
            var settings = Resolve(
                (TelemetrySettings.ConnectionStringKey, Connection),
                (TelemetrySettings.AlternativeConnectionStringKey, "ignored"));

            Assert.That(settings.ConnectionString, Is.EqualTo(Connection));
        }

        [Test]
        public void Resolve_AlternativeKeyUsedOnlyWhenAzureVariableIsAbsent()
        {
            Assert.That(Resolve((TelemetrySettings.AlternativeConnectionStringKey, Connection)).ConnectionString, Is.EqualTo(Connection));
            Assert.That(Resolve(
                (TelemetrySettings.ConnectionStringKey, ""),
                (TelemetrySettings.AlternativeConnectionStringKey, Connection)).Enabled, Is.False);
        }

        [Test]
        public void Resolve_ValidIdentity_CreatesManagedIdentityCredential()
        {
            var settings = Resolve(
                (TelemetrySettings.ConnectionStringKey, Connection),
                (TelemetrySettings.ManagedIdentityClientIdKey, Identity));

            Assert.That(settings.CreateCredential(), Is.InstanceOf<ManagedIdentityCredential>());
        }

        [TestCase("")]
        [TestCase("not-a-guid")]
        [TestCase("00000000-0000-0000-0000-000000000000")]
        public void Resolve_InvalidIdentity_Throws(string identity)
        {
            var error = Assert.Throws<InvalidOperationException>(() => Resolve(
                (TelemetrySettings.ConnectionStringKey, Connection),
                (TelemetrySettings.ManagedIdentityClientIdKey, identity)));

            Assert.That(error.Message, Does.Contain(TelemetrySettings.ManagedIdentityClientIdKey));
        }

        [TestCase(null)]
        [TestCase(" ")]
        public void Resolve_IdentityWithoutConnectionString_ThrowsInsteadOfDisablingTelemetry(string connection)
        {
            Assert.Throws<InvalidOperationException>(() => Resolve(
                (TelemetrySettings.ConnectionStringKey, connection),
                (TelemetrySettings.ManagedIdentityClientIdKey, Identity)));
        }
    }
}
