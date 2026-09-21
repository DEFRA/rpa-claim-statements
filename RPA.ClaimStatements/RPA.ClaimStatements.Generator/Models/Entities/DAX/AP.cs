using RPA.ClaimStatements.Generator.Models.Entities.CS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.Entities.DAX
{
    [Table("AP", Schema = "DAX")]
    public class AP
    {
        public Guid APID { get; set; }        

        public Guid? LogID { get; set; }       

        public DateTime LastSettlementDate { get; set; }

        public Int64 Supplier { get; set; }

        public string SupplierName { get; set; }

        public string Invoice { get; set; }

        public decimal LastPaymentAmount { get; set; }

        public DateTime? DocumentDate { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public DateTime? InvoiceDueDate { get; set; }

        public string Scheme { get; set; }

        public int MarketingYear { get; set; }

        public string DeliveryBody { get; set; }

        public string TransactionCurrency { get; set; }

        public decimal TransactionInvoiceValue { get; set; }

        public DateTime? LastHoldReleaseDate { get; set; }

        public string FESReference { get; set; }

        public string PaymentMethod { get; set; }

        public bool Active { get; set; }

        public virtual Log Log { get; set; }

        public void Inactivate()
        {
            Active = false;
        }

        public AP()
        {
            APID = Guid.NewGuid();
            Active = true;
        }

        public AP(long supplier, int marketingYear):this()
        {
            Supplier = supplier;
            MarketingYear = marketingYear;
        }

        public AP(long supplier, int marketingYear, string invoice):this(supplier, marketingYear)
        {
            Invoice = invoice;
        }

        public AP(long supplier, int marketingYear, string invoice, DateTime lastSettlementDate) : this(supplier, marketingYear, invoice)
        {
            LastSettlementDate = lastSettlementDate;
        }
    }
}