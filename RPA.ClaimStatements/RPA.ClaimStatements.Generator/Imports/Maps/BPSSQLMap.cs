using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Imports.Maps
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
