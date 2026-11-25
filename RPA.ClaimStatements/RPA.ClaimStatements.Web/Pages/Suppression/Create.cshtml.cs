using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RPA.ClaimStatements.Generator.Context;

namespace RPA.ClaimStatements.Web.Pages.Suppression
{
    [Authorize(Roles = "Claim Statement Generator: Manage Suppressions")]
    public class CreateModel : SuppressionPageModel
    {
        public CreateModel(IClaimStatementsContext db) : base(db) { }
        public IActionResult OnPost() => OnGet();
        public override IActionResult OnGet() => RenderPartial("/Pages/Suppression/_Create.cshtml");
    }
}
