using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using RPA.ClaimStatements.Generator.Components;
using RPA.ClaimStatements.Generator.Components.PumpingStation;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Services;
using RPA.ClaimStatements.Web.Pages;
using RPA.ClaimStatements.Web.Security;
using RPA.ClaimStatements.Web.Services;
using Unity;
using Unity.Microsoft.DependencyInjection;

namespace RPA.ClaimStatements.Web.Tests.Pages
{
    internal sealed class OperatorPageHost : IDisposable
    {
        internal const string Trigger = "ClaimStatementGenerator.TriggerGeneration";
        internal const string View = "ClaimStatementGenerator.ViewClaimStatements";
        internal const string Manage = "ClaimStatementGenerator.ManageSuppressions";
        internal const string Exceptions = "ClaimStatementGenerator.ManageExceptions";
        internal const string Download = "ClaimStatementGenerator.DownloadData";
        internal const string Manual = "ClaimStatementGenerator.ManualUpload";
        internal const string Force = "ClaimStatementGenerator.ForceGeneration";
        internal const string All = Trigger + "|" + View + "|" + Manage + "|" + Exceptions + "|" + Download + "|" + Manual + "|" + Force;

        internal readonly Mock<IClaimStatementsContext> Context = new Mock<IClaimStatementsContext>();
        internal readonly Mock<IPowerPlant> Plant = new Mock<IPowerPlant>();
        internal readonly Mock<IPumpingStation> Station = new Mock<IPumpingStation>();
        internal readonly Mock<ISuppressionService> Bulk = new Mock<ISuppressionService>();
        internal readonly Mock<ISampleService> Sample = new Mock<ISampleService>();
        internal readonly Mock<ISampleXBService> SampleXB = new Mock<ISampleXBService>();
        internal readonly Mock<IConfigurationService> Configuration = new Mock<IConfigurationService>();
        internal readonly List<Suppression> Suppressions = new List<Suppression>();
        internal readonly List<Suppression> Added = new List<Suppression>();
        internal readonly List<Log> Logs = new List<Log>();
        internal readonly List<Error> Errors = new List<Error>();
        internal readonly List<Configuration> Settings = new List<Configuration>();
        internal readonly List<SchemeYear> Years = new List<SchemeYear>();
        internal readonly TestServer Server;
        internal readonly HttpClient Client;
        private readonly IHost testHost;
        private readonly string samplePath;

