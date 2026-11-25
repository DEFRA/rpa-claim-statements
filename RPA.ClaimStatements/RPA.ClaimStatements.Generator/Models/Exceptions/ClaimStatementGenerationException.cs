using System;

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
