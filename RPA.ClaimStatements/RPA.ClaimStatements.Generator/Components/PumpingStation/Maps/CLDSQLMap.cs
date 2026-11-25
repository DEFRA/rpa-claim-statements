using System.Collections.Generic;

namespace RPA.ClaimStatements.Generator.Components.PumpingStation.Maps
{
    public static class CLDSQLMap
    {
        public static List<string> ColumnMapping()
        {
            var mappings = new List<string>();

            mappings.Add("CLDID,CLDID");
            mappings.Add("ClaimID,ClaimID");
            mappings.Add("NonSDAAreaOnApplication,NonSDAAreaOnApplication");
            mappings.Add("NonSDAAreaEligible,NonSDAAreaEligible");
            mappings.Add("NonSDAEntitlements,NonSDAEntitlements");
            mappings.Add("SDAAreaOnApplication,SDAAreaOnApplication");
            mappings.Add("SDAAreaEligible,SDAAreaEligible");
            mappings.Add("SDAEntitlements,SDAEntitlements");
            mappings.Add("MoorlandAreaOnApplication,MoorlandAreaOnApplication");
            mappings.Add("MoorlandAreaEligible,MoorlandAreaEligible");
            mappings.Add("MoorlandEntitlements,MoorlandEntitlements");

            return mappings;
        }
    }
}
