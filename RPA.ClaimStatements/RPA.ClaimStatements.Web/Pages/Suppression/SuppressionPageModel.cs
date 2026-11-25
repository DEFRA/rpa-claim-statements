using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPA.ClaimStatements.Generator.Context;
using X.PagedList;

namespace RPA.ClaimStatements.Web.Pages.Suppression
{
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = "Claim Statement Generator: View Claim Statements")]
    public abstract class SuppressionPageModel : OperatorPageModel
    {
        protected SuppressionPageModel(IClaimStatementsContext db) => this.db = db;

        protected readonly IClaimStatementsContext db;

        protected PagedList<Generator.Models.Entities.CS.Suppression> Existing(string searchString, int page, int pageSize)
        {
            var query = db.Suppressions.AsNoTracking().Where(x => x.SuppressionEnd == null);
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(x => x.FRN.ToString() == searchString.Trim());
            }
            ViewData["searchData"] = searchString;
            return new PagedList<Generator.Models.Entities.CS.Suppression>(
                query.OrderByDescending(x => x.SuppressionStart), page, pageSize);
        }
    }
}
