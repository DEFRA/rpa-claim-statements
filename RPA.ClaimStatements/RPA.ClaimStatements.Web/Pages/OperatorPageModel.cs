using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace RPA.ClaimStatements.Web.Pages
{
    // Unconstrained legacy actions accepted every verb. Keep that contract (including the
    // existing POST search forms); pages with POST-only actions leave OnGet returning 405.
    public abstract class OperatorPageModel : PageModel
    {
        public override void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            // Legacy action URLs never used Razor's reserved ?handler= query parameter.
            // Ignore it rather than letting it skip the operation; unrestricted actions also
            // accepted verbs without a Razor-named handler. POST-only endpoints are constrained
            // in endpoint metadata before this filter runs.
            context.HandlerMethod = context.ActionDescriptor.HandlerMethods.FirstOrDefault(handler =>
                handler.Name == null && string.Equals(handler.HttpMethod, Request.Method, StringComparison.OrdinalIgnoreCase))
                ?? context.ActionDescriptor.HandlerMethods.First(handler => handler.Name == null && handler.HttpMethod == "Get");
        }

        protected PartialViewResult RenderPartial(string viewName, object model = null) =>
            new PartialViewResult
            {
                ViewName = viewName,
                ViewData = new ViewDataDictionary<object>(ViewData) { Model = model }
            };

        public virtual IActionResult OnGet() => StatusCode(405);
        public virtual IActionResult OnPut() => OnGet();
        public virtual IActionResult OnPatch() => OnGet();
        public virtual IActionResult OnDelete() => OnGet();
        public virtual IActionResult OnHead() => OnGet();
        public virtual IActionResult OnOptions() => OnGet();
        public virtual IActionResult OnTrace() => OnGet();
    }
}
