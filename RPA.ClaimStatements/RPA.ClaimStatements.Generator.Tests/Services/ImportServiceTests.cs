using Moq;
using NUnit.Framework;
using RPA.ClaimStatements.Data.UnitOfWork;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Tests.Services
{
    [TestFixture]
    [Category("Import Service")]
    public class ImportServiceTests
    {
        [Test]
        public void Test_CreateAR_Creates_AR()
        {
            FakeUnitOfWork uow = new FakeUnitOfWork();
            Mock<ConfigurationService> configurationService = new Mock<ConfigurationService>();
            Mock<FolderService> folderService = new Mock<FolderService>();
            Mock<ErrorService> errorService = new Mock<ErrorService>();
            Mock<FileService> fileService = new Mock<FileService>();
            ImportService importService = new ImportService(uow, configurationService.Object, folderService.Object, errorService.Object, fileService.Object);

            DataTable table = new DataTable();

            for (int i = 1; i <=20; i++)
            {
                table.Columns.Add();
            }

            DataRow row = table.NewRow();
            row[0] = "Invoice";
            row[1] = "1234567890";
            row[2] = "01/01/2016";
            row[3] = "EUR";
            row[4] = "Fund";
            row[5] = "10501";
            row[6] = "2016";
            row[7] = "Delivery Body";
            row[8] = "1";
            row[9] = "Account";
            row[10] = "Fund";
            row[11] = "Scheme";
            row[12] = "2016";
            row[13] = "Delivery Body";
            row[14] = "100.00";
            row[15] = "Irregularity";
            row[16] = "Admin";
            row[17] = "Claim Reference";
            row[18] = "01/01/2016";
            row[19] = "10";


            importService.CreateAR(row);

            var result = uow.ARRepository.FindBy(x => x.H_InvoiceAccount == 1234567890).FirstOrDefault();

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_CreateAR_Creates_AR_Without_Invalid_Data()
        {
            FakeUnitOfWork uow = new FakeUnitOfWork();
            Mock<ConfigurationService> configurationService = new Mock<ConfigurationService>();
            Mock<FolderService> folderService = new Mock<FolderService>();
            Mock<ErrorService> errorService = new Mock<ErrorService>();
            Mock<FileService> fileService = new Mock<FileService>();
            ImportService importService = new ImportService(uow, configurationService.Object, folderService.Object, errorService.Object, fileService.Object);

            DataTable table = new DataTable();

            for (int i = 1; i <= 20; i++)
            {
                table.Columns.Add();
            }

            DataRow row = table.NewRow();
            row[0] = "Invoice";
            row[1] = "1234567890";
            row[2] = "01/01/2016";
            row[3] = "EUR";
            row[4] = "Fund";
            row[5] = "BAD DATA";
            row[6] = "2016";
            row[7] = "Delivery Body";
            row[8] = "1";
            row[9] = "Account";
            row[10] = "Fund";
            row[11] = "Scheme";
            row[12] = "2016";
            row[13] = "Delivery Body";
            row[14] = "BAD DATA";
            row[15] = "Irregularity";
            row[16] = "Admin";
            row[17] = "Claim Reference";
            row[18] = "BAD DATA";
            row[19] = "10";


            importService.CreateAR(row);

            var result = uow.ARRepository.FindBy(x => x.H_InvoiceAccount == 1234567890).FirstOrDefault();

            Assert.IsNotNull(result);
        }
    }
}
