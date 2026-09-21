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
    public class BPSPayment
    {
        IYCPService ycpService;

        public BPSPayment()
        {
            ycpService = new YCPService();
        }

        public BPSPayment(IYCPService ycpService)
        {
            this.ycpService = ycpService;
        }

        public int Regions { get; set; }

        public decimal NonSDANumber { get; set; }

        public decimal NonSDARate { get; set; }

        public decimal NonSDATotal { get; set; }

        public decimal SDANumber { get; set; }

        public decimal SDARate { get; set; }

        public decimal SDATotal { get; set; }

        public decimal MoorlandNumber { get; set; }

        public decimal MoorlandRate { get; set; }

        public decimal MoorlandTotal { get; set; }

        public decimal TotalNumber { get; set; }

        public decimal TotalTotal { get; set; }

        public decimal AverageEntitlementValue2015Value { get; set; }

        public string YCPStatus { get; set; }

        public decimal OverDeclarationHectares { get; set; }

        public decimal OverDeclarationPercent { get; set; }

        public decimal OverDeclarationReduction { get; set; }

        public decimal AdditionalOverDeclarationReduction { get; set; }
                    
        public decimal OverDeclarationTotal { get; set; }
        
        public decimal LateClaimSubmissionPercent { get; set; }

        public decimal LateClaimSubmissionReduction { get; set; }

        public decimal LateClaimSubmissionTotal { get; set; }

        public decimal LateEntitlementsApplicationPercent { get; set; }

        public decimal LateEntitlementsApplicationReduction { get; set; }

        public decimal LateEntitlementsApplicationTotal { get; set; }

        public decimal LateEvidencePercent { get; set; }

        public decimal LateEvidenceReduction { get; set; }

        public decimal LateEvidenceTotal { get; set; }

        public decimal LateEntitlementsAmendmentPercent { get; set; }

        public decimal LateEntitlementsAmendmentReduction { get; set; }

        public decimal LateEntitlementsAmendmentTotal { get; set; }

        public decimal NonDeclarationPercent { get; set; }

        public decimal NonDeclarationReduction { get; set; }

        public decimal NonDeclarationTotal { get; set; }

        public decimal LateChangePenaltyReduction { get; set; }

        public decimal LateChangePenaltyTotal { get; set; }

        public decimal FDMPercent { get; set; }

        public decimal FDMReduction { get; set; }

        public decimal FDMTotal { get; set; }

        public decimal ReductionOfPaymentsOver150kPercent { get; set; }

        public decimal ReductionOfPaymentsOver150kDeduction { get; set; }

        public decimal ReductionOfPaymentsOver150kTotal { get; set; }

        public decimal TotalBPSPayment { get; set; }

        public string LastPenalty { get; set; }

        public decimal BPSGross { get; set; }

        public virtual void Build(SUM2 sum2, BPS bps, BPSPEN bpspen, int schemeYear)
        {
            NonSDANumber = bps.NonSDANumber;
            NonSDATotal = bps.NonSDATotal;
            SDANumber = bps.SDANumber;
            SDATotal = bps.SDATotal;
            MoorlandNumber = bps.MoorlandNumber;
            MoorlandTotal = bps.MoorlandSDATotal;
            AverageEntitlementValue2015Value = bps.AVGEntitlementValue;
            OverDeclarationHectares = bpspen.OverDeclarationHectares;
            OverDeclarationPercent = bpspen.OverDeclarationPercent;
            OverDeclarationReduction = bpspen.OverDeclarationReduction;
            AdditionalOverDeclarationReduction = bpspen.AdditionalOverDeclarationReduction;
            LateClaimSubmissionPercent = bpspen.LateClaimSubmissionPercent;
            LateClaimSubmissionReduction = bpspen.LateClaimSubmissionReduction;
            LateEntitlementsApplicationPercent = bpspen.LateEntitlementsApplicationPercent;
            LateEntitlementsApplicationReduction = bpspen.LateEntitlementsApplicationReduction;
            LateEvidencePercent = bpspen.LateEvidencePercent;
            LateEvidenceReduction = bpspen.LateEvidenceReduction;
            LateEntitlementsAmendmentPercent = bpspen.LateEntitlementsAmendmentPercent;
            LateEntitlementsAmendmentReduction = bpspen.LateEntitlementsAmendmentReduction;
            NonDeclarationPercent = bpspen.NonDeclarationPercent;
            NonDeclarationReduction = bpspen.NonDeclarationReduction;
            LateChangePenaltyReduction = bpspen.LateChangePenaltyReduction;
            FDMPercent = bpspen.FDMPercent;
            FDMReduction = bpspen.FDMReduction;
            ReductionOfPaymentsOver150kPercent = bpspen.ReductionOfPaymentsOver150kPercent;
            ReductionOfPaymentsOver150kDeduction = bpspen.ReductionOfPaymentsOver150kReduction;
            TotalBPSPayment = bpspen.TotalBPS;
            BPSGross = sum2.BPSValue;            

            if (OverDeclarationReduction > 0)
            {
                YCPStatus = ycpService.GetRate(schemeYear, OverDeclarationPercent, OverDeclarationHectares, bpspen.PreviousYearOverDeclaration);                
            }

            Calculate();
        }

        public virtual void Build(XBData xbData)
        {
            NonSDANumber = xbData.EnglandBPSNonSDANumber;
            NonSDARate = xbData.EnglandBPSAverageEntitlementValue2015Value;
            NonSDATotal = xbData.EnglandBPSNonSDATotal;
            SDANumber = xbData.EnglandBPSSDANumber;
            SDARate = xbData.EnglandBPSAverageEntitlementValue2015Value;
            SDATotal = xbData.EnglandBPSSDATotal;
            MoorlandNumber = xbData.EnglandBPSMoorlandNumber;
            MoorlandRate = xbData.EnglandBPSAverageEntitlementValue2015Value;
            MoorlandTotal = xbData.EnglandBPSMoorlandTotal;
            AverageEntitlementValue2015Value = xbData.EnglandBPSAverageEntitlementValue2015Value;
            OverDeclarationHectares = xbData.EnglandBPSOverDeclarationHectares;
            OverDeclarationPercent = xbData.EnglandBPSOverDeclarationPercent;
            OverDeclarationReduction = xbData.EnglandBPSOverDeclarationReduction - xbData.EnglandBPSAdditionalOverDeclarationReduction;
            AdditionalOverDeclarationReduction = xbData.EnglandBPSAdditionalOverDeclarationReduction;
            LateClaimSubmissionPercent = xbData.EnglandBPSLateClaimSubmissionPercent;
            LateClaimSubmissionReduction = xbData.EnglandBPSLateClaimSubmissionReduction;
            LateEntitlementsApplicationPercent = xbData.EnglandBPSLateEntitlementsApplicationPercent;
            LateEntitlementsApplicationReduction = xbData.EnglandBPSLateEntitlementsApplicationReduction;
            LateChangePenaltyReduction = xbData.EnglandBPSLateAmendmentReduction;
            LateEvidencePercent = xbData.EnglandBPSLateEvidencePercent;
            LateEvidenceReduction = xbData.EnglandBPSLateEvidenceReduction;
            LateEntitlementsAmendmentPercent = xbData.EnglandBPSLateEntitlementsAmendmentPercent;
            LateEntitlementsAmendmentReduction = xbData.EnglandBPSLateEntitlementsAmendmentReduction;
            NonDeclarationPercent = xbData.EnglandBPSNonDeclarationPercent;
            NonDeclarationReduction = xbData.EnglandBPSNonDeclarationReduction;
            FDMPercent = xbData.EnglandBPSFDMPercent;
            FDMReduction = xbData.EnglandBPSFDMReduction;
            ReductionOfPaymentsOver150kPercent = xbData.EnglandBPSReductionOfPaymentsOver150kPercent;
            ReductionOfPaymentsOver150kDeduction = xbData.EnglandBPSReductionOfPaymentsOver150kDeduction;
            TotalBPSPayment = xbData.EnglandBPSTotalBPSPayment;
            BPSGross = xbData.EnglandBPSBPSGross;

            if (OverDeclarationReduction > 0)
            {
                YCPStatus = ycpService.GetRate(xbData.SchemeYear, OverDeclarationPercent, OverDeclarationHectares, xbData.EnglandBPSPreviousYearOverDeclaration);
            }

            Calculate();
        }

        public virtual void Calculate()
        {
            int bpsRegions = 0;

            if (NonSDANumber > 0)
            {
                bpsRegions++;
            }
            if (SDANumber > 0)
            {
                bpsRegions++;
            }
            if (MoorlandNumber > 0)
            {
                bpsRegions++;
            }

            if (LateEntitlementsApplicationReduction > 0 && LateEntitlementsApplicationPercent == 0)
            {
                LateEntitlementsApplicationReduction = 0;
            }

            Regions = bpsRegions;

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
            if (LateEvidenceReduction > 0)
            {
                LastPenalty = "LateEvidenceReduction";
            }
            if (LateEntitlementsAmendmentReduction > 0)
            {
                LastPenalty = "LateEntitlementsAmendmentReduction";
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
            TotalTotal = NonSDATotal + SDATotal + MoorlandTotal;

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

            LateEvidenceTotal = LateEntitlementsApplicationTotal - LateEvidenceReduction;

            if (LateEvidenceTotal < 0)
            {
                LateEvidenceTotal = 0;
            }

            LateEntitlementsAmendmentTotal = LateClaimSubmissionTotal - LateEntitlementsAmendmentReduction;

            if (LateEntitlementsAmendmentTotal < 0)
            {
                LateEntitlementsAmendmentTotal = 0;
            }

            NonDeclarationTotal = LateEntitlementsAmendmentTotal - NonDeclarationReduction;

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
