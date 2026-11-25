using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPA.ClaimStatements.Generator.Models.Entities.CS
{
    [Table("ConversionRates", Schema="CS")]
    public class ConversionRate
    {
        public Guid ConversionRateID { get; set; }

        public string Description { get; set; }

        public decimal Rate { get; set; }

        public int SchemeYear { get; set; }

        public ConversionRate()
        {
            ConversionRateID = Guid.NewGuid();
        }

        public ConversionRate(string description, decimal rate, int schemeYear):this()
        {
            Description = description;
            Rate = rate;
            SchemeYear = schemeYear;
        }
    }
}