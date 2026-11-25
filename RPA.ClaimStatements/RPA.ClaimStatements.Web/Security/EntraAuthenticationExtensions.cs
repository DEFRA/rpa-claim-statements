using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace RPA.ClaimStatements.Web.Security
{
    public static class EntraAuthenticationExtensions
    {
        public static IServiceCollection AddClaimStatementsEntraAuthentication(
            this IServiceCollection services, IConfigurationSection azureAd)
        {
            AzureAdConfigurationValidation.ValidateSingleTenant(azureAd["TenantId"]);
            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(azureAd);
            // Sign-in-only Microsoft.Identity.Web defaults to id_token, not confidential code flow.
            services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.UsePkce = true;
            });
            return services;
        }
    }
}
