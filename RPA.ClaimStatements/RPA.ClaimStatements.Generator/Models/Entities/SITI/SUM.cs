using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.Entities.SITI
{
    [Table("SUM", Schema = "SITI")]
    public class SUM
    {        
        public Guid SUMID { get; set; }

        public Guid ClaimID { get; set; }

        public int SchemeYear { get; set; }

        public string ApplicationID { get; set; }

        public DateTime CalculationDate { get; set; }

        public string InvoiceNumber { get; set; }

        public string CalculationRefNumber { get; set; }        

        public string BusinessName { get; set; }

        public int SBI { get; set; }

        public Int64 FRN { get; set; }        

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string AddressLine3 { get; set; }

        public string PostCode { get; set; }

        public SUM()
        {
            SUMID = Guid.NewGuid();
        }

        public SUM(Guid claimId, long frn, int schemeYear, string invoice, DateTime calculationDate):this()
        {
            ClaimID = claimId;
            FRN = frn;
            SchemeYear = schemeYear;
            InvoiceNumber = invoice;
            CalculationDate = calculationDate;
        }
    }
}