using System;
using System.Collections.Generic;
using System.Diagnostics;
using Moq;
using NUnit.Framework;
using RPA.ClaimStatements.Generator.Components;
using RPA.ClaimStatements.Generator.Components.FuelTank;
using RPA.ClaimStatements.Generator.Components.PumpingStation;
using RPA.ClaimStatements.Generator.Components.TrackingStation;
using RPA.ClaimStatements.Generator.Components.Turbine;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Services;

namespace RPA.ClaimStatements.Generator.Tests
{
    [TestFixture]
    public class TelemetryTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public void Run_RecordsOutcomeWithoutExceptionMessageAndPreservesFailure(bool canConnect)
        {
            var completed = new List<Activity>();
            using var listener = new ActivityListener
            {
                ShouldListenTo = source => source.Name == ApplicationTelemetry.SourceName,
                Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData,
                ActivityStopped = activity => completed.Add(activity)
            };
            ActivitySource.AddActivityListener(listener);
            var db = new Mock<IClaimStatementsContext>();
            db.Setup(context => context.CanConnect()).Returns(canConnect);
            var imports = new Mock<IPumpingStation>();
            var tracking = new Mock<ITrackingStation>();
            var plant = new PowerPlant(db.Object, imports.Object, Mock.Of<IFuelTank>(),
                Mock.Of<ITurbine>(), tracking.Object, Mock.Of<IConfigurationService>(), Mock.Of<IFolderService>());

            if (canConnect)
                plant.Activate("I");
            else
                Assert.Throws<InvalidOperationException>(() => plant.Activate("I"));

            Assert.That(completed, Has.Count.EqualTo(1));
            Assert.That(completed[0].Status,
                Is.EqualTo(canConnect ? ActivityStatusCode.Ok : ActivityStatusCode.Error));
            Assert.That(completed[0].GetTagItem("statement.type"), Is.EqualTo("I"));
            Assert.That(completed[0].GetTagItem("error.type"),
                Is.EqualTo(canConnect ? null : typeof(InvalidOperationException).FullName));
            Assert.That(completed[0].GetTagItem("exception.message"), Is.Null);
            Assert.That(completed[0].StatusDescription, Is.Null.Or.Empty);
            imports.Verify(service => service.Activate(), canConnect ? Times.Once() : Times.Never());
            tracking.Verify(service => service.Report(), canConnect ? Times.Once() : Times.Never());
        }
    }
}
