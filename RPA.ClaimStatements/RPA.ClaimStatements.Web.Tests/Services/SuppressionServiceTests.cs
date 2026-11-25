using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using OfficeOpenXml;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Services;
using RPA.ClaimStatements.Web.Services;

namespace RPA.ClaimStatements.Web.Tests.Services
{
    [TestFixture, NonParallelizable, SetCulture("en-GB")]
    [Category("SpreadsheetPortability")]
    public class SuppressionServiceTests
    {
        string root;
        Mock<IClaimStatementsContext> context;
        Mock<IFileService> files;
        Mock<DbSet<Suppression>> suppressions;
        List<Suppression> existing;
        List<Suppression> added;
        List<string> events;
        SuppressionService service;

        [SetUp]
        public void Setup()
        {
            root = Path.Combine(TestContext.CurrentContext.WorkDirectory, "SuppressionSpreadsheet_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(root, "Uploads"));
            context = new Mock<IClaimStatementsContext>();
            existing = new List<Suppression>();
            added = new List<Suppression>();
            events = new List<string>();
            suppressions = new Mock<DbSet<Suppression>>();
            // Same IQueryable-backed DbSet pattern as Generator's test helper, without another package.
            var query = existing.AsQueryable();
            suppressions.As<IQueryable<Suppression>>().Setup(x => x.Provider).Returns(query.Provider);
            suppressions.As<IQueryable<Suppression>>().Setup(x => x.Expression).Returns(query.Expression);
            suppressions.As<IQueryable<Suppression>>().Setup(x => x.ElementType).Returns(query.ElementType);
            suppressions.As<IQueryable<Suppression>>().Setup(x => x.GetEnumerator()).Returns(() => query.GetEnumerator());
            suppressions.Setup(x => x.Add(It.IsAny<Suppression>())).Callback<Suppression>(added.Add);
            context.Setup(x => x.Suppressions).Returns(suppressions.Object);
            context.Setup(x => x.SaveChanges()).Callback(() => events.Add("save")).Returns(1);
            var folders = new Mock<IFolderService>();
            folders.Setup(x => x.Root()).Returns(root);
            files = new Mock<IFileService>();
            files.Setup(x => x.Delete(Path.Combine(root, "Uploads", "synthetic.xlsx"))).Callback<string>(path =>
            {
                using (File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
                File.Delete(path);
                events.Add("delete");
            });
            service = new SuppressionService(context.Object, folders.Object, files.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            service?.Dispose();
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }

        static MemoryStream Workbook(params object[][] rows)
        {
            using (var workbook = new ExcelPackage())
            {
                var sheet = workbook.Workbook.Worksheets.Add("First");
                for (int row = 0; row < rows.Length; row++)
                    for (int column = 0; column < rows[row].Length; column++)
                        sheet.Cells[row + 1, column + 1].Value = rows[row][column];
                workbook.Workbook.Worksheets.Add("Ignored").Cells[1, 1].Value = "9999999999";
                return new MemoryStream(workbook.GetAsByteArray());
            }
        }

        static IFormFile Upload(Stream stream)
        {
            return new FormFile(stream, 0, stream.Length, "bulkSource", "synthetic.xlsx");
        }

        [Test]
        public void BulkData_ReadsFirstSheetWithoutHeaders_PreservesBlankCells_ThenDeletesUpload()
        {
            using (var stream = Workbook(new object[] { "1000000001", "note", 12.5m },
                new object[] { "1000000002", null, 0 }))
            {
                var table = service.BulkData(Upload(stream));
                Assert.That(table.TableName, Is.EqualTo("BulkSource"));
                Assert.That(table.Columns.Cast<System.Data.DataColumn>().Select(x => x.ColumnName),
                    Is.EqualTo(new[] { "column1", "column2", "column3" }));
                Assert.That(table.Rows.Count, Is.EqualTo(2));
                Assert.That(table.Rows[0].ItemArray, Is.EqualTo(new object[] { "1000000001", "note", "12.5" }));
                Assert.That(table.Rows[1].ItemArray, Is.EqualTo(new object[] { "1000000002", "", "0" }));
            }
            Assert.That(events, Is.EqualTo(new[] { "delete" }));
            Assert.That(Directory.GetFiles(Path.Combine(root, "Uploads")), Is.Empty);
            context.Verify(x => x.SaveChanges(), Times.Never);
            files.Verify(x => x.Delete(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void BulkUpdate_Add_TrimsFrn_SkipsInvalidAndExistingActive_PreservesOtherYearsAndEndedRows()
        {
            var active = new Suppression(1000000001, 2026);
            var duplicateActive = new Suppression(1000000001, 2026);
            var otherYear = new Suppression(1000000002, 2025);
            var ended = new Suppression(1000000003, 2026) { SuppressionEnd = new DateTime(2026, 1, 1) };
            existing.AddRange(new[] { active, duplicateActive, otherYear, ended });
            var before = DateTime.Now;
            using (var stream = Workbook(new object[] { "1000000001" }, new object[] { " 1000000002 " },
                new object[] { "1000000003" }, new object[] { "invalid" }))
                service.BulkUpdate("Add", "2026", Upload(stream));
            Assert.That(added.Select(x => x.FRN), Is.EqualTo(new[] { 1000000002L, 1000000003L }));
            Assert.That(added.All(x => x.SchemeYear == 2026 && x.SuppressionID != Guid.Empty &&
                x.SuppressionEnd == null && x.SuppressionStart >= before && x.SuppressionStart <= DateTime.Now), Is.True);
            Assert.That(active.SuppressionEnd, Is.Null);
            Assert.That(duplicateActive.SuppressionEnd, Is.Null);
            Assert.That(otherYear.SuppressionEnd, Is.Null);
            Assert.That(ended.SuppressionEnd, Is.EqualTo(new DateTime(2026, 1, 1)));
            context.Verify(x => x.SetModified(It.IsAny<object>()), Times.Never);
            Assert.That(events, Is.EqualTo(new[] { "delete", "save" }));
            Assert.That(Directory.GetFiles(Path.Combine(root, "Uploads")), Is.Empty);
        }

        [Test]
        public void BulkUpdate_Remove_EndsAllMatchingActiveDuplicates_LeavesOtherYearsAndEndedRowsUntouched()
        {
            var first = new Suppression(1000000001, 2026);
            var duplicate = new Suppression(1000000001, 2026);
            var otherYear = new Suppression(1000000001, 2025);
            var unmatched = new Suppression(1000000002, 2026);
            var ended = new Suppression(1000000001, 2026) { SuppressionEnd = new DateTime(2026, 1, 1) };
            existing.AddRange(new[] { first, duplicate, otherYear, unmatched, ended });
            var before = DateTime.Now;
            using (var stream = Workbook(new object[] { "1000000001" }, new object[] { "invalid" }))
                service.BulkUpdate("Remove", "2026", Upload(stream));
            Assert.That(first.SuppressionEnd, Is.InRange(before, DateTime.Now));
            Assert.That(duplicate.SuppressionEnd, Is.InRange(before, DateTime.Now));
            Assert.That(otherYear.SuppressionEnd, Is.Null);
            Assert.That(unmatched.SuppressionEnd, Is.Null);
            Assert.That(ended.SuppressionEnd, Is.EqualTo(new DateTime(2026, 1, 1)));
            Assert.That(added, Is.Empty);
            context.Verify(x => x.SetModified(first), Times.Once);
            context.Verify(x => x.SetModified(duplicate), Times.Once);
            context.Verify(x => x.SetModified(It.IsAny<object>()), Times.Exactly(2));
            Assert.That(events, Is.EqualTo(new[] { "delete", "save" }));
            Assert.That(Directory.GetFiles(Path.Combine(root, "Uploads")), Is.Empty);
        }
    }
}
