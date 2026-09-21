using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Components.PumpingStation.Maps
{
    public static class ClaimSQLMap
    {
        public static List<string> ColumnMapping()
        {
            var mappings = new List<string>();

            mappings.Add("ClaimID,ClaimID");
            mappings.Add("SUMID,SUMID");
            mappings.Add("SUM2ID,SUM2ID");
            mappings.Add("BPSID,BPSID");
            mappings.Add("BPSPENID,BPSPENID");
            mappings.Add("GRID,GRID");
            mappings.Add("GRPENID,GRPENID");
            mappings.Add("YFID,YFID");
            mappings.Add("CLDID,CLDID");
            mappings.Add("PRID,PRID");

            return mappings;
        }
    }
}
