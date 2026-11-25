using System;
using System.Collections.Generic;

namespace RPA.ClaimStatements.Web.Security
{
    /// <summary>
    /// Maps stable, Entra ID-valid application role values (dot-separated, no spaces — Entra ID
    /// app role <c>value</c> strings cannot contain spaces) to the exact legacy permission names
    /// already checked by every existing <c>[Authorize(Roles = "...")]</c> attribute and
    /// <c>User.IsInRole(...)</c> call in this project. No permission is added, renamed, or
    /// removed by this table and none of the existing call sites change — only the source of the
    /// role claim changes, from the legacy Security database to the signed-in user's Entra ID
    /// application role assignment.
    /// </summary>
    public static class AppRolePermissionMap
    {
        /// <summary>
        /// Entra ID app role <c>value</c> (as configured on the application registration's
        /// <c>appRoles</c> manifest entries, external to this repository — see the operator guide)
        /// to legacy permission name. Comparisons are case-insensitive,
        /// consistent with the legacy role lookup this replaces.
        /// </summary>
        public static readonly IReadOnlyDictionary<string, string> PermissionsByAppRoleValue =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["ClaimStatementGenerator.TriggerGeneration"] = "Claim Statement Generator: Trigger Generation",
                ["ClaimStatementGenerator.ViewClaimStatements"] = "Claim Statement Generator: View Claim Statements",
                ["ClaimStatementGenerator.ManageExceptions"] = "Claim Statement Generator: Manage Exceptions",
                ["ClaimStatementGenerator.ManageSuppressions"] = "Claim Statement Generator: Manage Suppressions",
                ["ClaimStatementGenerator.DownloadData"] = "Claim Statement Generator: Download Data",
                ["ClaimStatementGenerator.ManualUpload"] = "Claim Statement Generator: Manual Upload",
                ["ClaimStatementGenerator.ForceGeneration"] = "Claim Statement Generator: Force Generation",
            };

        /// <summary>
        /// Returns the legacy permission name for a known, validated app role value; returns
        /// <c>false</c> (and does not throw) for <c>null</c>, empty, or unrecognised values — an
        /// unknown app role value grants nothing (deny by default), it is never treated as a
        /// permission in its own right.
        /// </summary>
        public static bool TryGetPermission(string appRoleValue, out string permission)
        {
            if (string.IsNullOrEmpty(appRoleValue))
            {
                permission = null;
                return false;
            }

            return PermissionsByAppRoleValue.TryGetValue(appRoleValue, out permission);
        }
    }
}
