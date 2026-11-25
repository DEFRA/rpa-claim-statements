using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RPA.ClaimStatements.Generator.Components.PumpingStation;

namespace RPA.ClaimStatements.Web.Pages.Import
{
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = "Claim Statement Generator: Trigger Generation")]
    public class ImportModel : OperatorPageModel
    {
        private readonly IPumpingStation pumpingStation;

        public ImportModel(IPumpingStation pumpingStation) => this.pumpingStation = pumpingStation;

        public IActionResult OnPost() => OnGet();

        public override IActionResult OnGet()
        {
            pumpingStation.Activate();
            TempData["Message"] = "Import run complete";
            return RedirectToPage("/Home/Index");
        }
    }
}
