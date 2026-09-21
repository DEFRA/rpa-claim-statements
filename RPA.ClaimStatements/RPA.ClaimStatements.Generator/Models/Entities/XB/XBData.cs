using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.Entities.XB
{
    [Table("XBData", Schema = "XB")]
    public class XBData
    {
        public Guid XBDataID { get; set; }

        public Int64 FRN { get; set; }

        public int SBI { get; set; }

        public string InvoiceNumber { get; set; }

        public DateTime CalculationDate { get; set; }

        public int SchemeYear { get; set; }

        public decimal EnglandBPSValue { get; set; }

        public decimal EnglandGreeningValue { get; set; }

        public decimal EnglandYoungFarmerValue { get; set; }

        public decimal EnglandSubTotal { get; set; }

        public decimal EnglandCrossCompliancePercent { get; set; }

        public decimal EnglandCrossComplianceReduction { get; set; }

        public decimal EnglandCrossComplianceTotal { get; set; }

        public decimal EnglandTotalEuro { get; set; }

        public decimal WalesBPSValue { get; set; }

        public decimal WalesGreeningValue { get; set; }

        public decimal WalesYoungFarmerValue { get; set; }

        public decimal WalesRedistributiveValue { get; set; }

        public decimal WalesSubTotal { get; set; }

        public decimal WalesCrossCompliancePercent { get; set; }

        public decimal WalesCrossComplianceReduction { get; set; }

        public decimal WalesCrossComplianceTotal { get; set; }

        public decimal WalesTotalEuro { get; set; }

        public decimal ScotlandBPSValue { get; set; }

        public decimal ScotlandGreeningValue { get; set; }

        public decimal ScotlandYoungFarmerValue { get; set; }

        public decimal ScotlandSubTotal { get; set; }

        public decimal ScotlandCrossCompliancePercent { get; set; }

        public decimal ScotlandCrossComplianceReduction { get; set; }

        public decimal ScotlandCrossComplianceTotal { get; set; }

        public decimal ScotlandTotalEuro { get; set; }

        public decimal NIBPSValue { get; set; }

        public decimal NIGreeningValue { get; set; }

        public decimal NIYoungFarmerValue { get; set; }

        public decimal NISubTotal { get; set; }

        public decimal NICrossCompliancePercent { get; set; }

        public decimal NICrossComplianceReduction { get; set; }

        public decimal NICrossComplianceTotal { get; set; }

        public decimal NITotalEuro { get; set; }

        public decimal EnglandBPSNonSDANumber { get; set; }

        public decimal EnglandBPSNonSDARate { get; set; }

        public decimal EnglandBPSNonSDATotal { get; set; }

        public decimal EnglandBPSSDANumber { get; set; }

        public decimal EnglandBPSSDARate { get; set; }

        public decimal EnglandBPSSDATotal { get; set; }

        public decimal EnglandBPSMoorlandNumber { get; set; }

        public decimal EnglandBPSMoorlandRate { get; set; }

        public decimal EnglandBPSMoorlandTotal { get; set; }

        public decimal EnglandBPSAverageEntitlementValue2015Value { get; set; }

        public decimal EnglandBPSOverDeclarationHectares { get; set; }

        public decimal EnglandBPSOverDeclarationPercent { get; set; }

        public decimal EnglandBPSOverDeclarationReduction { get; set; }

        public decimal EnglandBPSLateClaimSubmissionPercent { get; set; }

        public decimal EnglandBPSLateClaimSubmissionReduction { get; set; }

        public decimal EnglandBPSLateEntitlementsApplicationPercent { get; set; }

        public decimal EnglandBPSLateEntitlementsApplicationReduction { get; set; }

        public decimal EnglandBPSLateAmendmentPercent { get; set; }

        public decimal EnglandBPSLateAmendmentReduction { get; set; }

        public decimal EnglandBPSLateEvidencePercent { get; set; }

        public decimal EnglandBPSLateEvidenceReduction { get; set; }

        public decimal EnglandBPSLateEntitlementsAmendmentPercent { get; set; }

        public decimal EnglandBPSLateEntitlementsAmendmentReduction { get; set; }

        public decimal EnglandBPSNonDeclarationPercent { get; set; }

        public decimal EnglandBPSNonDeclarationReduction { get; set; }

        public decimal EnglandBPSFDMPercent { get; set; }

        public decimal EnglandBPSFDMReduction { get; set; }

        public decimal EnglandBPSFDMTotal { get; set; }

        public decimal EnglandBPSReductionOfPaymentsOver150kPercent { get; set; }

        public decimal EnglandBPSReductionOfPaymentsOver150kDeduction { get; set; }

        public decimal EnglandBPSTotalBPSPayment { get; set; }

        public decimal EnglandBPSBPSGross { get; set; }

        public decimal WalesBPSRegion1Number { get; set; }

        public decimal WalesBPSRegion1Rate { get; set; }

        public decimal WalesBPSRegion1Total { get; set; }

        public decimal WalesBPSOverDeclarationHectares { get; set; }

        public decimal WalesBPSOverDeclarationPercent { get; set; }

        public decimal WalesBPSOverDeclarationReduction { get; set; }

        public decimal WalesBPSLateClaimSubmissionPercent { get; set; }

        public decimal WalesBPSLateClaimSubmissionReduction { get; set; }

        public decimal WalesBPSLateEntitlementsApplicationPercent { get; set; }

        public decimal WalesBPSLateEntitlementsApplicationReduction { get; set; }

        public decimal WalesBPSLateAmendmentPercent { get; set; }

        public decimal WalesBPSLateAmendmentReduction { get; set; }

        public decimal WalesBPSLateEntitlementsAmendmentPercent { get; set; }

        public decimal WalesBPSLateEntitlementsAmendmentReduction { get; set; }

        public decimal WalesBPSLateEvidencePercent { get; set; }

        public decimal WalesBPSLateEvidenceReduction { get; set; }

        public decimal WalesBPSLateEvidenceEntitlementsPercent { get; set; }

        public decimal WalesBPSLateEvidenceEntitlementsReduction { get; set; }

        public decimal WalesBPSNonDeclarationPercent { get; set; }

        public decimal WalesBPSNonDeclarationReduction { get; set; }

        public decimal WalesBPSFDMPercent { get; set; }

        public decimal WalesBPSFDMReduction { get; set; }

        public decimal WalesBPSReductionOfPaymentsOver150kPercent { get; set; }

        public decimal WalesBPSReductionOfPaymentsOver150kDeduction { get; set; }

        public decimal WalesBPSReductionOfPaymentsOver200kPercent { get; set; }

        public decimal WalesBPSReductionOfPaymentsOver200kDeduction { get; set; }

        public decimal WalesBPSReductionOfPaymentsOver250kPercent { get; set; }

        public decimal WalesBPSReductionOfPaymentsOver250kDeduction { get; set; }

        public decimal WalesBPSReductionOfPaymentsOver300kPercent { get; set; }

        public decimal WalesBPSReductionOfPaymentsOver300kDeduction { get; set; }

        public decimal WalesBPSTotalBPSPayment { get; set; }

        public decimal ScotlandBPSRegion1Number { get; set; }

        public decimal ScotlandBPSRegion1Rate { get; set; }

        public decimal ScotlandBPSRegion1Total { get; set; }

        public decimal ScotlandBPSRegion2Number { get; set; }

        public decimal ScotlandBPSRegion2Rate { get; set; }

        public decimal ScotlandBPSRegion2Total { get; set; }

        public decimal ScotlandBPSRegion3Number { get; set; }

        public decimal ScotlandBPSRegion3Rate { get; set; }

        public decimal ScotlandBPSRegion3Total { get; set; }

        public decimal ScotlandBPSAverageEntitlementValue2015Value { get; set; }

        public decimal ScotlandBPSOverDeclarationHectares { get; set; }

        public decimal ScotlandBPSOverDeclarationPercent { get; set; }

        public decimal ScotlandBPSOverDeclarationReduction { get; set; }

        public decimal ScotlandBPSLateClaimSubmissionPercent { get; set; }

        public decimal ScotlandBPSLateClaimSubmissionReduction { get; set; }

        public decimal ScotlandBPSLateEntitlementsApplicationPercent { get; set; }

        public decimal ScotlandBPSLateEntitlementsApplicationReduction { get; set; }

        public decimal ScotlandBPSLateAmendmentPercent { get; set; }

        public decimal ScotlandBPSLateAmendmentReduction { get; set; }

        public decimal ScotlandBPSLateEntitlementsAmendmentPercent { get; set; }

        public decimal ScotlandBPSLateEntitlementsAmendmentReduction { get; set; }

        public decimal ScotlandBPSLateEvidencePercent { get; set; }

        public decimal ScotlandBPSLateEvidenceReduction { get; set; }

        public decimal ScotlandBPSLateEvidenceEntitlementsPercent { get; set; }

        public decimal ScotlandBPSLateEvidenceEntitlementsReduction { get; set; }

        public decimal ScotlandBPSNonDeclarationPercent { get; set; }

        public decimal ScotlandBPSNonDeclarationReduction { get; set; }

        public decimal ScotlandBPSFDMPercent { get; set; }

        public decimal ScotlandBPSFDMReduction { get; set; }

        public decimal ScotlandBPSReductionOfPaymentsOver150kPercent { get; set; }

        public decimal ScotlandBPSReductionOfPaymentsOver150kDeduction { get; set; }

        public decimal ScotlandBPSTotalBPSPayment { get; set; }

        public decimal ScotlandBPSGross { get; set; }

        public decimal NIBPSRegion1Number { get; set; }

        public decimal NIBPSRegion1Rate { get; set; }

        public decimal NIBPSRegion1Total { get; set; }

        public decimal NIBPSOverDeclarationHectares { get; set; }

        public decimal NIBPSOverDeclarationPercent { get; set; }

        public decimal NIBPSOverDeclarationReduction { get; set; }

        public decimal NIBPSLateClaimSubmissionPercent { get; set; }

        public decimal NIBPSLateClaimSubmissionReduction { get; set; }

        public decimal NIBPSLateEntitlementsApplicationPercent { get; set; }

        public decimal NIBPSLateEntitlementsApplicationReduction { get; set; }

        public decimal NIBPSLateAmendmentPercent { get; set; }

        public decimal NIBPSLateAmendmentReduction { get; set; }

        public decimal NIBPSLateEntitlementsAmendmentPercent { get; set; }

        public decimal NIBPSLateEntitlementsAmendmentReduction { get; set; }

        public decimal NIBPSLateEvidencePercent { get; set; }

        public decimal NIBPSLateEvidenceReduction { get; set; }

        public decimal NIBPSLateEvidenceEntitlementsPercent { get; set; }

        public decimal NIBPSLateEvidenceEntitlementsReduction { get; set; }

        public decimal NIBPSNonDeclarationPercent { get; set; }

        public decimal NIBPSNonDeclarationReduction { get; set; }

        public decimal NIBPSFDMPercent { get; set; }

        public decimal NIBPSFDMReduction { get; set; }

        public decimal NIBPSReductionOfPaymentsOver150kPercent { get; set; }

        public decimal NIBPSReductionOfPaymentsOver150kDeduction { get; set; }

        public decimal NIBPSTotalBPSPayment { get; set; }

        public decimal EnglandGreeningNonSDANumber { get; set; }

        public decimal EnglandGreeningNonSDARate { get; set; }

        public decimal EnglandGreeningNonSDAReduction { get; set; }

        public decimal EnglandGreeningNonSDATotal { get; set; }

        public decimal EnglandGreeningSDANumber { get; set; }

        public decimal EnglandGreeningSDARate { get; set; }

        public decimal EnglandGreeningSDAReduction { get; set; }

        public decimal EnglandGreeningSDATotal { get; set; }

        public decimal EnglandGreeningMoorlandNumber { get; set; }

        public decimal EnglandGreeningMoorlandReduction { get; set; }

        public decimal EnglandGreeningMoorlandRate { get; set; }

        public decimal EnglandGreeningMoorlandTotal { get; set; }

        public decimal EnglandGreeningAverageGreeningValue2015Value { get; set; }

        public decimal EnglandGreeningCropDiversificationNumber { get; set; }

        public decimal EnglandGreeningCropDiversificationReduction { get; set; }

        public decimal EnglandGreeningPermanentGrasslandNumber { get; set; }

        public decimal EnglandGreeningPermanentGrasslandReduction { get; set; }

        public decimal EnglandGreeningEFANumber { get; set; }

        public decimal EnglandGreeningEFAReduction { get; set; }

        public decimal EnglandGreeningLateApplicationPercent { get; set; }

        public decimal EnglandGreeningLateApplicationReduction { get; set; }

        public decimal EnglandGreeningLateEvidencePercent { get; set; }

        public decimal EnglandGreeningLateEvidenceReduction { get; set; }

        public decimal EnglandGreeningLateChangePenalty { get; set; }

        public decimal EnglandGreeningNonDeclarationPercent { get; set; }

        public decimal EnglandGreeningNonDeclarationReduction { get; set; }

        public decimal EnglandGreeningFDMPercent { get; set; }

        public decimal EnglandGreeningFDMReduction { get; set; }

        public decimal EnglandGreeningTotalGreeningPayment { get; set; }

        public decimal EnglandGreeningGross { get; set; }

        public decimal WalesGreeningRegion1Number { get; set; }

        public decimal WalesGreeningRegion1Rate { get; set; }

        public decimal WalesGreeningRegion1Total { get; set; }

        public decimal WalesGreeningCropDiversificationNumber { get; set; }

        public decimal WalesGreeningCropDiversificationReduction { get; set; }

        public decimal WalesGreeningPermanentGrasslandNumber { get; set; }

        public decimal WalesGreeningPermanentGrasslandReduction { get; set; }

        public decimal WalesGreeningEFANumber { get; set; }

        public decimal WalesGreeningEFAReduction { get; set; }

        public decimal WalesGreeningLateApplicationPercent { get; set; }

        public decimal WalesGreeningLateApplicationReduction { get; set; }

        public decimal WalesGreeningLateAmendmentPercent { get; set; }

        public decimal WalesGreeningLateAmendmentReduction { get; set; }

        public decimal WalesGreeningLateEvidencePercent { get; set; }

        public decimal WalesGreeningLateEvidenceReduction { get; set; }

        public decimal WalesGreeningNonDeclarationPercent { get; set; }

        public decimal WalesGreeningNonDeclarationReduction { get; set; }

        public decimal WalesGreeningFDMPercent { get; set; }

        public decimal WalesGreeningFDMReduction { get; set; }

        public decimal WalesGreeningTotalGreeningPayment { get; set; }

        public decimal ScotlandGreeningRegion1Number { get; set; }

        public decimal ScotlandGreeningRegion1Rate { get; set; }

        public decimal ScotlandGreeningRegion1Total { get; set; }

        public decimal ScotlandGreeningRegion2Number { get; set; }

        public decimal ScotlandGreeningRegion2Rate { get; set; }

        public decimal ScotlandGreeningRegion2Total { get; set; }

        public decimal ScotlandGreeningRegion3Number { get; set; }

        public decimal ScotlandGreeningRegion3Rate { get; set; }

        public decimal ScotlandGreeningRegion3Total { get; set; }

        public decimal ScotlandGreeningAverageGreeningValue2015Value { get; set; }

        public decimal ScotlandGreeningCropDiversificationNumber { get; set; }

        public decimal ScotlandGreeningCropDiversificationReduction { get; set; }

        public decimal ScotlandGreeningPermanentGrasslandNumber { get; set; }

        public decimal ScotlandGreeningPermanentGrasslandReduction { get; set; }

        public decimal ScotlandGreeningEFANumber { get; set; }

        public decimal ScotlandGreeningEFAReduction { get; set; }

        public decimal ScotlandGreeningLateApplicationPercent { get; set; }

        public decimal ScotlandGreeningLateApplicationReduction { get; set; }

        public decimal ScotlandGreeningLateAmendmentPercent { get; set; }

        public decimal ScotlandGreeningLateAmendmentReduction { get; set; }

        public decimal ScotlandGreeningLateEvidencePercent { get; set; }

        public decimal ScotlandGreeningLateEvidenceReduction { get; set; }

        public decimal ScotlandGreeningNonDeclarationPercent { get; set; }

        public decimal ScotlandGreeningNonDeclarationReduction { get; set; }

        public decimal ScotlandGreeningFDMPercent { get; set; }

        public decimal ScotlandGreeningFDMReduction { get; set; }

        public decimal ScotlandGreeningTotalGreeningPayment { get; set; }

        public decimal ScotlandGreeningGross { get; set; }

        public decimal NIGreeningRegion1Number { get; set; }

        public decimal NIGreeningRegion1Rate { get; set; }

        public decimal NIGreeningRegion1Total { get; set; }

        public decimal NIGreeningCropDiversificationNumber { get; set; }

        public decimal NIGreeningCropDiversificationReduction { get; set; }

        public decimal NIGreeningPermanentGrasslandNumber { get; set; }

        public decimal NIGreeningPermanentGrasslandReduction { get; set; }

        public decimal NIGreeningEFANumber { get; set; }

        public decimal NIGreeningEFAReduction { get; set; }

        public decimal NIGreeningLateApplicationPercent { get; set; }

        public decimal NIGreeningLateApplicationReduction { get; set; }

        public decimal NIGreeningLateAmendmentPercent { get; set; }

        public decimal NIGreeningLateAmendmentReduction { get; set; }

        public decimal NIGreeningLateEvidencePercent { get; set; }

        public decimal NIGreeningLateEvidenceReduction { get; set; }

        public decimal NIGreeningNonDeclarationPercent { get; set; }

        public decimal NIGreeningNonDeclarationReduction { get; set; }

        public decimal NIGreeningFDMPercent { get; set; }

        public decimal NIGreeningFDMReduction { get; set; }

        public decimal NIGreeningTotalGreeningPayment { get; set; }

        public decimal EnglandYFEntitlementsUsedToClaim { get; set; }

        public decimal EnglandYFAvgEntitlementValue { get; set; }

        public decimal EnglandYFYFClaimValuePercent { get; set; }

        public decimal EnglandYFClaimValue { get; set; }

        public decimal EnglandYFOverDeclarationArea { get; set; }

        public decimal EnglandYFOverDeclarationPercent { get; set; }

        public decimal EnglandYFOverDeclarationReduction { get; set; }

        public decimal EnglandYFLateClaimSubmissionPercent { get; set; }

        public decimal EnglandYFLateClaimSubmissionReduction { get; set; }

        public decimal EnglandYFLateAmendmentPercent { get; set; }

        public decimal EnglandYFLateAmendmentReduction { get; set; }

        public decimal EnglandYFNonDeclarationPercent { get; set; }

        public decimal EnglandYFNonDeclarationReduction { get; set; }

        public decimal EnglandYFFDMPercent { get; set; }

        public decimal EnglandYFFDMReduction { get; set; }

        public decimal EnglandYFTotalYoungFarmerPayment { get; set; }

        public decimal WalesYFEntitlementsUsedToClaim { get; set; }

        public decimal WalesYFRate { get; set; }

        public decimal WalesYFClaimValue { get; set; }

        public decimal WalesYFOverDeclarationArea { get; set; }

        public decimal WalesYFOverDeclarationPercent { get; set; }

        public decimal WalesYFOverDeclarationReduction { get; set; }

        public decimal WalesYFDeclarationArea { get; set; }

        public decimal WalesYFDeclarationReduction { get; set; }

        public decimal WalesYFDeclarationTotal { get; set; }

        public decimal WalesYFLateClaimSubmissionPercent { get; set; }

        public decimal WalesYFLateClaimSubmissionReduction { get; set; }

        public decimal WalesYFLateAmendmentPercent { get; set; }

        public decimal WalesYFLateAmendmentReduction { get; set; }

        public decimal WalesYFLateEvidencePercent { get; set; }

        public decimal WalesYFLateEvidenceReduction { get; set; }

        public decimal WalesYFNonDeclarationPercent { get; set; }

        public decimal WalesYFNonDeclarationReduction { get; set; }

        public decimal WalesYFFDMPercent { get; set; }

        public decimal WalesYFFDMReduction { get; set; }

        public decimal WalesYFTotalYoungFarmerPayment { get; set; }

        public decimal ScotlandYFEntitlementsUsedToClaim { get; set; }

        public decimal ScotlandYFAvgEntitlementValue { get; set; }

        public decimal ScotlandYFClaimValuePercent { get; set; }

        public decimal ScotlandYFClaimValue { get; set; }

        public decimal ScotlandYFOverDeclarationArea { get; set; }

        public decimal ScotlandYFOverDeclarationPercent { get; set; }

        public decimal ScotlandYFOverDeclarationReduction { get; set; }

        public decimal ScotlandYFDeclarationArea { get; set; }

        public decimal ScotlandYFDeclarationReduction { get; set; }

        public decimal ScotlandYFLateClaimSubmissionPercent { get; set; }

        public decimal ScotlandYFLateClaimSubmissionReduction { get; set; }

        public decimal ScotlandYFLateAmendmentPercent { get; set; }

        public decimal ScotlandYFLateAmendmentReduction { get; set; }

        public decimal ScotlandYFLateEvidencePercent { get; set; }

        public decimal ScotlandYFLateEvidenceReduction { get; set; }

        public decimal ScotlandYFNonDeclarationPercent { get; set; }

        public decimal ScotlandYFNonDeclarationReduction { get; set; }

        public decimal ScotlandYFFDMPercent { get; set; }

        public decimal ScotlandYFFDMReduction { get; set; }

        public decimal ScotlandYFTotalYoungFarmerPayment { get; set; }

        public decimal NIYFEntitlementsUsedToClaim { get; set; }

        public decimal NIYFRate { get; set; }

        public decimal NIYFClaimValue { get; set; }

        public decimal NIYFOverDeclarationArea { get; set; }

        public decimal NIYFOverDeclarationPercent { get; set; }

        public decimal NIYFOverDeclarationReduction { get; set; }

        public decimal NIYFDeclarationPenalty { get; set; }

        public decimal NIYFDeclarationReduction { get; set; }

        public decimal NIYFLateClaimSubmissionPercent { get; set; }

        public decimal NIYFLateClaimSubmissionReduction { get; set; }

        public decimal NIYFLateAmendmentPercent { get; set; }

        public decimal NIYFLateAmendmentReduction { get; set; }

        public decimal NIYFLateEvidencePercent { get; set; }

        public decimal NIYFLateEvidenceReduction { get; set; }

        public decimal NIYFNonDeclarationPercent { get; set; }

        public decimal NIYFNonDeclarationReduction { get; set; }

        public decimal NIYFFDMPercent { get; set; }

        public decimal NIYFFDMReduction { get; set; }

        public decimal NIYFTotalYoungFarmerPayment { get; set; }

        public decimal WalesRedLandUsedToClaim { get; set; }

        public decimal WalesRedClaimValue { get; set; }

        public decimal WalesRedOverDeclarationArea { get; set; }

        public decimal WalesRedOverDeclarationPercent { get; set; }

        public decimal WalesRedOverDeclarationReduction { get; set; }

        public decimal WalesRedLateClaimSubmissionPercent { get; set; }

        public decimal WalesRedLateClaimSubmissionReduction { get; set; }

        public decimal WalesRedLateAmendmentPercent { get; set; }

        public decimal WalesRedLateAmendmentReduction { get; set; }

        public decimal WalesRedLateEvidencePercent { get; set; }

        public decimal WalesRedLateEvidenceReduction { get; set; }

        public decimal WalesRedNonDeclarationPercent { get; set; }

        public decimal WalesRedNonDeclarationReduction { get; set; }

        public decimal WalesRedFDMPercent { get; set; }

        public decimal WalesRedFDMReduction { get; set; }

        public decimal WalesRedTotalRedistributionPayment { get; set; }

        public decimal EnglandNonSDAAreaOnApplication { get; set; }

        public decimal EnglandNonSDAAreaIneligible { get; set; }

        public decimal EnglandNonSDAEntitlements { get; set; }

        public decimal EnglandSDAAreaOnApplication { get; set; }

        public decimal EnglandSDAAreaIneligible { get; set; }

        public decimal EnglandSDAEntitlements { get; set; }

        public decimal EnglandMoorlandAreaOnApplication { get; set; }

        public decimal EnglandMoorlandAreaIneligible { get; set; }

        public decimal EnglandMoorlandEntitlements { get; set; }

        public decimal WalesAreaOnApplication { get; set; }

        public decimal WalesAreaIneligible { get; set; }

        public decimal WalesEntitlements { get; set; }

        public decimal ScotlandRegion1AreaOnApplication { get; set; }

        public decimal ScotlandRegion1AreaIneligible { get; set; }

        public decimal ScotlandRegion1Entitlements { get; set; }

        public decimal ScotlandRegion2AreaOnApplication { get; set; }

        public decimal ScotlandRegion2AreaIneligible { get; set; }

        public decimal ScotlandRegion2Entitlements { get; set; }

        public decimal ScotlandRegion3AreaOnApplication { get; set; }

        public decimal ScotlandRegion3AreaIneligible { get; set; }

        public decimal ScotlandRegion3Entitlements { get; set; }

        public decimal NIAreaOnApplication { get; set; }

        public decimal NIAreaIneligible { get; set; }

        public decimal NIEntitlements { get; set; }

        public decimal EnglandBPSAdditionalOverDeclarationReduction { get; set; }

        public bool EnglandBPSPreviousYearOverDeclaration { get; set; }

        public decimal WalesBPSAdditionalOverDeclarationReduction { get; set; }

        public bool WalesBPSPreviousYearOverDeclaration { get; set; }

        public decimal ScotlandBPSAdditionalOverDeclarationReduction { get; set; }

        public bool ScotlandBPSPreviousYearOverDeclaration { get; set; }

        public decimal NIBPSAdditionalOverDeclarationReduction { get; set; }

        public bool NIBPSPreviousYearOverDeclaration { get; set; }

        public decimal EnglandYFAdditionalOverDeclarationReduction { get; set; }

        public bool EnglandYFPreviousYearOverDeclaration { get; set; }

        public decimal WalesYFAdditionalOverDeclarationReduction { get; set; }

        public bool WalesYFPreviousYearOverDeclaration { get; set; }

        public decimal ScotlandYFAdditionalOverDeclarationReduction { get; set; }

        public bool ScotlandYFPreviousYearOverDeclaration { get; set; }

        public decimal NIYFAdditionalOverDeclarationReduction { get; set; }

        public bool NIYFPreviousYearOverDeclaration { get; set; }

        public decimal WalesRedAdditionalOverDeclarationReduction { get; set; }

        public bool WalesRedPreviousYearOverDeclaration { get; set; }

        public decimal EnglandGreeningAdministrativeArea { get; set; }

        public decimal EnglandGreeningAdministrativeReduction { get; set; }

        public decimal WalesGreeningAdministrativeArea { get; set; }

        public decimal WalesGreeningAdministrativeReduction { get; set; }

        public decimal ScotlandGreeningAdministrativeArea { get; set; }

        public decimal ScotlandGreeningAdministrativeReduction { get; set; }

        public decimal NIGreeningAdministrativeArea { get; set; }

        public decimal NIGreeningAdministrativeReduction { get; set; }

        public decimal EnglandGreeningCropDiversificationReductionAdditional { get; set; }

        public decimal ScotlandGreeningCropDiversificationReductionAdditional { get; set; }

        public decimal WalesGreeningCropDiversificationReductionAdditional { get; set; }

        public decimal NIGreeningCropDiversificationReductionAdditional { get; set; }

        public decimal EnglandGreeningEFAReductionAdditional { get; set; }
        
        public decimal ScotlandGreeningEFAReductionAdditional { get; set; }
          
        public decimal WalesGreeningEFAReductionAdditional { get; set; }
        
        public decimal NIGreeningEFAReductionAdditional { get; set; }

        public XBData()
        {
            XBDataID = Guid.NewGuid();
        }

        public XBData(long frn, int schemeYear):this()
        {
            FRN = frn;
            SchemeYear = schemeYear;
        }

        public XBData(long frn, int schemeYear, string invoice, DateTime calculationDate) : this(frn, schemeYear)
        {
            InvoiceNumber = invoice;
            CalculationDate = calculationDate;
        }

        public XBData(long frn, int schemeYear, string invoice, DateTime calculationDate, decimal englandTotalEuro, decimal walesTotalEuro, decimal scotlandTotalEuro, decimal niTotalEuro) : this(frn, schemeYear, invoice, calculationDate)
        {
            EnglandTotalEuro = englandTotalEuro;
            WalesTotalEuro = walesTotalEuro;
            ScotlandTotalEuro = scotlandTotalEuro;
            NITotalEuro = niTotalEuro;
        }
    }
}