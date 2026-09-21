using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Components.PumpingStation.Maps
{
    public static class YFSQLMap
    {
        public static List<string> ColumnMapping()
        {
            var mappings = new List<string>();

            mappings.Add("YFID,YFID");
            mappings.Add("ClaimID,ClaimID");
            mappings.Add("NonSDANumber,NonSDANumber");
            mappings.Add("SDANumber,SDANumber");
            mappings.Add("MoorlandNumber,MoorlandNumber");
            mappings.Add("TotalEntitlementValue,TotalEntitlementValue");
            mappings.Add("EntitlementsUsedToClaim,EntitlementsUsedToClaim");
            mappings.Add("AvgEntitlementValue,AvgEntitlementValue");
            mappings.Add("OverDeclarationArea,OverDeclarationArea");
            mappings.Add("OverDeclarationReduction,OverDeclarationReduction");
            mappings.Add("LateClaimSubmissionPercent,LateClaimSubmissionPercent");
            mappings.Add("LateClaimSubmissionReduction,LateClaimSubmissionReduction");
            mappings.Add("LateEvidencePercent,LateEvidencePercent");
            mappings.Add("LateEvidenceReduction,LateEvidenceReduction");
            mappings.Add("LateChangePenaltyReduction,LateChangePenaltyReduction");
            mappings.Add("NonDeclarationPercent,NonDeclarationPercent");
            mappings.Add("NonDeclarationReduction,NonDeclarationReduction");
            mappings.Add("TotalYF,TotalYF");
            mappings.Add("FDMPercent,FDMPercent");
            mappings.Add("FDMReduction,FDMReduction");
            mappings.Add("OverDeclarationPercent,OverDeclarationPercent");
            mappings.Add("AdditionalOverDeclarationReduction,AdditionalOverDeclarationReduction");
            mappings.Add("PreviousYearOverDeclaration,PreviousYearOverDeclaration");

            return mappings;
        }
    }
}
