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
    public class NIBPSPayment:BPSPayment
    {
        IYCPService ycpService;

        public NIBPSPayment()
        {
            ycpService = new YCPService();
        }

        public NIBPSPayment(IYCPService ycpService)
        {
            this.ycpService = ycpService;
        }

        public decimal Region1Number { get; set; }

        public decimal Region1Rate { get; set; }

        public decimal Region1Total { get; set; }
                
        public decimal LateAmendmentPercent { get; set; }

        public decimal LateAmendmentReduction { get; set; }

        public decimal LateAmendmentTotal { get; set; }

        public decimal LateEvidenceEntitlementsPercent { get; set; }

        public decimal LateEvidenceEntitlementsReduction { get; set; }

        public decimal LateEvidenceEntitlementsTotal { get; set; }


        public override void Build(XBData xbData)
        {
            Region1Number = xbData.NIBPSRegion1Number;
            Region1Rate = xbData.EnglandBPSAverageEntitlementValue2015Value;
            Region1Total = xbData.NIBPSRegion1Total;
            OverDeclarationHectares = xbData.NIBPSOverDeclarationHectares;
            OverDeclarationPercent = xbData.NIBPSOverDeclarationPercent;
            OverDeclarationReduction = xbData.NIBPSOverDeclarationReduction - xbData.NIBPSAdditionalOverDeclarationReduction;
            AdditionalOverDeclarationReduction = xbData.NIBPSAdditionalOverDeclarationReduction;
            LateClaimSubmissionPercent = xbData.NIBPSLateClaimSubmissionPercent;
            LateClaimSubmissionReduction = xbData.NIBPSLateClaimSubmissionReduction;
            LateEntitlementsApplicationPercent = xbData.NIBPSLateEntitlementsApplicationPercent;
            LateEntitlementsApplicationReduction = xbData.NIBPSLateEntitlementsApplicationReduction;
            LateAmendmentPercent = xbData.NIBPSLateAmendmentPercent;
            LateEntitlementsAmendmentPercent = xbData.NIBPSLateEntitlementsAmendmentPercent;
            LateEntitlementsAmendmentReduction = xbData.NIBPSLateEntitlementsAmendmentReduction;
            LateEvidencePercent = xbData.NIBPSLateEvidencePercent;
            LateEvidenceReduction = xbData.NIBPSLateEvidenceReduction;
            LateEvidenceEntitlementsPercent = xbData.NIBPSLateEvidenceEntitlementsPercent;
            LateEvidenceEntitlementsReduction = xbData.NIBPSLateEvidenceEntitlementsReduction;
            NonDeclarationPercent = xbData.NIBPSNonDeclarationPercent;
            NonDeclarationReduction = xbData.NIBPSNonDeclarationReduction;
            LateChangePenaltyReduction = xbData.NIBPSLateAmendmentReduction;
            FDMPercent = xbData.NIBPSFDMPercent;
            FDMReduction = xbData.NIBPSFDMReduction;
            ReductionOfPaymentsOver150kPercent = xbData.NIBPSReductionOfPaymentsOver150kPercent;
            ReductionOfPaymentsOver150kDeduction = xbData.NIBPSReductionOfPaymentsOver150kDeduction;
            TotalBPSPayment = xbData.NIBPSTotalBPSPayment;

            if (OverDeclarationReduction > 0)
            {
                YCPStatus = ycpService.GetRate(xbData.SchemeYear, OverDeclarationPercent, OverDeclarationHectares, xbData.NIBPSPreviousYearOverDeclaration);
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
            if (LateEntitlementsApplicationReduction > 0)
            {
                LastPenalty = "LateEntitlementsApplicationReduction";
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

            OverDeclarationTotal = Region1Total - OverDeclarationReduction - AdditionalOverDeclarationReduction;

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

            LateEntitlementsAmendmentTotal = LateEntitlementsApplicationTotal - LateEntitlementsAmendmentReduction;

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
