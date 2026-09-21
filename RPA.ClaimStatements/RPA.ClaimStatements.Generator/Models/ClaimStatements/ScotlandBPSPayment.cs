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
    public class ScotlandBPSPayment:BPSPayment
    {
        IYCPService ycpService;

        public ScotlandBPSPayment()
        {
            ycpService = new YCPService();
        }

        public ScotlandBPSPayment(IYCPService ycpService)
        {
            this.ycpService = ycpService;
        }

        public decimal Region1Number { get; set; }

        public decimal Region1Rate { get; set; }

        public decimal Region1Total { get; set; }

        public decimal Region2Number { get; set; }

        public decimal Region2Rate { get; set; }

        public decimal Region2Total { get; set; }

        public decimal Region3Number { get; set; }

        public decimal Region3Rate { get; set; }

        public decimal Region3Total { get; set; }
                
        public decimal LateAmendmentPercent { get; set; }

        public decimal LateAmendmentReduction { get; set; }

        public decimal LateAmendmentTotal { get; set; }

        public decimal LateEvidenceEntitlementsPercent { get; set; }

        public decimal LateEvidenceEntitlementsReduction { get; set; }

        public decimal LateEvidenceEntitlementsTotal { get; set; }

        public override void Build(XBData xbData)
        {
            Region1Number = xbData.ScotlandBPSRegion1Number;
            Region1Rate = xbData.EnglandBPSAverageEntitlementValue2015Value;
            Region1Total = xbData.ScotlandBPSRegion1Total;
            Region2Number = xbData.ScotlandBPSRegion2Number;
            Region2Rate = xbData.EnglandBPSAverageEntitlementValue2015Value;
            Region2Total = xbData.ScotlandBPSRegion2Total;
            Region3Number = xbData.ScotlandBPSRegion3Number;
            Region3Rate = xbData.EnglandBPSAverageEntitlementValue2015Value;
            Region3Total = xbData.ScotlandBPSRegion3Total;
            AverageEntitlementValue2015Value = xbData.EnglandBPSAverageEntitlementValue2015Value;
            OverDeclarationHectares = xbData.ScotlandBPSOverDeclarationHectares;
            OverDeclarationPercent = xbData.ScotlandBPSOverDeclarationPercent;
            OverDeclarationReduction = xbData.ScotlandBPSOverDeclarationReduction - xbData.ScotlandBPSAdditionalOverDeclarationReduction;
            AdditionalOverDeclarationReduction = xbData.ScotlandBPSAdditionalOverDeclarationReduction;
            LateClaimSubmissionPercent = xbData.ScotlandBPSLateClaimSubmissionPercent;
            LateClaimSubmissionReduction = xbData.ScotlandBPSLateClaimSubmissionReduction;
            LateEntitlementsApplicationPercent = xbData.ScotlandBPSLateEntitlementsApplicationPercent;
            LateEntitlementsApplicationReduction = xbData.ScotlandBPSLateEntitlementsApplicationReduction;
            LateAmendmentPercent = xbData.ScotlandBPSLateAmendmentPercent;
            LateAmendmentReduction = xbData.ScotlandBPSLateAmendmentReduction;
            LateEntitlementsAmendmentPercent = xbData.ScotlandBPSLateEntitlementsAmendmentPercent;
            LateEntitlementsAmendmentReduction = xbData.ScotlandBPSLateEntitlementsAmendmentReduction;
            LateEvidencePercent = xbData.ScotlandBPSLateEvidencePercent;
            LateEvidenceReduction = xbData.ScotlandBPSLateEvidenceReduction;
            LateEvidenceEntitlementsPercent = xbData.ScotlandBPSLateEvidenceEntitlementsPercent;
            LateEvidenceEntitlementsReduction = xbData.ScotlandBPSLateEvidenceEntitlementsReduction;
            NonDeclarationPercent = xbData.ScotlandBPSNonDeclarationPercent;
            NonDeclarationReduction = xbData.ScotlandBPSNonDeclarationReduction;
            LateChangePenaltyReduction = xbData.ScotlandBPSLateAmendmentReduction;
            FDMPercent = xbData.ScotlandBPSFDMPercent;
            FDMReduction = xbData.ScotlandBPSFDMReduction;
            ReductionOfPaymentsOver150kPercent = xbData.ScotlandBPSReductionOfPaymentsOver150kPercent;
            ReductionOfPaymentsOver150kDeduction = xbData.ScotlandBPSReductionOfPaymentsOver150kDeduction;
            TotalBPSPayment = xbData.ScotlandBPSTotalBPSPayment;
            BPSGross = xbData.ScotlandBPSGross;

            if (OverDeclarationReduction > 0)
            {
                YCPStatus = ycpService.GetRate(xbData.SchemeYear, OverDeclarationPercent, OverDeclarationHectares, xbData.ScotlandBPSPreviousYearOverDeclaration);
            }

            Calculate();
        }

        public override void Calculate()
        {
            int scotlandBPSRegions = 0;

            if (Region1Number > 0)
            {
                scotlandBPSRegions++;
            }
            if (Region2Number > 0)
            {
                scotlandBPSRegions++;
            }
            if (Region3Number > 0)
            {
                scotlandBPSRegions++;
            }

            Regions = scotlandBPSRegions;

            LastPenalty = "NoPenalties";

            if (OverDeclarationReduction > 0 || AdditionalOverDeclarationReduction > 0)
            {
                LastPenalty = "OverDeclarationReduction";
            }
            if (LateClaimSubmissionReduction > 0)
            {
                LastPenalty = "LateClaimSubmissionReduction";
            }
            if (LateEntitlementsApplicationReduction > 0)
            {
                LastPenalty = "LateEntitlementsApplicationReduction";
            }
            if (LateAmendmentReduction > 0)
            {
                LastPenalty = "LateAmendmentReduction";
            }
            if (LateEntitlementsAmendmentReduction > 0)
            {
                LastPenalty = "LateEntitlementsAmendmentReduction";
            }
            if (LateEvidenceReduction > 0)
            {
                LastPenalty = "LateEvidenceReduction";
            }
            if (LateEvidenceEntitlementsReduction > 0)
            {
                LastPenalty = "LateEvidenceEntitlementsReduction";
            }
            if (NonDeclarationReduction > 0)
            {
                LastPenalty = "NonDeclarationReduction";
            }
            if (LateChangePenaltyReduction > 0)
            {
                LastPenalty = "LateChangePenaltyReduction";
            }

            TotalNumber = Region1Number + Region2Number + Region3Number;
            TotalTotal = Region1Total + Region2Total + Region3Total;

            OverDeclarationTotal = BPSGross - OverDeclarationReduction - AdditionalOverDeclarationReduction;

            if (OverDeclarationTotal < 0)
            {
                OverDeclarationTotal = 0;
            }

            LateClaimSubmissionTotal = OverDeclarationTotal - LateClaimSubmissionReduction;

            if (LateClaimSubmissionTotal < 0)
            {
                LateClaimSubmissionTotal = 0;
            }

            LateEntitlementsApplicationTotal = LateClaimSubmissionTotal - LateEntitlementsApplicationReduction;

            if (LateEntitlementsApplicationTotal < 0)
            {
                LateEntitlementsApplicationTotal = 0;
            }

            LateAmendmentTotal = LateEntitlementsApplicationTotal - LateAmendmentReduction;

            if (LateAmendmentTotal < 0)
            {
                LateAmendmentTotal = 0;
            }

            LateEntitlementsAmendmentTotal = LateAmendmentTotal - LateEntitlementsAmendmentReduction;

            if (LateEntitlementsAmendmentTotal < 0)
            {
                LateEntitlementsAmendmentTotal = 0;
            }

            LateEvidenceTotal = LateEntitlementsAmendmentTotal - LateEvidenceReduction;

            if (LateEvidenceTotal < 0)
            {
                LateEvidenceTotal = 0;
            }

            LateEvidenceEntitlementsTotal = LateEvidenceTotal - LateEvidenceEntitlementsReduction;

            if (LateEvidenceEntitlementsTotal < 0)
            {
                LateEvidenceEntitlementsTotal = 0;
            }

            NonDeclarationTotal = LateEvidenceEntitlementsTotal - NonDeclarationReduction;

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

            ReductionOfPaymentsOver150kTotal = FDMTotal - ReductionOfPaymentsOver150kDeduction;

            if (ReductionOfPaymentsOver150kTotal < 0)
            {
                ReductionOfPaymentsOver150kTotal = 0;
            }
        }
    }
}
