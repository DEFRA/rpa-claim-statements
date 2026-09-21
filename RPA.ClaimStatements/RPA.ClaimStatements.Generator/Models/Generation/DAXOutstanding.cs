using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.Generation
{
    public class DAXOutstanding
    {
        public string Invoice { get; set; }

        public Int64 FRN { get; set; }

        public int MarketingYear { get; set; }

        public DateTime SettlementDate { get; set; }

        public DAXOutstanding() { }

        public DAXOutstanding(string invoice, Int64 frn, int marketingYear, DateTime settlementDate):this()
        {
            Invoice = invoice;
            FRN = frn;
            MarketingYear = marketingYear;
            SettlementDate = settlementDate;
        }
    }
}