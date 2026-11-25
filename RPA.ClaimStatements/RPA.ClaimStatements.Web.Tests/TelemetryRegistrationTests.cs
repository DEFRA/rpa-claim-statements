using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenTelemetry.Trace;
using RPA.ClaimStatements.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RPA.ClaimStatements.Web.Tests
{
    [TestFixture]
    public class TelemetryRegistrationTests
    {
        private const string Connection = "InstrumentationKey=33333333-3333-3333-3333-333333333333;IngestionEndpoint=https://unused.invalid/";

        private static IConfiguration Configuration(Dictionary<string, string> settings) =>
            new ConfigurationBuilder().AddInMemoryCollection(settings).Build();

        [Test]
        public void NoConnectionString_RegistersNoTelemetry()
        {
            var services = new ServiceCollection();

            services.AddClaimStatementsTelemetry(Configuration(new Dictionary<string, string>()));

            Assert.That(services.Any(service => service.ServiceType == typeof(TracerProvider)), Is.False);
        }

        [Test]
        public void ConnectionString_RegistersTelemetry()
        {
            var services = new ServiceCollection();

            services.AddClaimStatementsTelemetry(Configuration(new Dictionary<string, string>
            {
                ["APPLICATIONINSIGHTS_CONNECTION_STRING"] = Connection
            }));

            Assert.That(services.Any(service => service.ServiceType == typeof(TracerProvider)), Is.True);
        }

        [Test]
        public void IdentityWithoutConnectionString_FailsAtRegistration()
        {
            var services = new ServiceCollection();

            Assert.Throws<InvalidOperationException>(() => services.AddClaimStatementsTelemetry(
                Configuration(new Dictionary<string, string>
                {
                    ["Telemetry:ManagedIdentityClientId"] = "33333333-3333-3333-3333-333333333333"
                })));
        }
    }
}
