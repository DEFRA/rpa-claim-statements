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
    public class NIYoungFarmerPayment:YoungFarmerPayment
    {
        IYCPService ycpService;

        public NIYoungFarmerPayment()
        {
            ycpService = new YCPService();
        }

        public NIYoungFarmerPayment(IYCPService ycpService)
        {
            this.ycpService = ycpService;
        }

        public decimal YFRate { get; set; }    

        public decimal YFDeclarationPenalty { get; set; }

        public decimal YFDeclarationReduction { get; set; }

        public decimal YFDeclarationTotal { get; set; }     

        public decimal LateAmendmentPercent { get; set; }

        public decimal LateAmendmentReduction { get; set; }

        public decimal LateAmendmentTotal { get; set; }

        public override void Build(XBData xbData)
        {
            EntitlementsUsedToClaim = xbData.NIYFEntitlementsUsedToClaim;
            YFClaimValue = xbData.NIYFClaimValue;
            OverDeclarationArea = xbData.NIYFOverDeclarationArea;
            OverDeclarationPercent = xbData.NIBPSOverDeclarationPercent;
            OverDeclarationReduction = xbData.NIYFOverDeclarationReduction - xbData.NIBPSAdditionalOverDeclarationReduction;
            AdditionalOverDeclarationReduction = xbData.NIBPSAdditionalOverDeclarationReduction;
            LateClaimSubmissionPercent = xbData.NIYFLateClaimSubmissionPercent;
            LateClaimSubmissionReduction = xbData.NIYFLateClaimSubmissionReduction;
            LateAmendmentPercent = xbData.NIYFLateAmendmentPercent;
            LateAmendmentReduction = xbData.NIYFLateAmendmentReduction;
            LateEvidencePercent = xbData.NIYFLateEvidencePercent;
            LateEvidenceReduction = xbData.NIYFLateEvidenceReduction;
            NonDeclarationPercent = xbData.NIYFNonDeclarationPercent;
            NonDeclarationReduction = xbData.NIYFNonDeclarationReduction;
            LateChangePenaltyReduction = xbData.NIYFLateAmendmentReduction;
            FDMPercent = xbData.NIYFFDMPercent;
            FDMReduction = xbData.NIYFFDMReduction;
            TotalYoungFarmerPayment = xbData.NIYFTotalYoungFarmerPayment;
            YFRate = xbData.NIYFRate;

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
