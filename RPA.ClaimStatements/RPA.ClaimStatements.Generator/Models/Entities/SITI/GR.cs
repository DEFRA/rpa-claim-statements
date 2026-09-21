using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.Entities.SITI
{
    [Table("GR", Schema = "SITI")]
    public class GR
    {
        public Guid GRID { get; set; }

        public Guid ClaimID { get; set; }                        

        public decimal NonSDANumber { get; set; }       

        public decimal NonSDATotal { get; set; }

        public decimal SDANumber { get; set; }     

        public decimal SDATotal { get; set; }

        public decimal MoorlandNumber { get; set; }       

        public decimal MoorlandTotal { get; set; }

        public decimal AvgGreeningRate { get; set; }

        public GR()
        {
            GRID = Guid.NewGuid();
        }
    }
}