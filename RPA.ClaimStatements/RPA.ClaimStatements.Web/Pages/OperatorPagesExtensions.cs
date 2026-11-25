using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using RPA.ClaimStatements.Web.Filters;

namespace RPA.ClaimStatements.Web.Pages
{
    public static class OperatorPagesExtensions
    {
        public static IMvcBuilder AddOperatorPages(this IServiceCollection services)
        {
            return services.AddRazorPages(options =>
            {
                // Keep both the conventional controller root and explicit Index URLs.
                options.Conventions.AddPageRoute("/Home/Index", "");
                options.Conventions.AddPageRoute("/Home/Index", "Home");
                options.Conventions.AddPageRoute("/Configuration/Index", "Configuration");
                options.Conventions.AddPageRoute("/Suppression/Index", "Suppression");
                options.Conventions.AddPageRoute("/Error/Index", "Error");
                options.Conventions.AddPageRoute("/Log/Index", "Log");
                foreach (string page in new[] { "/Home/Trigger", "/Account/Logout", "/Error/ResolveError" })
                {
                    options.Conventions.AddPageRouteModelConvention(page, model =>
                    {
                        foreach (var selector in model.Selectors)
                        {
                            selector.EndpointMetadata.Add(new HttpMethodMetadata(new[] { "POST" }));
                        }
                    });
                }
            })
            .AddMvcOptions(options => options.Filters.Add<HandleErrorFilter>())
            .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
        }

        public static void MapOperatorPages(this IEndpointRouteBuilder endpoints) => endpoints.MapRazorPages();

        public static void UseOperatorRequestPipeline(this IApplicationBuilder app)
        {
            // Process liveness only: do not resolve services or contact identity/data dependencies.
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value == "/health/live" &&
                    (HttpMethods.IsGet(context.Request.Method) || HttpMethods.IsHead(context.Request.Method)))
                {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    context.Response.ContentType = "text/plain";
                    context.Response.Headers.CacheControl = "no-store";
                    if (HttpMethods.IsGet(context.Request.Method))
                    {
                        await context.Response.WriteAsync("Alive");
                    }
                    return;
                }
                await next();
            });
            var culture = new CultureInfo("en-GB");
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(culture),
                SupportedCultures = new List<CultureInfo> { culture },
                SupportedUICultures = new List<CultureInfo> { culture },
                RequestCultureProviders = new List<IRequestCultureProvider>()
            });
            app.UseAuthentication();
            // Preserve <deny users="?"/>, including anonymous static-file requests.
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
        }
    }
}
