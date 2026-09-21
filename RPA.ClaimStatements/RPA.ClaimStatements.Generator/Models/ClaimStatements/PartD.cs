using RPA.ClaimStatements.Generator.Models.Entities.SITI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Models.ClaimStatements
{
    public class PartD
    {
        public decimal NonSDAAreaOnApplication { get; set; }

        public decimal NonSDAAreaIneligible { get; set; }

        public decimal NonSDAEntitlements { get; set; }

        public decimal SDAAreaOnApplication { get; set; }

        public decimal SDAAreaIneligible { get; set; }

        public decimal SDAEntitlements { get; set; }

        public decimal MoorlandAreaOnApplication { get; set; }

        public decimal MoorlandAreaIneligible { get; set; }

        public decimal MoorlandEntitlements { get; set; }

        public void Build(CLD cld)
        {
            NonSDAAreaOnApplication = cld.NonSDAAreaOnApplication * 0.0001M;
            NonSDAAreaIneligible = cld.NonSDAAreaEligible * 0.0001M;
            NonSDAEntitlements = cld.NonSDAEntitlements * 0.0001M;
            SDAAreaOnApplication = cld.SDAAreaOnApplication * 0.0001M;
            SDAAreaIneligible = cld.SDAAreaEligible * 0.0001M;
            SDAEntitlements = cld.SDAEntitlements * 0.0001M;
            MoorlandAreaOnApplication = cld.MoorlandAreaOnApplication * 0.0001M;
            MoorlandAreaIneligible = cld.MoorlandAreaEligible * 0.0001M;
            MoorlandEntitlements = cld.MoorlandEntitlements * 0.0001M;
        }
    }
}
