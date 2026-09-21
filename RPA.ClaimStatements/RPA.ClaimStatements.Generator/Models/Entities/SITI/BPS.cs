using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.Entities.SITI
{
    [Table("BPS", Schema = "SITI")]
    public class BPS
    {
        public Guid BPSID { get; set; }

        public Guid ClaimID { get; set; }
        
        public decimal NonSDANumber { get; set; }        

        public decimal NonSDATotal { get; set; }

        public decimal SDANumber { get; set; }      

        public decimal SDATotal { get; set; }

        public decimal MoorlandNumber { get; set; }       

        public decimal MoorlandSDATotal { get; set; }

        public decimal AVGEntitlementValue { get; set; }

        public BPS()
        {
            BPSID = Guid.NewGuid();
        }
    }   
}