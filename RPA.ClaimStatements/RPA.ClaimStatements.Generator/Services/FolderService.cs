using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Context;
namespace RPA.ClaimStatements.Generator.Services
{
    public class FolderService : IFolderService, IDisposable
    {
        IClaimStatementsContext db;

        // Absolute path of the folder that holds the working subfolders (Templates, XML, DOCX, HTML, PDF, Uploads).
        // Required: there is no fallback.
        public const string RootEnvironmentVariableName = "CLAIMSTATEMENTS_ROOT";

        public FolderService(IClaimStatementsContext context)
        {
            this.db = context;
        }

        public void CheckCreateWorking()
        {
            Directory.CreateDirectory(Path.Combine(Root(), "XML"));
            Directory.CreateDirectory(Path.Combine(Root(), "DOCX"));
            Directory.CreateDirectory(Path.Combine(Root(), "HTML"));
            Directory.CreateDirectory(Path.Combine(Root(), "PDF"));
            Directory.CreateDirectory(Path.Combine(Root(), "Templates"));
        }

        public Folder Get(string name)
        {
            return db.Folders.AsNoTracking().Where(x => x.Description == name).FirstOrDefault();
        }

        public string GetPath(string name)
        {
            return db.Folders.AsNoTracking().Where(x => x.Description == name).Select(x => x.Path).FirstOrDefault();            
        }

        public string Root()
        {
            return ResolveRoot(Environment.GetEnvironmentVariable(RootEnvironmentVariableName));
        }

        // Pure resolver kept separate from Root() so it can be unit tested without setting a
        // process-wide environment variable.
        public static string ResolveRoot(string configuredRoot)
        {
            if (string.IsNullOrWhiteSpace(configuredRoot))
            {
                throw new InvalidOperationException(string.Format(
                    "{0} is not set or is blank. Set it to the absolute path of the folder that holds the working folders.",
                    RootEnvironmentVariableName));
            }

            string trimmed = configuredRoot.Trim();

            // Path.IsPathFullyQualified rejects drive-relative/rooted-but-relative paths (e.g. "/data"
            // on Windows, "C:data" anywhere) as well as plain relative paths, so a misconfigured value
            // fails visibly instead of being silently resolved against an unpredictable current
            // working directory.
            if (!Path.IsPathFullyQualified(trimmed))
            {
                throw new InvalidOperationException(string.Format(
                    "{0} must be an absolute path for the current OS; '{1}' is not. Relative or drive-relative paths are rejected rather than silently resolved.",
                    RootEnvironmentVariableName, configuredRoot));
            }

            // Normalises separators and a trailing separator (cross-platform; root paths such as
            // "C:\" or "/" are left intact) without touching the filesystem.
            return Path.GetFullPath(Path.TrimEndingDirectorySeparator(trimmed));
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    db.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
