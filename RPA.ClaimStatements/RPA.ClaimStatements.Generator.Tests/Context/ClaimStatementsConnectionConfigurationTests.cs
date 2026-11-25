using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using RPA.ClaimStatements.Generator.Components.PumpingStation.BulkInsert;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.DAX;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Reflection;

namespace RPA.ClaimStatements.Generator.Tests.Context
{
    [TestFixture]
    [NonParallelizable]
    public class ClaimStatementsConnectionConfigurationTests
    {
        private const string WindowsConnection = "Server=synthetic.invalid;Database=SyntheticClaims;Integrated Security=true;";
        private const string ManagedIdentityConnection = "Server=synthetic.invalid;Database=SyntheticClaims;Authentication=Active Directory Managed Identity;";
        private string originalEnvironment;

        [SetUp]
        public void SetUp()
        {
            originalEnvironment = Environment.GetEnvironmentVariable(ClaimStatementsConnectionString.EnvironmentVariableName);
        }

        [TearDown]
        public void TearDown()
        {
            Environment.SetEnvironmentVariable(ClaimStatementsConnectionString.EnvironmentVariableName, originalEnvironment);
        }

        [Test]
        public void Constructors_ReadCurrentEnvironmentForBothConsumers_WithoutOpeningConnections()
        {
            foreach (string value in new[] { WindowsConnection, ManagedIdentityConnection })
            {
                Environment.SetEnvironmentVariable(ClaimStatementsConnectionString.EnvironmentVariableName, value);
                using (var context = new ClaimStatementsContext())
                using (var bulk = new SQLBulkInsert<AP>(new Mock<IClaimStatementsContext>().Object))
                {
                    AssertConnectionSettings(context.Database.GetConnectionString(), value);
                    Assert.That(context.Database.GetCommandTimeout(), Is.EqualTo(180));
                    Assert.That(context.Database.GetDbConnection().State, Is.EqualTo(System.Data.ConnectionState.Closed));
                    // Inspect the exact string Insert passes to SqlBulkCopy; never call Insert/WriteToServer.
                    Assert.That(typeof(SQLBulkInsert<AP>).GetField("connStr", BindingFlags.Instance | BindingFlags.NonPublic)
                        .GetValue(bulk), Is.EqualTo(value));
                }
            }
        }

        [TestCase(" ")]
        [TestCase("Authentication=synthetic-secret;")]
        public void Constructors_InvalidOverride_BothFailWithoutFallbackOrSecretDisclosure(string value)
        {
            Environment.SetEnvironmentVariable(ClaimStatementsConnectionString.EnvironmentVariableName, value);
            var contextError = Assert.Throws<InvalidOperationException>(() => new ClaimStatementsContext());
            var bulkError = Assert.Throws<InvalidOperationException>(() =>
                new SQLBulkInsert<AP>(new Mock<IClaimStatementsContext>().Object));
            Assert.That(bulkError.Message, Is.EqualTo(contextError.Message));
            Assert.That(contextError.ToString(), Does.Not.Contain("synthetic-secret"));
            Assert.That(bulkError.ToString(), Does.Not.Contain("synthetic-secret"));
            Assert.That(contextError.InnerException, Is.Null);
            Assert.That(bulkError.InnerException, Is.Null);
        }

        [Test]
        public void ExplicitOptionsConstructor_IgnoresInvalidEnvironmentAndPreservesOptions()
        {
            Environment.SetEnvironmentVariable(ClaimStatementsConnectionString.EnvironmentVariableName, " ");
            var options = new DbContextOptionsBuilder<ClaimStatementsContext>()
                .UseSqlServer(WindowsConnection, sql => sql.CommandTimeout(37)).Options;
            using (var context = new ClaimStatementsContext(options))
            {
                AssertConnectionSettings(context.Database.GetConnectionString(), WindowsConnection);
                Assert.That(context.Database.GetCommandTimeout(), Is.EqualTo(37));
                Assert.That(context.Database.GetDbConnection().State, Is.EqualTo(System.Data.ConnectionState.Closed));
            }
        }

        private static void AssertConnectionSettings(string actual, string expected)
        {
            // EF Core normalizes aliases and adds provider defaults to its connection string.
            var actualSettings = new SqlConnectionStringBuilder(actual);
            var expectedSettings = new SqlConnectionStringBuilder(expected);
            Assert.That(actualSettings.DataSource, Is.EqualTo(expectedSettings.DataSource));
            Assert.That(actualSettings.InitialCatalog, Is.EqualTo(expectedSettings.InitialCatalog));
            Assert.That(actualSettings.IntegratedSecurity, Is.EqualTo(expectedSettings.IntegratedSecurity));
            Assert.That(actualSettings.Authentication, Is.EqualTo(expectedSettings.Authentication));
        }
    }
}
