using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentFTP;
using Moq;
using NUnit.Framework;
using RPA.ClaimStatements.Generator.Components.PumpingStation;

namespace RPA.ClaimStatements.Generator.Tests.Components.PumpingStation
{
    [TestFixture]
    public class FTPServiceTests
    {
        Mock<IFtpClient> client;
        TestFTPService service;
        string destination;
        List<string> operations;
        readonly byte[] content = { 0, 13, 10, 128, 255 };

        [SetUp]
        public void SetUp()
        {
            destination = Path.Combine(Path.GetTempPath(), "FtpServiceTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(destination);
            operations = new List<string>();
            client = new Mock<IFtpClient>(MockBehavior.Strict);
            client.SetupGet(c => c.Config).Returns(new FtpConfig());
            client.Setup(c => c.Connect());
            client.Setup(c => c.Dispose());
            client.Setup(c => c.GetListing("/feed", FtpListOption.Auto)).Returns(new[]
            {
                FileItem("CTL_first.csv"), FileItem("CTL_second.csv"), FileItem("ignored.csv"),
                new FtpListItem { Name = "CTL_directory.csv", Type = FtpObjectType.Directory }
            });
            client.Setup(c => c.DownloadFile(It.IsAny<string>(), It.IsAny<string>(),
                    FtpLocalExists.Overwrite, FtpVerify.None, null))
                .Returns((string local, string remote, FtpLocalExists exists, FtpVerify verify, Action<FtpProgress> progress) =>
                {
                    operations.Add("get " + remote);
                    File.WriteAllBytes(local, content);
                    return FtpStatus.Success;
                });
            client.Setup(c => c.DeleteFile(It.IsAny<string>()))
                .Callback((string remote) =>
                {
                    string local = Path.Combine(destination, remote.Substring(remote.LastIndexOf('/') + 1));
                    Assert.That(File.ReadAllBytes(local), Is.EqualTo(content), "Publish complete bytes before removing remote file.");
                    operations.Add("delete " + remote);
                });
            service = new TestFTPService(client.Object);
            service.SetOptions("synthetic.invalid", "synthetic-user", "synthetic-password");
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(destination, true);
        }

        [TestCase("/feed")]
        public void Download_CompletesAllControlsBeforeData_AndRemovesOnlyPublishedFiles(string from)
        {
            File.WriteAllText(Path.Combine(destination, "first.csv"), "old content");
            service.Download(from, destination, "*.csv", "CTL_");

            Assert.That(operations, Is.EqualTo(new[]
            {
                "get /feed/CTL_first.csv", "delete /feed/CTL_first.csv",
                "get /feed/CTL_second.csv", "delete /feed/CTL_second.csv",
                "get /feed/first.csv", "delete /feed/first.csv",
                "get /feed/second.csv", "delete /feed/second.csv"
            }));
            Assert.That(Directory.GetFiles(destination).Select(Path.GetFileName), Is.EquivalentTo(
                new[] { "CTL_first.csv", "CTL_second.csv", "first.csv", "second.csv" }));
            Assert.That(client.Object.Config.DownloadDataType, Is.EqualTo(FtpDataType.Binary));
            Assert.That(client.Object.Config.EncryptionMode, Is.EqualTo(FtpEncryptionMode.None));
            Assert.That(client.Object.Config.DataConnectionType, Is.EqualTo(FtpDataConnectionType.AutoPassive));
            client.Verify(c => c.Connect(), Times.Once);
            client.Verify(c => c.Dispose(), Times.Once);
        }

        [Test]
        public void Download_NoMatchingControls_DoesNotDownloadOrRemoveData()
        {
            service.Download("/feed/", destination, "*.txt", "CTL_");
            Assert.That(operations, Is.Empty);
            Assert.That(Directory.GetFiles(destination), Is.Empty);
        }

        [Test]
        public void Download_MaskIsCaseInsensitive_AndQuestionMarkMatchesExactlyOneCharacter()
        {
            client.Setup(c => c.GetListing("/feed", FtpListOption.Auto)).Returns(new[]
            {
                FileItem("CTL_A.CSV"), FileItem("CTL_AB.csv"), FileItem("CTL_.csv")
            });
            service.Download("/feed/", destination, "?.csv", "CTL_");
            Assert.That(operations, Is.EqualTo(new[]
            {
                "get /feed/CTL_A.CSV", "delete /feed/CTL_A.CSV", "get /feed/A.CSV", "delete /feed/A.CSV"
            }));
        }

        [Test]
        public void Download_DataPathPreservesLegacyCaseSensitiveReplaceAllOccurrences()
        {
            client.Setup(c => c.GetListing("/feed", FtpListOption.Auto)).Returns(new[] { FileItem("CTL_CTL_A.csv") });
            service.Download("/feed", destination, "*.csv", "CTL_");
            Assert.That(operations.Last(), Is.EqualTo("delete /feed/A.csv"));
        }

        [TestCase(FtpStatus.Failed)]
        [TestCase(FtpStatus.Skipped)]
        public void Download_UnsuccessfulControl_IsExplicitAndRetainsRemoteAndExistingLocalFile(FtpStatus status)
        {
            File.WriteAllText(Path.Combine(destination, "CTL_first.csv"), "previous complete control");
            client.Setup(c => c.DownloadFile(It.IsAny<string>(), "/feed/CTL_first.csv",
                    FtpLocalExists.Overwrite, FtpVerify.None, null))
                .Returns((string local, string remote, FtpLocalExists exists, FtpVerify verify, Action<FtpProgress> progress) =>
                {
                    File.WriteAllText(local, "partial");
                    return status;
                });
            Assert.Throws<IOException>(() => service.Download("/feed", destination, "*.csv", "CTL_"));
            Assert.That(operations, Is.Empty);
            Assert.That(File.ReadAllText(Path.Combine(destination, "CTL_first.csv")), Is.EqualTo("previous complete control"));
            Assert.That(Directory.GetFiles(destination).Length, Is.EqualTo(1));
        }

        [Test]
        public void Download_ControlDeletionFailure_StopsBeforeRemainingControlsOrData()
        {
            var failure = new IOException("Synthetic delete failure");
            client.Setup(c => c.DeleteFile("/feed/CTL_first.csv")).Throws(failure);
            Assert.That(Assert.Throws<IOException>(() => service.Download("/feed", destination, "*.csv", "CTL_")), Is.SameAs(failure));
            Assert.That(operations, Is.EqualTo(new[] { "get /feed/CTL_first.csv" }));
            Assert.That(File.ReadAllBytes(Path.Combine(destination, "CTL_first.csv")), Is.EqualTo(content));
        }

        [Test]
        public void Download_DataFailure_RetainsStagedControlsAndStopsBeforeRemainingData()
        {
            client.Setup(c => c.DownloadFile(It.IsAny<string>(), "/feed/first.csv",
                FtpLocalExists.Overwrite, FtpVerify.None, null)).Returns(FtpStatus.Failed);
            Assert.Throws<IOException>(() => service.Download("/feed", destination, "*.csv", "CTL_"));
            Assert.That(operations, Is.EqualTo(new[]
            {
                "get /feed/CTL_first.csv", "delete /feed/CTL_first.csv",
                "get /feed/CTL_second.csv", "delete /feed/CTL_second.csv"
            }));
            Assert.That(File.Exists(Path.Combine(destination, "first.csv")), Is.False);
            Assert.That(Directory.GetFiles(destination).Length, Is.EqualTo(2));
        }

        [Test]
        public void Download_DataDeletionFailure_PreservesCompleteLocalDataAndDoesNotContinue()
        {
            client.Setup(c => c.DeleteFile("/feed/first.csv")).Throws(new IOException("Synthetic data delete failure"));
            Assert.Throws<IOException>(() => service.Download("/feed", destination, "*.csv", "CTL_"));
            Assert.That(operations.Last(), Is.EqualTo("get /feed/first.csv"));
            Assert.That(File.ReadAllBytes(Path.Combine(destination, "first.csv")), Is.EqualTo(content));
            Assert.That(File.Exists(Path.Combine(destination, "second.csv")), Is.False);
        }

        [Test]
        public void Download_ConnectionOrListingFailure_PropagatesAndDisposesClient()
        {
            client.Setup(c => c.GetListing("/feed", FtpListOption.Auto)).Throws(new IOException("Synthetic listing failure"));
            Assert.Throws<IOException>(() => service.Download("/feed", destination, "*.csv", "CTL_"));
            Assert.That(operations, Is.Empty);
            client.Verify(c => c.Dispose(), Times.Once);
        }

        [TestCase("CTL_../bad.csv")]
        [TestCase("CTL_bad\\file.csv")]
        public void Download_UnsafeListedName_FailsBeforeAnyDestructiveTransfer(string name)
        {
            client.Setup(c => c.GetListing("/feed", FtpListOption.Auto)).Returns(new[] { FileItem(name) });
            Assert.Throws<IOException>(() => service.Download("/feed", destination, "*", "CTL_"));
        }

        private static FtpListItem FileItem(string name)
        {
            return new FtpListItem { Name = name, Type = FtpObjectType.File };
        }

        private sealed class TestFTPService : FTPService
        {
            readonly IFtpClient client;

            public TestFTPService(IFtpClient client)
            {
                this.client = client;
            }

            protected override IFtpClient CreateClient()
            {
                return client;
            }
        }
    }
}
