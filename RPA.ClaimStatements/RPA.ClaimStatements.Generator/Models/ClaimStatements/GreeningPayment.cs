using RPA.ClaimStatements.Generator.Models.Entities.SITI;
using RPA.ClaimStatements.Generator.Models.Entities.XB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Models.ClaimStatements
{
    public class GreeningPayment
    {
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

        public decimal AverageGreeningValue2015Value { get; set; }

        public decimal CropDiversificationNumber { get; set; }

        public decimal CropDiversificatonRate { get; set; }

        public decimal CropDiversificatonReduction { get; set; }

        public decimal CropDiversificationTotal { get; set; }

        public decimal CropDiversificationNumberAdditional { get; set; }

        public decimal CropDiversificationReductionAdditional { get; set; }

        public decimal CropDiversificationTotalAdditional { get; set; }
        
        public decimal PermanentGrasslandNumber { get; set; }

        public decimal PermanentGrasslandRate { get; set; }

        public decimal PermanentGrasslandReduction { get; set; }

        public decimal PermanentGrasslandTotal { get; set; }

        public decimal EFANumber { get; set; }

        public decimal EFANumberAdditional { get; set; }

        public decimal EFARate { get; set; }

        public decimal EFAReduction { get; set; }

        public decimal EFAReductionAdditional { get; set; }

        public decimal EFATotal { get; set; }

        public decimal EFATotalAdditional { get; set; }

        public decimal AdministrativeArea { get; set; }

        public decimal AdministrativeRate { get; set; }

        public decimal AdministrativeReduction { get; set; }

        public decimal AdministrativeTotal { get; set; }

        public decimal LateApplicationPercent { get; set; }

        public decimal LateApplicationReduction { get; set; }

        public decimal LateApplicationTotal { get; set; }

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

        public decimal TotalGreeningPayment { get; set; }

        public string LastPenalty { get; set; }

        public string LastPenalty2 { get; set; }

        public decimal GreeningGross { get; set; }

        public void Build(SUM2 sum2, GR gr, GRPEN grpen)
        {
            NonSDANumber = gr.NonSDANumber;
            NonSDATotal = gr.NonSDATotal;
            SDANumber = gr.SDANumber;
            SDATotal = gr.SDATotal;
            MoorlandNumber = gr.MoorlandNumber;
            MoorlandTotal = gr.MoorlandTotal;
            AverageGreeningValue2015Value = gr.AvgGreeningRate;
            CropDiversificationNumber = grpen.CropDiversificationNumber;
            CropDiversificationNumberAdditional = grpen.CropDiversificationNumberAdditional;
            CropDiversificatonRate = grpen.CropDiversificationRate;
            CropDiversificatonReduction = grpen.CropDiversificationReduction;
            CropDiversificationReductionAdditional = grpen.CropDiversificationNumberAdditional > 0 ? grpen.CropDiversificationReduction : 0;
            PermanentGrasslandNumber = grpen.PermanentGrasslandNumber;
            PermanentGrasslandRate = grpen.PermanentGrasslandRate;
            PermanentGrasslandReduction = grpen.PermanentGrasslandReduction;
            EFANumber = grpen.EFANumber;
            EFANumberAdditional = grpen.EFANumberAdditional;
            EFARate = grpen.EFARate;
            EFAReduction = grpen.EFAReduction;
            EFAReductionAdditional = grpen.EFANumberAdditional > 0 ? grpen.EFAReduction : 0;
            AdministrativeArea = grpen.AdministrativeArea;
            AdministrativeRate = gr.AvgGreeningRate;
            AdministrativeReduction = grpen.AdministrativeReduction;
            LateApplicationPercent = grpen.LateApplicationPercent;
            LateApplicationReduction = grpen.LateApplicationReduction;
            LateEvidencePercent = grpen.LateEvidencePercent;
            LateEvidenceReduction = grpen.LateEvidenceReduction;
            NonDeclarationPercent = grpen.NonDeclarationPercent;
            NonDeclarationReduction = grpen.NonDeclarationReduction;
            LateChangePenaltyReduction = grpen.LateChangePenaltyReduction;
            FDMReduction = grpen.FDMReduction;
            TotalGreeningPayment = grpen.TotalGreening;
            GreeningGross = sum2.GreeningValue;

            Calculate();
        }

        public virtual void Build(XBData xbData)
        {
            NonSDANumber = xbData.EnglandGreeningNonSDANumber;
            NonSDARate = xbData.EnglandGreeningAverageGreeningValue2015Value;
            NonSDATotal = xbData.EnglandGreeningNonSDATotal;
            SDANumber = 0;
            SDARate = 0;
            SDATotal = 0;
            MoorlandNumber = 0;
            MoorlandRate = 0;
            MoorlandTotal = 0;
            AverageGreeningValue2015Value = xbData.EnglandGreeningAverageGreeningValue2015Value;
            CropDiversificationNumber = xbData.EnglandGreeningCropDiversificationNumber;
            CropDiversificatonReduction = xbData.EnglandGreeningCropDiversificationReduction;
            CropDiversificationReductionAdditional = xbData.EnglandGreeningCropDiversificationReductionAdditional;
            PermanentGrasslandNumber = xbData.EnglandGreeningPermanentGrasslandNumber;
            PermanentGrasslandReduction = xbData.EnglandGreeningPermanentGrasslandReduction;
            EFANumber = xbData.EnglandGreeningEFANumber;
            EFAReduction = xbData.EnglandGreeningEFAReduction;
            EFAReductionAdditional = xbData.EnglandGreeningEFAReductionAdditional;
            AdministrativeArea = xbData.EnglandGreeningAdministrativeArea;
            AdministrativeReduction = xbData.EnglandGreeningAdministrativeReduction;
            LateApplicationPercent = xbData.EnglandGreeningLateApplicationPercent;
            LateApplicationReduction = xbData.EnglandGreeningLateApplicationReduction;
            LateEvidencePercent = xbData.EnglandGreeningLateEvidencePercent;
            LateEvidenceReduction = xbData.EnglandGreeningLateEvidenceReduction;
            NonDeclarationPercent = xbData.EnglandGreeningNonDeclarationPercent;
            NonDeclarationReduction = xbData.EnglandGreeningNonDeclarationReduction;
            LateChangePenaltyReduction = xbData.EnglandGreeningLateChangePenalty;
            FDMReduction = xbData.EnglandGreeningFDMReduction;
            TotalGreeningPayment = xbData.EnglandGreeningTotalGreeningPayment;
            GreeningGross = xbData.EnglandGreeningGross;

            Calculate();
        }

        public virtual void Calculate()
        {
            int greeningRegions = 0;

            if (NonSDANumber > 0)
            {
                greeningRegions++;
            }
            if (SDANumber > 0)
            {
                greeningRegions++;
            }
            if (MoorlandNumber > 0)
            {
                greeningRegions++;
            }

            Regions = greeningRegions;

            LastPenalty = "NoPenalties";

            if (CropDiversificatonReduction > 0)
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

            TotalNumber = NonSDANumber + SDANumber + MoorlandNumber;
            TotalTotal = NonSDATotal + SDATotal + MoorlandTotal;

            CropDiversificationTotal = GreeningGross - CropDiversificatonReduction;

            if (CropDiversificationTotal < 0)
            {
                CropDiversificationTotal = 0;
            }

            PermanentGrasslandTotal = CropDiversificationTotal- PermanentGrasslandReduction;

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
            LateEvidenceTotal = LateApplicationTotal - LateEvidenceReduction;

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
