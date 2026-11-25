using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPA.ClaimStatements.Generator.Context;

namespace RPA.ClaimStatements.Web.Pages.Suppression
{
    [Authorize(Roles = "Claim Statement Generator: Manage Suppressions")]
    public class CreateSuppressionModel : SuppressionPageModel
    {
        public CreateSuppressionModel(IClaimStatementsContext db) : base(db) { }

        public override IActionResult OnGet() =>
            RenderPartial("/Pages/Suppression/_CreateSuppression.cshtml", new Generator.Models.Entities.CS.Suppression());

        public IActionResult OnPost(Generator.Models.Entities.CS.Suppression suppression)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { Ok = false });
            }

            var existing = db.Suppressions.AsNoTracking().FirstOrDefault(x =>
                x.FRN == suppression.FRN && x.SchemeYear == suppression.SchemeYear && x.SuppressionEnd != null);
            if (existing == null)
            {
                suppression.Start();
                db.Suppressions.Add(suppression);
                db.SaveChanges();
            }

            return new JsonResult(new { Ok = true });
        }
    }
}
