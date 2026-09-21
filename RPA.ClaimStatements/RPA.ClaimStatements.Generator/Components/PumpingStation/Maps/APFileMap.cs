using CsvHelper.Configuration;
using RPA.ClaimStatements.Generator.Models.Entities.DAX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Components.PumpingStation.Maps
{
    public sealed class APFileMap : CsvClassMap<AP>
    {
        public APFileMap()
        {
            Map(x => x.LastSettlementDate).Name("Last settlement date");
            Map(x => x.Supplier).Name("Supplier #");
            Map(x => x.SupplierName).Name("Supplier name");
            Map(x => x.Invoice).Name("Invoice #");
            Map(x => x.LastPaymentAmount).Name("Last payment amount");
            Map(x => x.DocumentDate).Name("Document date");
            Map(x => x.InvoiceDate).Name("Invoice date");
            Map(x => x.InvoiceDueDate).Name("Invoice due date");
            Map(x => x.Scheme).Name("Scheme");
            Map(x => x.MarketingYear).Name("Mkg Yr");
            Map(x => x.DeliveryBody).Name("Del Body");
            Map(x => x.TransactionCurrency).Name("Transaction currency");
            Map(x => x.TransactionInvoiceValue).Name("Transaction invoice value");
            Map(x => x.LastHoldReleaseDate).Name("Last hold release date");
            Map(x => x.FESReference).Name("FES reference");
            Map(x => x.PaymentMethod).Name("Payment method (on settlement)");
        }
    }
}
