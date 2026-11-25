using NUnit.Framework;
using RPA.ClaimStatements.Generator.Services;
using System;

namespace RPA.ClaimStatements.Generator.Tests.Services
{
    [TestFixture]
    [NonParallelizable]
    public class SchemaMigrationsTests
    {
        private string original;
        private string originalConnection;

        [SetUp]
        public void SetUp()
        {
            original = Environment.GetEnvironmentVariable(SchemaMigrations.ApplyVariableName);
            originalConnection = Environment.GetEnvironmentVariable(ClaimStatementsConnectionString.EnvironmentVariableName);
        }

        [TearDown]
        public void TearDown()
        {
            Environment.SetEnvironmentVariable(SchemaMigrations.ApplyVariableName, original);
            Environment.SetEnvironmentVariable(ClaimStatementsConnectionString.EnvironmentVariableName, originalConnection);
        }

        [TestCase(null)]
        [TestCase("false")]
        public void ApplyIfRequested_NotExplicitlyTrue_DoesNothing(string value)
        {
            // With no connection string a real attempt would throw, so a clean return proves nothing was attempted.
            Environment.SetEnvironmentVariable(SchemaMigrations.ApplyVariableName, value);
            Environment.SetEnvironmentVariable(ClaimStatementsConnectionString.EnvironmentVariableName, null);

            Assert.DoesNotThrow(SchemaMigrations.ApplyIfRequested);
        }
    }
}
