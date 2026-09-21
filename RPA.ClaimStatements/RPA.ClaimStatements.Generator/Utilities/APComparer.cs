using RPA.ClaimStatements.Generator.Models.Entities.DAX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Utilities
{
    public class APComparer : IEqualityComparer<AP>
    {
        public bool Equals(AP x, AP y)
        {
            return (x.Supplier == y.Supplier && x.Invoice == y.Invoice && x.LastSettlementDate == y.LastSettlementDate && x.Active == y.Active);
        }

        public int GetHashCode(AP x)
        {
            unchecked
            {
                int hash = 17;

                hash = hash * 23 + x.Supplier.GetHashCode();
                hash = hash * 23 + x.Invoice.GetHashCode();
                hash = hash * 23 + x.LastSettlementDate.GetHashCode();
                hash = hash * 23 + x.Active.GetHashCode();

                return hash;
            }
        }
    }
}
