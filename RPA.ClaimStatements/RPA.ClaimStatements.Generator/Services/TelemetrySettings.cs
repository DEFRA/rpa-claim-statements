using Azure.Core;
using Azure.Identity;
using System;

namespace RPA.ClaimStatements.Generator.Services
{
    // Application Insights settings shared by the Web application and the Generator. Same contract as the
    // other DEFRA applications: telemetry is off unless a connection string is configured, and an optional
    // user-assigned managed identity authenticates ingestion (required when local authentication is disabled
    // on the Application Insights resource).
    public sealed class TelemetrySettings
    {
        public const string ConnectionStringKey = "APPLICATIONINSIGHTS_CONNECTION_STRING";
        public const string AlternativeConnectionStringKey = "ApplicationInsights:ConnectionString";
        public const string ManagedIdentityClientIdKey = "Telemetry:ManagedIdentityClientId";

        public string ConnectionString { get; private set; }
        public string ManagedIdentityClientId { get; private set; }

        public bool Enabled => !string.IsNullOrWhiteSpace(ConnectionString);

        // Reads the environment, mapping the configuration-style keys to their environment form (':' -> '__').
        public static TelemetrySettings FromEnvironment()
        {
            return Resolve(key => Environment.GetEnvironmentVariable(key.Replace(":", "__")));
        }

        // getValue takes configuration-style keys, so it can be backed by IConfiguration or the environment.
        public static TelemetrySettings Resolve(Func<string, string> getValue)
        {
            // The Azure environment variable wins; the alternative key is used only when it is absent, not blank.
            string connectionString = getValue(ConnectionStringKey) ?? getValue(AlternativeConnectionStringKey);
            string identity = getValue(ManagedIdentityClientIdKey);

            // An explicitly empty identity is a configuration error, not an opt-out.
            if (identity != null)
            {
                if (!Guid.TryParse(identity, out Guid clientId) || clientId == Guid.Empty)
                {
                    throw new InvalidOperationException(ManagedIdentityClientIdKey + " must identify the user-assigned managed identity.");
                }

                // A deployment mistake must not silently disable telemetry.
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException(ManagedIdentityClientIdKey + " requires an Application Insights connection string.");
                }
            }

            return new TelemetrySettings { ConnectionString = connectionString, ManagedIdentityClientId = identity };
        }

        // Null when no identity is configured: ingestion then uses the connection string alone.
        // There is deliberately no developer-credential fallback.
        public TokenCredential CreateCredential()
        {
            return ManagedIdentityClientId == null
                ? null
                : new ManagedIdentityCredential(ManagedIdentityId.FromUserAssignedClientId(ManagedIdentityClientId));
        }
    }
}
