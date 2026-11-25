using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;

namespace RPA.ClaimStatements.Web.Pages.Configuration
{
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = "Claim Statement Generator: Trigger Generation")]
    public class SchemeYearModel : OperatorPageModel
    {
        private readonly IClaimStatementsContext db;

        public SchemeYearModel(IClaimStatementsContext db) => this.db = db;

        public List<SchemeYear> Items { get; set; }

        public override IActionResult OnGet()
        {
            Items = db.SchemeYears.AsNoTracking().OrderBy(x => x.SchemeYearNumber).ToList();
            return Page();
        }

        public IActionResult OnPost(List<SchemeYear> model)
        {
            foreach (var schemeYear in model)
            {
                db.SetModified(schemeYear);
            }
            db.SaveChanges();
            TempData["Message"] = "Configuration saved";
            return RedirectToPage("/Configuration/SchemeYear");
        }
    }
}
