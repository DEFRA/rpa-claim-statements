using Moq;
using NUnit.Framework;
using RPA.ClaimStatements.Generator.Components;
using RPA.ClaimStatements.Generator.Components.FuelTank;
using RPA.ClaimStatements.Generator.Components.PumpingStation;
using RPA.ClaimStatements.Generator.Components.TrackingStation;
using RPA.ClaimStatements.Generator.Components.Turbine;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Services;
using System;

namespace RPA.ClaimStatements.Generator.Tests.Components
{
    [TestFixture]
    public class PowerPlantSchemaTests
    {
        [Test]
        public void Activate_PendingMigrations_FailsBeforeAnyWorkAndNamesThem()
        {
            var db = new Mock<IClaimStatementsContext>();
            db.Setup(context => context.CanConnect()).Returns(true);
            db.Setup(context => context.GetPendingMigrations()).Returns(new[] { "20261119134049_InitialCreate" });
            var imports = new Mock<IPumpingStation>();
            var plant = new PowerPlant(db.Object, imports.Object, Mock.Of<IFuelTank>(), Mock.Of<ITurbine>(),
                Mock.Of<ITrackingStation>(), Mock.Of<IConfigurationService>(), Mock.Of<IFolderService>());

            var error = Assert.Throws<InvalidOperationException>(() => plant.Activate("I"));

            Assert.That(error.Message, Does.Contain("20261119134049_InitialCreate"));
            imports.Verify(service => service.Activate(), Times.Never());
        }
    }
}
