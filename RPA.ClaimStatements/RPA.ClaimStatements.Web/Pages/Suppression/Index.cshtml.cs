using Microsoft.AspNetCore.Mvc;
using RPA.ClaimStatements.Generator.Context;
using X.PagedList;

namespace RPA.ClaimStatements.Web.Pages.Suppression
{
    public class IndexModel : SuppressionPageModel
    {
        public IndexModel(IClaimStatementsContext db) : base(db) { }

        public PagedList<Generator.Models.Entities.CS.Suppression> Items { get; private set; }

        public IActionResult OnPost() => OnGet();

        public override IActionResult OnGet()
        {
            Items = Existing(null, 1, 15);
            return Page();
        }
    }
}
