using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Models.Entities.SITI
{ 
        [Table("PR", Schema = "SITI")]
        public class PR
        {

            public Guid PRID { get; set; }

            public Guid ClaimID { get; set; }

            public decimal PRB1MaxBand { get; set; }

            public decimal PRB1Percent { get; set; }

            public decimal PRB1Result { get; set; }

            public decimal PRB2MaxBand { get; set; }

            public decimal PRB2Percent { get; set; }

            public decimal PRB2Result { get; set; }

            public decimal PRB3MaxBand { get; set; }

            public decimal PRB3Percent { get; set; }

            public decimal PRB3Result { get; set; }

            public decimal PRB4MaxBand { get; set; }

            public decimal PRB4Percent { get; set; }

            public decimal PRB4Result { get; set; }

            public decimal PRBTotalResult { get; set; }
            public PR()
            {
                PRID = Guid.NewGuid();
            }

    }
}
