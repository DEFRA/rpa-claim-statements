using System;
using NUnit.Framework;
using RPA.ClaimStatements.Web.Security;

namespace RPA.ClaimStatements.Web.Tests.Security
{
    [TestFixture]
    [Category("Azure AD Configuration Validation")]
    public class AzureAdConfigurationValidationTests
    {
        [Test]
        public void Test_ValidateSingleTenant_Accepts_A_Tenant_Guid()
        {
            Assert.DoesNotThrow(() => AzureAdConfigurationValidation.ValidateSingleTenant("11111111-2222-3333-4444-555555555555"));
        }

        [TestCase(null)]
        [TestCase("   ")]
        public void Test_ValidateSingleTenant_Throws_When_Missing(string tenantId)
        {
            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(
                () => AzureAdConfigurationValidation.ValidateSingleTenant(tenantId));

            StringAssert.Contains("AzureAd:TenantId", ex.Message);
        }

        [TestCase("common")]
        [TestCase("organizations")]
        public void Test_ValidateSingleTenant_Throws_On_MultiTenant_Alias(string alias)
        {
            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(
                () => AzureAdConfigurationValidation.ValidateSingleTenant(alias));

            StringAssert.Contains("multi-tenant alias", ex.Message);
        }

        [Test]
        public void Test_ValidateSingleTenant_Throws_On_Empty_Guid()
        {
            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(
                () => AzureAdConfigurationValidation.ValidateSingleTenant("00000000-0000-0000-0000-000000000000"));

            StringAssert.Contains("empty GUID", ex.Message);
        }
    }
}
