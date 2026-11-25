using Microsoft.AspNetCore.Mvc;
using RPA.ClaimStatements.Web.Models;

namespace RPA.ClaimStatements.Web.Pages.Home
{
    [IgnoreAntiforgeryToken]
    public class IndexModel : HomePageModel
    {
        public IActionResult OnPost() => OnGet();

        public override IActionResult OnGet()
        {
            ViewData["Title"] = "Claim Statement Generator";
            ViewData["Location"] = "Home";
            ViewData["StatementTypes"] = SelectionValues.StatementTypes();
            return Page();
        }
    }
}
