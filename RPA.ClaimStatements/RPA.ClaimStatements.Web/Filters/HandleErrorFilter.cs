using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

namespace RPA.ClaimStatements.Web.Filters
{
    /// <summary>
    /// Replaces the global HandleErrorAttribute together with customErrors mode="RemoteOnly":
    /// unhandled action exceptions from remote requests render Views/Shared/Error; local
    /// requests are left unhandled so the host reports the detail.
    /// </summary>
    public sealed class HandleErrorFilter : IExceptionFilter
    {
        private readonly IModelMetadataProvider metadataProvider;
        private readonly ILogger<HandleErrorFilter> logger;

        public HandleErrorFilter(IModelMetadataProvider metadataProvider, ILogger<HandleErrorFilter> logger)
        {
            this.metadataProvider = metadataProvider;
            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled || IsLocal(context.HttpContext))
            {
                return;
            }

            logger.LogError(context.Exception, "Unhandled exception in {Action}", context.ActionDescriptor.DisplayName);

            context.Result = new ViewResult
            {
                ViewName = "Error",
                StatusCode = StatusCodes.Status500InternalServerError,
                ViewData = new ViewDataDictionary(metadataProvider, context.ModelState)
            };
            context.ExceptionHandled = true;
        }

        private static bool IsLocal(HttpContext context)
        {
            IPAddress remote = context.Connection.RemoteIpAddress;

            return remote == null || IPAddress.IsLoopback(remote) || remote.Equals(context.Connection.LocalIpAddress);
        }
    }
}
