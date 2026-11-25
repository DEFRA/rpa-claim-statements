using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using NUnit.Framework;
using RPA.ClaimStatements.Web.Security;

namespace RPA.ClaimStatements.Web.Tests.Security
{
    [TestFixture]
    [Category("App Role Permissions Claims Transformation")]
    public class AppRolePermissionsClaimsTransformationTests
    {
        private const string RolesClaimType = AppRolePermissionsClaimsTransformation.EntraAppRoleClaimType;

        private static ClaimsPrincipal AuthenticatedPrincipal(params Claim[] extraClaims)
        {
            var identity = new ClaimsIdentity(extraClaims, authenticationType: "TestAuth", nameType: ClaimTypes.Name, roleType: ClaimTypes.Role);
            return new ClaimsPrincipal(identity);
        }

        [Test]
        public async Task Test_TransformAsync_Returns_Unauthenticated_Principal_Unchanged()
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity()); // no authenticationType => IsAuthenticated == false
            var transformation = new AppRolePermissionsClaimsTransformation();

            ClaimsPrincipal result = await transformation.TransformAsync(anonymous);

            Assert.AreSame(anonymous, result);
            Assert.IsFalse(result.Claims.Any());
        }

        [Test]
        public async Task Test_TransformAsync_Grants_All_Active_Permissions_For_A_Multi_Role_Identity()
        {
            // Mirrors HomeController's class-level Authorize list (menu-visible combination).
            var principal = AuthenticatedPrincipal(
                new Claim(RolesClaimType, "ClaimStatementGenerator.TriggerGeneration"),
                new Claim(RolesClaimType, "ClaimStatementGenerator.ManageExceptions"),
                new Claim(RolesClaimType, "ClaimStatementGenerator.ManageSuppressions"),
                new Claim(RolesClaimType, "ClaimStatementGenerator.ViewClaimStatements"),
                new Claim(RolesClaimType, "ClaimStatementGenerator.ManualUpload"));
            var transformation = new AppRolePermissionsClaimsTransformation();

            ClaimsPrincipal result = await transformation.TransformAsync(principal);

            Assert.IsTrue(result.IsInRole("Claim Statement Generator: Trigger Generation"));
            Assert.IsTrue(result.IsInRole("Claim Statement Generator: Manage Exceptions"));
            Assert.IsTrue(result.IsInRole("Claim Statement Generator: Manage Suppressions"));
            Assert.IsTrue(result.IsInRole("Claim Statement Generator: View Claim Statements"));
            Assert.IsTrue(result.IsInRole("Claim Statement Generator: Manual Upload"));
            Assert.IsFalse(result.IsInRole("Claim Statement Generator: Download Data"));
            Assert.IsFalse(result.IsInRole("Claim Statement Generator: Force Generation"));
        }

        [Test]
        public async Task Test_TransformAsync_Authenticated_With_No_Role_Claim_Grants_Nothing()
        {
            // Authenticated but no app role assigned: every [Authorize(Roles = "...")] check must
            // still deny (this is the "authenticated no-role user" case from the batch scope).
            var principal = AuthenticatedPrincipal(new Claim(ClaimTypes.Name, "someone@example.com"));
            var transformation = new AppRolePermissionsClaimsTransformation();

            ClaimsPrincipal result = await transformation.TransformAsync(principal);

            Assert.IsFalse(result.Claims.Any(c => c.Type == ClaimTypes.Role));
        }

        [TestCase("SomeOtherApp.UnrelatedRole")]
        [TestCase("Claim Statement Generator: Trigger Generation")] // raw legacy display name is not a valid app role value
        public async Task Test_TransformAsync_Ignores_Unknown_Role_Claim_Values(string unknownAppRoleValue)
        {
            var principal = AuthenticatedPrincipal(new Claim(RolesClaimType, unknownAppRoleValue));
            var transformation = new AppRolePermissionsClaimsTransformation();

            ClaimsPrincipal result = await transformation.TransformAsync(principal);

            Assert.IsFalse(result.Claims.Any(c => c.Type == ClaimTypes.Role));
        }

        [Test]
        public void Test_TransformAsync_Does_Not_Trust_Group_Claims()
        {
            // "groups" (or a group overage indicator) must never, by itself, grant a permission.
            var principal = AuthenticatedPrincipal(new Claim("groups", "11111111-1111-1111-1111-111111111111"));
            var transformation = new AppRolePermissionsClaimsTransformation();

            ClaimsPrincipal result = transformation.TransformAsync(principal).Result;

            Assert.IsFalse(result.Claims.Any(c => c.Type == ClaimTypes.Role));
        }

        [Test]
        public void Test_TransformAsync_Does_Not_Map_Name_Or_Email_To_A_Permission()
        {
            var principal = AuthenticatedPrincipal(
                new Claim(ClaimTypes.Name, "Claim Statement Generator: Trigger Generation"),
                new Claim(ClaimTypes.Email, "trigger.generation@example.com"));
            var transformation = new AppRolePermissionsClaimsTransformation();

            ClaimsPrincipal result = transformation.TransformAsync(principal).Result;

            Assert.IsFalse(result.IsInRole("Claim Statement Generator: Trigger Generation"));
        }

        [Test]
        public async Task Test_TransformAsync_Is_Idempotent_No_Duplicate_Claims_When_Invoked_Twice()
        {
            var principal = AuthenticatedPrincipal(new Claim(RolesClaimType, "ClaimStatementGenerator.TriggerGeneration"));
            var transformation = new AppRolePermissionsClaimsTransformation();

            ClaimsPrincipal onceTransformed = await transformation.TransformAsync(principal);
            ClaimsPrincipal twiceTransformed = await transformation.TransformAsync(onceTransformed);

            int matchingClaims = twiceTransformed.Claims
                .Count(c => c.Type == ClaimTypes.Role && c.Value == "Claim Statement Generator: Trigger Generation");
            Assert.AreEqual(1, matchingClaims);
        }
    }
}
