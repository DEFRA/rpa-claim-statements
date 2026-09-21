using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Imports.Maps
{
    public static class APSQLMap
    {
        public static List<string> ColumnMapping()
        {
            var mappings = new List<string>();

            mappings.Add("APID,APID");
            mappings.Add("LastSettlementDate,LastSettlementDate");
            mappings.Add("Supplier,Supplier");
            mappings.Add("SupplierName,SupplierName");
            mappings.Add("Invoice,Invoice");
            mappings.Add("LastPaymentAmount,LastPaymentAmount");
            mappings.Add("DocumentDate,DocumentDate");
            mappings.Add("InvoiceDate,InvoiceDate");
            mappings.Add("InvoiceDueDate,InvoiceDueDate");
            mappings.Add("Scheme,Scheme");
            mappings.Add("MarketingYear,MarketingYear");
            mappings.Add("DeliveryBody,DeliveryBody");
            mappings.Add("TransactionCurrency,TransactionCurrency");
            mappings.Add("TransactionInvoiceValue,TransactionInvoiceValue");
            mappings.Add("LastHoldReleaseDate,LastHoldReleaseDate");
            mappings.Add("FESReference,FESReference");
            mappings.Add("PaymentMethod,PaymentMethod");
            mappings.Add("Active,Active");

            return mappings;
        }
    }
}
