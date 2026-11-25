using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace RPA.ClaimStatements.Web.Security
{
    /// <summary>
    /// Maps validated Entra ID application-role claims to the legacy permission names via
    /// <see cref="AppRolePermissionMap"/>, so every existing <c>[Authorize(Roles = "...")]</c>
    /// attribute and <c>User.IsInRole(...)</c> view check keeps working unmodified against the
    /// new identity provider.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Claim source</b>: Microsoft identity platform application roles are issued on the
    /// validated token as a claim of type <c>"roles"</c> (Microsoft.Identity.Web's well-known
    /// claim type for app roles, which it also configures as the <c>RoleClaimType</c> used by
    /// <c>ClaimsPrincipal.IsInRole</c>). This transformation also recognises
    /// <see cref="ClaimTypes.Role"/> defensively, in case a role claim has already been normalised
    /// to it upstream. The literal string is used because "roles" is the claim type Microsoft
    /// documents for Entra ID application roles.
    /// </para>
    /// <para>
    /// <b>No other claim is trusted for authorization.</b> Group membership claims ("groups",
    /// "hasgroups" overage indicator), <c>name</c>/<c>preferred_username</c>/<c>email</c>, and the
    /// tenant/object identifier are all ignored here — only role claim values that exactly match a
    /// known entry in <see cref="AppRolePermissionMap"/> grant anything. An authenticated identity
    /// with no recognised role claim ends up with no mapped permission claims at all (deny by
    /// default); an unauthenticated principal is returned unchanged.
    /// </para>
    /// <para>
    /// <b>Idempotent, not cumulative, no cross-identity leakage.</b> This transformation is a pure
    /// function of the claims already present on the principal passed to it for the current
    /// request; it holds no static/shared state and never consults anything keyed by username, so
    /// there is no way for one request's lookup to influence another's. Before adding a mapped
    /// permission claim it checks the identity does not already carry an identical one
    /// (<see cref="ClaimsIdentity.HasClaim(string, string)"/>), so re-invoking it against an
    /// already-transformed principal — including multiple raw "roles" claims mapping to the same
    /// permission — cannot create duplicate claims or grant anything beyond the roles actually
    /// present on the validated token.
    /// </para>
    /// </remarks>
    public sealed class AppRolePermissionsClaimsTransformation : IClaimsTransformation
    {
        /// <summary>
        /// The claim type Entra ID issues application roles under, and the value
        /// Microsoft.Identity.Web configures as <c>TokenValidationParameters.RoleClaimType</c>
        /// (so it is also what <c>ClaimsPrincipal.IsInRole</c> reads for the raw app role value).
        /// </summary>
        public const string EntraAppRoleClaimType = "roles";

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            if (principal.Identity is not { IsAuthenticated: true })
            {
                return Task.FromResult(principal);
            }

            if (principal.Identities.FirstOrDefault(i => i.IsAuthenticated) is not ClaimsIdentity identity)
            {
                return Task.FromResult(principal);
            }

            // Snapshot first: AddClaim below mutates the same identity's claim collection, and
            // ClaimTypes.Role is also a target type, so iterating live could re-visit claims this
            // method itself just added.
            foreach (Claim roleClaim in principal.Claims
                .Where(c => c.Type == EntraAppRoleClaimType || c.Type == ClaimTypes.Role)
                .ToList())
            {
                if (AppRolePermissionMap.TryGetPermission(roleClaim.Value, out string permission)
                    && !identity.HasClaim(ClaimTypes.Role, permission))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, permission));
                }
            }

            return Task.FromResult(principal);
        }
    }
}
