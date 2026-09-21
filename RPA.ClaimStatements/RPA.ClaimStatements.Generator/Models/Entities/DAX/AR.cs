using RPA.ClaimStatements.Generator.Models.Entities.CS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.Entities.DAX
{
    [Table("AR", Schema = "DAX")]
    public class AR
    {
        public Guid ARID { get; set; }               

        public Guid? LogID { get; set; }

        public string InvoiceNumber { get; set; }

        public Int64 H_InvoiceAccount { get; set; }

        public DateTime H_InvoiceDate { get; set; }

        public string H_CurrencyCode { get; set; }

        public string H_Fund { get; set; }

        public string H_Scheme { get; set; }

        public int H_MarketingYear { get; set; }

        public string H_DeliveryBody { get; set; }

        public int L_LineNumber { get; set; }

        public string L_LedgerAccount { get; set; }

        public string L_Fund { get; set; }

        public string L_Scheme { get; set; }

        public int L_MarketingYear { get; set; }

        public string L_DeliveryBody { get; set; }

        public decimal L_LineAmount { get; set; }

        public string Irregularity { get; set; }

        public string Admin { get; set; }

        public string OriginalClaimReference { get; set; }

        public DateTime? OriginalClaimSettlementDate { get; set; }

        public int OpenBalance { get; set; }        

        public virtual Log Log { get; set; }

        public AR()
        {
            ARID = Guid.NewGuid();            
        }

        public AR(long invoiceAccount, int marketingYear):this()
        {
            H_InvoiceAccount = invoiceAccount;
            H_MarketingYear = marketingYear;
        }

        public AR(long invoiceAccount, int marketingYear, string invoice):this(invoiceAccount, marketingYear)
        {
            InvoiceNumber = invoice;
        }

        public AR(long invoiceAccount, int marketingYear, string invoice, DateTime invoiceDate) : this(invoiceAccount, marketingYear, invoice)
        {
            H_InvoiceDate = invoiceDate;
        }
    }
}