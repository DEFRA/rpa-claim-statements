using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Web.Extensions;

namespace RPA.ClaimStatements.Web.Pages.Error
{
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = "Claim Statement Generator: View Claim Statements")]
    [Authorize(Roles = "Claim Statement Generator: Download Data")]
    public class DownloadModel : OperatorPageModel
    {
        private readonly IClaimStatementsContext db;
        public DownloadModel(IClaimStatementsContext db) => this.db = db;
        public IActionResult OnPost() => OnGet();

        public override IActionResult OnGet()
        {
            var errors = db.Errors.AsNoTracking()
                .Select(e => new { e.ErrorDate, e.FRN, e.SchemeYear, e.ErrorMessage, e.Resolved })
                .Where(x => !x.Resolved).OrderByDescending(x => x.ErrorDate);
            Response.Headers["Content-Disposition"] = "attachment;filename=Errors.csv";
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true))
                {
                    writer.Write(errors.ToCsv());
                }
                return File(stream.ToArray(), "application/text");
            }
        }
    }
}
