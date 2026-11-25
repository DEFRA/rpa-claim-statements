using NUnit.Framework;
using RPA.ClaimStatements.Web.Security;

namespace RPA.ClaimStatements.Web.Tests.Security
{
    [TestFixture]
    [Category("App Role Permission Map")]
    public class AppRolePermissionMapTests
    {
        // Every app role value this application assigns and the legacy permission name every
        // [Authorize(Roles = "...")] attribute/User.IsInRole(...) check already expects.
        [TestCase("ClaimStatementGenerator.TriggerGeneration", "Claim Statement Generator: Trigger Generation")]
        [TestCase("ClaimStatementGenerator.ViewClaimStatements", "Claim Statement Generator: View Claim Statements")]
        [TestCase("ClaimStatementGenerator.ManageExceptions", "Claim Statement Generator: Manage Exceptions")]
        [TestCase("ClaimStatementGenerator.ManageSuppressions", "Claim Statement Generator: Manage Suppressions")]
        [TestCase("ClaimStatementGenerator.DownloadData", "Claim Statement Generator: Download Data")]
        [TestCase("ClaimStatementGenerator.ManualUpload", "Claim Statement Generator: Manual Upload")]
        [TestCase("ClaimStatementGenerator.ForceGeneration", "Claim Statement Generator: Force Generation")]
        public void Test_TryGetPermission_Maps_Every_Known_AppRole_Value(string appRoleValue, string expectedPermission)
        {
            bool found = AppRolePermissionMap.TryGetPermission(appRoleValue, out string permission);

            Assert.IsTrue(found);
            Assert.AreEqual(expectedPermission, permission);
        }

        [TestCase(null)]
        [TestCase("Claim Statement Generator: Trigger Generation")] // the legacy display name itself is not a valid app role value
        [TestCase("ClaimStatementGenerator.TriggerGenerationExtra")]
        public void Test_TryGetPermission_Denies_Unknown_Or_Absent_Values(string appRoleValue)
        {
            bool found = AppRolePermissionMap.TryGetPermission(appRoleValue, out string permission);

            Assert.IsFalse(found);
            Assert.IsNull(permission);
        }
    }
}
