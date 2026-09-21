using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Components.PumpingStation.Maps
{
    public static class SUM2SQLMap
    {
        public static List<string> ColumnMapping()
        {
            var mappings = new List<string>();

            mappings.Add("SUM2ID,SUM2ID");
            mappings.Add("ClaimID,ClaimID");
            mappings.Add("BPSValue,BPSValue");
            mappings.Add("GreeningValue,GreeningValue");
            mappings.Add("YoungFarmerValue,YoungFarmerValue");
            mappings.Add("SubTotal,SubTotal");
            mappings.Add("CrossCompliancePercent,CrossCompliancePercent");
            mappings.Add("CrossComplianceReduction,CrossComplianceReduction");
            mappings.Add("TotalClaimEuro,TotalClaimEuro");

            return mappings;
        }
    }
}
