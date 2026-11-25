using System;

namespace RPA.ClaimStatements.Generator.Components.Turbine
{
    public interface ILogService
    {
        void Log(long frn, int sbi, int schemeYear, string filePath, Guid claimId);
    }
}
