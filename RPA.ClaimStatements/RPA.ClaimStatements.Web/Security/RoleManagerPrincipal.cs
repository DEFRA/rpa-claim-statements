using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace RPA.ClaimStatements.Web.Security
{
    /// <summary>
    /// Mirrors the ASP.NET <c>RolePrincipal</c> used by the legacy roleManager: roles come only
    /// from the role store for <c>Identity.Name</c> (Windows group membership is not used),
    /// are loaded once, on first use, and are compared case-insensitively.
    /// </summary>
    public sealed class RoleManagerPrincipal : ClaimsPrincipal
    {
        private readonly Lazy<HashSet<string>> roles;

        public RoleManagerPrincipal(ClaimsPrincipal principal, IRoleStore roleStore)
            : base(principal.Identities)
        {
            string userName = principal.Identity.Name;

            roles = new Lazy<HashSet<string>>(
                () => new HashSet<string>(roleStore.GetRolesForUser(userName), StringComparer.OrdinalIgnoreCase));
        }

        public override bool IsInRole(string role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return roles.Value.Contains(role);
        }
    }

    public sealed class RoleManagerClaimsTransformation : IClaimsTransformation
    {
        private readonly IRoleStore roleStore;

        public RoleManagerClaimsTransformation(IRoleStore roleStore)
        {
            this.roleStore = roleStore;
        }

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity == null || !principal.Identity.IsAuthenticated || principal is RoleManagerPrincipal)
            {
                return Task.FromResult(principal);
            }

            return Task.FromResult<ClaimsPrincipal>(new RoleManagerPrincipal(principal, roleStore));
        }
    }
}
