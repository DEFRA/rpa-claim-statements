using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RPA.ClaimStatements.Generator.Context;

namespace RPA.ClaimStatements.Web.Pages.Suppression
{
    [Authorize(Roles = "Claim Statement Generator: Manage Suppressions")]
    public class RemoveSuppressionModel : SuppressionPageModel
    {
        public RemoveSuppressionModel(IClaimStatementsContext db) : base(db) { }

        [BindProperty(SupportsGet = true)]
        public Guid id { get; set; }

        public IActionResult OnPost() => OnGet();

        public override IActionResult OnGet()
        {
            var suppression = db.Suppressions.Find(id);
            if (suppression != null)
            {
                suppression.End();
                db.SetModified(suppression);
                db.SaveChanges();
            }
            return Content("");
        }
    }
}
