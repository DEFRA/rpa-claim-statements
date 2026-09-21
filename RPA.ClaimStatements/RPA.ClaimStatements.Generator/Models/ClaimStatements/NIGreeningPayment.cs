using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPA.ClaimStatements.Generator.Models.Entities.XB;

namespace RPA.ClaimStatements.Generator.Models.ClaimStatements
{
    public class NIGreeningPayment:GreeningPayment
    {
        public decimal Region1Number { get; set; }

        public decimal Region1Rate { get; set; }

        public decimal Region1Total { get; set; }        

        public decimal CropDiversificationReduction { get; set; }        

        public decimal LateAmendmentPercent { get; set; }

        public decimal LateAmendmentReduction { get; set; }

        public decimal LateAmendmentTotal { get; set; }

        public override void Build(XBData xbData)
        {
            Region1Number = xbData.NIGreeningRegion1Number;
            Region1Rate = xbData.EnglandGreeningAverageGreeningValue2015Value;
            Region1Total = xbData.NIGreeningRegion1Total;
            CropDiversificationNumber = xbData.NIGreeningCropDiversificationNumber;
            CropDiversificationReduction = xbData.NIGreeningCropDiversificationReduction;
            CropDiversificationNumberAdditional = xbData.NIGreeningCropDiversificationReductionAdditional;
            CropDiversificationReductionAdditional = xbData.NIGreeningCropDiversificationReductionAdditional;
            PermanentGrasslandNumber = xbData.NIGreeningPermanentGrasslandNumber;
            PermanentGrasslandReduction = xbData.NIGreeningPermanentGrasslandReduction;
            EFANumber = xbData.NIGreeningEFANumber;
            EFAReduction = xbData.NIGreeningEFAReduction;
            EFAReductionAdditional = xbData.NIGreeningEFAReductionAdditional;
            AdministrativeArea = xbData.NIGreeningAdministrativeArea;
            AdministrativeReduction = xbData.NIGreeningAdministrativeReduction;
            LateApplicationPercent = xbData.NIGreeningLateApplicationPercent;
            LateApplicationReduction = xbData.NIGreeningLateApplicationReduction;
            LateAmendmentPercent = xbData.NIGreeningLateAmendmentPercent;
            LateAmendmentReduction = xbData.NIGreeningLateAmendmentReduction;
            LateEvidencePercent = xbData.NIGreeningLateEvidencePercent;
            LateEvidenceReduction = xbData.NIGreeningLateEvidenceReduction;
            NonDeclarationPercent = xbData.NIGreeningNonDeclarationPercent;
            NonDeclarationReduction = xbData.NIGreeningNonDeclarationReduction;
            LateChangePenaltyReduction = xbData.NIGreeningLateAmendmentReduction;
            FDMReduction = xbData.NIGreeningFDMReduction;
            TotalGreeningPayment = xbData.NIGreeningTotalGreeningPayment;

            Calculate();
        }

        public override void Calculate()
        {
            LastPenalty = "NoPenalties";

            if (CropDiversificationReduction > 0)
            {
                LastPenalty = "CropDiversificationReduction";
            }
            if (PermanentGrasslandReduction > 0)
            {
                LastPenalty = "PermanentGrasslandReduction";
            }
            if (EFAReduction > 0)
            {
                LastPenalty = "EFAReduction";
            }

            LastPenalty2 = "NoPenalties";

            if (AdministrativeReduction > 0)
            {
                LastPenalty2 = "AdministrativeReduction";
            }
            if (LateApplicationReduction > 0)
            {
                LastPenalty2 = "LateApplicationReduction";
            }
            if (LateAmendmentReduction > 0)
            {
                LastPenalty2 = "LateAmendmentReduction";
            }
            if (LateEvidenceReduction > 0)
            {
                LastPenalty2 = "LateEvidenceReduction";
            }
            if (NonDeclarationReduction > 0)
            {
                LastPenalty2 = "NonDeclarationReduction";
            }
            if (LateChangePenaltyReduction > 0)
            {
                LastPenalty2 = "LateChangePenaltyReduction";
            }

            CropDiversificationTotal = Region1Total - CropDiversificationReduction;

            if (CropDiversificationTotal < 0)
            {
                CropDiversificationTotal = 0;
            }


            PermanentGrasslandTotal = CropDiversificationTotal - PermanentGrasslandReduction;

            if (PermanentGrasslandTotal < 0)
            {
                PermanentGrasslandTotal = 0;
            }
            EFATotal = PermanentGrasslandTotal - EFAReduction;

            if (EFATotal < 0)
            {
                EFATotal = 0;
            }

            AdministrativeTotal = EFATotal - AdministrativeReduction;

            if (AdministrativeTotal < 0)
            {
                AdministrativeTotal = 0;
            }
            LateApplicationTotal = AdministrativeTotal - LateApplicationReduction;

            if (LateApplicationTotal < 0)
            {
                LateApplicationTotal = 0;
            }
            LateAmendmentTotal = LateApplicationTotal - LateAmendmentReduction;

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

            LateChangePenaltyTotal = NonDeclarationTotal - LateChangePenaltyTotal;

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
