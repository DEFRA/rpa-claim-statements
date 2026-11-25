using System.Collections.Generic;

namespace RPA.ClaimStatements.Generator.Components.PumpingStation.Maps
{
    public static class BPSSQLMap
    {
        public static List<string> ColumnMapping()
        {
            var mappings = new List<string>();

            mappings.Add("BPSID,BPSID");
            mappings.Add("ClaimID,ClaimID");
            mappings.Add("NonSDANumber,NonSDANumber");
            mappings.Add("NonSDATotal,NonSDATotal");
            mappings.Add("SDANumber,SDANumber");
            mappings.Add("SDATotal,SDATotal");
            mappings.Add("MoorlandNumber,MoorlandNumber");
            mappings.Add("MoorlandSDATotal,MoorlandSDATotal");
            mappings.Add("AVGEntitlementValue,AVGEntitlementValue");

            return mappings;
        }
    }
}
