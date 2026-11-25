using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RPA.ClaimStatements.Generator.Context;

namespace RPA.ClaimStatements.Web.Pages.Error
{
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = "Claim Statement Generator: View Claim Statements")]
    [Authorize(Roles = "Claim Statement Generator: Manage Exceptions")]
    public class ResolveErrorModel : OperatorPageModel
    {
        private readonly IClaimStatementsContext db;
        public ResolveErrorModel(IClaimStatementsContext db) => this.db = db;

        public IActionResult OnPost(Guid id)
        {
            var error = db.Errors.Find(id);
            if (error != null)
            {
                error.Resolved = true;
                db.SaveChanges();
            }
            return Content("");
        }
    }
}
