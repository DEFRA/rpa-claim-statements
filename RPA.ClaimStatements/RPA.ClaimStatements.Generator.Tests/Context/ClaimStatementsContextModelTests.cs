using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Tests.TestSupport;

namespace RPA.ClaimStatements.Generator.Tests.Context
{
    // The EF Core migrations are the source of truth for the schema. These tests never open a connection.
    [TestFixture]
    [Category("EF Core Model")]
    public class ClaimStatementsContextModelTests
    {
        private ClaimStatementsContext _ctx;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _ctx = new ClaimStatementsContext(ClaimStatementsContextTestFactory.CreateSyntheticOptions());
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _ctx?.Dispose();
        }

        [Test]
        public void Model_Has_No_Changes_Missing_From_The_Migrations()
        {
            Assert.That(_ctx.Database.HasPendingModelChanges(), Is.False,
                "The model changed without a migration. Run: dotnet ef migrations add <Name> " +
                "--project RPA.ClaimStatements.Generator --output-dir Context\\Migrations");
        }

        [Test]
        public void Migrations_Are_Discovered_In_The_Generator_Assembly()
        {
            Assert.That(_ctx.Database.GetMigrations(), Is.Not.Empty);
        }

        // Unity must always pick the parameterless constructor; it cannot construct the DbContextOptions overload.
        // The attribute is matched by name so the test project needs no direct Unity reference.
        [Test]
        public void ClaimStatementsContext_Unity_Selects_The_Parameterless_Constructor()
        {
            var ctors = typeof(ClaimStatementsContext).GetConstructors();
            bool IsInjection(ConstructorInfo c) =>
                c.GetCustomAttributes().Any(a => a.GetType().FullName == "Unity.InjectionConstructorAttribute");

            Assert.That(IsInjection(ctors.Single(c => c.GetParameters().Length == 0)), Is.True);
            Assert.That(ctors.Where(c => c.GetParameters().Length > 0).Any(IsInjection), Is.False);
        }
    }
}
