using RPA.ClaimStatements.Generator.Components;
using RPA.ClaimStatements.Generator.Components.PumpingStation;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RPA.ClaimStatements.Web.Controllers
{
    [Authorize(Roles = "Claim Statement Generator: Trigger Generation")]
    public class ImportController : Controller
    {
        IClaimStatementsContext db;
        IPumpingStation pumpingStation;
        
        public ImportController(IClaimStatementsContext context, IPumpingStation pumpingStation)
        {
            this.db = context;
            this.pumpingStation = pumpingStation;
        }
        
        public ActionResult Import()
        {
            pumpingStation.Activate();

            TempData["Message"] = "Import run complete";

            return RedirectToAction("Index", "Home");
        }
    }
}