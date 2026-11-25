using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Web.Extensions;

namespace RPA.ClaimStatements.Web.Pages.Log
{
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = "Claim Statement Generator: View Claim Statements")]
    [Authorize(Roles = "Claim Statement Generator: Download Data")]
    public class DownloadModel : OperatorPageModel
    {
        private readonly IClaimStatementsContext db;
        public DownloadModel(IClaimStatementsContext db) => this.db = db;

        [BindProperty(SupportsGet = true)]
        public int count { get; set; }

        public IActionResult OnPost() => OnGet();

        public override IActionResult OnGet()
        {
            var logs = db.Log.AsNoTracking()
                .Select(l => new { l.SBI, l.FRN, l.SchemeYear, l.ClaimProduced, l.PDFFile })
                .OrderByDescending(x => x.ClaimProduced).Take(count);
            Response.Headers["Content-Disposition"] = "attachment;filename=Log.csv";
            return File(new UTF8Encoding(false).GetBytes(logs.ToCsv()), "application/text");
        }
    }
}
