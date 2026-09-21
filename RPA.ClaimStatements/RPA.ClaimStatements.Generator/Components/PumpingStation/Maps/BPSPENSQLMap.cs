using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Components.PumpingStation.Maps
{
    public static class BPSPENSQLMap
    {
        public static List<string> ColumnMapping()
        {
            var mappings = new List<string>();

            mappings.Add("BPSPENID,BPSPENID");
            mappings.Add("ClaimID,ClaimID");
            mappings.Add("OverDeclarationHectares,OverDeclarationHectares");
            mappings.Add("OverDeclarationReduction,OverDeclarationReduction");
            mappings.Add("LateClaimSubmissionPercent,LateClaimSubmissionPercent");
            mappings.Add("LateClaimSubmissionReduction,LateClaimSubmissionReduction");
            mappings.Add("LateEntitlementsApplicationPercent,LateEntitlementsApplicationPercent");
            mappings.Add("LateEntitlementsApplicationReduction,LateEntitlementsApplicationReduction");
            mappings.Add("LateEvidencePercent,LateEvidencePercent");
            mappings.Add("LateEvidenceReduction,LateEvidenceReduction");
            mappings.Add("LateChangePenaltyReduction,LateChangePenaltyReduction");
            mappings.Add("LateEntitlementsAmendmentPercent,LateEntitlementsAmendmentPercent");
            mappings.Add("LateEntitlementsAmendmentReduction,LateEntitlementsAmendmentReduction");
            mappings.Add("NonDeclarationPercent,NonDeclarationPercent");
            mappings.Add("NonDeclarationReduction,NonDeclarationReduction");
            mappings.Add("FDMPercent,FDMPercent");
            mappings.Add("FDMReduction,FDMReduction");
            mappings.Add("ReductionOfPaymentsOver150kPercent,ReductionOfPaymentsOver150kPercent");
            mappings.Add("ReductionOfPaymentsOver150kReduction,ReductionOfPaymentsOver150kReduction");
            mappings.Add("TotalBPS,TotalBPS");
            mappings.Add("OverDeclarationPercent,OverDeclarationPercent");
            mappings.Add("AdditionalOverDeclarationReduction,AdditionalOverDeclarationReduction");
            mappings.Add("PreviousYearOverDeclaration,PreviousYearOverDeclaration");

            return mappings;
        }
    }
}
