using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Components.PumpingStation.Maps
{
    public static class PRSQLMap
    {
        public static List<string> ColumnMapping()
        {
            var mappings = new List<string>();

            mappings.Add("PRID,PRID");
            mappings.Add("ClaimID,ClaimID");
            mappings.Add("PRB1MaxBand,PRB1MaxBand");
            mappings.Add("PRB1Percent,PRB1Percent");
            mappings.Add("PRB1Result,PRB1Result");
            mappings.Add("PRB2MaxBand,PRB2MaxBand");
            mappings.Add("PRB2Percent,PRB2Percent");
            mappings.Add("PRB2Result,PRB2Result");
            mappings.Add("PRB3MaxBand,PRB3MaxBand");
            mappings.Add("PRB3Percent,PRB3Percent");
            mappings.Add("PRB3Result,PRB3Result");
            mappings.Add("PRB4MaxBand,PRB4MaxBand");
            mappings.Add("PRB4Percent,PRB4Percent");
            mappings.Add("PRB4Result,PRB4Result");
            mappings.Add("PRBTotalResult,PRBTotalResult");

            return mappings;
        }
    }
}
