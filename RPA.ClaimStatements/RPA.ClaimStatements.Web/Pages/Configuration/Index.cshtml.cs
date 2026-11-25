using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Web.Models;

namespace RPA.ClaimStatements.Web.Pages.Configuration
{
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = "Claim Statement Generator: Trigger Generation")]
    public class IndexModel : OperatorPageModel
    {
        private readonly IClaimStatementsContext db;

        public IndexModel(IClaimStatementsContext db) => this.db = db;

        public List<Generator.Models.Entities.CS.Configuration> Items { get; set; }

        public override IActionResult OnGet()
        {
            Items = db.Configurations.AsNoTracking().OrderBy(x => x.Setting).ToList();
            ViewData["ListValues"] = SelectionValues.ListValues();
            return Page();
        }

        public IActionResult OnPost(List<Generator.Models.Entities.CS.Configuration> model)
        {
            foreach (var configuration in model)
            {
                db.SetModified(configuration);
            }
            db.SaveChanges();
            TempData["Message"] = "Configuration saved";
            return RedirectToPage("/Configuration/Index");
        }
    }
}
