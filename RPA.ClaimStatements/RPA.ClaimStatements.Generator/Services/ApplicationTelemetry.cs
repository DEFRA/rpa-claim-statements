using System.Diagnostics;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace RPA.ClaimStatements.Generator.Services
{
    public static class ApplicationTelemetry
    {
        public const string SourceName = "RPA.ClaimStatements";
        public static readonly ActivitySource Source = new ActivitySource(SourceName);

        public static TracerProvider CreateProvider(string serviceName)
        {
            var settings = TelemetrySettings.FromEnvironment();
            if (!settings.Enabled)
                return null;

            var credential = settings.CreateCredential();
            return Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(ResourceBuilder.CreateEmpty().AddService(serviceName))
                .AddSource(SourceName)
                .AddAzureMonitorTraceExporter(options =>
                {
                    options.ConnectionString = settings.ConnectionString;
                    if (credential != null)
                        options.Credential = credential;
                })
                .Build();
        }
    }
}
