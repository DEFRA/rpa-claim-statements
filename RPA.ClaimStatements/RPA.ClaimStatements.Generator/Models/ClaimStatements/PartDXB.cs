using RPA.ClaimStatements.Generator.Models.Entities.XB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Models.ClaimStatements
{
    public class PartDXB : PartD
    {
        public decimal WalesAreaOnApplication { get; set; }

        public decimal WalesAreaIneligible { get; set; }

        public decimal WalesEntitlements { get; set; }

        public decimal ScotlandRegion1AreaOnApplication { get; set; }

        public decimal ScotlandRegion1AreaIneligible { get; set; }

        public decimal ScotlandRegion1Entitlements { get; set; }

        public decimal ScotlandRegion2AreaOnApplication { get; set; }

        public decimal ScotlandRegion2AreaIneligible { get; set; }

        public decimal ScotlandRegion2Entitlements { get; set; }

        public decimal ScotlandRegion3AreaOnApplication { get; set; }

        public decimal ScotlandRegion3AreaIneligible { get; set; }

        public decimal ScotlandRegion3Entitlements { get; set; }

        public decimal NIAreaOnApplication { get; set; }

        public decimal NIAreaIneligible { get; set; }

        public decimal NIEntitlements { get; set; }

        public void Build(XBData xbData)
        {
            NonSDAAreaIneligible = xbData.EnglandNonSDAAreaIneligible;
            NonSDAEntitlements = xbData.EnglandNonSDAEntitlements;
            SDAAreaOnApplication = xbData.EnglandSDAAreaOnApplication;
            NonSDAAreaOnApplication = xbData.EnglandNonSDAAreaOnApplication;
            SDAAreaIneligible = xbData.EnglandSDAAreaIneligible;
            SDAEntitlements = xbData.EnglandSDAEntitlements;
            MoorlandAreaIneligible = xbData.EnglandMoorlandAreaIneligible;
            MoorlandEntitlements = xbData.EnglandMoorlandEntitlements;
            WalesAreaOnApplication = xbData.WalesAreaOnApplication;
            WalesAreaIneligible = xbData.WalesAreaIneligible;
            WalesEntitlements = xbData.WalesEntitlements;
            ScotlandRegion1AreaOnApplication = xbData.ScotlandRegion1AreaOnApplication;
            ScotlandRegion1AreaIneligible = xbData.ScotlandRegion1AreaIneligible;
            ScotlandRegion1Entitlements = xbData.ScotlandRegion1Entitlements;
            ScotlandRegion2AreaOnApplication = xbData.ScotlandRegion2AreaOnApplication;
            ScotlandRegion2AreaIneligible = xbData.ScotlandRegion2AreaIneligible;
            ScotlandRegion2Entitlements = xbData.ScotlandRegion2Entitlements;
            ScotlandRegion3AreaOnApplication = xbData.ScotlandRegion3AreaOnApplication;
            ScotlandRegion3AreaIneligible = xbData.ScotlandRegion3AreaIneligible;
            ScotlandRegion3Entitlements = xbData.ScotlandRegion3Entitlements;
            NIAreaOnApplication = xbData.NIAreaOnApplication;
            NIAreaIneligible = xbData.NIAreaIneligible;
            NIEntitlements = xbData.NIEntitlements;
        }
    }
}
