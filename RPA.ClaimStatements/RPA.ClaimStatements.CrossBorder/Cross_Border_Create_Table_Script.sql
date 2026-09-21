USE [RPA.Finance.PaymentFilter.Integration (PreProd)]
GO

/****** Object:  Table [dbo].[claim_statement_data]    Script Date: 13/09/2018 12:54:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[claim_statement_data](
	[XBDataID] [uniqueidentifier] NOT NULL,
	[InstanceID] [int] NULL,
	[ImportSetID] [int] NULL,
	[SBI] [int] NOT NULL,
	[InvoiceNumber] [nvarchar](max) NULL,
	[CalculationDate] [datetime] NOT NULL,
	[SchemeYear] [int] NOT NULL,
	[EnglandBPSValue] [decimal](18, 2) NOT NULL,
	[EnglandGreeningValue] [decimal](18, 2) NOT NULL,
	[EnglandYoungFarmerValue] [decimal](18, 2) NOT NULL,
	[EnglandSubTotal] [decimal](18, 2) NOT NULL,
	[EnglandCrossCompliancePercent] [decimal](18, 2) NOT NULL,
	[EnglandCrossComplianceReduction] [decimal](18, 2) NOT NULL,
	[EnglandCrossComplianceTotal] [decimal](18, 2) NOT NULL,
	[EnglandTotalEuro] [decimal](18, 2) NOT NULL,
	[WalesBPSValue] [decimal](18, 2) NOT NULL,
	[WalesGreeningValue] [decimal](18, 2) NOT NULL,
	[WalesYoungFarmerValue] [decimal](18, 2) NOT NULL,
	[WalesRedistributiveValue] [decimal](18, 2) NOT NULL,
	[WalesSubTotal] [decimal](18, 2) NOT NULL,
	[WalesCrossCompliancePercent] [decimal](18, 2) NOT NULL,
	[WalesCrossComplianceReduction] [decimal](18, 2) NOT NULL,
	[WalesCrossComplianceTotal] [decimal](18, 2) NOT NULL,
	[WalesTotalEuro] [decimal](18, 2) NOT NULL,
	[ScotlandBPSValue] [decimal](18, 2) NOT NULL,
	[ScotlandGreeningValue] [decimal](18, 2) NOT NULL,
	[ScotlandYoungFarmerValue] [decimal](18, 2) NOT NULL,
	[ScotlandSubTotal] [decimal](18, 2) NOT NULL,
	[ScotlandCrossCompliancePercent] [decimal](18, 2) NOT NULL,
	[ScotlandCrossComplianceReduction] [decimal](18, 2) NOT NULL,
	[ScotlandCrossComplianceTotal] [decimal](18, 2) NOT NULL,
	[ScotlandTotalEuro] [decimal](18, 2) NOT NULL,
	[NIBPSValue] [decimal](18, 2) NOT NULL,
	[NIGreeningValue] [decimal](18, 2) NOT NULL,
	[NIYoungFarmerValue] [decimal](18, 2) NOT NULL,
	[NISubTotal] [decimal](18, 2) NOT NULL,
	[NICrossCompliancePercent] [decimal](18, 2) NOT NULL,
	[NICrossComplianceReduction] [decimal](18, 2) NOT NULL,
	[NICrossComplianceTotal] [decimal](18, 2) NOT NULL,
	[NITotalEuro] [decimal](18, 2) NOT NULL,
	[EnglandBPSNonSDANumber] [decimal](18, 4) NOT NULL,
	[EnglandBPSNonSDATotal] [decimal](18, 4) NOT NULL,
	[EnglandBPSSDANumber] [decimal](18, 4) NOT NULL,
	[EnglandBPSSDATotal] [decimal](18, 4) NOT NULL,
	[EnglandBPSMoorlandNumber] [decimal](18, 4) NOT NULL,
	[EnglandBPSMoorlandTotal] [decimal](18, 4) NOT NULL,
	[EnglandBPSAverageEntitlementValue2015Value] [decimal](18, 4) NOT NULL,
	[EnglandBPSOverDeclarationHectares] [decimal](18, 4) NOT NULL,
	[EnglandBPSOverDeclarationReduction] [decimal](18, 2) NOT NULL,
	[EnglandBPSLateClaimSubmissionPercent] [decimal](18, 2) NOT NULL,
	[EnglandBPSLateClaimSubmissionReduction] [decimal](18, 2) NOT NULL,
	[EnglandBPSLateEntitlementsApplicationPercent] [decimal](18, 2) NOT NULL,
	[EnglandBPSLateEntitlementsApplicationReduction] [decimal](18, 2) NOT NULL,
	[EnglandBPSLateAmendmentPercent] [decimal](18, 2) NOT NULL,
	[EnglandBPSLateAmendmentReduction] [decimal](18, 2) NOT NULL,
	[EnglandBPSLateEvidenceReduction] [decimal](18, 2) NOT NULL,
	[EnglandBPSLateEntitlementsAmendmentPercent] [decimal](18, 2) NOT NULL,
	[EnglandBPSLateEntitlementsAmendmentReduction] [decimal](18, 2) NOT NULL,
	[EnglandBPSFDMPercent] [decimal](18, 2) NOT NULL,
	[EnglandBPSFDMReduction] [decimal](18, 2) NOT NULL,
	[EnglandBPSFDMTotal] [decimal](18, 2) NOT NULL,
	[EnglandBPSReductionOfPaymentsOver150kPercent] [decimal](18, 2) NOT NULL,
	[EnglandBPSReductionOfPaymentsOver150kDeduction] [decimal](18, 2) NOT NULL,
	[EnglandBPSTotalBPSPayment] [decimal](18, 2) NOT NULL,
	[EnglandBPSBPSGross] [decimal](18, 2) NOT NULL,
	[EnglandBPSNonDeclarationPercent] [decimal](18, 2) NULL,
	[EnglandBPSNonDeclarationReduction] [decimal](18, 2) NULL,
	[EnglandGreeningNonDeclarationPercent] [decimal](18, 2) NULL,
	[EnglandGreeningNonDeclarationReduction] [decimal](18, 2) NULL,
	[EnglandYFNonDeclarationPercent] [decimal](18, 2) NULL,
	[EnglandYFNonDeclarationReduction] [decimal](18, 2) NULL,
	[WalesBPSRegion1Number] [decimal](18, 4) NOT NULL,
	[WalesBPSRegion1Total] [decimal](18, 2) NOT NULL,
	[WalesBPSOverDeclarationHectares] [decimal](18, 4) NOT NULL,
	[WalesBPSOverDeclarationReduction] [decimal](18, 2) NOT NULL,
	[WalesBPSLateClaimSubmissionPercent] [decimal](18, 2) NOT NULL,
	[WalesBPSLateClaimSubmissionReduction] [decimal](18, 2) NOT NULL,
	[WalesBPSLateEntitlementsApplicationPercent] [decimal](18, 2) NOT NULL,
	[WalesBPSLateEntitlementsApplicationReduction] [decimal](18, 2) NOT NULL,
	[WalesBPSLateAmendmentPercent] [decimal](18, 2) NOT NULL,
	[WalesBPSLateAmendmentReduction] [decimal](18, 2) NOT NULL,
	[WalesBPSLateEntitlementsAmendmentPercent] [decimal](18, 2) NOT NULL,
	[WalesBPSLateEntitlementsAmendmentReduction] [decimal](18, 2) NOT NULL,
	[WalesBPSLateEvidencePercent] [decimal](18, 2) NOT NULL,
	[WalesBPSLateEvidenceReduction] [decimal](18, 2) NOT NULL,
	[WalesBPSNonDeclarationPercent] [decimal](18, 2) NOT NULL,
	[WalesBPSNonDeclarationReduction] [decimal](18, 2) NOT NULL,
	[WalesBPSFDMPercent] [decimal](18, 2) NOT NULL,
	[WalesBPSFDMReduction] [decimal](18, 2) NOT NULL,
	[WalesBPSReductionOfPaymentsOver150kPercent] [decimal](18, 2) NOT NULL,
	[WalesBPSReductionOfPaymentsOver150kDeduction] [decimal](18, 2) NOT NULL,
	[WalesBPSReductionOfPaymentsOver200kPercent] [decimal](18, 2) NOT NULL,
	[WalesBPSReductionOfPaymentsOver200kDeduction] [decimal](18, 2) NOT NULL,
	[WalesBPSReductionOfPaymentsOver250kPercent] [decimal](18, 2) NOT NULL,
	[WalesBPSReductionOfPaymentsOver250kDeduction] [decimal](18, 2) NOT NULL,
	[WalesBPSReductionOfPaymentsOver300kPercent] [decimal](18, 2) NOT NULL,
	[WalesBPSReductionOfPaymentsOver300kDeduction] [decimal](18, 2) NOT NULL,
	[WalesBPSTotalBPSPayment] [decimal](18, 2) NOT NULL,
	[ScotlandBPSRegion1Number] [decimal](18, 4) NOT NULL,
	[ScotlandBPSRegion1Total] [decimal](18, 4) NOT NULL,
	[ScotlandBPSRegion2Number] [decimal](18, 4) NOT NULL,
	[ScotlandBPSRegion2Total] [decimal](18, 4) NOT NULL,
	[ScotlandBPSRegion3Number] [decimal](18, 4) NOT NULL,
	[ScotlandBPSRegion3Total] [decimal](18, 4) NOT NULL,
	[ScotlandBPSAverageEntitlementValue2015Value] [decimal](18, 2) NOT NULL,
	[ScotlandBPSOverDeclarationHectares] [decimal](18, 4) NOT NULL,
	[ScotlandBPSOverDeclarationReduction] [decimal](18, 2) NOT NULL,
	[ScotlandBPSLateClaimSubmissionPercent] [decimal](18, 2) NOT NULL,
	[ScotlandBPSLateClaimSubmissionReduction] [decimal](18, 2) NOT NULL,
	[ScotlandBPSLateEntitlementsApplicationPercent] [decimal](18, 2) NOT NULL,
	[ScotlandBPSLateEntitlementsApplicationReduction] [decimal](18, 2) NOT NULL,
	[ScotlandBPSLateAmendmentPercent] [decimal](18, 2) NOT NULL,
	[ScotlandBPSLateAmendmentReduction] [decimal](18, 2) NOT NULL,
	[ScotlandBPSLateEntitlementsAmendmentPercent] [decimal](18, 2) NOT NULL,
	[ScotlandBPSLateEntitlementsAmendmentReduction] [decimal](18, 2) NOT NULL,
	[ScotlandBPSLateEvidenceReduction] [decimal](18, 2) NOT NULL,
	[ScotlandBPSNonDeclarationPercent] [decimal](18, 2) NOT NULL,
	[ScotlandBPSNonDeclarationReduction] [decimal](18, 2) NOT NULL,
	[ScotlandBPSFDMPercent] [decimal](18, 2) NOT NULL,
	[ScotlandBPSFDMReduction] [decimal](18, 2) NOT NULL,
	[ScotlandBPSReductionOfPaymentsOver150kPercent] [decimal](18, 2) NOT NULL,
	[ScotlandBPSReductionOfPaymentsOver150kDeduction] [decimal](18, 2) NOT NULL,
	[ScotlandBPSTotalBPSPayment] [decimal](18, 2) NOT NULL,
	[ScotlandBPSGross] [decimal](18, 2) NOT NULL,
	[NIBPSRegion1Number] [decimal](18, 4) NOT NULL,
	[NIBPSRegion1Total] [decimal](18, 4) NOT NULL,
	[NIBPSOverDeclarationHectares] [decimal](18, 4) NOT NULL,
	[NIBPSOverDeclarationReduction] [decimal](18, 2) NOT NULL,
	[NIBPSLateClaimSubmissionPercent] [decimal](18, 2) NOT NULL,
	[NIBPSLateClaimSubmissionReduction] [decimal](18, 2) NOT NULL,
	[NIBPSLateEntitlementsApplicationPercent] [decimal](18, 2) NOT NULL,
	[NIBPSLateEntitlementsApplicationReduction] [decimal](18, 2) NOT NULL,
	[NIBPSLateAmendmentPercent] [decimal](18, 2) NOT NULL,
	[NIBPSLateAmendmentReduction] [decimal](18, 2) NOT NULL,
	[NIBPSLateEntitlementsAmendmentPercent] [decimal](18, 2) NOT NULL,
	[NIBPSLateEntitlementsAmendmentReduction] [decimal](18, 2) NOT NULL,
	[NIBPSLateEvidencePercent] [decimal](18, 2) NOT NULL,
	[NIBPSLateEvidenceReduction] [decimal](18, 2) NOT NULL,
	[NIBPSNonDeclarationPercent] [decimal](18, 2) NOT NULL,
	[NIBPSNonDeclarationReduction] [decimal](18, 2) NOT NULL,
	[NIBPSFDMPercent] [decimal](18, 2) NOT NULL,
	[NIBPSFDMReduction] [decimal](18, 2) NOT NULL,
	[NIBPSReductionOfPaymentsOver150kPercent] [decimal](18, 2) NOT NULL,
	[NIBPSReductionOfPaymentsOver150kDeduction] [decimal](18, 2) NOT NULL,
	[NIBPSTotalBPSPayment] [decimal](18, 2) NOT NULL,
	[EnglandGreeningNonSDANumber] [decimal](18, 4) NOT NULL,
	[EnglandGreeningNonSDARate] [decimal](18, 2) NULL,
	[EnglandGreeningNonSDAReduction] [decimal](18, 2) NOT NULL,
	[EnglandGreeningNonSDATotal] [decimal](18, 4) NOT NULL,
	[EnglandGreeningSDANumber] [decimal](18, 4) NOT NULL,
	[EnglandGreeningSDAReduction] [decimal](18, 2) NOT NULL,
	[EnglandGreeningSDATotal] [decimal](18, 4) NOT NULL,
	[EnglandGreeningMoorlandNumber] [decimal](18, 4) NOT NULL,
	[EnglandGreeningMoorlandReduction] [decimal](18, 2) NOT NULL,
	[EnglandGreeningMoorlandTotal] [decimal](18, 4) NOT NULL,
	[EnglandGreeningAverageGreeningValue2015Value] [decimal](18, 4) NOT NULL,
	[EnglandGreeningCropDiversificationNumber] [decimal](18, 4) NOT NULL,
	[EnglandGreeningPermanentGrasslandNumber] [decimal](18, 4) NOT NULL,
	[EnglandGreeningPermanentGrasslandReduction] [decimal](18, 2) NOT NULL,
	[EnglandGreeningEFANumber] [decimal](18, 4) NOT NULL,
	[EnglandGreeningEFAReduction] [decimal](18, 2) NOT NULL,
	[EnglandGreeningLateApplicationPercent] [decimal](18, 2) NOT NULL,
	[EnglandGreeningLateApplicationReduction] [decimal](18, 2) NOT NULL,
	[EnglandGreeningFDMPercent] [decimal](18, 2) NOT NULL,
	[EnglandGreeningFDMReduction] [decimal](18, 2) NOT NULL,
	[EnglandGreeningTotalGreeningPayment] [decimal](18, 2) NOT NULL,
	[EnglandGreeningGross] [decimal](18, 2) NOT NULL,
	[WalesGreeningRegion1Number] [decimal](18, 4) NOT NULL,
	[WalesGreeningRegion1Total] [decimal](18, 4) NOT NULL,
	[WalesGreeningCropDiversificationNumber] [decimal](18, 4) NOT NULL,
	[WalesGreeningPermanentGrasslandNumber] [decimal](18, 4) NOT NULL,
	[WalesGreeningPermanentGrasslandReduction] [decimal](18, 2) NOT NULL,
	[WalesGreeningEFANumber] [decimal](18, 4) NOT NULL,
	[WalesGreeningEFAReduction] [decimal](18, 2) NOT NULL,
	[WalesGreeningLateApplicationPercent] [decimal](18, 2) NOT NULL,
	[WalesGreeningLateApplicationReduction] [decimal](18, 2) NOT NULL,
	[WalesGreeningLateAmendmentPercent] [decimal](18, 2) NOT NULL,
	[WalesGreeningLateAmendmentReduction] [decimal](18, 2) NOT NULL,
	[WalesGreeningLateEvidencePercent] [decimal](18, 2) NOT NULL,
	[WalesGreeningLateEvidenceReduction] [decimal](18, 2) NOT NULL,
	[WalesGreeningNonDeclarationPercent] [decimal](18, 2) NOT NULL,
	[WalesGreeningNonDeclarationReduction] [decimal](18, 2) NOT NULL,
	[WalesGreeningFDMPercent] [decimal](18, 2) NOT NULL,
	[WalesGreeningFDMReduction] [decimal](18, 2) NOT NULL,
	[WalesGreeningTotalGreeningPayment] [decimal](18, 2) NOT NULL,
	[ScotlandGreeningRegion1Number] [decimal](18, 4) NOT NULL,
	[ScotlandGreeningRegion1Total] [decimal](18, 4) NOT NULL,
	[ScotlandGreeningRegion2Number] [decimal](18, 4) NOT NULL,
	[ScotlandGreeningRegion2Total] [decimal](18, 4) NOT NULL,
	[ScotlandGreeningRegion3Number] [decimal](18, 4) NOT NULL,
	[ScotlandGreeningRegion3Total] [decimal](18, 4) NOT NULL,
	[ScotlandGreeningAverageGreeningValue2015Value] [decimal](18, 4) NOT NULL,
	[ScotlandGreeningCropDiversificationNumber] [decimal](18, 4) NOT NULL,
	[ScotlandGreeningPermanentGrasslandNumber] [decimal](18, 4) NOT NULL,
	[ScotlandGreeningPermanentGrasslandReduction] [decimal](18, 2) NOT NULL,
	[ScotlandGreeningEFANumber] [decimal](18, 4) NOT NULL,
	[ScotlandGreeningEFAReduction] [decimal](18, 2) NOT NULL,
	[ScotlandGreeningLateApplicationPercent] [decimal](18, 2) NOT NULL,
	[ScotlandGreeningLateApplicationReduction] [decimal](18, 2) NOT NULL,
	[ScotlandGreeningLateAmendmentPercent] [decimal](18, 2) NOT NULL,
	[ScotlandGreeningLateAmendmentReduction] [decimal](18, 2) NOT NULL,
	[ScotlandGreeningLateEvidencePercent] [decimal](18, 2) NOT NULL,
	[ScotlandGreeningLateEvidenceReduction] [decimal](18, 2) NOT NULL,
	[ScotlandGreeningNonDeclarationPercent] [decimal](18, 2) NOT NULL,
	[ScotlandGreeningNonDeclarationReduction] [decimal](18, 2) NOT NULL,
	[ScotlandGreeningFDMPercent] [decimal](18, 2) NOT NULL,
	[ScotlandGreeningFDMReduction] [decimal](18, 2) NOT NULL,
	[ScotlandGreeningTotalGreeningPayment] [decimal](18, 2) NOT NULL,
	[ScotlandGreeningGross] [decimal](18, 2) NOT NULL,
	[NIGreeningRegion1Number] [decimal](18, 4) NOT NULL,
	[NIGreeningRegion1Total] [decimal](18, 4) NOT NULL,
	[NIGreeningCropDiversificationNumber] [decimal](18, 4) NOT NULL,
	[NIGreeningPermanentGrasslandNumber] [decimal](18, 4) NOT NULL,
	[NIGreeningPermanentGrasslandReduction] [decimal](18, 2) NOT NULL,
	[NIGreeningEFANumber] [decimal](18, 4) NOT NULL,
	[NIGreeningEFAReduction] [decimal](18, 2) NOT NULL,
	[NIGreeningLateApplicationPercent] [decimal](18, 2) NOT NULL,
	[NIGreeningLateApplicationReduction] [decimal](18, 2) NOT NULL,
	[NIGreeningLateAmendmentPercent] [decimal](18, 2) NOT NULL,
	[NIGreeningLateAmendmentReduction] [decimal](18, 2) NOT NULL,
	[NIGreeningLateEvidencePercent] [decimal](18, 2) NOT NULL,
	[NIGreeningLateEvidenceReduction] [decimal](18, 2) NOT NULL,
	[NIGreeningNonDeclarationPercent] [decimal](18, 2) NOT NULL,
	[NIGreeningNonDeclarationReduction] [decimal](18, 2) NOT NULL,
	[NIGreeningFDMPercent] [decimal](18, 2) NOT NULL,
	[NIGreeningFDMReduction] [decimal](18, 2) NOT NULL,
	[NIGreeningTotalGreeningPayment] [decimal](18, 2) NOT NULL,
	[EnglandYFEntitlementsUsedToClaim] [decimal](18, 2) NOT NULL,
	[EnglandYFAvgEntitlementValue] [decimal](18, 2) NOT NULL,
	[EnglandYFYFClaimValuePercent] [decimal](18, 2) NOT NULL,
	[EnglandYFClaimValue] [decimal](18, 2) NOT NULL,
	[EnglandYFOverDeclarationArea] [decimal](18, 2) NOT NULL,
	[EnglandYFOverDeclarationReduction] [decimal](18, 2) NOT NULL,
	[EnglandYFLateClaimSubmissionPercent] [decimal](18, 2) NOT NULL,
	[EnglandYFLateClaimSubmissionReduction] [decimal](18, 2) NOT NULL,
	[EnglandYFLateAmendmentPercent] [decimal](18, 2) NOT NULL,
	[EnglandYFLateAmendmentReduction] [decimal](18, 2) NOT NULL,
	[EnglandYFFDMPercent] [decimal](18, 2) NOT NULL,
	[EnglandYFFDMReduction] [decimal](18, 2) NOT NULL,
	[EnglandYFTotalYoungFarmerPayment] [decimal](18, 2) NOT NULL,
	[WalesYFEntitlementsUsedToClaim] [decimal](18, 2) NOT NULL,
	[WalesYFRate] [decimal](18, 2) NOT NULL,
	[WalesYFClaimValue] [decimal](18, 2) NOT NULL,
	[WalesYFOverDeclarationArea] [decimal](18, 2) NOT NULL,
	[WalesYFOverDeclarationReduction] [decimal](18, 2) NOT NULL,
	[WalesYFDeclarationArea] [decimal](18, 2) NOT NULL,
	[WalesYFDeclarationReduction] [decimal](18, 2) NOT NULL,
	[WalesYFDeclarationTotal] [decimal](18, 2) NOT NULL,
	[WalesYFLateClaimSubmissionPercent] [decimal](18, 2) NOT NULL,
	[WalesYFLateClaimSubmissionReduction] [decimal](18, 2) NOT NULL,
	[WalesYFLateAmendmentPercent] [decimal](18, 2) NOT NULL,
	[WalesYFLateAmendmentReduction] [decimal](18, 2) NOT NULL,
	[WalesYFLateEvidencePercent] [decimal](18, 2) NOT NULL,
	[WalesYFLateEvidenceReduction] [decimal](18, 2) NOT NULL,
	[WalesYFNonDeclarationPercent] [decimal](18, 2) NOT NULL,
	[WalesYFNonDeclarationReduction] [decimal](18, 2) NOT NULL,
	[WalesYFFDMPercent] [decimal](18, 2) NOT NULL,
	[WalesYFFDMReduction] [decimal](18, 2) NOT NULL,
	[WalesYFTotalYoungFarmerPayment] [decimal](18, 2) NOT NULL,
	[ScotlandYFEntitlementsUsedToClaim] [decimal](18, 2) NOT NULL,
	[ScotlandYFAvgEntitlementValue] [decimal](18, 2) NOT NULL,
	[ScotlandYFClaimValuePercent] [decimal](18, 2) NOT NULL,
	[ScotlandYFClaimValue] [decimal](18, 2) NOT NULL,
	[ScotlandYFOverDeclarationArea] [decimal](18, 2) NOT NULL,
	[ScotlandYFOverDeclarationReduction] [decimal](18, 2) NOT NULL,
	[ScotlandYFDeclarationArea] [decimal](18, 2) NOT NULL,
	[ScotlandYFDeclarationReduction] [decimal](18, 2) NOT NULL,
	[ScotlandYFLateClaimSubmissionPercent] [decimal](18, 2) NOT NULL,
	[ScotlandYFLateClaimSubmissionReduction] [decimal](18, 2) NOT NULL,
	[ScotlandYFLateAmendmentPercent] [decimal](18, 2) NOT NULL,
	[ScotlandYFLateAmendmentReduction] [decimal](18, 2) NOT NULL,
	[ScotlandYFLateEvidencePercent] [decimal](18, 2) NOT NULL,
	[ScotlandYFLateEvidenceReduction] [decimal](18, 2) NOT NULL,
	[ScotlandYFNonDeclarationPercent] [decimal](18, 2) NOT NULL,
	[ScotlandYFNonDeclarationReduction] [decimal](18, 2) NOT NULL,
	[ScotlandYFFDMPercent] [decimal](18, 2) NOT NULL,
	[ScotlandYFFDMReduction] [decimal](18, 2) NOT NULL,
	[ScotlandYFTotalYoungFarmerPayment] [decimal](18, 2) NOT NULL,
	[NIYFEntitlementsUsedToClaim] [decimal](18, 2) NOT NULL,
	[NIYFRate] [decimal](18, 2) NOT NULL,
	[NIYFClaimValue] [decimal](18, 2) NOT NULL,
	[NIYFOverDeclarationArea] [decimal](18, 2) NOT NULL,
	[NIYFOverDeclarationReduction] [decimal](18, 2) NOT NULL,
	[NIYFDeclarationPenalty] [decimal](18, 2) NOT NULL,
	[NIYFDeclarationReduction] [decimal](18, 2) NOT NULL,
	[NIYFLateClaimSubmissionPercent] [decimal](18, 2) NOT NULL,
	[NIYFLateClaimSubmissionReduction] [decimal](18, 2) NOT NULL,
	[NIYFLateAmendmentPercent] [decimal](18, 2) NOT NULL,
	[NIYFLateAmendmentReduction] [decimal](18, 2) NOT NULL,
	[NIYFLateEvidencePercent] [decimal](18, 2) NOT NULL,
	[NIYFLateEvidenceReduction] [decimal](18, 2) NOT NULL,
	[NIYFNonDeclarationPercent] [decimal](18, 2) NOT NULL,
	[NIYFNonDeclarationReduction] [decimal](18, 2) NOT NULL,
	[NIYFFDMPercent] [decimal](18, 2) NOT NULL,
	[NIYFFDMReduction] [decimal](18, 2) NOT NULL,
	[NIYFTotalYoungFarmerPayment] [decimal](18, 2) NOT NULL,
	[WalesRedLandUsedToClaim] [decimal](18, 4) NOT NULL,
	[WalesRedOverDeclarationArea] [decimal](18, 4) NOT NULL,
	[WalesRedOverDeclarationReduction] [decimal](18, 2) NOT NULL,
	[WalesRedLateClaimSubmissionPercent] [decimal](18, 2) NOT NULL,
	[WalesRedLateClaimSubmissionReduction] [decimal](18, 2) NOT NULL,
	[WalesRedLateAmendmentPercent] [decimal](18, 2) NOT NULL,
	[WalesRedLateAmendmentReduction] [decimal](18, 2) NOT NULL,
	[WalesRedLateEvidencePercent] [decimal](18, 2) NOT NULL,
	[WalesRedLateEvidenceReduction] [decimal](18, 2) NOT NULL,
	[WalesRedNonDeclarationPercent] [decimal](18, 2) NOT NULL,
	[WalesRedNonDeclarationReduction] [decimal](18, 2) NOT NULL,
	[WalesRedFDMPercent] [decimal](18, 2) NOT NULL,
	[WalesRedFDMReduction] [decimal](18, 2) NOT NULL,
	[WalesRedTotalRedistributionPayment] [decimal](18, 2) NOT NULL,
	[EnglandNonSDAAreaOnApplication] [decimal](18, 4) NOT NULL,
	[EnglandNonSDAAreaIneligible] [decimal](18, 4) NOT NULL,
	[EnglandNonSDAEntitlements] [decimal](18, 4) NOT NULL,
	[EnglandSDAAreaOnApplication] [decimal](18, 4) NOT NULL,
	[EnglandSDAAreaIneligible] [decimal](18, 4) NOT NULL,
	[EnglandSDAEntitlements] [decimal](18, 4) NOT NULL,
	[EnglandMoorlandAreaOnApplication] [decimal](18, 4) NOT NULL,
	[EnglandMoorlandAreaIneligible] [decimal](18, 4) NOT NULL,
	[EnglandMoorlandEntitlements] [decimal](18, 4) NOT NULL,
	[WalesAreaOnApplication] [decimal](18, 4) NOT NULL,
	[WalesAreaIneligible] [decimal](18, 4) NOT NULL,
	[WalesEntitlements] [decimal](18, 4) NOT NULL,
	[ScotlandRegion1AreaOnApplication] [decimal](18, 4) NOT NULL,
	[ScotlandRegion1AreaIneligible] [decimal](18, 4) NOT NULL,
	[ScotlandRegion1Entitlements] [decimal](18, 4) NOT NULL,
	[ScotlandRegion2AreaOnApplication] [decimal](18, 4) NOT NULL,
	[ScotlandRegion2AreaIneligible] [decimal](18, 4) NOT NULL,
	[ScotlandRegion2Entitlements] [decimal](18, 4) NOT NULL,
	[ScotlandRegion3AreaOnApplication] [decimal](18, 2) NOT NULL,
	[ScotlandRegion3AreaIneligible] [decimal](18, 2) NOT NULL,
	[ScotlandRegion3Entitlements] [decimal](18, 4) NOT NULL,
	[NIAreaOnApplication] [decimal](18, 4) NOT NULL,
	[NIAreaIneligible] [decimal](18, 4) NOT NULL,
	[NIEntitlements] [decimal](18, 4) NOT NULL,
	[EnglandGreeningCropDiversificationReduction] [decimal](18, 2) NOT NULL,
	[WalesGreeningCropDiversificationReduction] [decimal](18, 2) NOT NULL,
	[ScotlandGreeningCropDiversificationReduction] [decimal](18, 2) NOT NULL,
	[NIGreeningCropDiversificationReduction] [decimal](18, 2) NOT NULL,
	[WalesRedClaimValue] [decimal](18, 2) NOT NULL,
	[EnglandBPSLateEvidencePercent] [decimal](18, 2) NOT NULL,
	[FRN] [bigint] NOT NULL,
	[WalesBPSLateEvidenceEntitlementsPercent] [decimal](18, 2) NOT NULL,
	[WalesBPSLateEvidenceEntitlementsReduction] [decimal](18, 2) NOT NULL,
	[ScotlandBPSLateEvidencePercent] [decimal](18, 2) NOT NULL,
	[ScotlandBPSLateEvidenceEntitlementsPercent] [decimal](18, 2) NOT NULL,
	[ScotlandBPSLateEvidenceEntitlementsReduction] [decimal](18, 2) NOT NULL,
	[NIBPSLateEvidenceEntitlementsPercent] [decimal](18, 2) NOT NULL,
	[NIBPSLateEvidenceEntitlementsReduction] [decimal](18, 2) NOT NULL,
	[EnglandGreeningLateEvidencePercent] [decimal](18, 2) NOT NULL,
	[EnglandGreeningLateEvidenceReduction] [decimal](18, 2) NOT NULL,
	[EnglandBPSNonSDARate] [decimal](18, 4) NOT NULL,
	[EnglandBPSSDARate] [decimal](18, 4) NOT NULL,
	[EnglandBPSMoorlandRate] [decimal](18, 4) NOT NULL,
	[WalesBPSRegion1Rate] [decimal](18, 4) NOT NULL,
	[ScotlandBPSRegion1Rate] [decimal](18, 4) NOT NULL,
	[ScotlandBPSRegion2Rate] [decimal](18, 4) NOT NULL,
	[ScotlandBPSRegion3Rate] [decimal](18, 4) NOT NULL,
	[NIBPSRegion1Rate] [decimal](18, 4) NOT NULL,
	[WalesGreeningRegion1Rate] [decimal](18, 4) NOT NULL,
	[ScotlandGreeningRegion1Rate] [decimal](18, 4) NOT NULL,
	[ScotlandGreeningRegion2Rate] [decimal](18, 4) NOT NULL,
	[ScotlandGreeningRegion3Rate] [decimal](18, 4) NOT NULL,
	[NIGreeningRegion1Rate] [decimal](18, 4) NOT NULL,
	[EnglandBPSOverDeclarationPercent] [decimal](18, 2) NOT NULL,
	[WalesBPSOverDeclarationPercent] [decimal](18, 2) NOT NULL,
	[ScotlandBPSOverDeclarationPercent] [decimal](18, 2) NOT NULL,
	[NIBPSOverDeclarationPercent] [decimal](18, 2) NOT NULL,
	[EnglandYFOverDeclarationPercent] [decimal](18, 2) NOT NULL,
	[WalesYFOverDeclarationPercent] [decimal](18, 2) NOT NULL,
	[ScotlandYFOverDeclarationPercent] [decimal](18, 2) NOT NULL,
	[NIYFOverDeclarationPercent] [decimal](18, 2) NOT NULL,
	[WalesRedOverDeclarationPercent] [decimal](18, 2) NOT NULL,
	[EnglandBPSAdditionalOverDeclarationReduction] [decimal](18, 2) NULL,
	[EnglandBPSPreviousYearOverDeclaration] [bit] NULL,
	[WalesBPSAdditionalOverDeclarationReduction] [decimal](18, 2) NULL,
	[WalesBPSPreviousYearOverDeclaration] [bit] NULL,
	[ScotlandBPSAdditionalOverDeclarationReduction] [decimal](18, 2) NULL,
	[ScotlandBPSPreviousYearOverDeclaration] [bit] NULL,
	[NIBPSAdditionalOverDeclarationReduction] [decimal](18, 2) NULL,
	[NIBPSPreviousYearOverDeclaration] [bit] NULL,
	[EnglandYFAdditionalOverDeclarationReduction] [decimal](18, 2) NULL,
	[EnglandYFPreviousYearOverDeclaration] [bit] NULL,
	[WalesYFAdditionalOverDeclarationReduction] [decimal](18, 2) NULL,
	[WalesYFPreviousYearOverDeclaration] [bit] NULL,
	[ScotlandYFAdditionalOverDeclarationReduction] [decimal](18, 2) NULL,
	[ScotlandYFPreviousYearOverDeclaration] [bit] NULL,
	[NIYFAdditionalOverDeclarationReduction] [decimal](18, 2) NULL,
	[NIYFPreviousYearOverDeclaration] [bit] NULL,
	[WalesRedAdditionalOverDeclarationReduction] [decimal](18, 2) NULL,
	[WalesRedPreviousYearOverDeclaration] [bit] NULL,
	[EnglandGreeningAdministrativeArea] [decimal](18, 2) NULL,
	[EnglandGreeningAdministrativeReduction] [decimal](18, 2) NULL,
	[WalesGreeningAdministrativeArea] [decimal](18, 2) NULL,
	[WalesGreeningAdministrativeReduction] [decimal](18, 2) NULL,
	[ScotlandGreeningAdministrativeArea] [decimal](18, 2) NULL,
	[ScotlandGreeningAdministrativeReduction] [decimal](18, 2) NULL,
	[NIGreeningAdministrativeArea] [decimal](18, 2) NULL,
	[NIGreeningAdministrativeReduction] [decimal](18, 2) NULL,
	[EnglandGreeningCropDiversificationReductionAdditional] [decimal](18, 2) NULL,
	[ScotlandGreeningCropDiversificationReductionAdditional] [decimal](18, 2) NULL,
	[WalesGreeningCropDiversificationReductionAdditional] [decimal](18, 2) NULL,
	[NIGreeningCropDiversificationReductionAdditional] [decimal](18, 2) NULL,
 CONSTRAINT [PK_XB.XBStage] PRIMARY KEY CLUSTERED 
(
	[XBDataID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Schem__2685A772]  DEFAULT ((0)) FOR [SchemeYear]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__2779CBAB]  DEFAULT ((0)) FOR [EnglandBPSValue]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__286DEFE4]  DEFAULT ((0)) FOR [EnglandGreeningValue]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__2962141D]  DEFAULT ((0)) FOR [EnglandYoungFarmerValue]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__2A563856]  DEFAULT ((0)) FOR [EnglandSubTotal]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__2B4A5C8F]  DEFAULT ((0)) FOR [EnglandCrossCompliancePercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__2C3E80C8]  DEFAULT ((0)) FOR [EnglandCrossComplianceReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__2D32A501]  DEFAULT ((0)) FOR [EnglandCrossComplianceTotal]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__2E26C93A]  DEFAULT ((0)) FOR [EnglandTotalEuro]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__2F1AED73]  DEFAULT ((0)) FOR [WalesBPSValue]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__300F11AC]  DEFAULT ((0)) FOR [WalesGreeningValue]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__310335E5]  DEFAULT ((0)) FOR [WalesYoungFarmerValue]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__31F75A1E]  DEFAULT ((0)) FOR [WalesRedistributiveValue]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__32EB7E57]  DEFAULT ((0)) FOR [WalesSubTotal]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__33DFA290]  DEFAULT ((0)) FOR [WalesCrossCompliancePercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__34D3C6C9]  DEFAULT ((0)) FOR [WalesCrossComplianceReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__35C7EB02]  DEFAULT ((0)) FOR [WalesCrossComplianceTotal]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__36BC0F3B]  DEFAULT ((0)) FOR [WalesTotalEuro]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__37B03374]  DEFAULT ((0)) FOR [ScotlandBPSValue]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__38A457AD]  DEFAULT ((0)) FOR [ScotlandGreeningValue]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__39987BE6]  DEFAULT ((0)) FOR [ScotlandYoungFarmerValue]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__3A8CA01F]  DEFAULT ((0)) FOR [ScotlandSubTotal]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__3B80C458]  DEFAULT ((0)) FOR [ScotlandCrossCompliancePercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__3C74E891]  DEFAULT ((0)) FOR [ScotlandCrossComplianceReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__3D690CCA]  DEFAULT ((0)) FOR [ScotlandCrossComplianceTotal]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__3E5D3103]  DEFAULT ((0)) FOR [ScotlandTotalEuro]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__3F51553C]  DEFAULT ((0)) FOR [NIBPSValue]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIGre__40457975]  DEFAULT ((0)) FOR [NIGreeningValue]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIYou__41399DAE]  DEFAULT ((0)) FOR [NIYoungFarmerValue]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NISub__422DC1E7]  DEFAULT ((0)) FOR [NISubTotal]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NICro__4321E620]  DEFAULT ((0)) FOR [NICrossCompliancePercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NICro__44160A59]  DEFAULT ((0)) FOR [NICrossComplianceReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NICro__450A2E92]  DEFAULT ((0)) FOR [NICrossComplianceTotal]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NITot__45FE52CB]  DEFAULT ((0)) FOR [NITotalEuro]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__46F27704]  DEFAULT ((0)) FOR [EnglandBPSNonSDANumber]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__47E69B3D]  DEFAULT ((0)) FOR [EnglandBPSNonSDATotal]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__48DABF76]  DEFAULT ((0)) FOR [EnglandBPSSDANumber]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__49CEE3AF]  DEFAULT ((0)) FOR [EnglandBPSSDATotal]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__4AC307E8]  DEFAULT ((0)) FOR [EnglandBPSMoorlandNumber]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__4BB72C21]  DEFAULT ((0)) FOR [EnglandBPSMoorlandTotal]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__4CAB505A]  DEFAULT ((0)) FOR [EnglandBPSAverageEntitlementValue2015Value]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__4D9F7493]  DEFAULT ((0)) FOR [EnglandBPSOverDeclarationHectares]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__4E9398CC]  DEFAULT ((0)) FOR [EnglandBPSOverDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__4F87BD05]  DEFAULT ((0)) FOR [EnglandBPSLateClaimSubmissionPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__507BE13E]  DEFAULT ((0)) FOR [EnglandBPSLateClaimSubmissionReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__51700577]  DEFAULT ((0)) FOR [EnglandBPSLateEntitlementsApplicationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__526429B0]  DEFAULT ((0)) FOR [EnglandBPSLateEntitlementsApplicationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__53584DE9]  DEFAULT ((0)) FOR [EnglandBPSLateAmendmentPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__544C7222]  DEFAULT ((0)) FOR [EnglandBPSLateAmendmentReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__5540965B]  DEFAULT ((0)) FOR [EnglandBPSLateEvidenceReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__5634BA94]  DEFAULT ((0)) FOR [EnglandBPSLateEntitlementsAmendmentPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__5728DECD]  DEFAULT ((0)) FOR [EnglandBPSLateEntitlementsAmendmentReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__581D0306]  DEFAULT ((0)) FOR [EnglandBPSFDMPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__5911273F]  DEFAULT ((0)) FOR [EnglandBPSFDMReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__5A054B78]  DEFAULT ((0)) FOR [EnglandBPSFDMTotal]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__5AF96FB1]  DEFAULT ((0)) FOR [EnglandBPSReductionOfPaymentsOver150kPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__5BED93EA]  DEFAULT ((0)) FOR [EnglandBPSReductionOfPaymentsOver150kDeduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__5CE1B823]  DEFAULT ((0)) FOR [EnglandBPSTotalBPSPayment]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__5DD5DC5C]  DEFAULT ((0)) FOR [EnglandBPSBPSGross]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF_claim_statement_data_EnglandBPSNonDeclarationPercent]  DEFAULT ((0)) FOR [EnglandBPSNonDeclarationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF_claim_statement_data_EnglandBPSNonDeclarationReduction]  DEFAULT ((0)) FOR [EnglandBPSNonDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF_claim_statement_data_EnglandGreeningNonDeclarationPercent]  DEFAULT ((0)) FOR [EnglandGreeningNonDeclarationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF_claim_statement_data_EnglandGreeningNonDeclarationReduction]  DEFAULT ((0)) FOR [EnglandGreeningNonDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF_claim_statement_data_EnglandYFNonDeclarationPercent]  DEFAULT ((0)) FOR [EnglandYFNonDeclarationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF_claim_statement_data_EnglandYFNonDeclarationReduction]  DEFAULT ((0)) FOR [EnglandYFNonDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__5ECA0095]  DEFAULT ((0)) FOR [WalesBPSRegion1Number]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__5FBE24CE]  DEFAULT ((0)) FOR [WalesBPSRegion1Total]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__60B24907]  DEFAULT ((0)) FOR [WalesBPSOverDeclarationHectares]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__61A66D40]  DEFAULT ((0)) FOR [WalesBPSOverDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__629A9179]  DEFAULT ((0)) FOR [WalesBPSLateClaimSubmissionPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__638EB5B2]  DEFAULT ((0)) FOR [WalesBPSLateClaimSubmissionReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__6482D9EB]  DEFAULT ((0)) FOR [WalesBPSLateEntitlementsApplicationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__6576FE24]  DEFAULT ((0)) FOR [WalesBPSLateEntitlementsApplicationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__666B225D]  DEFAULT ((0)) FOR [WalesBPSLateAmendmentPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__675F4696]  DEFAULT ((0)) FOR [WalesBPSLateAmendmentReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__68536ACF]  DEFAULT ((0)) FOR [WalesBPSLateEntitlementsAmendmentPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__69478F08]  DEFAULT ((0)) FOR [WalesBPSLateEntitlementsAmendmentReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__6A3BB341]  DEFAULT ((0)) FOR [WalesBPSLateEvidencePercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__6B2FD77A]  DEFAULT ((0)) FOR [WalesBPSLateEvidenceReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__6C23FBB3]  DEFAULT ((0)) FOR [WalesBPSNonDeclarationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__6D181FEC]  DEFAULT ((0)) FOR [WalesBPSNonDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__6E0C4425]  DEFAULT ((0)) FOR [WalesBPSFDMPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__6F00685E]  DEFAULT ((0)) FOR [WalesBPSFDMReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__6FF48C97]  DEFAULT ((0)) FOR [WalesBPSReductionOfPaymentsOver150kPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__70E8B0D0]  DEFAULT ((0)) FOR [WalesBPSReductionOfPaymentsOver150kDeduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__71DCD509]  DEFAULT ((0)) FOR [WalesBPSReductionOfPaymentsOver200kPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__72D0F942]  DEFAULT ((0)) FOR [WalesBPSReductionOfPaymentsOver200kDeduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__73C51D7B]  DEFAULT ((0)) FOR [WalesBPSReductionOfPaymentsOver250kPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__74B941B4]  DEFAULT ((0)) FOR [WalesBPSReductionOfPaymentsOver250kDeduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__75AD65ED]  DEFAULT ((0)) FOR [WalesBPSReductionOfPaymentsOver300kPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__76A18A26]  DEFAULT ((0)) FOR [WalesBPSReductionOfPaymentsOver300kDeduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__7795AE5F]  DEFAULT ((0)) FOR [WalesBPSTotalBPSPayment]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__7889D298]  DEFAULT ((0)) FOR [ScotlandBPSRegion1Number]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__797DF6D1]  DEFAULT ((0)) FOR [ScotlandBPSRegion1Total]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__7A721B0A]  DEFAULT ((0)) FOR [ScotlandBPSRegion2Number]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__7B663F43]  DEFAULT ((0)) FOR [ScotlandBPSRegion2Total]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__7C5A637C]  DEFAULT ((0)) FOR [ScotlandBPSRegion3Number]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__7D4E87B5]  DEFAULT ((0)) FOR [ScotlandBPSRegion3Total]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__7E42ABEE]  DEFAULT ((0)) FOR [ScotlandBPSAverageEntitlementValue2015Value]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__7F36D027]  DEFAULT ((0)) FOR [ScotlandBPSOverDeclarationHectares]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__002AF460]  DEFAULT ((0)) FOR [ScotlandBPSOverDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__011F1899]  DEFAULT ((0)) FOR [ScotlandBPSLateClaimSubmissionPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__02133CD2]  DEFAULT ((0)) FOR [ScotlandBPSLateClaimSubmissionReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__0307610B]  DEFAULT ((0)) FOR [ScotlandBPSLateEntitlementsApplicationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__03FB8544]  DEFAULT ((0)) FOR [ScotlandBPSLateEntitlementsApplicationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__04EFA97D]  DEFAULT ((0)) FOR [ScotlandBPSLateAmendmentPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__05E3CDB6]  DEFAULT ((0)) FOR [ScotlandBPSLateAmendmentReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__06D7F1EF]  DEFAULT ((0)) FOR [ScotlandBPSLateEntitlementsAmendmentPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__07CC1628]  DEFAULT ((0)) FOR [ScotlandBPSLateEntitlementsAmendmentReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__08C03A61]  DEFAULT ((0)) FOR [ScotlandBPSLateEvidenceReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__09B45E9A]  DEFAULT ((0)) FOR [ScotlandBPSNonDeclarationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__0AA882D3]  DEFAULT ((0)) FOR [ScotlandBPSNonDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__0B9CA70C]  DEFAULT ((0)) FOR [ScotlandBPSFDMPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__0C90CB45]  DEFAULT ((0)) FOR [ScotlandBPSFDMReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__0D84EF7E]  DEFAULT ((0)) FOR [ScotlandBPSReductionOfPaymentsOver150kPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__0E7913B7]  DEFAULT ((0)) FOR [ScotlandBPSReductionOfPaymentsOver150kDeduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__0F6D37F0]  DEFAULT ((0)) FOR [ScotlandBPSTotalBPSPayment]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__10615C29]  DEFAULT ((0)) FOR [ScotlandBPSGross]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__11558062]  DEFAULT ((0)) FOR [NIBPSRegion1Number]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__1249A49B]  DEFAULT ((0)) FOR [NIBPSRegion1Total]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__133DC8D4]  DEFAULT ((0)) FOR [NIBPSOverDeclarationHectares]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__1431ED0D]  DEFAULT ((0)) FOR [NIBPSOverDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__15261146]  DEFAULT ((0)) FOR [NIBPSLateClaimSubmissionPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__161A357F]  DEFAULT ((0)) FOR [NIBPSLateClaimSubmissionReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__170E59B8]  DEFAULT ((0)) FOR [NIBPSLateEntitlementsApplicationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__18027DF1]  DEFAULT ((0)) FOR [NIBPSLateEntitlementsApplicationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__18F6A22A]  DEFAULT ((0)) FOR [NIBPSLateAmendmentPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__19EAC663]  DEFAULT ((0)) FOR [NIBPSLateAmendmentReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__1ADEEA9C]  DEFAULT ((0)) FOR [NIBPSLateEntitlementsAmendmentPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__1BD30ED5]  DEFAULT ((0)) FOR [NIBPSLateEntitlementsAmendmentReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__1CC7330E]  DEFAULT ((0)) FOR [NIBPSLateEvidencePercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__1DBB5747]  DEFAULT ((0)) FOR [NIBPSLateEvidenceReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__1EAF7B80]  DEFAULT ((0)) FOR [NIBPSNonDeclarationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__1FA39FB9]  DEFAULT ((0)) FOR [NIBPSNonDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__2097C3F2]  DEFAULT ((0)) FOR [NIBPSFDMPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__218BE82B]  DEFAULT ((0)) FOR [NIBPSFDMReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__22800C64]  DEFAULT ((0)) FOR [NIBPSReductionOfPaymentsOver150kPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__2374309D]  DEFAULT ((0)) FOR [NIBPSReductionOfPaymentsOver150kDeduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__246854D6]  DEFAULT ((0)) FOR [NIBPSTotalBPSPayment]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__255C790F]  DEFAULT ((0)) FOR [EnglandGreeningNonSDANumber]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__26509D48]  DEFAULT ((0)) FOR [EnglandGreeningNonSDAReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__2744C181]  DEFAULT ((0)) FOR [EnglandGreeningNonSDATotal]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__2838E5BA]  DEFAULT ((0)) FOR [EnglandGreeningSDANumber]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__292D09F3]  DEFAULT ((0)) FOR [EnglandGreeningSDAReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__2A212E2C]  DEFAULT ((0)) FOR [EnglandGreeningSDATotal]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__2B155265]  DEFAULT ((0)) FOR [EnglandGreeningMoorlandNumber]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__2C09769E]  DEFAULT ((0)) FOR [EnglandGreeningMoorlandReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__2CFD9AD7]  DEFAULT ((0)) FOR [EnglandGreeningMoorlandTotal]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__2DF1BF10]  DEFAULT ((0)) FOR [EnglandGreeningAverageGreeningValue2015Value]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__2EE5E349]  DEFAULT ((0)) FOR [EnglandGreeningCropDiversificationNumber]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__2FDA0782]  DEFAULT ((0)) FOR [EnglandGreeningPermanentGrasslandNumber]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__30CE2BBB]  DEFAULT ((0)) FOR [EnglandGreeningPermanentGrasslandReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__31C24FF4]  DEFAULT ((0)) FOR [EnglandGreeningEFANumber]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__32B6742D]  DEFAULT ((0)) FOR [EnglandGreeningEFAReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__33AA9866]  DEFAULT ((0)) FOR [EnglandGreeningLateApplicationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__349EBC9F]  DEFAULT ((0)) FOR [EnglandGreeningLateApplicationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__3592E0D8]  DEFAULT ((0)) FOR [EnglandGreeningFDMPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__36870511]  DEFAULT ((0)) FOR [EnglandGreeningFDMReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__377B294A]  DEFAULT ((0)) FOR [EnglandGreeningTotalGreeningPayment]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__386F4D83]  DEFAULT ((0)) FOR [EnglandGreeningGross]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__396371BC]  DEFAULT ((0)) FOR [WalesGreeningRegion1Number]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__3A5795F5]  DEFAULT ((0)) FOR [WalesGreeningRegion1Total]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__3B4BBA2E]  DEFAULT ((0)) FOR [WalesGreeningCropDiversificationNumber]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__3C3FDE67]  DEFAULT ((0)) FOR [WalesGreeningPermanentGrasslandNumber]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__3D3402A0]  DEFAULT ((0)) FOR [WalesGreeningPermanentGrasslandReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__3E2826D9]  DEFAULT ((0)) FOR [WalesGreeningEFANumber]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__3F1C4B12]  DEFAULT ((0)) FOR [WalesGreeningEFAReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__40106F4B]  DEFAULT ((0)) FOR [WalesGreeningLateApplicationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__41049384]  DEFAULT ((0)) FOR [WalesGreeningLateApplicationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__41F8B7BD]  DEFAULT ((0)) FOR [WalesGreeningLateAmendmentPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__42ECDBF6]  DEFAULT ((0)) FOR [WalesGreeningLateAmendmentReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__43E1002F]  DEFAULT ((0)) FOR [WalesGreeningLateEvidencePercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__44D52468]  DEFAULT ((0)) FOR [WalesGreeningLateEvidenceReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__45C948A1]  DEFAULT ((0)) FOR [WalesGreeningNonDeclarationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__46BD6CDA]  DEFAULT ((0)) FOR [WalesGreeningNonDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__47B19113]  DEFAULT ((0)) FOR [WalesGreeningFDMPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__48A5B54C]  DEFAULT ((0)) FOR [WalesGreeningFDMReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__4999D985]  DEFAULT ((0)) FOR [WalesGreeningTotalGreeningPayment]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__4A8DFDBE]  DEFAULT ((0)) FOR [ScotlandGreeningRegion1Number]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__4B8221F7]  DEFAULT ((0)) FOR [ScotlandGreeningRegion1Total]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__4C764630]  DEFAULT ((0)) FOR [ScotlandGreeningRegion2Number]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__4D6A6A69]  DEFAULT ((0)) FOR [ScotlandGreeningRegion2Total]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__4E5E8EA2]  DEFAULT ((0)) FOR [ScotlandGreeningRegion3Number]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__4F52B2DB]  DEFAULT ((0)) FOR [ScotlandGreeningRegion3Total]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__5046D714]  DEFAULT ((0)) FOR [ScotlandGreeningAverageGreeningValue2015Value]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__513AFB4D]  DEFAULT ((0)) FOR [ScotlandGreeningCropDiversificationNumber]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__522F1F86]  DEFAULT ((0)) FOR [ScotlandGreeningPermanentGrasslandNumber]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__532343BF]  DEFAULT ((0)) FOR [ScotlandGreeningPermanentGrasslandReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__541767F8]  DEFAULT ((0)) FOR [ScotlandGreeningEFANumber]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__550B8C31]  DEFAULT ((0)) FOR [ScotlandGreeningEFAReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__55FFB06A]  DEFAULT ((0)) FOR [ScotlandGreeningLateApplicationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__56F3D4A3]  DEFAULT ((0)) FOR [ScotlandGreeningLateApplicationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__57E7F8DC]  DEFAULT ((0)) FOR [ScotlandGreeningLateAmendmentPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__58DC1D15]  DEFAULT ((0)) FOR [ScotlandGreeningLateAmendmentReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__59D0414E]  DEFAULT ((0)) FOR [ScotlandGreeningLateEvidencePercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__5AC46587]  DEFAULT ((0)) FOR [ScotlandGreeningLateEvidenceReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__5BB889C0]  DEFAULT ((0)) FOR [ScotlandGreeningNonDeclarationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__5CACADF9]  DEFAULT ((0)) FOR [ScotlandGreeningNonDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__5DA0D232]  DEFAULT ((0)) FOR [ScotlandGreeningFDMPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__5E94F66B]  DEFAULT ((0)) FOR [ScotlandGreeningFDMReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__5F891AA4]  DEFAULT ((0)) FOR [ScotlandGreeningTotalGreeningPayment]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__607D3EDD]  DEFAULT ((0)) FOR [ScotlandGreeningGross]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIGre__61716316]  DEFAULT ((0)) FOR [NIGreeningRegion1Number]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIGre__6265874F]  DEFAULT ((0)) FOR [NIGreeningRegion1Total]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIGre__6359AB88]  DEFAULT ((0)) FOR [NIGreeningCropDiversificationNumber]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIGre__644DCFC1]  DEFAULT ((0)) FOR [NIGreeningPermanentGrasslandNumber]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIGre__6541F3FA]  DEFAULT ((0)) FOR [NIGreeningPermanentGrasslandReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIGre__66361833]  DEFAULT ((0)) FOR [NIGreeningEFANumber]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIGre__672A3C6C]  DEFAULT ((0)) FOR [NIGreeningEFAReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIGre__681E60A5]  DEFAULT ((0)) FOR [NIGreeningLateApplicationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIGre__691284DE]  DEFAULT ((0)) FOR [NIGreeningLateApplicationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIGre__6A06A917]  DEFAULT ((0)) FOR [NIGreeningLateAmendmentPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIGre__6AFACD50]  DEFAULT ((0)) FOR [NIGreeningLateAmendmentReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIGre__6BEEF189]  DEFAULT ((0)) FOR [NIGreeningLateEvidencePercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIGre__6CE315C2]  DEFAULT ((0)) FOR [NIGreeningLateEvidenceReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIGre__6DD739FB]  DEFAULT ((0)) FOR [NIGreeningNonDeclarationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIGre__6ECB5E34]  DEFAULT ((0)) FOR [NIGreeningNonDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIGre__6FBF826D]  DEFAULT ((0)) FOR [NIGreeningFDMPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIGre__70B3A6A6]  DEFAULT ((0)) FOR [NIGreeningFDMReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIGre__71A7CADF]  DEFAULT ((0)) FOR [NIGreeningTotalGreeningPayment]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__729BEF18]  DEFAULT ((0)) FOR [EnglandYFEntitlementsUsedToClaim]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__73901351]  DEFAULT ((0)) FOR [EnglandYFAvgEntitlementValue]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__7484378A]  DEFAULT ((0)) FOR [EnglandYFYFClaimValuePercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__75785BC3]  DEFAULT ((0)) FOR [EnglandYFClaimValue]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__766C7FFC]  DEFAULT ((0)) FOR [EnglandYFOverDeclarationArea]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__7760A435]  DEFAULT ((0)) FOR [EnglandYFOverDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__7854C86E]  DEFAULT ((0)) FOR [EnglandYFLateClaimSubmissionPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__7948ECA7]  DEFAULT ((0)) FOR [EnglandYFLateClaimSubmissionReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__7A3D10E0]  DEFAULT ((0)) FOR [EnglandYFLateAmendmentPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__7B313519]  DEFAULT ((0)) FOR [EnglandYFLateAmendmentReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__7C255952]  DEFAULT ((0)) FOR [EnglandYFFDMPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__7D197D8B]  DEFAULT ((0)) FOR [EnglandYFFDMReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__7E0DA1C4]  DEFAULT ((0)) FOR [EnglandYFTotalYoungFarmerPayment]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__7F01C5FD]  DEFAULT ((0)) FOR [WalesYFEntitlementsUsedToClaim]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__7FF5EA36]  DEFAULT ((0)) FOR [WalesYFRate]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__00EA0E6F]  DEFAULT ((0)) FOR [WalesYFClaimValue]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__01DE32A8]  DEFAULT ((0)) FOR [WalesYFOverDeclarationArea]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__02D256E1]  DEFAULT ((0)) FOR [WalesYFOverDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__03C67B1A]  DEFAULT ((0)) FOR [WalesYFDeclarationArea]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__04BA9F53]  DEFAULT ((0)) FOR [WalesYFDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__05AEC38C]  DEFAULT ((0)) FOR [WalesYFDeclarationTotal]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__06A2E7C5]  DEFAULT ((0)) FOR [WalesYFLateClaimSubmissionPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__07970BFE]  DEFAULT ((0)) FOR [WalesYFLateClaimSubmissionReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__088B3037]  DEFAULT ((0)) FOR [WalesYFLateAmendmentPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__097F5470]  DEFAULT ((0)) FOR [WalesYFLateAmendmentReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__0A7378A9]  DEFAULT ((0)) FOR [WalesYFLateEvidencePercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__0B679CE2]  DEFAULT ((0)) FOR [WalesYFLateEvidenceReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__0C5BC11B]  DEFAULT ((0)) FOR [WalesYFNonDeclarationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__0D4FE554]  DEFAULT ((0)) FOR [WalesYFNonDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__0E44098D]  DEFAULT ((0)) FOR [WalesYFFDMPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__0F382DC6]  DEFAULT ((0)) FOR [WalesYFFDMReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__102C51FF]  DEFAULT ((0)) FOR [WalesYFTotalYoungFarmerPayment]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__11207638]  DEFAULT ((0)) FOR [ScotlandYFEntitlementsUsedToClaim]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__12149A71]  DEFAULT ((0)) FOR [ScotlandYFAvgEntitlementValue]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__1308BEAA]  DEFAULT ((0)) FOR [ScotlandYFClaimValuePercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__13FCE2E3]  DEFAULT ((0)) FOR [ScotlandYFClaimValue]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__14F1071C]  DEFAULT ((0)) FOR [ScotlandYFOverDeclarationArea]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__15E52B55]  DEFAULT ((0)) FOR [ScotlandYFOverDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__16D94F8E]  DEFAULT ((0)) FOR [ScotlandYFDeclarationArea]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__17CD73C7]  DEFAULT ((0)) FOR [ScotlandYFDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__18C19800]  DEFAULT ((0)) FOR [ScotlandYFLateClaimSubmissionPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__19B5BC39]  DEFAULT ((0)) FOR [ScotlandYFLateClaimSubmissionReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__1AA9E072]  DEFAULT ((0)) FOR [ScotlandYFLateAmendmentPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__1B9E04AB]  DEFAULT ((0)) FOR [ScotlandYFLateAmendmentReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__1C9228E4]  DEFAULT ((0)) FOR [ScotlandYFLateEvidencePercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__1D864D1D]  DEFAULT ((0)) FOR [ScotlandYFLateEvidenceReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__1E7A7156]  DEFAULT ((0)) FOR [ScotlandYFNonDeclarationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__1F6E958F]  DEFAULT ((0)) FOR [ScotlandYFNonDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__2062B9C8]  DEFAULT ((0)) FOR [ScotlandYFFDMPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__2156DE01]  DEFAULT ((0)) FOR [ScotlandYFFDMReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__224B023A]  DEFAULT ((0)) FOR [ScotlandYFTotalYoungFarmerPayment]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIYFE__233F2673]  DEFAULT ((0)) FOR [NIYFEntitlementsUsedToClaim]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIYFR__24334AAC]  DEFAULT ((0)) FOR [NIYFRate]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIYFC__25276EE5]  DEFAULT ((0)) FOR [NIYFClaimValue]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIYFO__261B931E]  DEFAULT ((0)) FOR [NIYFOverDeclarationArea]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIYFO__270FB757]  DEFAULT ((0)) FOR [NIYFOverDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIYFD__2803DB90]  DEFAULT ((0)) FOR [NIYFDeclarationPenalty]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIYFD__28F7FFC9]  DEFAULT ((0)) FOR [NIYFDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIYFL__29EC2402]  DEFAULT ((0)) FOR [NIYFLateClaimSubmissionPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIYFL__2AE0483B]  DEFAULT ((0)) FOR [NIYFLateClaimSubmissionReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIYFL__2BD46C74]  DEFAULT ((0)) FOR [NIYFLateAmendmentPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIYFL__2CC890AD]  DEFAULT ((0)) FOR [NIYFLateAmendmentReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIYFL__2DBCB4E6]  DEFAULT ((0)) FOR [NIYFLateEvidencePercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIYFL__2EB0D91F]  DEFAULT ((0)) FOR [NIYFLateEvidenceReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIYFN__2FA4FD58]  DEFAULT ((0)) FOR [NIYFNonDeclarationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIYFN__30992191]  DEFAULT ((0)) FOR [NIYFNonDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIYFF__318D45CA]  DEFAULT ((0)) FOR [NIYFFDMPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIYFF__32816A03]  DEFAULT ((0)) FOR [NIYFFDMReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIYFT__33758E3C]  DEFAULT ((0)) FOR [NIYFTotalYoungFarmerPayment]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__3469B275]  DEFAULT ((0)) FOR [WalesRedLandUsedToClaim]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__355DD6AE]  DEFAULT ((0)) FOR [WalesRedOverDeclarationArea]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__3651FAE7]  DEFAULT ((0)) FOR [WalesRedOverDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__37461F20]  DEFAULT ((0)) FOR [WalesRedLateClaimSubmissionPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__383A4359]  DEFAULT ((0)) FOR [WalesRedLateClaimSubmissionReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__392E6792]  DEFAULT ((0)) FOR [WalesRedLateAmendmentPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__3A228BCB]  DEFAULT ((0)) FOR [WalesRedLateAmendmentReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__3B16B004]  DEFAULT ((0)) FOR [WalesRedLateEvidencePercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__3C0AD43D]  DEFAULT ((0)) FOR [WalesRedLateEvidenceReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__3CFEF876]  DEFAULT ((0)) FOR [WalesRedNonDeclarationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__3DF31CAF]  DEFAULT ((0)) FOR [WalesRedNonDeclarationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__3EE740E8]  DEFAULT ((0)) FOR [WalesRedFDMPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__3FDB6521]  DEFAULT ((0)) FOR [WalesRedFDMReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__40CF895A]  DEFAULT ((0)) FOR [WalesRedTotalRedistributionPayment]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__41C3AD93]  DEFAULT ((0)) FOR [EnglandNonSDAAreaOnApplication]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__42B7D1CC]  DEFAULT ((0)) FOR [EnglandNonSDAAreaIneligible]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__43ABF605]  DEFAULT ((0)) FOR [EnglandNonSDAEntitlements]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__44A01A3E]  DEFAULT ((0)) FOR [EnglandSDAAreaOnApplication]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__45943E77]  DEFAULT ((0)) FOR [EnglandSDAAreaIneligible]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__468862B0]  DEFAULT ((0)) FOR [EnglandSDAEntitlements]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__477C86E9]  DEFAULT ((0)) FOR [EnglandMoorlandAreaOnApplication]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__4870AB22]  DEFAULT ((0)) FOR [EnglandMoorlandAreaIneligible]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__4964CF5B]  DEFAULT ((0)) FOR [EnglandMoorlandEntitlements]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__4A58F394]  DEFAULT ((0)) FOR [WalesAreaOnApplication]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__4B4D17CD]  DEFAULT ((0)) FOR [WalesAreaIneligible]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__4C413C06]  DEFAULT ((0)) FOR [WalesEntitlements]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__4D35603F]  DEFAULT ((0)) FOR [ScotlandRegion1AreaOnApplication]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__4E298478]  DEFAULT ((0)) FOR [ScotlandRegion1AreaIneligible]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__4F1DA8B1]  DEFAULT ((0)) FOR [ScotlandRegion1Entitlements]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__5011CCEA]  DEFAULT ((0)) FOR [ScotlandRegion2AreaOnApplication]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__5105F123]  DEFAULT ((0)) FOR [ScotlandRegion2AreaIneligible]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__51FA155C]  DEFAULT ((0)) FOR [ScotlandRegion2Entitlements]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__52EE3995]  DEFAULT ((0)) FOR [ScotlandRegion3AreaOnApplication]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__53E25DCE]  DEFAULT ((0)) FOR [ScotlandRegion3AreaIneligible]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__54D68207]  DEFAULT ((0)) FOR [ScotlandRegion3Entitlements]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIAre__55CAA640]  DEFAULT ((0)) FOR [NIAreaOnApplication]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIAre__56BECA79]  DEFAULT ((0)) FOR [NIAreaIneligible]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIEnt__57B2EEB2]  DEFAULT ((0)) FOR [NIEntitlements]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__58A712EB]  DEFAULT ((0)) FOR [EnglandGreeningCropDiversificationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__599B3724]  DEFAULT ((0)) FOR [WalesGreeningCropDiversificationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__5A8F5B5D]  DEFAULT ((0)) FOR [ScotlandGreeningCropDiversificationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIGre__5B837F96]  DEFAULT ((0)) FOR [NIGreeningCropDiversificationReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__5C77A3CF]  DEFAULT ((0)) FOR [WalesRedClaimValue]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__5D6BC808]  DEFAULT ((0)) FOR [EnglandBPSLateEvidencePercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_state__FRN__5E5FEC41]  DEFAULT ((0)) FOR [FRN]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__5F54107A]  DEFAULT ((0)) FOR [WalesBPSLateEvidenceEntitlementsPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__604834B3]  DEFAULT ((0)) FOR [WalesBPSLateEvidenceEntitlementsReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__613C58EC]  DEFAULT ((0)) FOR [ScotlandBPSLateEvidencePercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__62307D25]  DEFAULT ((0)) FOR [ScotlandBPSLateEvidenceEntitlementsPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__6324A15E]  DEFAULT ((0)) FOR [ScotlandBPSLateEvidenceEntitlementsReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__6418C597]  DEFAULT ((0)) FOR [NIBPSLateEvidenceEntitlementsPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__650CE9D0]  DEFAULT ((0)) FOR [NIBPSLateEvidenceEntitlementsReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__66010E09]  DEFAULT ((0)) FOR [EnglandGreeningLateEvidencePercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Engla__66F53242]  DEFAULT ((0)) FOR [EnglandGreeningLateEvidenceReduction]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF_claim_statement_data_EnglandBPSNonSDARate]  DEFAULT ((0)) FOR [EnglandBPSNonSDARate]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF_claim_statement_data_EnglandBPSSDARate]  DEFAULT ((0)) FOR [EnglandBPSSDARate]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF_claim_statement_data_EnglandBPSMoorlandRate]  DEFAULT ((0)) FOR [EnglandBPSMoorlandRate]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__67E9567B]  DEFAULT ((0)) FOR [WalesBPSRegion1Rate]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__68DD7AB4]  DEFAULT ((0)) FOR [ScotlandBPSRegion1Rate]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__69D19EED]  DEFAULT ((0)) FOR [ScotlandBPSRegion2Rate]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__6AC5C326]  DEFAULT ((0)) FOR [ScotlandBPSRegion3Rate]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIBPS__6BB9E75F]  DEFAULT ((0)) FOR [NIBPSRegion1Rate]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Wales__6CAE0B98]  DEFAULT ((0)) FOR [WalesGreeningRegion1Rate]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__6DA22FD1]  DEFAULT ((0)) FOR [ScotlandGreeningRegion1Rate]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__6E96540A]  DEFAULT ((0)) FOR [ScotlandGreeningRegion2Rate]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__Scotl__6F8A7843]  DEFAULT ((0)) FOR [ScotlandGreeningRegion3Rate]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF__claim_sta__NIGre__707E9C7C]  DEFAULT ((0)) FOR [NIGreeningRegion1Rate]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF_claim_statement_data_EnglandBPSOverDeclarationPercent]  DEFAULT ((0)) FOR [EnglandBPSOverDeclarationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF_claim_statement_data_WalesBPSOverDeclarationPercent]  DEFAULT ((0)) FOR [WalesBPSOverDeclarationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF_claim_statement_data_ScotlandBPSOverDeclarationPercent]  DEFAULT ((0)) FOR [ScotlandBPSOverDeclarationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF_claim_statement_data_NIBPSOverDeclarationPercent]  DEFAULT ((0)) FOR [NIBPSOverDeclarationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF_claim_statement_data_EnglandYFOverDeclarationPercent]  DEFAULT ((0)) FOR [EnglandYFOverDeclarationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF_claim_statement_data_WalesYFOverDeclarationPercent]  DEFAULT ((0)) FOR [WalesYFOverDeclarationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF_claim_statement_data_ScotlandYFOverDeclarationPercent]  DEFAULT ((0)) FOR [ScotlandYFOverDeclarationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF_claim_statement_data_NIYFOverDeclarationPercent]  DEFAULT ((0)) FOR [NIYFOverDeclarationPercent]
GO

ALTER TABLE [dbo].[claim_statement_data] ADD  CONSTRAINT [DF_claim_statement_data_WalesRedOverDeclarationPercent]  DEFAULT ((0)) FOR [WalesRedOverDeclarationPercent]
GO


