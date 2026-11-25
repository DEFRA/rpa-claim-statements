using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Web.Extensions;

namespace RPA.ClaimStatements.Web.Pages.Suppression
{
    [Authorize(Roles = "Claim Statement Generator: Download Data")]
    public class DownloadModel : SuppressionPageModel
    {
        public DownloadModel(IClaimStatementsContext db) : base(db) { }
        public IActionResult OnPost() => OnGet();

        public override IActionResult OnGet()
        {
            var suppressions = db.Suppressions.AsNoTracking()
                .Select(s => new { s.FRN, s.SuppressionStart, s.SuppressionEnd, s.SchemeYear })
                .Where(x => x.SuppressionEnd == null).OrderByDescending(x => x.SuppressionStart);
            Response.Headers["Content-Disposition"] = "attachment;filename=Suppressions.csv";
            return File(new UTF8Encoding(false).GetBytes(suppressions.ToCsv()), "application/text");
        }
    }
}
