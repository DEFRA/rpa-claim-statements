using System;

namespace RPA.ClaimStatements.Generator.Services
{
    // Run first by both applications: fails with a clear message, before any work starts, when a required
    // setting is missing, then applies migrations when the local developer opt-in is set.
    public static class StartupChecks
    {
        public static void EnsureReady()
        {
            ClaimStatementsConnectionString.Resolve();
            FolderService.ResolveRoot(Environment.GetEnvironmentVariable(FolderService.RootEnvironmentVariableName));
            SchemaMigrations.ApplyIfRequested();
        }
    }
}
