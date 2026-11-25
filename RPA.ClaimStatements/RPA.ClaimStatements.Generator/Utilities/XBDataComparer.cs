using RPA.ClaimStatements.Generator.Models.Entities.XB;
using System.Collections.Generic;

namespace RPA.ClaimStatements.Generator.Utilities
{
    public class XBDataComparer : IEqualityComparer<XBData>
    {
        public bool Equals(XBData x, XBData y)
        {
            return (x.FRN == y.FRN && x.InvoiceNumber == y.InvoiceNumber && x.CalculationDate == y.CalculationDate && x.SchemeYear == y.SchemeYear);
        }

        public int GetHashCode(XBData x)
        {
            unchecked
            {
                int hash = 17;

                hash = hash * 23 + x.FRN.GetHashCode();
                hash = hash * 23 + x.InvoiceNumber.GetHashCode();
                hash = hash * 23 + x.CalculationDate.GetHashCode();
                hash = hash * 23 + x.SchemeYear.GetHashCode();

                return hash;
            }
        }
    }
}
