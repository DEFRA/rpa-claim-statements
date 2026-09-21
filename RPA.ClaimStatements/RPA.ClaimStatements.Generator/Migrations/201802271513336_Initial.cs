namespace RPA.ClaimStatements.Generator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "DAX.AP",
                c => new
                    {
                        APID = c.Guid(nullable: false),
                        LogID = c.Guid(),
                        LastSettlementDate = c.DateTime(nullable: false),
                        Supplier = c.Long(nullable: false),
                        SupplierName = c.String(),
                        Invoice = c.String(),
                        LastPaymentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DocumentDate = c.DateTime(),
                        InvoiceDate = c.DateTime(),
                        InvoiceDueDate = c.DateTime(),
                        Scheme = c.String(),
                        MarketingYear = c.Int(nullable: false),
                        DeliveryBody = c.String(),
                        TransactionCurrency = c.String(),
                        TransactionInvoiceValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LastHoldReleaseDate = c.DateTime(),
                        FESReference = c.String(),
                        PaymentMethod = c.String(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.APID)
                .ForeignKey("Log.Log", t => t.LogID)
                .Index(t => t.LogID);
            
            CreateTable(
                "Log.Log",
                c => new
                    {
                        LogID = c.Guid(nullable: false),
                        ClaimID = c.Guid(),
                        SBI = c.Int(nullable: false),
                        FRN = c.Long(nullable: false),
                        SchemeYear = c.Int(nullable: false),
                        ClaimProduced = c.DateTime(),
                        PDFFile = c.String(),
                        Uploaded = c.DateTime(),
                        UploadedBy = c.String(),
                    })
                .PrimaryKey(t => t.LogID)
                .ForeignKey("SITI.Claims", t => t.ClaimID)
                .Index(t => t.ClaimID);
            
            CreateTable(
                "SITI.Claims",
                c => new
                    {
                        ClaimID = c.Guid(nullable: false),
                        SUMID = c.Guid(nullable: false),
                        SUM2ID = c.Guid(nullable: false),
                        BPSID = c.Guid(nullable: false),
                        BPSPENID = c.Guid(nullable: false),
                        GRID = c.Guid(nullable: false),
                        GRPENID = c.Guid(nullable: false),
                        YFID = c.Guid(),
                        CLDID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ClaimID)
                .ForeignKey("SITI.BPS", t => t.BPSID, cascadeDelete: true)
                .ForeignKey("SITI.BPSPEN", t => t.BPSPENID, cascadeDelete: true)
                .ForeignKey("SITI.CLD", t => t.CLDID, cascadeDelete: true)
                .ForeignKey("SITI.GR", t => t.GRID, cascadeDelete: true)
                .ForeignKey("SITI.GRPEN", t => t.GRPENID, cascadeDelete: true)
                .ForeignKey("SITI.SUM", t => t.SUMID, cascadeDelete: true)
                .ForeignKey("SITI.SUM2", t => t.SUM2ID, cascadeDelete: true)
                .ForeignKey("SITI.YF", t => t.YFID)
                .Index(t => t.SUMID)
                .Index(t => t.SUM2ID)
                .Index(t => t.BPSID)
                .Index(t => t.BPSPENID)
                .Index(t => t.GRID)
                .Index(t => t.GRPENID)
                .Index(t => t.YFID)
                .Index(t => t.CLDID);
            
            CreateTable(
                "SITI.BPS",
                c => new
                    {
                        BPSID = c.Guid(nullable: false),
                        ClaimID = c.Guid(nullable: false),
                        NonSDANumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NonSDATotal = c.Decimal(nullable: false, precision: 18, scale: 4),
                        SDANumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        SDATotal = c.Decimal(nullable: false, precision: 18, scale: 4),
                        MoorlandNumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        MoorlandSDATotal = c.Decimal(nullable: false, precision: 18, scale: 4),
                        AVGEntitlementValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.BPSID);
            
            CreateTable(
                "SITI.BPSPEN",
                c => new
                    {
                        BPSPENID = c.Guid(nullable: false),
                        ClaimID = c.Guid(nullable: false),
                        OverDeclarationHectares = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OverDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LateClaimSubmissionPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LateClaimSubmissionReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LateEntitlementsApplicationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LateEntitlementsApplicationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LateEvidencePercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LateEvidenceReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LateEntitlementsAmendmentPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LateEntitlementsAmendmentReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NonDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NonDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FDMPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FDMReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReductionOfPaymentsOver150kPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReductionOfPaymentsOver150kReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalBPS = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdditionalOverDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PreviousYearOverDeclaration = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.BPSPENID);
            
            CreateTable(
                "SITI.CLD",
                c => new
                    {
                        CLDID = c.Guid(nullable: false),
                        ClaimID = c.Guid(nullable: false),
                        NonSDAAreaOnApplication = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NonSDAAreaEligible = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NonSDAEntitlements = c.Decimal(nullable: false, precision: 18, scale: 4),
                        SDAAreaOnApplication = c.Decimal(nullable: false, precision: 18, scale: 4),
                        SDAAreaEligible = c.Decimal(nullable: false, precision: 18, scale: 4),
                        SDAEntitlements = c.Decimal(nullable: false, precision: 18, scale: 4),
                        MoorlandAreaOnApplication = c.Decimal(nullable: false, precision: 18, scale: 4),
                        MoorlandAreaEligible = c.Decimal(nullable: false, precision: 18, scale: 4),
                        MoorlandEntitlements = c.Decimal(nullable: false, precision: 18, scale: 4),
                    })
                .PrimaryKey(t => t.CLDID);
            
            CreateTable(
                "SITI.GR",
                c => new
                    {
                        GRID = c.Guid(nullable: false),
                        ClaimID = c.Guid(nullable: false),
                        NonSDANumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NonSDATotal = c.Decimal(nullable: false, precision: 18, scale: 4),
                        SDANumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        SDATotal = c.Decimal(nullable: false, precision: 18, scale: 4),
                        MoorlandNumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        MoorlandTotal = c.Decimal(nullable: false, precision: 18, scale: 4),
                        AvgGreeningRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.GRID);
            
            CreateTable(
                "SITI.GRPEN",
                c => new
                    {
                        GRPENID = c.Guid(nullable: false),
                        ClaimID = c.Guid(nullable: false),
                        CropDiversificationNumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        CropDiversificationRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CropDiversificationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PermanentGrasslandNumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PermanentGrasslandRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PermanentGrasslandReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EFANumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EFARate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EFAReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LateApplicationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LateApplicationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LateEvidencePercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LateEvidenceReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NonDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NonDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FDMReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalGreening = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdministrativeArea = c.Decimal(nullable: false, precision: 18, scale: 4),
                        AdministrativeReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.GRPENID);
            
            CreateTable(
                "SITI.SUM",
                c => new
                    {
                        SUMID = c.Guid(nullable: false),
                        ClaimID = c.Guid(nullable: false),
                        SchemeYear = c.Int(nullable: false),
                        ApplicationID = c.String(),
                        CalculationDate = c.DateTime(nullable: false),
                        InvoiceNumber = c.String(),
                        CalculationRefNumber = c.String(),
                        BusinessName = c.String(),
                        SBI = c.Int(nullable: false),
                        FRN = c.Long(nullable: false),
                        AddressLine1 = c.String(),
                        AddressLine2 = c.String(),
                        AddressLine3 = c.String(),
                        PostCode = c.String(),
                    })
                .PrimaryKey(t => t.SUMID);
            
            CreateTable(
                "SITI.SUM2",
                c => new
                    {
                        SUM2ID = c.Guid(nullable: false),
                        ClaimID = c.Guid(nullable: false),
                        BPSValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GreeningValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        YoungFarmerValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CrossCompliancePercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CrossComplianceReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalClaimEuro = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.SUM2ID);
            
            CreateTable(
                "SITI.YF",
                c => new
                    {
                        YFID = c.Guid(nullable: false),
                        ClaimID = c.Guid(nullable: false),
                        NonSDANumber = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SDANumber = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MoorlandNumber = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalEntitlementValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EntitlementsUsedToClaim = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AvgEntitlementValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverDeclarationArea = c.Decimal(nullable: false, precision: 18, scale: 4),
                        OverDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LateClaimSubmissionPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LateClaimSubmissionReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LateEvidencePercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LateEvidenceReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NonDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NonDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FDMPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FDMReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalYF = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdditionalOverDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PreviousYearOverDeclaration = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.YFID);
            
            CreateTable(
                "DAX.AR",
                c => new
                    {
                        ARID = c.Guid(nullable: false),
                        LogID = c.Guid(),
                        InvoiceNumber = c.String(),
                        H_InvoiceAccount = c.Long(nullable: false),
                        H_InvoiceDate = c.DateTime(nullable: false),
                        H_CurrencyCode = c.String(),
                        H_Fund = c.String(),
                        H_Scheme = c.String(),
                        H_MarketingYear = c.Int(nullable: false),
                        H_DeliveryBody = c.String(),
                        L_LineNumber = c.Int(nullable: false),
                        L_LedgerAccount = c.String(),
                        L_Fund = c.String(),
                        L_Scheme = c.String(),
                        L_MarketingYear = c.Int(nullable: false),
                        L_DeliveryBody = c.String(),
                        L_LineAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Irregularity = c.String(),
                        Admin = c.String(),
                        OriginalClaimReference = c.String(),
                        OriginalClaimSettlementDate = c.DateTime(),
                        OpenBalance = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ARID)
                .ForeignKey("Log.Log", t => t.LogID)
                .Index(t => t.LogID);
            
            CreateTable(
                "CS.Configuration",
                c => new
                    {
                        ConfigurationID = c.Guid(nullable: false),
                        Setting = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.ConfigurationID);
            
            CreateTable(
                "CS.ConversionRates",
                c => new
                    {
                        ConversionRateID = c.Guid(nullable: false),
                        Description = c.String(),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 6),
                        SchemeYear = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ConversionRateID);
            
            CreateTable(
                "CS.Errors",
                c => new
                    {
                        ErrorID = c.Guid(nullable: false),
                        ErrorDate = c.DateTime(nullable: false),
                        FRN = c.Long(nullable: false),
                        SchemeYear = c.Int(),
                        ErrorMessage = c.String(),
                        XML = c.String(),
                        Resolved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ErrorID);
            
            CreateTable(
                "FDMR.Extract",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        scheme_year = c.Int(nullable: false),
                        business_name = c.String(maxLength: 255),
                        sbi = c.Int(nullable: false),
                        frn = c.Long(nullable: false),
                        gross_after_net4 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        cross_compliance_percent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        country = c.String(nullable: false, maxLength: 16),
                        claim_number = c.String(nullable: false, maxLength: 8),
                        last_invoice_number = c.String(nullable: false, maxLength: 50),
                        last_invoice_currency = c.String(maxLength: 3),
                        Sent_to_FDMR = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "CS.Folders",
                c => new
                    {
                        FolderID = c.Guid(nullable: false),
                        Description = c.String(),
                        Path = c.String(),
                        Mask = c.String(),
                        Host = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.FolderID);
            
            CreateTable(
                "Monitor.Monitor",
                c => new
                    {
                        MonitorID = c.Guid(nullable: false),
                        MonitorProcessID = c.Guid(nullable: false),
                        Reference = c.String(),
                        StartProcess = c.DateTime(nullable: false),
                        EndProcess = c.DateTime(),
                    })
                .PrimaryKey(t => t.MonitorID)
                .ForeignKey("Monitor.MonitorProcesses", t => t.MonitorProcessID, cascadeDelete: true)
                .Index(t => t.MonitorProcessID);
            
            CreateTable(
                "Monitor.MonitorProcesses",
                c => new
                    {
                        MonitorProcessID = c.Guid(nullable: false),
                        ProcessName = c.String(),
                    })
                .PrimaryKey(t => t.MonitorProcessID);
            
            CreateTable(
                "CS.SchemeCodes",
                c => new
                    {
                        SchemeCodeID = c.Guid(nullable: false),
                        SchemeCodeName = c.String(),
                    })
                .PrimaryKey(t => t.SchemeCodeID);
            
            CreateTable(
                "CS.SchemeYears",
                c => new
                    {
                        SchemeYearID = c.Guid(nullable: false),
                        SchemeYearNumber = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.SchemeYearID);
            
            CreateTable(
                "CS.Suppressions",
                c => new
                    {
                        SuppressionID = c.Guid(nullable: false),
                        FRN = c.Long(nullable: false),
                        SuppressionStart = c.DateTime(nullable: false),
                        SuppressionEnd = c.DateTime(),
                        SchemeYear = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SuppressionID);
            
            CreateTable(
                "CS.Transformations",
                c => new
                    {
                        TransformationID = c.Guid(nullable: false),
                        StatementType = c.String(),
                        XSLT = c.String(),
                        Template = c.String(),
                    })
                .PrimaryKey(t => t.TransformationID);
            
            CreateTable(
                "XB.XB",
                c => new
                    {
                        XBID = c.Guid(nullable: false),
                        XBDataID = c.Guid(nullable: false),
                        FRN = c.Long(nullable: false),
                        SBI = c.Int(nullable: false),
                        InvoiceNumber = c.String(),
                        CalculationDate = c.DateTime(nullable: false),
                        SchemeYear = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.XBID)
                .ForeignKey("XB.XBData", t => t.XBDataID, cascadeDelete: true)
                .Index(t => t.XBDataID);
            
            CreateTable(
                "XB.XBData",
                c => new
                    {
                        XBDataID = c.Guid(nullable: false),
                        FRN = c.Long(nullable: false),
                        SBI = c.Int(nullable: false),
                        InvoiceNumber = c.String(),
                        CalculationDate = c.DateTime(nullable: false),
                        SchemeYear = c.Int(nullable: false),
                        EnglandBPSValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandGreeningValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandYoungFarmerValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandSubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandCrossCompliancePercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandCrossComplianceReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandCrossComplianceTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandTotalEuro = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesGreeningValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesYoungFarmerValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesRedistributiveValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesSubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesCrossCompliancePercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesCrossComplianceReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesCrossComplianceTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesTotalEuro = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandGreeningValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandYoungFarmerValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandSubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandCrossCompliancePercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandCrossComplianceReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandCrossComplianceTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandTotalEuro = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIBPSValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIGreeningValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIYoungFarmerValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NISubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NICrossCompliancePercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NICrossComplianceReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NICrossComplianceTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NITotalEuro = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSNonSDANumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandBPSNonSDARate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSNonSDATotal = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandBPSSDANumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandBPSSDARate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSSDATotal = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandBPSMoorlandNumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandBPSMoorlandRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSMoorlandTotal = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandBPSAverageEntitlementValue2015Value = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandBPSOverDeclarationHectares = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandBPSOverDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSOverDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSLateClaimSubmissionPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSLateClaimSubmissionReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSLateEntitlementsApplicationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSLateEntitlementsApplicationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSLateAmendmentPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSLateAmendmentReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSLateEvidencePercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSLateEvidenceReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSLateEntitlementsAmendmentPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSLateEntitlementsAmendmentReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSNonDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSNonDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSFDMPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSFDMReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSFDMTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSReductionOfPaymentsOver150kPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSReductionOfPaymentsOver150kDeduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSTotalBPSPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSBPSGross = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSRegion1Number = c.Decimal(nullable: false, precision: 18, scale: 4),
                        WalesBPSRegion1Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSRegion1Total = c.Decimal(nullable: false, precision: 18, scale: 4),
                        WalesBPSOverDeclarationHectares = c.Decimal(nullable: false, precision: 18, scale: 4),
                        WalesBPSOverDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSOverDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSLateClaimSubmissionPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSLateClaimSubmissionReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSLateEntitlementsApplicationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSLateEntitlementsApplicationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSLateAmendmentPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSLateAmendmentReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSLateEntitlementsAmendmentPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSLateEntitlementsAmendmentReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSLateEvidencePercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSLateEvidenceReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSLateEvidenceEntitlementsPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSLateEvidenceEntitlementsReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSNonDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSNonDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSFDMPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSFDMReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSReductionOfPaymentsOver150kPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSReductionOfPaymentsOver150kDeduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSReductionOfPaymentsOver200kPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSReductionOfPaymentsOver200kDeduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSReductionOfPaymentsOver250kPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSReductionOfPaymentsOver250kDeduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSReductionOfPaymentsOver300kPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSReductionOfPaymentsOver300kDeduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSTotalBPSPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSRegion1Number = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScotlandBPSRegion1Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSRegion1Total = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScotlandBPSRegion2Number = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScotlandBPSRegion2Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSRegion2Total = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScotlandBPSRegion3Number = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScotlandBPSRegion3Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSRegion3Total = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScotlandBPSAverageEntitlementValue2015Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSOverDeclarationHectares = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScotlandBPSOverDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSOverDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSLateClaimSubmissionPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSLateClaimSubmissionReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSLateEntitlementsApplicationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSLateEntitlementsApplicationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSLateAmendmentPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSLateAmendmentReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSLateEntitlementsAmendmentPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSLateEntitlementsAmendmentReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSLateEvidencePercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSLateEvidenceReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSLateEvidenceEntitlementsPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSLateEvidenceEntitlementsReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSNonDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSNonDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSFDMPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSFDMReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSReductionOfPaymentsOver150kPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSReductionOfPaymentsOver150kDeduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSTotalBPSPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSGross = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIBPSRegion1Number = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NIBPSRegion1Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIBPSRegion1Total = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NIBPSOverDeclarationHectares = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NIBPSOverDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIBPSOverDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIBPSLateClaimSubmissionPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIBPSLateClaimSubmissionReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIBPSLateEntitlementsApplicationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIBPSLateEntitlementsApplicationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIBPSLateAmendmentPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIBPSLateAmendmentReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIBPSLateEntitlementsAmendmentPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIBPSLateEntitlementsAmendmentReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIBPSLateEvidencePercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIBPSLateEvidenceReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIBPSLateEvidenceEntitlementsPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIBPSLateEvidenceEntitlementsReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIBPSNonDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIBPSNonDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIBPSFDMPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIBPSFDMReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIBPSReductionOfPaymentsOver150kPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIBPSReductionOfPaymentsOver150kDeduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIBPSTotalBPSPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandGreeningNonSDANumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandGreeningNonSDARate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandGreeningNonSDAReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandGreeningNonSDATotal = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandGreeningSDANumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandGreeningSDARate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandGreeningSDAReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandGreeningSDATotal = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandGreeningMoorlandNumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandGreeningMoorlandReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandGreeningMoorlandRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandGreeningMoorlandTotal = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandGreeningAverageGreeningValue2015Value = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandGreeningCropDiversificationNumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandGreeningCropDiversificationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandGreeningPermanentGrasslandNumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandGreeningPermanentGrasslandReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandGreeningEFANumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandGreeningEFAReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandGreeningLateApplicationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandGreeningLateApplicationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandGreeningLateEvidencePercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandGreeningLateEvidenceReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandGreeningNonDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandGreeningNonDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandGreeningFDMPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandGreeningFDMReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandGreeningTotalGreeningPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandGreeningGross = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesGreeningRegion1Number = c.Decimal(nullable: false, precision: 18, scale: 4),
                        WalesGreeningRegion1Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesGreeningRegion1Total = c.Decimal(nullable: false, precision: 18, scale: 4),
                        WalesGreeningCropDiversificationNumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        WalesGreeningCropDiversificationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesGreeningPermanentGrasslandNumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        WalesGreeningPermanentGrasslandReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesGreeningEFANumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        WalesGreeningEFAReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesGreeningLateApplicationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesGreeningLateApplicationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesGreeningLateAmendmentPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesGreeningLateAmendmentReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesGreeningLateEvidencePercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesGreeningLateEvidenceReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesGreeningNonDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesGreeningNonDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesGreeningFDMPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesGreeningFDMReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesGreeningTotalGreeningPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandGreeningRegion1Number = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScotlandGreeningRegion1Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandGreeningRegion1Total = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScotlandGreeningRegion2Number = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScotlandGreeningRegion2Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandGreeningRegion2Total = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScotlandGreeningRegion3Number = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScotlandGreeningRegion3Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandGreeningRegion3Total = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScotlandGreeningAverageGreeningValue2015Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandGreeningCropDiversificationNumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScotlandGreeningCropDiversificationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandGreeningPermanentGrasslandNumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScotlandGreeningPermanentGrasslandReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandGreeningEFANumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScotlandGreeningEFAReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandGreeningLateApplicationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandGreeningLateApplicationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandGreeningLateAmendmentPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandGreeningLateAmendmentReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandGreeningLateEvidencePercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandGreeningLateEvidenceReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandGreeningNonDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandGreeningNonDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandGreeningFDMPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandGreeningFDMReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandGreeningTotalGreeningPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandGreeningGross = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIGreeningRegion1Number = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NIGreeningRegion1Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIGreeningRegion1Total = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NIGreeningCropDiversificationNumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NIGreeningCropDiversificationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIGreeningPermanentGrasslandNumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NIGreeningPermanentGrasslandReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIGreeningEFANumber = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NIGreeningEFAReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIGreeningLateApplicationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIGreeningLateApplicationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIGreeningLateAmendmentPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIGreeningLateAmendmentReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIGreeningLateEvidencePercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIGreeningLateEvidenceReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIGreeningNonDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIGreeningNonDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIGreeningFDMPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIGreeningFDMReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIGreeningTotalGreeningPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandYFEntitlementsUsedToClaim = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandYFAvgEntitlementValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandYFYFClaimValuePercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandYFClaimValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandYFOverDeclarationArea = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandYFOverDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandYFOverDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandYFLateClaimSubmissionPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandYFLateClaimSubmissionReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandYFLateAmendmentPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandYFLateAmendmentReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandYFNonDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandYFNonDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandYFFDMPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandYFFDMReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandYFTotalYoungFarmerPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesYFEntitlementsUsedToClaim = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesYFRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesYFClaimValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesYFOverDeclarationArea = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesYFOverDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesYFOverDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesYFDeclarationArea = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesYFDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesYFDeclarationTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesYFLateClaimSubmissionPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesYFLateClaimSubmissionReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesYFLateAmendmentPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesYFLateAmendmentReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesYFLateEvidencePercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesYFLateEvidenceReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesYFNonDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesYFNonDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesYFFDMPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesYFFDMReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesYFTotalYoungFarmerPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandYFEntitlementsUsedToClaim = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandYFAvgEntitlementValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandYFClaimValuePercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandYFClaimValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandYFOverDeclarationArea = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandYFOverDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandYFOverDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandYFDeclarationArea = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandYFDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandYFLateClaimSubmissionPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandYFLateClaimSubmissionReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandYFLateAmendmentPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandYFLateAmendmentReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandYFLateEvidencePercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandYFLateEvidenceReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandYFNonDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandYFNonDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandYFFDMPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandYFFDMReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandYFTotalYoungFarmerPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIYFEntitlementsUsedToClaim = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIYFRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIYFClaimValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIYFOverDeclarationArea = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIYFOverDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIYFOverDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIYFDeclarationPenalty = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIYFDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIYFLateClaimSubmissionPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIYFLateClaimSubmissionReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIYFLateAmendmentPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIYFLateAmendmentReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIYFLateEvidencePercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIYFLateEvidenceReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIYFNonDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIYFNonDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIYFFDMPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIYFFDMReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIYFTotalYoungFarmerPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesRedLandUsedToClaim = c.Decimal(nullable: false, precision: 18, scale: 4),
                        WalesRedClaimValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesRedOverDeclarationArea = c.Decimal(nullable: false, precision: 18, scale: 4),
                        WalesRedOverDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesRedOverDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesRedLateClaimSubmissionPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesRedLateClaimSubmissionReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesRedLateAmendmentPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesRedLateAmendmentReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesRedLateEvidencePercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesRedLateEvidenceReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesRedNonDeclarationPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesRedNonDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesRedFDMPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesRedFDMReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesRedTotalRedistributionPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandNonSDAAreaOnApplication = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandNonSDAAreaIneligible = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandNonSDAEntitlements = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandSDAAreaOnApplication = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandSDAAreaIneligible = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandSDAEntitlements = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandMoorlandAreaOnApplication = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandMoorlandAreaIneligible = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandMoorlandEntitlements = c.Decimal(nullable: false, precision: 18, scale: 4),
                        WalesAreaOnApplication = c.Decimal(nullable: false, precision: 18, scale: 4),
                        WalesAreaIneligible = c.Decimal(nullable: false, precision: 18, scale: 4),
                        WalesEntitlements = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScotlandRegion1AreaOnApplication = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScotlandRegion1AreaIneligible = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScotlandRegion1Entitlements = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScotlandRegion2AreaOnApplication = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScotlandRegion2AreaIneligible = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScotlandRegion2Entitlements = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScotlandRegion3AreaOnApplication = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandRegion3AreaIneligible = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandRegion3Entitlements = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NIAreaOnApplication = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NIAreaIneligible = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NIEntitlements = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandBPSAdditionalOverDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandBPSPreviousYearOverDeclaration = c.Boolean(nullable: false),
                        WalesBPSAdditionalOverDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesBPSPreviousYearOverDeclaration = c.Boolean(nullable: false),
                        ScotlandBPSAdditionalOverDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandBPSPreviousYearOverDeclaration = c.Boolean(nullable: false),
                        NIBPSAdditionalOverDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIBPSPreviousYearOverDeclaration = c.Boolean(nullable: false),
                        EnglandYFAdditionalOverDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EnglandYFPreviousYearOverDeclaration = c.Boolean(nullable: false),
                        WalesYFAdditionalOverDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesYFPreviousYearOverDeclaration = c.Boolean(nullable: false),
                        ScotlandYFAdditionalOverDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandYFPreviousYearOverDeclaration = c.Boolean(nullable: false),
                        NIYFAdditionalOverDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIYFPreviousYearOverDeclaration = c.Boolean(nullable: false),
                        WalesRedAdditionalOverDeclarationReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesRedPreviousYearOverDeclaration = c.Boolean(nullable: false),
                        EnglandGreeningAdministrativeArea = c.Decimal(nullable: false, precision: 18, scale: 4),
                        EnglandGreeningAdministrativeReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WalesGreeningAdministrativeArea = c.Decimal(nullable: false, precision: 18, scale: 4),
                        WalesGreeningAdministrativeReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScotlandGreeningAdministrativeArea = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ScotlandGreeningAdministrativeReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NIGreeningAdministrativeArea = c.Decimal(nullable: false, precision: 18, scale: 4),
                        NIGreeningAdministrativeReduction = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.XBDataID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("XB.XB", "XBDataID", "XB.XBData");
            DropForeignKey("Monitor.Monitor", "MonitorProcessID", "Monitor.MonitorProcesses");
            DropForeignKey("DAX.AR", "LogID", "Log.Log");
            DropForeignKey("DAX.AP", "LogID", "Log.Log");
            DropForeignKey("Log.Log", "ClaimID", "SITI.Claims");
            DropForeignKey("SITI.Claims", "YFID", "SITI.YF");
            DropForeignKey("SITI.Claims", "SUM2ID", "SITI.SUM2");
            DropForeignKey("SITI.Claims", "SUMID", "SITI.SUM");
            DropForeignKey("SITI.Claims", "GRPENID", "SITI.GRPEN");
            DropForeignKey("SITI.Claims", "GRID", "SITI.GR");
            DropForeignKey("SITI.Claims", "CLDID", "SITI.CLD");
            DropForeignKey("SITI.Claims", "BPSPENID", "SITI.BPSPEN");
            DropForeignKey("SITI.Claims", "BPSID", "SITI.BPS");
            DropIndex("XB.XB", new[] { "XBDataID" });
            DropIndex("Monitor.Monitor", new[] { "MonitorProcessID" });
            DropIndex("DAX.AR", new[] { "LogID" });
            DropIndex("SITI.Claims", new[] { "CLDID" });
            DropIndex("SITI.Claims", new[] { "YFID" });
            DropIndex("SITI.Claims", new[] { "GRPENID" });
            DropIndex("SITI.Claims", new[] { "GRID" });
            DropIndex("SITI.Claims", new[] { "BPSPENID" });
            DropIndex("SITI.Claims", new[] { "BPSID" });
            DropIndex("SITI.Claims", new[] { "SUM2ID" });
            DropIndex("SITI.Claims", new[] { "SUMID" });
            DropIndex("Log.Log", new[] { "ClaimID" });
            DropIndex("DAX.AP", new[] { "LogID" });
            DropTable("XB.XBData");
            DropTable("XB.XB");
            DropTable("CS.Transformations");
            DropTable("CS.Suppressions");
            DropTable("CS.SchemeYears");
            DropTable("CS.SchemeCodes");
            DropTable("Monitor.MonitorProcesses");
            DropTable("Monitor.Monitor");
            DropTable("CS.Folders");
            DropTable("FDMR.Extract");
            DropTable("CS.Errors");
            DropTable("CS.ConversionRates");
            DropTable("CS.Configuration");
            DropTable("DAX.AR");
            DropTable("SITI.YF");
            DropTable("SITI.SUM2");
            DropTable("SITI.SUM");
            DropTable("SITI.GRPEN");
            DropTable("SITI.GR");
            DropTable("SITI.CLD");
            DropTable("SITI.BPSPEN");
            DropTable("SITI.BPS");
            DropTable("SITI.Claims");
            DropTable("Log.Log");
            DropTable("DAX.AP");
        }
    }
}
