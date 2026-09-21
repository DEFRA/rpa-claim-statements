using RPA.ClaimStatements.Generator.Models.Entities.XB;
using RPA.ClaimStatements.Generator.Models.Generation;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Models.ClaimStatements
{
    public class WalesRedistributivePayment
    {
        IYCPService ycpService;

        public WalesRedistributivePayment()
        {
            ycpService = new YCPService();
        }

        public WalesRedistributivePayment(IYCPService ycpService)
        {
            this.ycpService = ycpService;
        }

        public decimal LandUsedToClaim { get; set; }

        public decimal RedClaimValue { get; set; }

        public string YCPStatus { get; set; }

        public decimal OverDeclarationArea { get; set; }

        public decimal OverDeclarationPercent { get; set; }

        public decimal OverDeclarationReduction { get; set; }

        public decimal AdditionalOverDeclarationReduction { get; set; }

        public bool PreviousYearOverDeclaration { get; set; }

        public decimal OverDeclarationTotal { get; set; }

        public decimal LateClaimSubmissionPercent { get; set; }

        public decimal LateClaimSubmissionReduction { get; set; }

        public decimal LateClaimSubmissionTotal { get; set; }

        public decimal LateAmendmentPercent { get; set; }

        public decimal LateAmendmentReduction { get; set; }

        public decimal LateAmendmentTotal { get; set; }

        public decimal LateEvidencePercent { get; set; }

        public decimal LateEvidenceReduction { get; set; }

        public decimal LateEvidenceTotal { get; set; }

        public decimal NonDeclarationPercent { get; set; }

        public decimal NonDeclarationReduction { get; set; }

        public decimal NonDeclarationTotal { get; set; }

        public decimal LateChangePenaltyReduction { get; set; }

        public decimal LateChangePenaltyTotal { get; set; }

        public decimal FDMPercent { get; set; }

        public decimal FDMReduction { get; set; }

        public decimal FDMTotal { get; set; }

        public decimal TotalRedistributionPayment { get; set; }

        public string LastPenalty { get; set; }

        public void Build(XBData xbData)
        {
            LandUsedToClaim = xbData.WalesRedLandUsedToClaim > 54 ? 54 : xbData.WalesRedLandUsedToClaim;
            RedClaimValue = xbData.WalesRedClaimValue;
            OverDeclarationArea = xbData.WalesRedOverDeclarationArea;
            OverDeclarationPercent = xbData.WalesRedOverDeclarationPercent;
            OverDeclarationReduction = xbData.WalesRedOverDeclarationReduction - xbData.WalesRedAdditionalOverDeclarationReduction;
            AdditionalOverDeclarationReduction = xbData.WalesRedAdditionalOverDeclarationReduction;
            LateClaimSubmissionPercent = xbData.WalesRedLateClaimSubmissionPercent;
            LateClaimSubmissionReduction = xbData.WalesRedLateClaimSubmissionReduction;
            LateAmendmentPercent = xbData.WalesRedLateAmendmentPercent;
            LateAmendmentReduction = xbData.WalesRedLateAmendmentReduction;
            LateEvidencePercent = xbData.WalesRedLateEvidencePercent;
            LateEvidenceReduction = xbData.WalesRedLateEvidenceReduction;
            NonDeclarationPercent = xbData.WalesRedNonDeclarationPercent;
            NonDeclarationReduction = xbData.WalesRedNonDeclarationReduction;
            LateChangePenaltyReduction = xbData.WalesRedLateAmendmentReduction;
            FDMPercent = xbData.WalesRedFDMPercent;
            FDMReduction = xbData.WalesRedFDMReduction;
            TotalRedistributionPayment = xbData.WalesRedTotalRedistributionPayment;

            if (OverDeclarationReduction > 0)
            {
                YCPStatus = YCPStatus = ycpService.GetRate(xbData.SchemeYear, OverDeclarationPercent, OverDeclarationArea, xbData.WalesRedPreviousYearOverDeclaration);
            }

            Calculate();
        }

        public void Calculate()
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

            OverDeclarationTotal = RedClaimValue - OverDeclarationReduction - AdditionalOverDeclarationReduction;

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

            LateChangePenaltyTotal = NonDeclarationTotal - LateChangePenaltyReduction;

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
