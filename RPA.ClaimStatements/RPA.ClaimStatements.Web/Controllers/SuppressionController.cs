using PagedList;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Web.Models;
using RPA.ClaimStatements.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RPA.ClaimStatements.Web.Controllers
{
    [Authorize(Roles = "Claim Statement Generator: View Claim Statements")]
    public class SuppressionController : Controller
    {
        IClaimStatementsContext db;
        ISuppressionService suppressionService;
        
        public SuppressionController(IClaimStatementsContext context, ISuppressionService suppressionService)
        {
            this.db = context;
            this.suppressionService = suppressionService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _Existing(string searchString = null, int page = 1, int pageSize = 15)
        {
            PagedList<Suppression> suppressions;

            if (!string.IsNullOrEmpty(searchString))
            {
                suppressions = new PagedList<Suppression>(db.Suppressions.AsNoTracking().Where(x => (x.SuppressionEnd == null) && x.FRN.ToString() == searchString.Trim()).OrderByDescending(x => x.SuppressionStart), page, pageSize);
            }
            else
            {
                suppressions = new PagedList<Suppression>(db.Suppressions.AsNoTracking().Where(x => x.SuppressionEnd == null).OrderByDescending(x => x.SuppressionStart), page, pageSize);
            }
            
            ViewBag.searchData = searchString;

            return PartialView(suppressions);
        }

        [Authorize(Roles = "Claim Statement Generator: Download Data")]
        public void Download()
        {
            var suppressions = db.Suppressions
                .AsNoTracking()
                .Select(s => new { s.FRN, s.SuppressionStart, s.SuppressionEnd, s.SchemeYear})
                .Where(x => x.SuppressionEnd == null)
                .OrderByDescending(x => x.SuppressionStart);

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=Suppressions.csv");
            Response.Charset = "";
            Response.ContentType = "application/text";
            Response.Output.Write(suppressions.ToCsv());
            Response.Flush();
            Response.End();
        }

        [Authorize(Roles = "Claim Statement Generator: Manage Suppressions")]
        public ActionResult _Create()
        {
            ViewBag.CreateOptions = SelectionValues.CreateOptions();

            return PartialView();
        }

        [Authorize(Roles = "Claim Statement Generator: Manage Suppressions")]
        public ActionResult _CreateSuppression()
        {
            return PartialView(new Suppression());
        }

        [HttpPost]
        [Authorize(Roles = "Claim Statement Generator: Manage Suppressions")]
        public ActionResult _CreateSuppression(Suppression suppression)
        {
            if (ModelState.IsValid)
            {
                Suppression existing = db.Suppressions.AsNoTracking().Where(x => x.FRN == suppression.FRN && x.SchemeYear == suppression.SchemeYear && x.SuppressionEnd != null).FirstOrDefault();

                if(existing == null)
                {
                    suppression.Start();
                    db.Suppressions.Add(suppression);
                    db.SaveChanges();
                }

                return Json(new { Ok = true });
            }

            return Json(new { Ok = false });
        }

        [Authorize(Roles = "Claim Statement Generator: Manage Suppressions")]
        public ActionResult _BulkSuppression()
        {
            ViewBag.BulkDirections = SelectionValues.BulkDirections();
            ViewBag.SchemeYears = SelectionValues.GetSchemeYears();

            return PartialView();
        }

        [HttpPost]
        [Authorize(Roles = "Claim Statement Generator: Manage Suppressions")]
        public ActionResult _BulkSuppression(string BulkDirections, string SchemeYears, HttpPostedFileBase bulkSource)
        {
            try
            {
                suppressionService.BulkUpdate(BulkDirections, SchemeYears, bulkSource);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to bulk update suppressions.  Please ensure that supplied file contains only ten digit FRNs and is in .xslx format.", ex);
            }
        }

        [Authorize(Roles = "Claim Statement Generator: Manage Suppressions")]
        public ActionResult RemoveSuppression(Guid id)
        {
            Suppression suppression = db.Suppressions.Find(id);

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