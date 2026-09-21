using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Models.ClaimStatements
{
    public class PartBXB : PartB
    {
        public WalesBPSPayment WalesBPSPayment { get; set; }

        public WalesGreeningPayment WalesGreeningPayment { get; set; }

        public WalesYoungFarmerPayment WalesYoungFarmerPayment { get; set; }

        public WalesRedistributivePayment WalesRedistributivePayment { get; set; }

        public ScotlandBPSPayment ScotlandBPSPayment { get; set; }

        public ScotlandGreeningPayment ScotlandGreeningPayment { get; set; }

        public ScotlandYoungFarmerPayment ScotlandYoungFarmerPayment { get; set; }

        public NIBPSPayment NIBPSPayment { get; set; }

        public NIGreeningPayment NIGreeningPayment { get; set; }

        public NIYoungFarmerPayment NIYoungFarmerPayment { get; set; }
    }
}
