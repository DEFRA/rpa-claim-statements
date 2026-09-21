using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.Entities.SITI
{
    [Table("SUM2", Schema = "SITI")]
    public class SUM2
    {
        public Guid SUM2ID { get; set; }

        public Guid ClaimID { get; set; }

        public decimal BPSValue { get; set; }    

        public decimal GreeningValue { get; set; }                

        public decimal YoungFarmerValue { get; set; }        

        public decimal SubTotal { get; set; }

        public decimal CrossCompliancePercent { get; set; }

        public decimal CrossComplianceReduction { get; set; }        

        public decimal TotalClaimEuro { get; set; }

        public SUM2()
        {
            SUM2ID = Guid.NewGuid();
        }

        public SUM2(Guid claimId, decimal totalClaimEuro):this()
        {
            ClaimID = claimId;
            TotalClaimEuro = totalClaimEuro;
        }
    }
}