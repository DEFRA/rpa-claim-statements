using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Models.ClaimStatements
{
    public class Invoice
    {
        public string Date { get; set; }

        public DateTime DateDate { get; set; }

        public string Reference { get; set; }

        public decimal ClaimValueEuro { get; set; }

        public decimal ClaimValueSterling { get; set; }

        public string TransactionType { get; set; }

        public decimal TransactionValue { get; set; }

        public string Currency { get; set; }

        public Invoice() { }

        public Invoice(string date, DateTime datedate, string reference, string transactionType, decimal transactionValue, string transactionCurrency) : this()
        {
            Date = date;
            DateDate = datedate;
            Reference = reference;
            TransactionType = transactionType;
            TransactionValue = transactionValue;

            if (transactionCurrency != "EUR")
            {
                Currency = "Sterling";
            }
            else
            {
                Currency = "Euros";
            }
        }
    }
}
