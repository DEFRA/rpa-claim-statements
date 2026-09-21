using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Services
{
    public interface ILogService
    {
        void Log(long frn, int sbi, int schemeYear, string filePath, Guid claimId);
    }
}
