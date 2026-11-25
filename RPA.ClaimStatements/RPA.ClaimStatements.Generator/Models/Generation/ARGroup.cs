using System;

namespace RPA.ClaimStatements.Generator.Models.Generation
{
    public class ARGroup
    {
        public DateTime Date { get; set; }

        public string InvoiceNumber { get; set; }

        public decimal TransactionValue { get; set; }

        public string Currency { get; set; }
    }
}