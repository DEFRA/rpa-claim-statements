using System;
using Azure.Core;
using Azure.Identity;

namespace RPA.ClaimStatements.Generator.Services
{
    public sealed class MailSettings
    {
        public const string EnabledVariable = "Mail__Enabled";
        public const string ManagedIdentityClientIdVariable = "Mail__ManagedIdentityClientId";

        public bool Enabled { get; private set; }
        public string ManagedIdentityClientId { get; private set; }

        public static MailSettings FromEnvironment() => Parse(Environment.GetEnvironmentVariable);

        public static MailSettings Parse(Func<string, string> getVariable)
        {
            string value = getVariable(EnabledVariable);
            bool enabled = false;
            if (value != null && !bool.TryParse(value, out enabled))
                throw new InvalidOperationException(EnabledVariable + " must be true or false.");

            string identity = getVariable(ManagedIdentityClientIdVariable);
            if (enabled && (!Guid.TryParse(identity, out Guid clientId) || clientId == Guid.Empty))
                throw new InvalidOperationException(ManagedIdentityClientIdVariable + " must identify the user-assigned managed identity when mail is enabled.");

            return new MailSettings { Enabled = enabled, ManagedIdentityClientId = identity };
        }

        public TokenCredential CreateCredential() =>
            new ManagedIdentityCredential(ManagedIdentityId.FromUserAssignedClientId(ManagedIdentityClientId));
    }
}
