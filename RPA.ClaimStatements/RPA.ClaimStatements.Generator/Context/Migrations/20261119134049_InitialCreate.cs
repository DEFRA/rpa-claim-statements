using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPA.ClaimStatements.Generator.Context.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "DAX");

            migrationBuilder.EnsureSchema(
                name: "SITI");

            migrationBuilder.EnsureSchema(
                name: "CS");

            migrationBuilder.EnsureSchema(
                name: "FDMR");

            migrationBuilder.EnsureSchema(
                name: "Log");

            migrationBuilder.EnsureSchema(
                name: "Monitor");

            migrationBuilder.EnsureSchema(
                name: "XB");

            migrationBuilder.CreateTable(
                name: "BPS",
                schema: "SITI",
                columns: table => new
                {
                    BPSID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NonSDANumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    NonSDATotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    SDANumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    SDATotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MoorlandNumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MoorlandSDATotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    AVGEntitlementValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SITI.BPS", x => x.BPSID);
                });

            migrationBuilder.CreateTable(
                name: "BPSPEN",
                schema: "SITI",
                columns: table => new
                {
                    BPSPENID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OverDeclarationHectares = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    OverDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateClaimSubmissionPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateClaimSubmissionReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateEntitlementsApplicationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateEntitlementsApplicationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateEvidencePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateEvidenceReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateEntitlementsAmendmentPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateEntitlementsAmendmentReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NonDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NonDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FDMPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FDMReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReductionOfPaymentsOver150kPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReductionOfPaymentsOver150kReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalBPS = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OverDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdditionalOverDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PreviousYearOverDeclaration = table.Column<bool>(type: "bit", nullable: false),
                    LateChangePenaltyReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SITI.BPSPEN", x => x.BPSPENID);
                });

            migrationBuilder.CreateTable(
                name: "CLD",
                schema: "SITI",
                columns: table => new
                {
                    CLDID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NonSDAAreaOnApplication = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    NonSDAAreaEligible = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    NonSDAEntitlements = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    SDAAreaOnApplication = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    SDAAreaEligible = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    SDAEntitlements = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MoorlandAreaOnApplication = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MoorlandAreaEligible = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MoorlandEntitlements = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SITI.CLD", x => x.CLDID);
                });

            migrationBuilder.CreateTable(
                name: "Configuration",
                schema: "CS",
                columns: table => new
                {
                    ConfigurationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Setting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CS.Configuration", x => x.ConfigurationID);
                });

            migrationBuilder.CreateTable(
                name: "ConversionRates",
                schema: "CS",
                columns: table => new
                {
                    ConversionRateID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rate = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    SchemeYear = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CS.ConversionRates", x => x.ConversionRateID);
                });

            migrationBuilder.CreateTable(
                name: "Errors",
                schema: "CS",
                columns: table => new
                {
                    ErrorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ErrorDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    FRN = table.Column<long>(type: "bigint", nullable: false),
                    SchemeYear = table.Column<int>(type: "int", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    XML = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Resolved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CS.Errors", x => x.ErrorID);
                });

            migrationBuilder.CreateTable(
                name: "Extract",
                schema: "FDMR",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    scheme_year = table.Column<int>(type: "int", nullable: false),
                    business_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    sbi = table.Column<int>(type: "int", nullable: false),
                    frn = table.Column<long>(type: "bigint", nullable: false),
                    gross_after_net4 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    cross_compliance_percent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    country = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    claim_number = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    last_invoice_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    last_invoice_currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Sent_to_FDMR = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FDMR.Extract", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Folders",
                schema: "CS",
                columns: table => new
                {
                    FolderID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mask = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Host = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CS.Folders", x => x.FolderID);
                });

            migrationBuilder.CreateTable(
                name: "GR",
                schema: "SITI",
                columns: table => new
                {
                    GRID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NonSDANumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    NonSDATotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    SDANumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    SDATotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MoorlandNumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MoorlandTotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    AvgGreeningRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SITI.GR", x => x.GRID);
                });

            migrationBuilder.CreateTable(
                name: "GRPEN",
                schema: "SITI",
                columns: table => new
                {
                    GRPENID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CropDiversificationNumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CropDiversificationNumberAdditional = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CropDiversificationRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CropDiversificationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PermanentGrasslandNumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PermanentGrasslandRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PermanentGrasslandReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EFANumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EFANumberAdditional = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EFARate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EFAReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateApplicationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateApplicationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateEvidencePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateEvidenceReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NonDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NonDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FDMReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalGreening = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdministrativeArea = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    AdministrativeReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateChangePenaltyReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SITI.GRPEN", x => x.GRPENID);
                });

            migrationBuilder.CreateTable(
                name: "MonitorProcesses",
                schema: "Monitor",
                columns: table => new
                {
                    MonitorProcessID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProcessName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monitor.MonitorProcesses", x => x.MonitorProcessID);
                });

            migrationBuilder.CreateTable(
                name: "PR",
                schema: "SITI",
                columns: table => new
                {
                    PRID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PRB1MaxBand = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PRB1Percent = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PRB1Result = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PRB2MaxBand = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PRB2Percent = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PRB2Result = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PRB3MaxBand = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PRB3Percent = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PRB3Result = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PRB4MaxBand = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PRB4Percent = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PRB4Result = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PRBTotalResult = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SITI.PR", x => x.PRID);
                });

            migrationBuilder.CreateTable(
                name: "SchemeCodes",
                schema: "CS",
                columns: table => new
                {
                    SchemeCodeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SchemeCodeName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CS.SchemeCodes", x => x.SchemeCodeID);
                });

            migrationBuilder.CreateTable(
                name: "SchemeYears",
                schema: "CS",
                columns: table => new
                {
                    SchemeYearID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SchemeYearNumber = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CS.SchemeYears", x => x.SchemeYearID);
                });

            migrationBuilder.CreateTable(
                name: "SUM",
                schema: "SITI",
                columns: table => new
                {
                    SUMID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SchemeYear = table.Column<int>(type: "int", nullable: false),
                    ApplicationID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CalculationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CalculationRefNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SBI = table.Column<int>(type: "int", nullable: false),
                    FRN = table.Column<long>(type: "bigint", nullable: false),
                    AddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressLine3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SITI.SUM", x => x.SUMID);
                });

            migrationBuilder.CreateTable(
                name: "SUM2",
                schema: "SITI",
                columns: table => new
                {
                    SUM2ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BPSValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GreeningValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    YoungFarmerValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CrossCompliancePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CrossComplianceReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalClaimEuro = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SITI.SUM2", x => x.SUM2ID);
                });

            migrationBuilder.CreateTable(
                name: "Suppressions",
                schema: "CS",
                columns: table => new
                {
                    SuppressionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FRN = table.Column<long>(type: "bigint", nullable: false),
                    SuppressionStart = table.Column<DateTime>(type: "datetime", nullable: false),
                    SuppressionEnd = table.Column<DateTime>(type: "datetime", nullable: true),
                    SchemeYear = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CS.Suppressions", x => x.SuppressionID);
                });

            migrationBuilder.CreateTable(
                name: "Transformations",
                schema: "CS",
                columns: table => new
                {
                    TransformationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatementType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    XSLT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Template = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CS.Transformations", x => x.TransformationID);
                });

            migrationBuilder.CreateTable(
                name: "XBData",
                schema: "XB",
                columns: table => new
                {
                    XBDataID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FRN = table.Column<long>(type: "bigint", nullable: false),
                    SBI = table.Column<int>(type: "int", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CalculationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    SchemeYear = table.Column<int>(type: "int", nullable: false),
                    EnglandBPSValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandGreeningValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandYoungFarmerValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandSubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandCrossCompliancePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandCrossComplianceReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandCrossComplianceTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandTotalEuro = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesGreeningValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesYoungFarmerValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesRedistributiveValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesSubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesCrossCompliancePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesCrossComplianceReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesCrossComplianceTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesTotalEuro = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandGreeningValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandYoungFarmerValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandSubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandCrossCompliancePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandCrossComplianceReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandCrossComplianceTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandTotalEuro = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIBPSValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIGreeningValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIYoungFarmerValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NISubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NICrossCompliancePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NICrossComplianceReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NICrossComplianceTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NITotalEuro = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSNonSDANumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandBPSNonSDARate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSNonSDATotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandBPSSDANumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandBPSSDARate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSSDATotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandBPSMoorlandNumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandBPSMoorlandRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSMoorlandTotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandBPSAverageEntitlementValue2015Value = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandBPSOverDeclarationHectares = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandBPSOverDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSOverDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSLateClaimSubmissionPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSLateClaimSubmissionReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSLateEntitlementsApplicationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSLateEntitlementsApplicationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSLateAmendmentPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSLateAmendmentReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSLateEvidencePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSLateEvidenceReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSLateEntitlementsAmendmentPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSLateEntitlementsAmendmentReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSNonDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSNonDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSFDMPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSFDMReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSFDMTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSReductionOfPaymentsOver150kPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSReductionOfPaymentsOver150kDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSTotalBPSPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSBPSGross = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSRegion1Number = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    WalesBPSRegion1Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSRegion1Total = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    WalesBPSOverDeclarationHectares = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    WalesBPSOverDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSOverDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSLateClaimSubmissionPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSLateClaimSubmissionReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSLateEntitlementsApplicationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSLateEntitlementsApplicationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSLateAmendmentPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSLateAmendmentReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSLateEntitlementsAmendmentPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSLateEntitlementsAmendmentReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSLateEvidencePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSLateEvidenceReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSLateEvidenceEntitlementsPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSLateEvidenceEntitlementsReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSNonDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSNonDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSFDMPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSFDMReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSReductionOfPaymentsOver150kPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSReductionOfPaymentsOver150kDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSReductionOfPaymentsOver200kPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSReductionOfPaymentsOver200kDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSReductionOfPaymentsOver250kPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSReductionOfPaymentsOver250kDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSReductionOfPaymentsOver300kPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSReductionOfPaymentsOver300kDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSTotalBPSPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSRegion1Number = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ScotlandBPSRegion1Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSRegion1Total = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ScotlandBPSRegion2Number = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ScotlandBPSRegion2Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSRegion2Total = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ScotlandBPSRegion3Number = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ScotlandBPSRegion3Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSRegion3Total = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ScotlandBPSAverageEntitlementValue2015Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSOverDeclarationHectares = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ScotlandBPSOverDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSOverDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSLateClaimSubmissionPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSLateClaimSubmissionReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSLateEntitlementsApplicationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSLateEntitlementsApplicationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSLateAmendmentPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSLateAmendmentReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSLateEntitlementsAmendmentPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSLateEntitlementsAmendmentReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSLateEvidencePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSLateEvidenceReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSLateEvidenceEntitlementsPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSLateEvidenceEntitlementsReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSNonDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSNonDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSFDMPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSFDMReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSReductionOfPaymentsOver150kPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSReductionOfPaymentsOver150kDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSTotalBPSPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSGross = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIBPSRegion1Number = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    NIBPSRegion1Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIBPSRegion1Total = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    NIBPSOverDeclarationHectares = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    NIBPSOverDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIBPSOverDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIBPSLateClaimSubmissionPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIBPSLateClaimSubmissionReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIBPSLateEntitlementsApplicationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIBPSLateEntitlementsApplicationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIBPSLateAmendmentPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIBPSLateAmendmentReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIBPSLateEntitlementsAmendmentPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIBPSLateEntitlementsAmendmentReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIBPSLateEvidencePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIBPSLateEvidenceReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIBPSLateEvidenceEntitlementsPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIBPSLateEvidenceEntitlementsReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIBPSNonDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIBPSNonDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIBPSFDMPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIBPSFDMReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIBPSReductionOfPaymentsOver150kPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIBPSReductionOfPaymentsOver150kDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIBPSTotalBPSPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandGreeningNonSDANumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandGreeningNonSDARate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandGreeningNonSDAReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandGreeningNonSDATotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandGreeningSDANumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandGreeningSDARate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandGreeningSDAReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandGreeningSDATotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandGreeningMoorlandNumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandGreeningMoorlandReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandGreeningMoorlandRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandGreeningMoorlandTotal = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandGreeningAverageGreeningValue2015Value = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandGreeningCropDiversificationNumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandGreeningCropDiversificationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandGreeningPermanentGrasslandNumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandGreeningPermanentGrasslandReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandGreeningEFANumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandGreeningEFAReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandGreeningLateApplicationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandGreeningLateApplicationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandGreeningLateEvidencePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandGreeningLateEvidenceReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandGreeningLateChangePenalty = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandGreeningNonDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandGreeningNonDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandGreeningFDMPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandGreeningFDMReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandGreeningTotalGreeningPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandGreeningGross = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesGreeningRegion1Number = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    WalesGreeningRegion1Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesGreeningRegion1Total = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    WalesGreeningCropDiversificationNumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    WalesGreeningCropDiversificationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesGreeningPermanentGrasslandNumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    WalesGreeningPermanentGrasslandReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesGreeningEFANumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    WalesGreeningEFAReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesGreeningLateApplicationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesGreeningLateApplicationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesGreeningLateAmendmentPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesGreeningLateAmendmentReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesGreeningLateEvidencePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesGreeningLateEvidenceReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesGreeningNonDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesGreeningNonDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesGreeningFDMPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesGreeningFDMReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesGreeningTotalGreeningPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandGreeningRegion1Number = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ScotlandGreeningRegion1Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandGreeningRegion1Total = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ScotlandGreeningRegion2Number = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ScotlandGreeningRegion2Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandGreeningRegion2Total = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ScotlandGreeningRegion3Number = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ScotlandGreeningRegion3Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandGreeningRegion3Total = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ScotlandGreeningAverageGreeningValue2015Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandGreeningCropDiversificationNumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ScotlandGreeningCropDiversificationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandGreeningPermanentGrasslandNumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ScotlandGreeningPermanentGrasslandReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandGreeningEFANumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ScotlandGreeningEFAReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandGreeningLateApplicationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandGreeningLateApplicationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandGreeningLateAmendmentPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandGreeningLateAmendmentReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandGreeningLateEvidencePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandGreeningLateEvidenceReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandGreeningNonDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandGreeningNonDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandGreeningFDMPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandGreeningFDMReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandGreeningTotalGreeningPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandGreeningGross = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIGreeningRegion1Number = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    NIGreeningRegion1Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIGreeningRegion1Total = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    NIGreeningCropDiversificationNumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    NIGreeningCropDiversificationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIGreeningPermanentGrasslandNumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    NIGreeningPermanentGrasslandReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIGreeningEFANumber = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    NIGreeningEFAReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIGreeningLateApplicationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIGreeningLateApplicationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIGreeningLateAmendmentPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIGreeningLateAmendmentReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIGreeningLateEvidencePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIGreeningLateEvidenceReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIGreeningNonDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIGreeningNonDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIGreeningFDMPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIGreeningFDMReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIGreeningTotalGreeningPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandYFEntitlementsUsedToClaim = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandYFAvgEntitlementValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandYFYFClaimValuePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandYFClaimValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandYFOverDeclarationArea = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandYFOverDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandYFOverDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandYFLateClaimSubmissionPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandYFLateClaimSubmissionReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandYFLateAmendmentPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandYFLateAmendmentReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandYFNonDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandYFNonDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandYFFDMPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandYFFDMReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandYFTotalYoungFarmerPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesYFEntitlementsUsedToClaim = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesYFRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesYFClaimValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesYFOverDeclarationArea = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesYFOverDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesYFOverDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesYFDeclarationArea = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesYFDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesYFDeclarationTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesYFLateClaimSubmissionPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesYFLateClaimSubmissionReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesYFLateAmendmentPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesYFLateAmendmentReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesYFLateEvidencePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesYFLateEvidenceReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesYFNonDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesYFNonDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesYFFDMPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesYFFDMReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesYFTotalYoungFarmerPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandYFEntitlementsUsedToClaim = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandYFAvgEntitlementValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandYFClaimValuePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandYFClaimValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandYFOverDeclarationArea = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandYFOverDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandYFOverDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandYFDeclarationArea = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandYFDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandYFLateClaimSubmissionPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandYFLateClaimSubmissionReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandYFLateAmendmentPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandYFLateAmendmentReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandYFLateEvidencePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandYFLateEvidenceReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandYFNonDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandYFNonDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandYFFDMPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandYFFDMReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandYFTotalYoungFarmerPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIYFEntitlementsUsedToClaim = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIYFRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIYFClaimValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIYFOverDeclarationArea = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIYFOverDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIYFOverDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIYFDeclarationPenalty = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIYFDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIYFLateClaimSubmissionPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIYFLateClaimSubmissionReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIYFLateAmendmentPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIYFLateAmendmentReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIYFLateEvidencePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIYFLateEvidenceReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIYFNonDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIYFNonDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIYFFDMPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIYFFDMReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIYFTotalYoungFarmerPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesRedLandUsedToClaim = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    WalesRedClaimValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesRedOverDeclarationArea = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    WalesRedOverDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesRedOverDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesRedLateClaimSubmissionPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesRedLateClaimSubmissionReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesRedLateAmendmentPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesRedLateAmendmentReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesRedLateEvidencePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesRedLateEvidenceReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesRedNonDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesRedNonDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesRedFDMPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesRedFDMReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesRedTotalRedistributionPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandNonSDAAreaOnApplication = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandNonSDAAreaIneligible = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandNonSDAEntitlements = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandSDAAreaOnApplication = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandSDAAreaIneligible = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandSDAEntitlements = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandMoorlandAreaOnApplication = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandMoorlandAreaIneligible = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandMoorlandEntitlements = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    WalesAreaOnApplication = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    WalesAreaIneligible = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    WalesEntitlements = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ScotlandRegion1AreaOnApplication = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ScotlandRegion1AreaIneligible = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ScotlandRegion1Entitlements = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ScotlandRegion2AreaOnApplication = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ScotlandRegion2AreaIneligible = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ScotlandRegion2Entitlements = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ScotlandRegion3AreaOnApplication = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandRegion3AreaIneligible = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandRegion3Entitlements = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    NIAreaOnApplication = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    NIAreaIneligible = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    NIEntitlements = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandBPSAdditionalOverDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandBPSPreviousYearOverDeclaration = table.Column<bool>(type: "bit", nullable: false),
                    WalesBPSAdditionalOverDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesBPSPreviousYearOverDeclaration = table.Column<bool>(type: "bit", nullable: false),
                    ScotlandBPSAdditionalOverDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandBPSPreviousYearOverDeclaration = table.Column<bool>(type: "bit", nullable: false),
                    NIBPSAdditionalOverDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIBPSPreviousYearOverDeclaration = table.Column<bool>(type: "bit", nullable: false),
                    EnglandYFAdditionalOverDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandYFPreviousYearOverDeclaration = table.Column<bool>(type: "bit", nullable: false),
                    WalesYFAdditionalOverDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesYFPreviousYearOverDeclaration = table.Column<bool>(type: "bit", nullable: false),
                    ScotlandYFAdditionalOverDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandYFPreviousYearOverDeclaration = table.Column<bool>(type: "bit", nullable: false),
                    NIYFAdditionalOverDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIYFPreviousYearOverDeclaration = table.Column<bool>(type: "bit", nullable: false),
                    WalesRedAdditionalOverDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesRedPreviousYearOverDeclaration = table.Column<bool>(type: "bit", nullable: false),
                    EnglandGreeningAdministrativeArea = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EnglandGreeningAdministrativeReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesGreeningAdministrativeArea = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    WalesGreeningAdministrativeReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandGreeningAdministrativeArea = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ScotlandGreeningAdministrativeReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIGreeningAdministrativeArea = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    NIGreeningAdministrativeReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandGreeningCropDiversificationReductionAdditional = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandGreeningCropDiversificationReductionAdditional = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesGreeningCropDiversificationReductionAdditional = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIGreeningCropDiversificationReductionAdditional = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnglandGreeningEFAReductionAdditional = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ScotlandGreeningEFAReductionAdditional = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalesGreeningEFAReductionAdditional = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NIGreeningEFAReductionAdditional = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XB.XBData", x => x.XBDataID);
                });

            migrationBuilder.CreateTable(
                name: "YF",
                schema: "SITI",
                columns: table => new
                {
                    YFID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NonSDANumber = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SDANumber = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MoorlandNumber = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalEntitlementValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EntitlementsUsedToClaim = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    AvgEntitlementValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OverDeclarationArea = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    OverDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateClaimSubmissionPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateClaimSubmissionReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateEvidencePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LateEvidenceReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NonDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NonDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FDMPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FDMReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalYF = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OverDeclarationPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdditionalOverDeclarationReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PreviousYearOverDeclaration = table.Column<bool>(type: "bit", nullable: false),
                    LateChangePenaltyReduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SITI.YF", x => x.YFID);
                });

            migrationBuilder.CreateTable(
                name: "Monitor",
                schema: "Monitor",
                columns: table => new
                {
                    MonitorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MonitorProcessID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartProcess = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndProcess = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monitor.Monitor", x => x.MonitorID);
                    table.ForeignKey(
                        name: "FK_Monitor.Monitor_Monitor.MonitorProcesses_MonitorProcessID",
                        column: x => x.MonitorProcessID,
                        principalSchema: "Monitor",
                        principalTable: "MonitorProcesses",
                        principalColumn: "MonitorProcessID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "XB",
                schema: "XB",
                columns: table => new
                {
                    XBID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    XBDataID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FRN = table.Column<long>(type: "bigint", nullable: false),
                    SBI = table.Column<int>(type: "int", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CalculationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    SchemeYear = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XB.XB", x => x.XBID);
                    table.ForeignKey(
                        name: "FK_XB.XB_XB.XBData_XBDataID",
                        column: x => x.XBDataID,
                        principalSchema: "XB",
                        principalTable: "XBData",
                        principalColumn: "XBDataID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Claims",
                schema: "SITI",
                columns: table => new
                {
                    ClaimID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SUMID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SUM2ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BPSID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BPSPENID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GRID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GRPENID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    YFID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CLDID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PRID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SITI.Claims", x => x.ClaimID);
                    table.ForeignKey(
                        name: "FK_SITI.Claims_SITI.BPSPEN_BPSPENID",
                        column: x => x.BPSPENID,
                        principalSchema: "SITI",
                        principalTable: "BPSPEN",
                        principalColumn: "BPSPENID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SITI.Claims_SITI.BPS_BPSID",
                        column: x => x.BPSID,
                        principalSchema: "SITI",
                        principalTable: "BPS",
                        principalColumn: "BPSID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SITI.Claims_SITI.CLD_CLDID",
                        column: x => x.CLDID,
                        principalSchema: "SITI",
                        principalTable: "CLD",
                        principalColumn: "CLDID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SITI.Claims_SITI.GRPEN_GRPENID",
                        column: x => x.GRPENID,
                        principalSchema: "SITI",
                        principalTable: "GRPEN",
                        principalColumn: "GRPENID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SITI.Claims_SITI.GR_GRID",
                        column: x => x.GRID,
                        principalSchema: "SITI",
                        principalTable: "GR",
                        principalColumn: "GRID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SITI.Claims_SITI.PR_PRID",
                        column: x => x.PRID,
                        principalSchema: "SITI",
                        principalTable: "PR",
                        principalColumn: "PRID");
                    table.ForeignKey(
                        name: "FK_SITI.Claims_SITI.SUM2_SUM2ID",
                        column: x => x.SUM2ID,
                        principalSchema: "SITI",
                        principalTable: "SUM2",
                        principalColumn: "SUM2ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SITI.Claims_SITI.SUM_SUMID",
                        column: x => x.SUMID,
                        principalSchema: "SITI",
                        principalTable: "SUM",
                        principalColumn: "SUMID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SITI.Claims_SITI.YF_YFID",
                        column: x => x.YFID,
                        principalSchema: "SITI",
                        principalTable: "YF",
                        principalColumn: "YFID");
                });

            migrationBuilder.CreateTable(
                name: "Log",
                schema: "Log",
                columns: table => new
                {
                    LogID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SBI = table.Column<int>(type: "int", nullable: false),
                    FRN = table.Column<long>(type: "bigint", nullable: false),
                    SchemeYear = table.Column<int>(type: "int", nullable: false),
                    ClaimProduced = table.Column<DateTime>(type: "datetime", nullable: true),
                    PDFFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Uploaded = table.Column<DateTime>(type: "datetime", nullable: true),
                    UploadedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log.Log", x => x.LogID);
                    table.ForeignKey(
                        name: "FK_Log.Log_SITI.Claims_ClaimID",
                        column: x => x.ClaimID,
                        principalSchema: "SITI",
                        principalTable: "Claims",
                        principalColumn: "ClaimID");
                });

            migrationBuilder.CreateTable(
                name: "AP",
                schema: "DAX",
                columns: table => new
                {
                    APID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LogID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastSettlementDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Supplier = table.Column<long>(type: "bigint", nullable: false),
                    SupplierName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Invoice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastPaymentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    InvoiceDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    InvoiceDueDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Scheme = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MarketingYear = table.Column<int>(type: "int", nullable: false),
                    DeliveryBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionInvoiceValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastHoldReleaseDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    FESReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DAX.AP", x => x.APID);
                    table.ForeignKey(
                        name: "FK_DAX.AP_Log.Log_LogID",
                        column: x => x.LogID,
                        principalSchema: "Log",
                        principalTable: "Log",
                        principalColumn: "LogID");
                });

            migrationBuilder.CreateTable(
                name: "AR",
                schema: "DAX",
                columns: table => new
                {
                    ARID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LogID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    H_InvoiceAccount = table.Column<long>(type: "bigint", nullable: false),
                    H_InvoiceDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    H_CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    H_Fund = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    H_Scheme = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    H_MarketingYear = table.Column<int>(type: "int", nullable: false),
                    H_DeliveryBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    L_LineNumber = table.Column<int>(type: "int", nullable: false),
                    L_LedgerAccount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    L_Fund = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    L_Scheme = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    L_MarketingYear = table.Column<int>(type: "int", nullable: false),
                    L_DeliveryBody = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    L_LineAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Irregularity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Admin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginalClaimReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginalClaimSettlementDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    OpenBalance = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DAX.AR", x => x.ARID);
                    table.ForeignKey(
                        name: "FK_DAX.AR_Log.Log_LogID",
                        column: x => x.LogID,
                        principalSchema: "Log",
                        principalTable: "Log",
                        principalColumn: "LogID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogID",
                schema: "DAX",
                table: "AP",
                column: "LogID");

            migrationBuilder.CreateIndex(
                name: "IX_LogID",
                schema: "DAX",
                table: "AR",
                column: "LogID");

            migrationBuilder.CreateIndex(
                name: "IX_BPSID",
                schema: "SITI",
                table: "Claims",
                column: "BPSID");

            migrationBuilder.CreateIndex(
                name: "IX_BPSPENID",
                schema: "SITI",
                table: "Claims",
                column: "BPSPENID");

            migrationBuilder.CreateIndex(
                name: "IX_CLDID",
                schema: "SITI",
                table: "Claims",
                column: "CLDID");

            migrationBuilder.CreateIndex(
                name: "IX_GRID",
                schema: "SITI",
                table: "Claims",
                column: "GRID");

            migrationBuilder.CreateIndex(
                name: "IX_GRPENID",
                schema: "SITI",
                table: "Claims",
                column: "GRPENID");

            migrationBuilder.CreateIndex(
                name: "IX_PRID",
                schema: "SITI",
                table: "Claims",
                column: "PRID");

            migrationBuilder.CreateIndex(
                name: "IX_SUM2ID",
                schema: "SITI",
                table: "Claims",
                column: "SUM2ID");

            migrationBuilder.CreateIndex(
                name: "IX_SUMID",
                schema: "SITI",
                table: "Claims",
                column: "SUMID");

            migrationBuilder.CreateIndex(
                name: "IX_YFID",
                schema: "SITI",
                table: "Claims",
                column: "YFID");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimID",
                schema: "Log",
                table: "Log",
                column: "ClaimID");

            migrationBuilder.CreateIndex(
                name: "IX_MonitorProcessID",
                schema: "Monitor",
                table: "Monitor",
                column: "MonitorProcessID");

            migrationBuilder.CreateIndex(
                name: "IX_XBDataID",
                schema: "XB",
                table: "XB",
                column: "XBDataID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AP",
                schema: "DAX");

            migrationBuilder.DropTable(
                name: "AR",
                schema: "DAX");

            migrationBuilder.DropTable(
                name: "Configuration",
                schema: "CS");

            migrationBuilder.DropTable(
                name: "ConversionRates",
                schema: "CS");

            migrationBuilder.DropTable(
                name: "Errors",
                schema: "CS");

            migrationBuilder.DropTable(
                name: "Extract",
                schema: "FDMR");

            migrationBuilder.DropTable(
                name: "Folders",
                schema: "CS");

            migrationBuilder.DropTable(
                name: "Monitor",
                schema: "Monitor");

            migrationBuilder.DropTable(
                name: "SchemeCodes",
                schema: "CS");

            migrationBuilder.DropTable(
                name: "SchemeYears",
                schema: "CS");

            migrationBuilder.DropTable(
                name: "Suppressions",
                schema: "CS");

            migrationBuilder.DropTable(
                name: "Transformations",
                schema: "CS");

            migrationBuilder.DropTable(
                name: "XB",
                schema: "XB");

            migrationBuilder.DropTable(
                name: "Log",
                schema: "Log");

            migrationBuilder.DropTable(
                name: "MonitorProcesses",
                schema: "Monitor");

            migrationBuilder.DropTable(
                name: "XBData",
                schema: "XB");

            migrationBuilder.DropTable(
                name: "Claims",
                schema: "SITI");

            migrationBuilder.DropTable(
                name: "BPSPEN",
                schema: "SITI");

            migrationBuilder.DropTable(
                name: "BPS",
                schema: "SITI");

            migrationBuilder.DropTable(
                name: "CLD",
                schema: "SITI");

            migrationBuilder.DropTable(
                name: "GRPEN",
                schema: "SITI");

            migrationBuilder.DropTable(
                name: "GR",
                schema: "SITI");

            migrationBuilder.DropTable(
                name: "PR",
                schema: "SITI");

            migrationBuilder.DropTable(
                name: "SUM2",
                schema: "SITI");

            migrationBuilder.DropTable(
                name: "SUM",
                schema: "SITI");

            migrationBuilder.DropTable(
                name: "YF",
                schema: "SITI");
        }
    }
}
