using RPA.ClaimStatements.Generator.Context;
using Microsoft.EntityFrameworkCore;
using System;

namespace RPA.ClaimStatements.Generator.Services
{
    // Deployed environments never migrate at startup: the pipeline applies the EF migration bundle with a
    // privileged identity. Developers can opt in locally so a fresh local database is created and updated.
    public static class SchemaMigrations
    {
        public const string ApplyVariableName = "CLAIMSTATEMENTS_APPLY_MIGRATIONS";

        public static void ApplyIfRequested()
        {
            if (!string.Equals(Environment.GetEnvironmentVariable(ApplyVariableName), "true", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            using (var context = new ClaimStatementsContext())
            {
                context.Database.Migrate();
            }
        }
    }
}
