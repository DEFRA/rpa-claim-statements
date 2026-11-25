using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;

namespace RPA.ClaimStatements.Web.Pages
{
    // Razor reserves the route value "page" for the page path. The old paging parameter
    // must continue to read the operator's form/query value, not that framework path.
    public sealed class LegacyPageNumberBinder : IModelBinder
    {
        private readonly SimpleTypeModelBinder binder;

        public LegacyPageNumberBinder(ILoggerFactory loggerFactory) =>
            binder = new SimpleTypeModelBinder(typeof(int), loggerFactory);

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var original = bindingContext.ValueProvider;
            if (original is CompositeValueProvider composite)
            {
                bindingContext.ValueProvider = new CompositeValueProvider(
                    composite.Where(provider => provider is not RouteValueProvider).ToList());
            }
            try
            {
                await binder.BindModelAsync(bindingContext);
            }
            finally
            {
                bindingContext.ValueProvider = original;
            }
        }
    }
}
