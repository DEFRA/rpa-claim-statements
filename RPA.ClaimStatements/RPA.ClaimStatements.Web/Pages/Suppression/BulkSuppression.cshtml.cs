using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Web.Models;
using RPA.ClaimStatements.Web.Services;

namespace RPA.ClaimStatements.Web.Pages.Suppression
{
    [Authorize(Roles = "Claim Statement Generator: Manage Suppressions")]
    public class BulkSuppressionModel : SuppressionPageModel
    {
        private readonly ISuppressionService suppressionService;

        public BulkSuppressionModel(IClaimStatementsContext db, ISuppressionService suppressionService) : base(db) =>
            this.suppressionService = suppressionService;

        public override IActionResult OnGet()
        {
            ViewData["BulkDirections"] = SelectionValues.BulkDirections();
            ViewData["SchemeYears"] = SelectionValues.GetSchemeYears();
            return RenderPartial("/Pages/Suppression/_BulkSuppression.cshtml");
        }

        public IActionResult OnPost(string BulkDirections, string SchemeYears, IFormFile bulkSource)
        {
            try
            {
                suppressionService.BulkUpdate(BulkDirections, SchemeYears, bulkSource);
                return RedirectToPage("/Suppression/Index");
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to bulk update suppressions.  Please ensure that supplied file contains only ten digit FRNs and is in .xslx format.", ex);
            }
        }
    }
}
