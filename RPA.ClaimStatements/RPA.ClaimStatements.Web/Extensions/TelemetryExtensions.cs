using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using RPA.ClaimStatements.Generator.Services;

namespace RPA.ClaimStatements.Web.Extensions
{
    public static class TelemetryExtensions
    {
        // Standard Azure Monitor OpenTelemetry distro with its default collection (requests, dependencies,
        // exceptions, logs, live metrics). Registered only when a connection string is configured. The
        // Generator's own "ClaimStatements.Run" spans are added as a source so they share the request trace.
        public static IServiceCollection AddClaimStatementsTelemetry(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = TelemetrySettings.Resolve(key => configuration[key]);
            if (!settings.Enabled)
            {
                return services;
            }

            var credential = settings.CreateCredential();
            // Liveness probes are frequent and carry no information.
            services.Configure<AspNetCoreTraceInstrumentationOptions>(options =>
                options.Filter = context => context.Request.Path != "/health/live");
            services.AddOpenTelemetry()
                .UseAzureMonitor(options =>
                {
                    options.ConnectionString = settings.ConnectionString;
                    if (credential != null)
                        options.Credential = credential;
                })
                .ConfigureResource(resource => resource.AddService("RPA.ClaimStatements.Web"))
                .WithTracing(tracing => tracing.AddSource(ApplicationTelemetry.SourceName));

            return services;
        }
    }
}
