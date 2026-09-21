using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPA.ClaimStatements.Generator.Models.Entities.XB;
using RPA.ClaimStatements.Generator.Models.Generation;
using RPA.ClaimStatements.Generator.Services;

namespace RPA.ClaimStatements.Generator.Models.ClaimStatements
{
    public class ScotlandYoungFarmerPayment:YoungFarmerPayment
    {
        IYCPService ycpService;

        public ScotlandYoungFarmerPayment()
        {
            ycpService = new YCPService();
        }

        public ScotlandYoungFarmerPayment(IYCPService ycpService)
        {
            this.ycpService = ycpService;
        }

        public decimal YFDeclarationArea { get; set; }

        public decimal YFDeclarationReduction { get; set; }

        public decimal YFDeclarationTotal { get; set; }        

        public decimal LateAmendmentPercent { get; set; }

        public decimal LateAmendmentReduction { get; set; }

        public decimal LateAmendmentTotal { get; set; }

        public override void Build(XBData xbData)
        {
            EntitlementsUsedToClaim = xbData.ScotlandYFEntitlementsUsedToClaim;
            AvgEntitlementValue = xbData.ScotlandYFAvgEntitlementValue;
            YFClaimValuePercent = xbData.ScotlandYFClaimValuePercent;
            YFClaimValue = xbData.ScotlandYFClaimValue;
            OverDeclarationArea = xbData.ScotlandYFOverDeclarationArea;
            OverDeclarationPercent = xbData.ScotlandYFOverDeclarationPercent;
            OverDeclarationReduction = xbData.ScotlandYFOverDeclarationReduction - xbData.ScotlandBPSAdditionalOverDeclarationReduction;
            AdditionalOverDeclarationReduction = xbData.ScotlandBPSAdditionalOverDeclarationReduction;
            LateClaimSubmissionPercent = xbData.ScotlandYFLateClaimSubmissionPercent;
            LateClaimSubmissionReduction = xbData.ScotlandYFLateClaimSubmissionReduction;
            LateAmendmentPercent = xbData.ScotlandYFLateAmendmentPercent;
            LateAmendmentReduction = xbData.ScotlandYFLateAmendmentReduction;
            LateEvidencePercent = xbData.ScotlandYFLateEvidencePercent;
            LateEvidenceReduction = xbData.ScotlandYFLateEvidenceReduction;
            NonDeclarationPercent = xbData.ScotlandYFNonDeclarationPercent;
            NonDeclarationReduction = xbData.ScotlandYFNonDeclarationReduction;
            LateChangePenaltyReduction = xbData.ScotlandYFLateAmendmentReduction;
            FDMPercent = xbData.ScotlandYFFDMPercent;
            FDMReduction = xbData.ScotlandYFFDMReduction;
            TotalYoungFarmerPayment = xbData.ScotlandYFTotalYoungFarmerPayment;

            if (OverDeclarationReduction > 0)
            {
                YCPStatus = YCPStatus = ycpService.GetRate(xbData.SchemeYear, OverDeclarationPercent, OverDeclarationArea, xbData.ScotlandYFPreviousYearOverDeclaration);
            }

            Calculate();
        }

        public override void Calculate()
        {
            LastPenalty = "NoPenalties";

            if (OverDeclarationReduction > 0 || AdditionalOverDeclarationReduction > 0)
            {
                LastPenalty = "OverDeclarationReduction";
            }
            if (LateClaimSubmissionReduction > 0)
            {
                LastPenalty = "LateClaimSubmissionReduction";
            }
            if (LateAmendmentReduction > 0)
            {
                LastPenalty = "LateAmendmentReduction";
            }
            if (LateEvidenceReduction > 0)
            {
                LastPenalty = "LateEvidenceReduction";
            }
            if (NonDeclarationReduction > 0)
            {
                LastPenalty = "NonDeclarationReduction";
            }
            if (LateChangePenaltyReduction > 0)
            {
                LastPenalty = "LateChangePenaltyReduction";
            }

            OverDeclarationTotal = YFClaimValue - OverDeclarationReduction - AdditionalOverDeclarationReduction;

            if (OverDeclarationTotal < 0)
            {
                OverDeclarationTotal = 0;
            }

            LateClaimSubmissionTotal = OverDeclarationTotal - LateClaimSubmissionReduction;

            if (LateClaimSubmissionTotal < 0)
            {
                LateClaimSubmissionTotal = 0;
            }

            LateAmendmentTotal = LateClaimSubmissionTotal - LateAmendmentReduction;

            if (LateAmendmentTotal < 0)
            {
                LateAmendmentTotal = 0;
            }

            LateEvidenceTotal = LateAmendmentTotal - LateEvidenceReduction;

            if (LateEvidenceTotal < 0)
            {
                LateEvidenceTotal = 0;
            }

            NonDeclarationTotal = LateEvidenceTotal - NonDeclarationReduction;

            if (NonDeclarationTotal < 0)
            {
                NonDeclarationTotal = 0;
            }

            if (LateChangePenaltyTotal < 0)
            {
                LateChangePenaltyTotal = 0;
            }
            FDMTotal = LateChangePenaltyTotal - FDMReduction;

            if (FDMTotal < 0)
            {
                FDMTotal = 0;
            }
        }
    }
}
