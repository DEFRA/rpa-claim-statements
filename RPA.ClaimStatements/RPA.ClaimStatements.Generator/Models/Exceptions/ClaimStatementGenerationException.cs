using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Models.Exceptions
{
    [Serializable]
    public class ClaimStatementGenerationException : Exception
    {
        public long FRN { get; }
        public int SchemeYear { get; }

        public ClaimStatementGenerationException(string message, long frn, int schemeYear) : base(message)
        {
            FRN = frn;
            SchemeYear = schemeYear;
        }
    }
}
