using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RPA.ClaimStatements.Generator.Services;
using RPA.ClaimStatements.Web;
using RPA.ClaimStatements.Web.Extensions;
using RPA.ClaimStatements.Web.Security;
using RPA.ClaimStatements.Web.Pages;
using Unity.Microsoft.DependencyInjection;

// Fail fast at startup rather than on the first database or file access.
StartupChecks.EnsureReady();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddClaimStatementsTelemetry(builder.Configuration);

builder.Services.AddClaimStatementsDataProtection(builder.Configuration);

// The Generator's services are registered in the existing Unity container, which acts as the service provider.
builder.Host.UseUnityServiceProvider(UnityConfig.Container);

builder.Services.AddOperatorPages();

// Entra ID sign-in (single tenant, authorization-code flow with PKCE, cookie session). The tenant,
// client and credential values come from environment variables, with secrets injected from Key Vault.
builder.Services.AddClaimStatementsEntraAuthentication(builder.Configuration.GetSection("AzureAd"));

builder.Services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    // A signed-in user without the required role gets a plain 403. The default would redirect to an
    // access-denied page that does not exist in this application.
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});

builder.Services.AddAuthorization();

// Maps Entra ID app-role claims to the permission names used by the [Authorize(Roles = "...")]
// attributes and User.IsInRole(...) checks (see Security/AppRolePermissionMap.cs).
builder.Services.AddTransient<IClaimsTransformation, AppRolePermissionsClaimsTransformation>();

var app = builder.Build();

app.UseOperatorRequestPipeline();

app.MapOperatorPages();

app.Run();
