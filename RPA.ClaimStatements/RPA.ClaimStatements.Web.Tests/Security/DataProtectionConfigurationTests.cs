using System;
using System.Collections.Generic;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using RPA.ClaimStatements.Web.Security;

namespace RPA.ClaimStatements.Web.Tests.Security
{
    [TestFixture]
    public class DataProtectionConfigurationTests
    {
        private const string BlobUri = "https://offlinetest.blob.core.windows.net/keyring/claimstatements.xml";
        private const string KeyUri = "https://offline-test.vault.azure.net/keys/claimstatements-key";

        private static Dictionary<string, string> Settings() => new Dictionary<string, string>
        {
            ["DataProtection:Enabled"] = "true",
            ["DataProtection:BlobUri"] = BlobUri,
            ["DataProtection:KeyIdentifier"] = KeyUri,
            ["DataProtection:ApplicationName"] = "ClaimStatements.Offline.Production",
            ["DataProtection:ManagedIdentityClientId"] = "33333333-3333-3333-3333-333333333333"
        };

        private static ServiceCollection Register(Dictionary<string, string> settings)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddClaimStatementsDataProtection(new ConfigurationBuilder().AddInMemoryCollection(settings).Build());
            return services;
        }

        [TestCase("Enabled", "not-a-bool")]
        [TestCase("BlobUri", "")]
        [TestCase("BlobUri", "http://offlinetest.blob.core.windows.net/keyring/claimstatements.xml")]
        [TestCase("BlobUri", "https://offlinetest.blob.core.windows.net/keyring")]
        [TestCase("BlobUri", "https://offlinetest.blob.core.windows.net/keyring/claimstatements.xml?sig=not-allowed")]
        [TestCase("KeyIdentifier", "https://offline-test.vault.azure.net/keys/claimstatements-key/version")]
        [TestCase("KeyIdentifier", "https://offline-test.vault.azure.net/secrets/claimstatements-key")]
        [TestCase("ApplicationName", "   ")]
        [TestCase("ManagedIdentityClientId", "00000000-0000-0000-0000-000000000000")]
        [TestCase("ManagedIdentityClientId", "DefaultAzureCredential")]
        [TestCase("KeyDirectory", "/var/lib/claimstatements/keys")]
        public void InvalidOrUnknownSetting_FailsRegistration(string key, string value)
        {
            var settings = Settings();
            settings["DataProtection:" + key] = value;

            Assert.Throws<InvalidOperationException>(() => Register(settings));
        }

        [TestCase("BlobUri")]
        [TestCase("ManagedIdentityClientId")]
        public void MissingRequiredSetting_FailsRegistration(string key)
        {
            var settings = Settings();
            settings.Remove("DataProtection:" + key);

            Assert.Throws<InvalidOperationException>(() => Register(settings));
        }

        [Test]
        public void SettingsWithoutEnabledFlag_DoNotSilentlyUseLocalKeys()
        {
            var settings = Settings();
            settings.Remove("DataProtection:Enabled");

            Assert.Throws<InvalidOperationException>(() => Register(settings));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void AbsentOrDisabled_KeepsTheLocalDefaults(bool explicitlyDisabled)
        {
            var settings = new Dictionary<string, string>();
            if (explicitlyDisabled)
                settings["DataProtection:Enabled"] = "false";

            using var provider = Register(settings).BuildServiceProvider();

            Assert.That(provider.GetService<ManagedIdentityCredential>(), Is.Null);
            var options = provider.GetRequiredService<IOptions<KeyManagementOptions>>().Value;
            Assert.That(options.XmlRepository, Is.Null);
            Assert.That(options.XmlEncryptor, Is.Null);
        }

        [Test]
        public void Enabled_RegistersBlobRepositoryKeyVaultEncryptorAndOnlyManagedIdentity()
        {
            using var provider = Register(Settings()).BuildServiceProvider();

            Assert.That(provider.GetRequiredService<ManagedIdentityCredential>(), Is.Not.Null);
            Assert.That(provider.GetService<DefaultAzureCredential>(), Is.Null);
            Assert.That(provider.GetRequiredService<BlobClient>().Uri, Is.EqualTo(new Uri(BlobUri)));
            var options = provider.GetRequiredService<IOptions<KeyManagementOptions>>().Value;
            Assert.That(options.XmlRepository, Is.Not.Null);
            Assert.That(options.XmlEncryptor, Is.Not.Null);
            Assert.That(provider.GetRequiredService<IOptions<DataProtectionOptions>>().Value.ApplicationDiscriminator,
                Is.EqualTo("ClaimStatements.Offline.Production"));
        }
    }
}
