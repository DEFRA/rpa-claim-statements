using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using OfficeOpenXml;
using RPA.ClaimStatements.Generator.Components.PumpingStation;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Models.Entities.DAX;
using RPA.ClaimStatements.Generator.Services;
using RPA.ClaimStatements.Generator.Tests.TestSupport;
using Monitor = RPA.ClaimStatements.Generator.Models.Entities.CS.Monitor;

namespace RPA.ClaimStatements.Generator.Tests.Components.PumpingStation
{
    [TestFixture, NonParallelizable, SetCulture("en-GB")]
    [Category("SpreadsheetPortability")]
    public class ARImportServiceTests
    {
        string root;
        string source;
        Mock<IClaimStatementsContext> context;
        Mock<IConfigurationService> configuration;
        Mock<IFolderService> folders;
        Mock<IFileService> files;
        Mock<IMonitorService> monitors;
        Mock<IErrorService> errors;
        Mock<DbSet<AR>> arSet;
        List<AR> added;
        List<string> events;
        ARImportService service;

        [SetUp]
        public void Setup()
        {
            root = Path.Combine(TestContext.CurrentContext.WorkDirectory, "ARSpreadsheet_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(root);
            source = Path.Combine(root, "synthetic.xlsx");
            added = new List<AR>();
            events = new List<string>();
            context = new Mock<IClaimStatementsContext>();
            arSet = new Mock<DbSet<AR>>().SetupData(new List<AR>());
            arSet.Setup(x => x.Add(It.IsAny<AR>())).Callback<AR>(added.Add);
            context.Setup(x => x.AR).Returns(arSet.Object);
            context.Setup(x => x.SchemeCodes).Returns(new Mock<DbSet<SchemeCode>>()
                .SetupData(new[] { new SchemeCode { SchemeCodeName = "BPS" } }).Object);
            context.Setup(x => x.SaveChanges()).Callback(() => events.Add("save")).Returns(1);
            configuration = new Mock<IConfigurationService>();
            configuration.Setup(x => x.IsActive("AR Load")).Returns(true);
            folders = new Mock<IFolderService>();
            folders.Setup(x => x.Get("DAX AR Inbound")).Returns(new Folder("synthetic", root, "*.xlsx"));
            folders.Setup(x => x.GetPath("DAX AR Archive")).Returns(Path.Combine(root, "Archive"));
            files = new Mock<IFileService>();
            files.Setup(x => x.Archive(source, Path.Combine(root, "Archive"), true, true))
                .Callback(() =>
                {
                    using (File.Open(source, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
                    Directory.CreateDirectory(Path.Combine(root, "Archive"));
                    File.Move(source, Path.Combine(root, "Archive", "synthetic.xlsx"));
                    events.Add("archive");
                }).Returns("synthetic.xlsx");
            monitors = new Mock<IMonitorService>();
            monitors.Setup(x => x.CreateMonitor(It.IsAny<Monitor>())).Callback(() => events.Add("start"));
            monitors.Setup(x => x.EndMonitor(It.IsAny<Monitor>())).Callback(() => events.Add("end"));
            errors = new Mock<IErrorService>();
            service = new ARImportService(context.Object, configuration.Object, folders.Object, errors.Object, files.Object, monitors.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            service?.Dispose();
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }

        static object[] Row(string invoice = "INV-001")
        {
            return new object[] { invoice, "1000000001", "2026-03-14", "GBP", "HF", "BPS",
                "2026", "HD", "2", "LEDGER", "LF", "LS", "2025", "LD", "1234.56",
                "IRREG", "ADMIN", "CLAIM-1", "2025-11-30", "42" };
        }

        void WriteWorkbook(params object[][][] sheets)
        {
            using (var workbook = new ExcelPackage())
            {
                for (int i = 0; i < sheets.Length; i++)
                {
                    var sheet = workbook.Workbook.Worksheets.Add("Sheet" + (i + 1));
                    for (int column = 1; column <= 20; column++) sheet.Cells[1, column].Value = "Header" + column;
                    for (int row = 0; row < sheets[i].Length; row++)
                        for (int column = 0; column < 20; column++)
                            sheet.Cells[row + 2, column + 1].Value = sheets[i][row][column];
                }
                workbook.SaveAs(new FileInfo(source));
            }
        }

        [Test]
        public void Import_ReadsAllWorksheets_MapsFieldsAndFiltersSchemes_BeforeArchiveAndEnd()
        {
            var optional = Row("INV-002");
            foreach (int column in new[] { 6, 12, 14, 18, 19 }) optional[column] = null;
            var excluded = Row("EXCLUDED");
            excluded[5] = "OTHER";
            WriteWorkbook(new[] { Row(), excluded }, new[] { optional });

            service.Import();

            Assert.That(added.Count, Is.EqualTo(2));
            var item = added.Single(x => x.InvoiceNumber == "INV-001");
            Assert.Multiple(() =>
            {
                Assert.That(item.ARID, Is.Not.EqualTo(Guid.Empty));
                Assert.That(item.H_InvoiceAccount, Is.EqualTo(1000000001L));
                Assert.That(item.H_InvoiceDate, Is.EqualTo(new DateTime(2026, 3, 14)));
                Assert.That(item.L_LineNumber, Is.EqualTo(2));
                Assert.That(item.H_CurrencyCode, Is.EqualTo("GBP"));
                Assert.That(item.H_Fund, Is.EqualTo("HF"));
                Assert.That(item.H_Scheme, Is.EqualTo("BPS"));
                Assert.That(item.H_MarketingYear, Is.EqualTo(2026));
                Assert.That(item.H_DeliveryBody, Is.EqualTo("HD"));
                Assert.That(item.L_LedgerAccount, Is.EqualTo("LEDGER"));
                Assert.That(item.L_Fund, Is.EqualTo("LF"));
                Assert.That(item.L_Scheme, Is.EqualTo("LS"));
                Assert.That(item.L_MarketingYear, Is.EqualTo(2025));
                Assert.That(item.L_DeliveryBody, Is.EqualTo("LD"));
                Assert.That(item.L_LineAmount, Is.EqualTo(-1234.56m));
                Assert.That(item.Irregularity, Is.EqualTo("IRREG"));
                Assert.That(item.Admin, Is.EqualTo("ADMIN"));
                Assert.That(item.OriginalClaimReference, Is.EqualTo("CLAIM-1"));
                Assert.That(item.OriginalClaimSettlementDate, Is.EqualTo(new DateTime(2025, 11, 30)));
                Assert.That(item.OpenBalance, Is.EqualTo(42));
            });
            var empty = added.Single(x => x.InvoiceNumber == "INV-002");
            Assert.Multiple(() =>
            {
                Assert.That(empty.H_MarketingYear, Is.EqualTo(0));
                Assert.That(empty.L_MarketingYear, Is.EqualTo(0));
                Assert.That(empty.L_LineAmount, Is.EqualTo(0m));
                Assert.That(empty.OriginalClaimSettlementDate, Is.Null);
                Assert.That(empty.OpenBalance, Is.EqualTo(0));
            });
            Assert.That(events, Is.EqualTo(new[] { "start", "save", "archive", "end" }));
            Assert.That(File.Exists(source), Is.False);
            Assert.That(File.Exists(Path.Combine(root, "Archive", "synthetic.xlsx")), Is.True);
            errors.VerifyNoOtherCalls();
        }

        [Test]
        public void Import_SkipsExistingCompositeKey_ButKeepsDifferentLine()
        {
            arSet.SetupData(new[] { new AR { H_InvoiceAccount = 1000000001L, InvoiceNumber = "INV-001", L_LineNumber = 2 } });
            var nextLine = Row();
            nextLine[8] = "3";
            WriteWorkbook(new[] { Row(), nextLine });
            service.Import();
            Assert.That(added.Select(x => x.L_LineNumber), Is.EqualTo(new[] { 3 }));
            Assert.That(events, Is.EqualTo(new[] { "start", "save", "archive", "end" }));
            errors.VerifyNoOtherCalls();
        }

        [TestCase(1), TestCase(2), TestCase(8)]
        public void Import_SkipsInvalidRequiredKey_ButStillSavesAndArchives(int column)
        {
            var invalid = Row();
            invalid[column] = "invalid";
            WriteWorkbook(new[] { invalid, Row("VALID") });
            service.Import();
            Assert.That(added.Select(x => x.InvoiceNumber), Is.EqualTo(new[] { "VALID" }));
            Assert.That(events, Is.EqualTo(new[] { "start", "save", "archive", "end" }));
            errors.VerifyNoOtherCalls();
        }

        [Test]
        public void Import_SaveFailure_IsLoggedWithoutArchiveOrEnd()
        {
            WriteWorkbook(new[] { Row() });
            context.Setup(x => x.SaveChanges()).Throws(new InvalidOperationException("synthetic save failure"));
            service.Import();
            Assert.That(added.Count, Is.EqualTo(1));
            Assert.That(events, Is.EqualTo(new[] { "start" }));
            Assert.That(File.Exists(source), Is.True);
            errors.Verify(x => x.Log(0, 0, It.Is<string>(s => s.Contains("synthetic save failure"))), Times.Once);
            files.VerifyNoOtherCalls();
        }

        [Test]
        public void Import_Disabled_DoesNotAccessFoldersFilesOrDatabase()
        {
            configuration.Setup(x => x.IsActive("AR Load")).Returns(false);
            Directory.Delete(root);
            service.Import();
            Assert.That(Directory.Exists(root), Is.False);
            context.Verify(x => x.SaveChanges(), Times.Never);
            context.Verify(x => x.AR, Times.Never);
            context.Verify(x => x.SchemeCodes, Times.Never);
            folders.VerifyNoOtherCalls();
            files.VerifyNoOtherCalls();
            monitors.VerifyNoOtherCalls();
            errors.VerifyNoOtherCalls();
        }
    }
}
