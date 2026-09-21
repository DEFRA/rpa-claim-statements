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
    public class WalesBPSPayment : BPSPayment
    {
        IYCPService ycpService;

        public WalesBPSPayment()
        {
            ycpService = new YCPService();
        }

        public WalesBPSPayment(IYCPService ycpService)
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

        public decimal ReductionOfPaymentsOver200kPercent { get; set; }

        public decimal ReductionOfPaymentsOver200kDeduction { get; set; }

        public decimal ReductionOfPaymentsOver200kTotal { get; set; }

        public decimal ReductionOfPaymentsOver250kPercent { get; set; }

        public decimal ReductionOfPaymentsOver250kDeduction { get; set; }

        public decimal ReductionOfPaymentsOver250kTotal { get; set; }

        public decimal ReductionOfPaymentsOver300kPercent { get; set; }

        public decimal ReductionOfPaymentsOver300kDeduction { get; set; }

        public decimal ReductionOfPaymentsOver300kTotal { get; set; }

        public string LastPenalty2 { get; set; }

        public override void Build(XBData xbData)
        {
            Region1Number = xbData.WalesBPSRegion1Number;
            Region1Rate = xbData.EnglandBPSAverageEntitlementValue2015Value;
            Region1Total = xbData.WalesBPSRegion1Total;
            OverDeclarationHectares = xbData.WalesBPSOverDeclarationHectares;
            OverDeclarationPercent = xbData.WalesBPSOverDeclarationPercent;
            OverDeclarationReduction = xbData.WalesBPSOverDeclarationReduction - xbData.WalesBPSAdditionalOverDeclarationReduction;
            AdditionalOverDeclarationReduction = xbData.WalesBPSAdditionalOverDeclarationReduction;
            LateClaimSubmissionPercent = xbData.WalesBPSLateClaimSubmissionPercent;
            LateClaimSubmissionReduction = xbData.WalesBPSLateClaimSubmissionReduction;
            LateEntitlementsApplicationPercent = xbData.WalesBPSLateEntitlementsApplicationPercent;
            LateEntitlementsApplicationReduction = xbData.WalesBPSLateEntitlementsApplicationReduction;
            LateAmendmentPercent = xbData.WalesBPSLateAmendmentPercent;
            LateAmendmentReduction = xbData.WalesBPSLateAmendmentReduction;
            LateEntitlementsAmendmentPercent = xbData.WalesBPSLateEntitlementsAmendmentPercent;
            LateEntitlementsAmendmentReduction = xbData.WalesBPSLateEntitlementsAmendmentReduction;
            LateEvidencePercent = xbData.WalesBPSLateEvidencePercent;
            LateEvidenceReduction = xbData.WalesBPSLateEvidenceReduction;
            LateEvidenceEntitlementsPercent = xbData.WalesBPSLateEvidenceEntitlementsPercent;
            LateEvidenceEntitlementsReduction = xbData.WalesBPSLateEvidenceEntitlementsReduction;
            NonDeclarationPercent = xbData.WalesBPSNonDeclarationPercent;
            NonDeclarationReduction = xbData.WalesBPSNonDeclarationReduction;
            LateChangePenaltyReduction = xbData.WalesBPSLateAmendmentReduction;
            FDMPercent = xbData.WalesBPSFDMPercent;
            FDMReduction = xbData.WalesBPSFDMReduction;
            ReductionOfPaymentsOver150kPercent = xbData.WalesBPSReductionOfPaymentsOver150kPercent;
            ReductionOfPaymentsOver150kDeduction = xbData.WalesBPSReductionOfPaymentsOver150kDeduction;
            ReductionOfPaymentsOver200kPercent = xbData.WalesBPSReductionOfPaymentsOver200kPercent;
            ReductionOfPaymentsOver200kDeduction = xbData.WalesBPSReductionOfPaymentsOver200kDeduction;
            ReductionOfPaymentsOver250kPercent = xbData.WalesBPSReductionOfPaymentsOver250kPercent;
            ReductionOfPaymentsOver250kDeduction = xbData.WalesBPSReductionOfPaymentsOver250kDeduction;
            ReductionOfPaymentsOver300kPercent = xbData.WalesBPSReductionOfPaymentsOver300kPercent;
            ReductionOfPaymentsOver300kDeduction = xbData.WalesBPSReductionOfPaymentsOver300kDeduction;
            TotalBPSPayment = xbData.WalesBPSTotalBPSPayment;

            if (OverDeclarationReduction > 0)
            {
                YCPStatus = ycpService.GetRate(xbData.SchemeYear, OverDeclarationPercent, OverDeclarationHectares, xbData.WalesBPSPreviousYearOverDeclaration);
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

            LastPenalty2 = "NoPenalties";

            if (FDMReduction > 0)
            {
                LastPenalty2 = "FDMReduction";
            }
            if (ReductionOfPaymentsOver150kDeduction > 0)
            {
                LastPenalty2 = "ReductionOfPaymentsOver150kDeduction";
            }
            if (ReductionOfPaymentsOver200kDeduction > 0)
            {
                LastPenalty2 = "ReductionOfPaymentsOver200kDeduction";
            }
            if (ReductionOfPaymentsOver250kDeduction > 0)
            {
                LastPenalty2 = "ReductionOfPaymentsOver250kDeduction";
            }
            if (ReductionOfPaymentsOver300kDeduction > 0)
            {
                LastPenalty2 = "ReductionOfPaymentsOver300kDeduction";
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

            ReductionOfPaymentsOver200kTotal = ReductionOfPaymentsOver150kTotal - ReductionOfPaymentsOver200kDeduction;

            if (ReductionOfPaymentsOver200kTotal < 0)
            {
                ReductionOfPaymentsOver200kTotal = 0;
            }

            ReductionOfPaymentsOver250kTotal = ReductionOfPaymentsOver200kTotal - ReductionOfPaymentsOver250kDeduction;

            if (ReductionOfPaymentsOver250kTotal < 0)
            {
                ReductionOfPaymentsOver250kTotal = 0;
            }

            ReductionOfPaymentsOver300kTotal = ReductionOfPaymentsOver250kTotal - ReductionOfPaymentsOver300kDeduction;

            if (ReductionOfPaymentsOver300kTotal < 0)
            {
                ReductionOfPaymentsOver300kTotal = 0;
            }
        }
    }
}
