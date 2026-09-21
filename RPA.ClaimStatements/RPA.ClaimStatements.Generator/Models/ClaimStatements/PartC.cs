using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Models.ClaimStatements
{
    public class PartC
    {
        protected ICurrencyService currencyService; 
        public List<Invoice> Invoices { get; set; }

        public decimal TotalPayments { get; set; }

        public PartC()
        {
            this.currencyService = new CurrencyService();
        }

        public PartC(ICurrencyService currencyService)
        {
            this.currencyService = currencyService;
        }

        public void Build(List<Invoice> invoices)
        {
            foreach (Invoice invoice in invoices)
            {
                if (invoice.Currency == "Sterling")
                {
                    bool tolerance = currencyService.CheckTolerance(invoice.ClaimValueSterling, invoice.TransactionValue);

                    if(tolerance)
                    {
                        invoice.ClaimValueSterling = invoice.TransactionValue;
                    }
                }
            }

            invoices = invoices.OrderBy(x => x.DateDate).ToList();

            Invoices = invoices;

            TotalPayments = invoices.Sum(x => x.TransactionValue);
        }
    }

    
}
