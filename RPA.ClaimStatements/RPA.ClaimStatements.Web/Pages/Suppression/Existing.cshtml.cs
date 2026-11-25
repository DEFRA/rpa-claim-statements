using Microsoft.AspNetCore.Mvc;
using RPA.ClaimStatements.Generator.Context;

namespace RPA.ClaimStatements.Web.Pages.Suppression
{
    public class ExistingModel : SuppressionPageModel
    {
        public ExistingModel(IClaimStatementsContext db) : base(db) { }

        [BindProperty(SupportsGet = true)]
        public string searchString { get; set; }
        [BindProperty(SupportsGet = true)]
        [ModelBinder(BinderType = typeof(LegacyPageNumberBinder))]
        public int page { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int pageSize { get; set; } = 15;

        public IActionResult OnPost() => OnGet();

        public override IActionResult OnGet() =>
            RenderPartial("/Pages/Suppression/_Existing.cshtml", Existing(searchString, page, pageSize));
    }
}
