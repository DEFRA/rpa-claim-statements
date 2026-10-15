using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RPA.ClaimStatements.Generator;
using RPA.ClaimStatements.Web;
using RPA.ClaimStatements.Web.Filters;
using RPA.ClaimStatements.Web.Security;
using Unity.Microsoft.DependencyInjection;

// The Generator resolves its connection strings, EF configuration and SMTP settings through
// ConfigurationManager. Pin it to this application's own configuration file, whichever process hosts it.
AppContext.SetData("APP_CONFIG_FILE", Path.Combine(AppContext.BaseDirectory, "RPA.ClaimStatements.Web.dll.config"));

// FolderService.Root() uses the host's base directory when web hosted.
HostEnvironment.IsWebHosted = true;

var builder = WebApplication.CreateBuilder(args);

// Keep the existing Unity registrations (UnityConfig.RegisterTypes) as the service provider.
builder.Host.UseUnityServiceProvider(UnityConfig.Container);

builder.Services
    .AddControllersWithViews(options => options.Filters.Add<HandleErrorFilter>())
    // Clients read the JSON property names as written (PascalCase), e.g. data.Ok.
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null)
    .AddControllersAsServices();

// Windows authentication: IIS authenticates the request when hosted in-process by IIS,
// Kestrel negotiates it itself (local development and console hosting).
builder.Services
    .AddAuthentication("WindowsOrNegotiate")
    .AddPolicyScheme("WindowsOrNegotiate", "Windows authentication", options =>
        options.ForwardDefaultSelector = context =>
            context.RequestServices.GetRequiredService<IServer>().GetType().Name == "IISHttpServer"
                ? IISServerDefaults.AuthenticationScheme
                : NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();

builder.Services.AddAuthorization();

// Roles come from the Security database for Identity.Name, as the legacy roleManager provided them.
builder.Services.AddSingleton<IRoleStore>(_ =>
{
    ConnectionStringSettings security = ConfigurationManager.ConnectionStrings["IDTSecurityConnection"]
        ?? throw new InvalidOperationException("Connection string 'IDTSecurityConnection' is not configured.");
    string applicationName = ConfigurationManager.AppSettings["IDTSecurityApplicationName"]
        ?? throw new InvalidOperationException("App setting 'IDTSecurityApplicationName' is not configured.");

    return new SecurityDatabaseRoleStore(security.ConnectionString, applicationName);
});
builder.Services.AddTransient<IClaimsTransformation, RoleManagerClaimsTransformation>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// globalization culture="en-GB" uiCulture="en-GB": fixed, not negotiated from the client.
var culture = new CultureInfo("en-GB");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culture),
    SupportedCultures = new List<CultureInfo> { culture },
    SupportedUICultures = new List<CultureInfo> { culture },
    RequestCultureProviders = new List<IRequestCultureProvider>()
});

app.UseAuthentication();

// <deny users="?" />: nothing, static files included, is served to an anonymous user.
app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated != true)
    {
        await context.ChallengeAsync();
        return;
    }

    await next();
});

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "Default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
