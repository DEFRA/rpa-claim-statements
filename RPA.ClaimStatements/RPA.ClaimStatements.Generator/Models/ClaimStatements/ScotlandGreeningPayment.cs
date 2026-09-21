using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPA.ClaimStatements.Generator.Models.Entities.XB;

namespace RPA.ClaimStatements.Generator.Models.ClaimStatements
{
    public class ScotlandGreeningPayment:GreeningPayment
    {
        public decimal Region1Number { get; set; }

        public decimal Region1Rate { get; set; }

        public decimal Region1Total { get; set; }

        public decimal Region2Number { get; set; }

        public decimal Region2Rate { get; set; }

        public decimal Region2Total { get; set; }

        public decimal Region3Number { get; set; }

        public decimal Region3Rate { get; set; }

        public decimal Region3Total { get; set; }        

        public decimal CropDiversificationReduction { get; set; }       

        public decimal LateAmendmentPercent { get; set; }

        public decimal LateAmendmentReduction { get; set; }

        public decimal LateAmendmentTotal { get; set; }

        public override void Build(XBData xbData)
        {
            Region1Number = xbData.ScotlandGreeningRegion1Number;
            Region1Rate = xbData.EnglandGreeningAverageGreeningValue2015Value;
            Region1Total = xbData.ScotlandGreeningRegion1Total;
            Region2Number = xbData.ScotlandGreeningRegion2Number;
            Region2Rate = 0;
            Region2Total = xbData.ScotlandGreeningRegion2Total;
            Region3Number = xbData.ScotlandGreeningRegion3Number;
            Region3Rate = 0;
            Region3Total = xbData.ScotlandGreeningRegion1Total;
            AverageGreeningValue2015Value = xbData.EnglandGreeningAverageGreeningValue2015Value;
            CropDiversificationNumber = xbData.ScotlandGreeningCropDiversificationNumber;
            CropDiversificationReduction = xbData.ScotlandGreeningCropDiversificationReduction;
            CropDiversificationReductionAdditional = xbData.ScotlandGreeningCropDiversificationReductionAdditional;
            PermanentGrasslandNumber = xbData.ScotlandGreeningPermanentGrasslandNumber;
            PermanentGrasslandReduction = xbData.ScotlandGreeningPermanentGrasslandReduction;
            EFANumber = xbData.ScotlandGreeningEFANumber;
            EFAReduction = xbData.ScotlandGreeningEFAReduction;
            EFAReductionAdditional = xbData.ScotlandGreeningEFAReductionAdditional;
            AdministrativeArea = xbData.ScotlandGreeningAdministrativeArea;
            AdministrativeReduction = xbData.ScotlandGreeningAdministrativeReduction;
            LateApplicationPercent = xbData.ScotlandGreeningLateApplicationPercent;
            LateApplicationReduction = xbData.ScotlandGreeningLateApplicationReduction;
            LateAmendmentPercent = xbData.ScotlandGreeningLateAmendmentPercent;
            LateAmendmentReduction = xbData.ScotlandGreeningLateAmendmentReduction;
            LateEvidencePercent = xbData.ScotlandGreeningLateEvidencePercent;
            LateEvidenceReduction = xbData.ScotlandGreeningLateEvidenceReduction;
            NonDeclarationPercent = xbData.ScotlandGreeningNonDeclarationPercent;
            NonDeclarationReduction = xbData.ScotlandGreeningNonDeclarationReduction;
            LateChangePenaltyReduction = xbData.ScotlandGreeningLateAmendmentReduction;
            FDMReduction = xbData.ScotlandGreeningFDMReduction;
            TotalGreeningPayment = xbData.ScotlandGreeningTotalGreeningPayment;
            GreeningGross = xbData.ScotlandGreeningGross;

            Calculate();
        }

        public override void Calculate()
        {
            int scotlandGreeningRegions = 0;

            if (Region1Number > 0)
            {
                scotlandGreeningRegions++;
            }
            if (Region2Number > 0)
            {
                scotlandGreeningRegions++;
            }
            if (Region3Number > 0)
            {
                scotlandGreeningRegions++;
            }

            Regions = scotlandGreeningRegions;

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

            TotalNumber = Region1Number + Region2Number + Region3Number;
            TotalTotal = Region1Total + Region2Total + Region3Total;

            CropDiversificationTotal = GreeningGross - CropDiversificationReduction;

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