        internal OperatorPageHost()
        {
            var suppressions = Set(Suppressions);
            suppressions.Setup(s => s.Add(It.IsAny<Suppression>())).Callback<Suppression>(Added.Add);
            suppressions.Setup(s => s.Find(It.IsAny<object[]>())).Returns<object[]>(keys =>
                Suppressions.FirstOrDefault(s => s.SuppressionID.Equals(keys[0])));
            var errors = Set(Errors);
            errors.Setup(s => s.Find(It.IsAny<object[]>())).Returns<object[]>(keys =>
                Errors.FirstOrDefault(e => e.ErrorID.Equals(keys[0])));
            Context.Setup(c => c.Suppressions).Returns(suppressions.Object);
            Context.Setup(c => c.Log).Returns(Set(Logs).Object);
            Context.Setup(c => c.Errors).Returns(errors.Object);
            Context.Setup(c => c.Configurations).Returns(Set(Settings).Object);
            Context.Setup(c => c.SchemeYears).Returns(Set(Years).Object);
            samplePath = Path.Combine(NUnit.Framework.TestContext.CurrentContext.WorkDirectory,
                "PageSample_" + Guid.NewGuid().ToString("N") + ".docx");
            Sample.Setup(s => s.Sample()).Returns(() => MakeSample("BPS"));
            SampleXB.Setup(s => s.Sample()).Returns(() => MakeSample("XB"));

            var container = new UnityContainer();
            Web.UnityConfig.RegisterTypes(container);
            var builder = new HostBuilder().UseUnityServiceProvider(container).ConfigureWebHost(web => web
                .UseTestServer()
                .UseContentRoot(AppContext.BaseDirectory)
                .UseWebRoot(Path.Combine(AppContext.BaseDirectory, "wwwroot"))
                .UseEnvironment("Production")
                .UseSetting(WebHostDefaults.ApplicationKey, typeof(OperatorPagesExtensions).Assembly.GetName().Name)
                .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Error))
                .ConfigureServices(services =>
                {
                    services.AddSingleton(Context.Object);
                    services.AddSingleton(Plant.Object);
                    services.AddSingleton(Station.Object);
                    services.AddSingleton(Bulk.Object);
                    services.AddSingleton(Sample.Object);
                    services.AddSingleton(SampleXB.Object);
                    services.AddSingleton(Configuration.Object);
                    services.AddDataProtection().UseEphemeralDataProtectionProvider();
                    services.AddOperatorPages().AddApplicationPart(typeof(OperatorPagesExtensions).Assembly);
                    services.AddAuthentication("Synthetic")
                        .AddScheme<AuthenticationSchemeOptions, SyntheticAuthenticationHandler>("Synthetic", _ => { })
                        .AddScheme<AuthenticationSchemeOptions, SyntheticAuthenticationHandler>(CookieAuthenticationDefaults.AuthenticationScheme, _ => { })
                        .AddScheme<AuthenticationSchemeOptions, SyntheticAuthenticationHandler>(OpenIdConnectDefaults.AuthenticationScheme, _ => { });
                    services.AddTransient<IClaimsTransformation, AppRolePermissionsClaimsTransformation>();
                    services.AddAuthorization();
                })
                .Configure(app =>
                {
                    app.Use(async (http, next) =>
                    {
                        http.Connection.LocalIpAddress = IPAddress.Parse("192.0.2.1");
                        http.Connection.RemoteIpAddress = http.Request.Headers.ContainsKey("X-Test-Remote")
                            ? IPAddress.Parse("192.0.2.2") : IPAddress.Loopback;
                        await next();
                    });
                    app.UseOperatorRequestPipeline();
                    app.UseEndpoints(endpoints => endpoints.MapOperatorPages());
                }));
            testHost = builder.Start();
            Server = testHost.GetTestServer();
            Client = Server.CreateClient();
            Client.DefaultRequestHeaders.Add("X-Test-Roles", All);
        }

        private string MakeSample(string text)
        {
            File.WriteAllBytes(samplePath, System.Text.Encoding.UTF8.GetBytes("synthetic-" + text));
            return samplePath;
        }

        private static Mock<DbSet<T>> Set<T>(List<T> rows) where T : class
        {
            var set = new Mock<DbSet<T>>();
            var query = rows.AsQueryable();
            set.As<IQueryable<T>>().Setup(s => s.Provider).Returns(query.Provider);
            set.As<IQueryable<T>>().Setup(s => s.Expression).Returns(query.Expression);
            set.As<IQueryable<T>>().Setup(s => s.ElementType).Returns(query.ElementType);
            set.As<IQueryable<T>>().Setup(s => s.GetEnumerator()).Returns(() => query.GetEnumerator());
            return set;
        }

        internal void Roles(string roles)
        {
            Client.DefaultRequestHeaders.Remove("X-Test-Roles");
            if (roles != null) Client.DefaultRequestHeaders.Add("X-Test-Roles", roles);
        }

        public void Dispose()
        {
            Client.Dispose();
            testHost.Dispose();
            if (File.Exists(samplePath)) File.Delete(samplePath);
        }
    }

    // Test assembly only. Production continues to validate Entra tokens and cookie sessions.
    internal sealed class SyntheticAuthenticationHandler :
        AuthenticationHandler<AuthenticationSchemeOptions>, IAuthenticationSignOutHandler
    {
        public SyntheticAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("X-Test-Roles", out var roles))
                return Task.FromResult(AuthenticateResult.NoResult());
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "synthetic-operator") };
            claims.AddRange(roles.ToString().Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(role => new Claim("roles", role)));
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(
                new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name)), Scheme.Name)));
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 401;
            return Task.CompletedTask;
        }

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 403;
            return Task.CompletedTask;
        }

        public Task SignOutAsync(AuthenticationProperties properties)
        {
            Response.Headers.Append("X-Test-Signed-Out", Scheme.Name);
            Response.StatusCode = 302;
            Response.Headers.Location = properties.RedirectUri;
            return Task.CompletedTask;
        }
    }
}
