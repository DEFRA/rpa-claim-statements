using RPA.ClaimStatements.Generator.Models.Entities.SITI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Models.ClaimStatements
{
    public class ProgressiveReductionsReduction
    {
        public decimal PRB1MaxBand { get; set; }

        public decimal PRB1Percent { get; set; }

        public decimal PRB1Result { get; set; }

        public decimal PRB1Amount { get; set; }

        public decimal PRB2MaxBand { get; set; }

        public decimal PRB2Percent { get; set; }

        public decimal PRB2Result { get; set; }

        public decimal PRB2Amount { get; set; }

        public decimal PRB3MaxBand { get; set; }

        public decimal PRB3Percent { get; set; }

        public decimal PRB3Result { get; set; }

        public decimal PRB3Amount { get; set; }

        public decimal PRB4MaxBand { get; set; }

        public decimal PRB4Percent { get; set; }

        public decimal PRB4Amount { get; set; }

        public decimal PRB4Result { get; set; }

        public decimal PRBTotalResult { get; set; }

        public void Build(SUM2 sum2, PR pr)
        {
            PRB1Percent = pr.PRB1Percent;
            PRB1Result = pr.PRB1Result;
            PRB1MaxBand = pr.PRB1MaxBand;
            PRB1Amount = (pr.PRB1Result / PRB1Percent) * 100;
            PRB2Percent = pr.PRB2Percent;
            PRB2Result = pr.PRB2Result;
            PRB2MaxBand = pr.PRB2MaxBand;
            PRB2Amount = (pr.PRB2Result / PRB2Percent) * 100;
            PRB3Percent = pr.PRB3Percent;
            PRB3Result = pr.PRB3Result;
            PRB3MaxBand = pr.PRB3MaxBand;
            PRB3Amount = (pr.PRB3Result / PRB3Percent) * 100;
            PRB4Percent = pr.PRB4Percent;
            PRB4Result = pr.PRB4Result;
            PRB4MaxBand = pr.PRB4MaxBand;
            PRB4Amount = (pr.PRB4Result / PRB4Percent) * 100;
            PRBTotalResult = pr.PRBTotalResult;

        }

        }
}
