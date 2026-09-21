using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.Entities.SITI
{
    [Table("CLD", Schema = "SITI")]
    public class CLD
    {
        public Guid CLDID { get; set; }

        public Guid ClaimID { get; set; }

        public decimal NonSDAAreaOnApplication { get; set; }

        public decimal NonSDAAreaEligible { get; set; }

        public decimal NonSDAEntitlements { get; set; }

        public decimal SDAAreaOnApplication { get; set; }

        public decimal SDAAreaEligible { get; set; }

        public decimal SDAEntitlements { get; set; }

        public decimal MoorlandAreaOnApplication { get; set; }

        public decimal MoorlandAreaEligible { get; set; }

        public decimal MoorlandEntitlements { get; set; }

        public CLD()
        {
            CLDID = Guid.NewGuid();
        }
    }
}