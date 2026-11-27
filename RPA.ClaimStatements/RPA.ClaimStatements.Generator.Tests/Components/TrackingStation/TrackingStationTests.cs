using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using RPA.ClaimStatements.Generator.Components.TrackingStation;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Services;
using RPA.ClaimStatements.Generator.Tests.TestSupport;
using ProcessMonitor = RPA.ClaimStatements.Generator.Models.Entities.CS.Monitor;
using ReportStation = RPA.ClaimStatements.Generator.Components.TrackingStation.TrackingStation;

namespace RPA.ClaimStatements.Generator.Tests.Components.TrackingStation
{
    [TestFixture]
    public class TrackingStationTests
    {
        [Test]
        [NonParallelizable]
        public void Report_SendFailure_PrintsMessageRecordsExceptionAndDoesNotThrow()
        {
            var activities = new List<Activity>();
            using var listener = new ActivityListener
            {
                ShouldListenTo = source => source.Name == ApplicationTelemetry.SourceName,
                Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData,
                ActivityStopped = activity => activities.Add(activity)
            };
            ActivitySource.AddActivityListener(listener);
            var mail = new Mock<IEmailService>();
            mail.Setup(service => service.Send(It.IsAny<string>())).Throws(new InvalidOperationException("Graph submission failed (HTTP 403)."));
            var station = new ReportStation(Context("Batch Generation BPS - 1"), mail.Object);
            var originalError = Console.Error;
            using var error = new StringWriter();
            try
            {
                Console.SetError(error);
                using (ApplicationTelemetry.Source.StartActivity("ClaimStatements.Run"))
                {
                    Assert.DoesNotThrow(() => station.Report());
                }
            }
            finally
            {
                Console.SetError(originalError);
            }
            Assert.That(error.ToString(), Does.Contain("HTTP 403"));
            Assert.That(activities.Single().Events.Select(e => e.Name), Does.Contain("exception"));
        }

        [Test]
        public void Report_OnlyLoadMonitor_DoesNotSendMail()
        {
            var mail = new Mock<IEmailService>(MockBehavior.Strict);
            new ReportStation(Context("Load Claim Statement Data - BPS"), mail.Object).Report();
            mail.VerifyNoOtherCalls();
        }

        private static IClaimStatementsContext Context(string reference)
        {
            var context = new Mock<IClaimStatementsContext>();
            var start = DateTime.Now.AddSeconds(1);
            context.Setup(db => db.Monitor).Returns(new Mock<DbSet<ProcessMonitor>>().SetupData(new List<ProcessMonitor>
            {
                new ProcessMonitor { Reference = reference, StartProcess = start, EndProcess = start.AddSeconds(1) }
            }).Object);
            context.Setup(db => db.Log).Returns(new Mock<DbSet<Log>>().SetupData(new List<Log>()).Object);
            context.Setup(db => db.Errors).Returns(new Mock<DbSet<Error>>().SetupData(new List<Error>()).Object);
            return context.Object;
        }
    }
}
