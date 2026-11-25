using RPA.ClaimStatements.Generator.Models.Entities.SITI;
using System.Collections.Generic;

namespace RPA.ClaimStatements.Generator.Utilities
{
    public class SITIComparer : IEqualityComparer<Claim>
    {
        public bool Equals(Claim x, Claim y)
        {
            return (x.SUM?.FRN == y.SUM?.FRN && x.SUM?.InvoiceNumber == y.SUM?.InvoiceNumber && x.SUM?.SchemeYear == y.SUM?.SchemeYear);
        }

        public int GetHashCode(Claim x)
        {
            unchecked
            {
                int hash = 17;

                hash = hash * 23 + (x.SUM?.FRN).GetHashCode();
                hash = hash * 23 + (x.SUM?.InvoiceNumber).GetHashCode();
                hash = hash * 23 + (x.SUM?.SchemeYear).GetHashCode();

                return hash;
            }
        }
    }
}
