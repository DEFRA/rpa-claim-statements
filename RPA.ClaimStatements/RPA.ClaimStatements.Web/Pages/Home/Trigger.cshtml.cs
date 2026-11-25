using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RPA.ClaimStatements.Generator.Components;

namespace RPA.ClaimStatements.Web.Pages.Home
{
    [Authorize(Roles = "Claim Statement Generator: Trigger Generation")]
    public class TriggerModel : HomePageModel
    {
        private readonly IPowerPlant powerPlant;

        public TriggerModel(IPowerPlant powerPlant) => this.powerPlant = powerPlant;

        public IActionResult OnPost(string StatementTypes = "ALL", int maximumBatchSize = 10)
        {
            if (maximumBatchSize > 0)
            {
                powerPlant.Activate(StatementTypes, maximumBatchSize);
            }

            TempData["Message"] = "Generation complete";
            return RedirectToPage("/Home/Index");
        }
    }
}
