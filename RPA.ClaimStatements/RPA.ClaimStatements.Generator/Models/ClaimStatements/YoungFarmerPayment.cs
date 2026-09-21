using RPA.ClaimStatements.Generator.Models.Entities.SITI;
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
    public class YoungFarmerPayment
    {
        IYCPService ycpService;

        public YoungFarmerPayment()
        {
            ycpService = new YCPService();
        }

        public YoungFarmerPayment(IYCPService ycpService)
        {
            this.ycpService = ycpService;
        }

        public int Regions { get; set; }

        public decimal NonSDANumber { get; set; }

        public decimal SDANumber { get; set; }

        public decimal MoorlandNumber { get; set; }

        public decimal TotalNumber { get; set; }

        public decimal EntitlementValueTotal { get; set; }

        public decimal EntitlementsUsedToClaim { get; set; }

        public decimal AvgEntitlementValue { get; set; }

        public decimal YFClaimValuePercent { get; set; }

        public decimal YFClaimValue { get; set; }

        public string YCPStatus { get; set; }

        public decimal OverDeclarationArea { get; set; }

        public decimal OverDeclarationPercent { get; set; }

        public decimal OverDeclarationReduction { get; set; }

        public decimal AdditionalOverDeclarationReduction { get; set; }
        
        public decimal OverDeclarationTotal { get; set; }

        public decimal LateClaimSubmissionPercent { get; set; }

        public decimal LateClaimSubmissionReduction { get; set; }

        public decimal LateClaimSubmissionTotal { get; set; }

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

        public decimal TotalYoungFarmerPayment { get; set; }

        public string LastPenalty { get; set; }

        public bool YFDeductions { get; set; }


        public virtual void Build(SUM2 sum2, YF yf, int schemeYear)
        {
            NonSDANumber = yf.NonSDANumber;
            SDANumber = yf.SDANumber;
            MoorlandNumber = yf.MoorlandNumber;
            EntitlementsUsedToClaim = yf.EntitlementsUsedToClaim;
            AvgEntitlementValue = yf.AvgEntitlementValue;
            OverDeclarationArea = yf.OverDeclarationArea;
            OverDeclarationPercent = yf.OverDeclarationPercent;
            OverDeclarationReduction = yf.OverDeclarationReduction - yf.AdditionalOverDeclarationReduction;
            AdditionalOverDeclarationReduction = yf.AdditionalOverDeclarationReduction;
            LateClaimSubmissionPercent = yf.LateClaimSubmissionPercent;
            LateClaimSubmissionReduction = yf.LateClaimSubmissionReduction;
            LateEvidencePercent = yf.LateEvidencePercent;
            LateEvidenceReduction = yf.LateEvidenceReduction;
            NonDeclarationPercent = yf.NonDeclarationPercent;
            NonDeclarationReduction = yf.NonDeclarationReduction;
            LateChangePenaltyReduction = yf.LateChangePenaltyReduction;
            FDMPercent = yf.FDMPercent;
            FDMReduction = yf.FDMReduction;
            TotalYoungFarmerPayment = yf.TotalYF;
            YFClaimValue = sum2.YoungFarmerValue;

            if (OverDeclarationReduction > 0)
            {
                YCPStatus = ycpService.GetRate(schemeYear, OverDeclarationPercent, OverDeclarationArea, yf.PreviousYearOverDeclaration);
            }

            if (OverDeclarationReduction == 0 && AdditionalOverDeclarationReduction == 0 && LateClaimSubmissionReduction == 0 && LateEvidenceReduction == 0 && NonDeclarationReduction == 0 && LateChangePenaltyReduction == 0 && FDMReduction == 0)
            {
                YFDeductions = false;
            }
            else
            {
                YFDeductions = true;
            }

            Calculate();
        }

        public virtual void Build(XBData xbData)
        {
            EntitlementsUsedToClaim = xbData.EnglandYFEntitlementsUsedToClaim;
            AvgEntitlementValue = xbData.EnglandYFAvgEntitlementValue;
            YFClaimValuePercent = xbData.EnglandYFYFClaimValuePercent;
            YFClaimValue = xbData.EnglandYFClaimValue;
            OverDeclarationArea = xbData.EnglandYFOverDeclarationArea;
            OverDeclarationPercent = xbData.EnglandYFOverDeclarationPercent;
            OverDeclarationReduction = xbData.EnglandYFOverDeclarationReduction;
            AdditionalOverDeclarationReduction = xbData.EnglandYFAdditionalOverDeclarationReduction;
            LateClaimSubmissionPercent = xbData.EnglandYFLateClaimSubmissionPercent;
            LateClaimSubmissionReduction = xbData.EnglandYFLateClaimSubmissionReduction;
            LateEvidencePercent = xbData.EnglandYFLateAmendmentPercent;
            LateEvidenceReduction = xbData.EnglandYFLateAmendmentReduction;
            NonDeclarationPercent = xbData.EnglandYFNonDeclarationPercent;
            NonDeclarationReduction = xbData.EnglandYFNonDeclarationReduction;
            LateChangePenaltyReduction = xbData.EnglandYFLateAmendmentReduction;
            FDMPercent = xbData.EnglandYFFDMPercent;
            FDMReduction = xbData.EnglandYFFDMReduction;
            TotalYoungFarmerPayment = xbData.EnglandYFTotalYoungFarmerPayment;

            if (OverDeclarationReduction > 0)
            {
                YCPStatus = YCPStatus = ycpService.GetRate(xbData.SchemeYear, OverDeclarationPercent, OverDeclarationArea, xbData.EnglandYFPreviousYearOverDeclaration);
            }

            if (OverDeclarationReduction == 0 && AdditionalOverDeclarationReduction == 0 && LateClaimSubmissionReduction == 0 && LateEvidenceReduction == 0 && NonDeclarationReduction == 0 && LateChangePenaltyReduction == 0 && FDMReduction == 0)
            {
                YFDeductions = false;
            }
            else
            {
                YFDeductions = true;
            }

            Calculate();
        }
        public virtual void Calculate()
        {
            int yfRegions = 0;

            if (NonSDANumber > 0)
            {
                yfRegions++;
            }
            if (SDANumber > 0)
            {
                yfRegions++;
            }
            if (MoorlandNumber > 0)
            {
                yfRegions++;
            }

            Regions = yfRegions;

            LastPenalty = "NoPenalties";

            if (OverDeclarationReduction > 0 || AdditionalOverDeclarationReduction > 0)
            {
                LastPenalty = "OverDeclarationReduction";
            }
            if (LateClaimSubmissionReduction > 0)
            {
                LastPenalty = "LateClaimSubmissionReduction";
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


            TotalNumber = NonSDANumber + SDANumber + MoorlandNumber;            

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

            LateEvidenceTotal = LateClaimSubmissionTotal - LateEvidenceReduction;

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

