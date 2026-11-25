using System;

namespace RPA.ClaimStatements.Web.Security
{
    /// <summary>
    /// Extra, explicit startup validation for <c>AzureAd:TenantId</c>, on top of whatever
    /// Microsoft.Identity.Web's own options validation already does for a missing/absent value.
    /// This application is single-tenant (one organisation): Microsoft.Identity.Web would accept
    /// the multi-tenant aliases <c>common</c>/<c>organizations</c>/<c>consumers</c> as "valid"
    /// configuration (they resolve to a real authority), but they relax issuer validation to
    /// "any Microsoft Entra tenant" rather than this organisation's tenant specifically — which is
    /// not what was approved. Call <see cref="ValidateSingleTenant"/> before registering
    /// authentication so a misconfiguration is a visible startup failure, not a silent broadening
    /// of who can sign in.
    /// </summary>
    public static class AzureAdConfigurationValidation
    {
        private static readonly string[] MultiTenantAliases = { "common", "organizations", "consumers" };

        public static void ValidateSingleTenant(string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                throw new InvalidOperationException(
                    "Configuration 'AzureAd:TenantId' is not set. Provide this organisation's own " +
                    "Entra ID tenant GUID (see the README, \"Configuration\").");
            }

            foreach (string alias in MultiTenantAliases)
            {
                if (string.Equals(tenantId, alias, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        $"Configuration 'AzureAd:TenantId' is set to '{tenantId}', a multi-tenant " +
                        "alias. This application requires single-tenant issuer validation for one " +
                        "organisation: set the organisation's own tenant GUID instead (see the README, " +
                        "\"Configuration\").");
                }
            }

            // "00000000-0000-0000-0000-000000000000" is not a real, assignable Entra ID tenant
            // (Guid.Empty): reject it explicitly rather than let it pass through to Microsoft.Identity.Web/
            // Entra ID at sign-in time, where it would surface later (if at all) as a harder-to-diagnose
            // runtime failure instead of a clear startup error.
            if (Guid.TryParse(tenantId, out Guid parsedTenantId) && parsedTenantId == Guid.Empty)
            {
                throw new InvalidOperationException(
                    "Configuration 'AzureAd:TenantId' is set to the empty GUID " +
                    "('00000000-0000-0000-0000-000000000000'), which is not a real Entra ID tenant. " +
                    "Set the organisation's own tenant GUID instead (see the README, \"Configuration\").");
            }
        }
    }
}
