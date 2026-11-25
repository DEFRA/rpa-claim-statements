using System;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RPA.ClaimStatements.Web.Security
{
    public sealed class SharedDataProtectionOptions
    {
        public bool Enabled { get; set; }
        public string BlobUri { get; set; }
        public string KeyIdentifier { get; set; }
        public string ApplicationName { get; set; }
        public string ManagedIdentityClientId { get; set; }
    }

    // Same settings contract as the other DEFRA applications. Without a DataProtection section the ASP.NET Core
    // local defaults apply (local development). With Enabled=true the key ring is stored in Azure Blob storage and
    // encrypted with a Key Vault key, both through the user-assigned managed identity, so replicas and revisions
    // share keys. There is no developer-credential or stored-secret fallback.
    public static class DataProtectionConfiguration
    {
        public static IServiceCollection AddClaimStatementsDataProtection(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("DataProtection");
            // Unknown setting names fail startup, so a misspelled or leftover setting cannot be ignored silently.
            var settings = section.Get<SharedDataProtectionOptions>(options => options.ErrorOnUnknownConfiguration = true) ?? new SharedDataProtectionOptions();

            if (section.Exists() && !bool.TryParse(section["Enabled"], out _))
            {
                throw new InvalidOperationException("DataProtection:Enabled must explicitly be true or false.");
            }

            if (!settings.Enabled)
            {
                return services;
            }

            var blob = Endpoint(settings.BlobUri, "BlobUri");
            if (blob.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries).Length < 2 || blob.AbsolutePath.EndsWith('/'))
            {
                throw Invalid("BlobUri", "must identify a container/blob");
            }

            var key = Endpoint(settings.KeyIdentifier, "KeyIdentifier");

            // A versionless URI lets newly generated Data Protection keys use the current wrapping-key version.
            var segments = key.AbsolutePath.Split('/');
            if (segments.Length != 3 || segments[1] != "keys" || string.IsNullOrEmpty(segments[2]))
            {
                throw Invalid("KeyIdentifier", "must be a versionless /keys/<key-name> identifier");
            }

            if (string.IsNullOrWhiteSpace(settings.ApplicationName))
            {
                throw Invalid("ApplicationName", "must be a stable environment-specific name");
            }

            if (!Guid.TryParse(settings.ManagedIdentityClientId, out var clientId) || clientId == Guid.Empty)
            {
                throw Invalid("ManagedIdentityClientId", "must identify the user-assigned managed identity");
            }

            services.AddSingleton(new ManagedIdentityCredential(ManagedIdentityId.FromUserAssignedClientId(clientId.ToString())));
            services.AddSingleton(provider => new BlobClient(blob, provider.GetRequiredService<ManagedIdentityCredential>()));
            // ApplicationName must be identical within an environment and distinct between environments.
            services.AddDataProtection()
                .SetApplicationName(settings.ApplicationName)
                .PersistKeysToAzureBlobStorage(provider => provider.GetRequiredService<BlobClient>())
                .ProtectKeysWithAzureKeyVault(key, provider => provider.GetRequiredService<ManagedIdentityCredential>());

            return services;
        }

        private static Uri Endpoint(string value, string setting)
        {
            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps ||
                !string.IsNullOrEmpty(uri.UserInfo) || !string.IsNullOrEmpty(uri.Query) || !string.IsNullOrEmpty(uri.Fragment))
            {
                throw Invalid(setting, "must be an HTTPS resource identifier without credentials, query or fragment");
            }

            return uri;
        }

        private static InvalidOperationException Invalid(string name, string rule) =>
            new InvalidOperationException($"DataProtection:{name} {rule}.");
    }
}
