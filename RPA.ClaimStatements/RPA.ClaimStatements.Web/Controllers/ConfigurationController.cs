using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RPA.ClaimStatements.Web.Controllers
{
    [Authorize(Roles = "Claim Statement Generator: Trigger Generation")]
    public class ConfigurationController : Controller
    {
        IClaimStatementsContext db;
        
        public ConfigurationController(IClaimStatementsContext context)
        {
            this.db = context;
        }

        public ActionResult Index()
        {
            var model = db.Configurations.AsNoTracking().OrderBy(x => x.Setting).ToList();
            ViewBag.ListValues = SelectionValues.ListValues();

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(List<Configuration> model)
        {
            foreach(Configuration configuration in model)
            {
                db.SetModified(configuration);
            }

            db.SaveChanges();

            TempData["Message"] = "Configuration saved";

            return RedirectToAction("Index");
        }

        public ActionResult SchemeYear()
        {
            var model = db.SchemeYears.AsNoTracking().OrderBy(x=>x.SchemeYearNumber).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult SchemeYear(List<SchemeYear> model)
        {
            foreach(SchemeYear schemeYear in model)
            {
                db.SetModified(schemeYear);
            }

            db.SaveChanges();

            TempData["Message"] = "Configuration saved";

            return RedirectToAction("SchemeYear");
        }
    }
}