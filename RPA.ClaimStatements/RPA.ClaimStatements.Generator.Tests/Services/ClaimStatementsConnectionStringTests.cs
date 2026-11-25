using NUnit.Framework;
using RPA.ClaimStatements.Generator.Services;
using System;

namespace RPA.ClaimStatements.Generator.Tests.Services
{
    [TestFixture]
    public class ClaimStatementsConnectionStringTests
    {
        private const string WindowsConnection = "Server=synthetic.invalid;Database=SyntheticClaims;Integrated Security=true;";
        private const string ManagedIdentityConnection = "Server=synthetic.invalid;Database=SyntheticClaims;Authentication=Active Directory Managed Identity;";

        [TestCase(WindowsConnection)]
        [TestCase(ManagedIdentityConnection + "User Id=00000000-0000-0000-0000-000000000001;")]
        public void Resolve_ValidConnectionString_ReturnsUnchanged(string value)
        {
            Assert.That(ClaimStatementsConnectionString.Resolve(value), Is.EqualTo(value));
        }

        [TestCase(null)]
        [TestCase(" ")]
        public void Resolve_MissingOrBlank_ThrowsNamingTheEnvironmentVariable(string value)
        {
            var error = Assert.Throws<InvalidOperationException>(() => ClaimStatementsConnectionString.Resolve(value));
            Assert.That(error.Message, Does.Contain(ClaimStatementsConnectionString.EnvironmentVariableName));
            Assert.That(error.InnerException, Is.Null);
        }

        [TestCase("synthetic-secret")]
        [TestCase("Server=synthetic.invalid;Password=\"synthetic-secret")]
        [TestCase("Server=synthetic.invalid;Authentication=Active Directory Managed Identity;Password=synthetic-secret;")]
        public void Resolve_InvalidSqlOrAuthentication_ThrowsSanitizedError(string value)
        {
            var error = Assert.Throws<InvalidOperationException>(() => ClaimStatementsConnectionString.Resolve(value));
            Assert.That(error.Message, Is.EqualTo("Connection string 'ClaimStatementsContext' has invalid SQL connection or authentication settings."));
            Assert.That(error.ToString(), Does.Not.Contain("synthetic-secret"));
            Assert.That(error.InnerException, Is.Null);
        }
    }
}
