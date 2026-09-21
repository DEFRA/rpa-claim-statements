using RPA.ClaimStatements.Generator.Components;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Services;
using RPA.ClaimStatements.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RPA.ClaimStatements.Web.Controllers
{
    [Authorize(Roles = "Claim Statement Generator: Trigger Generation, Claim Statement Generator: Manage Exceptions, Claim Statement Generator: Manage Suppressions, Claim Statement Generator: View Claim Statements, Claim Statement Generator: Manual Upload")]
    public class HomeController : Controller
    {
        IPowerPlant powerPlant;
        ISampleService sampleService;
        ISampleXBService sampleXBService;

        public HomeController(IPowerPlant powerPlant, ISampleService sampleService, ISampleXBService sampleXBService)
        {
            this.powerPlant = powerPlant;
            this.sampleService = sampleService;
            this.sampleXBService = sampleXBService;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Claim Statement Generator";
            ViewBag.Location = "Home";
            ViewBag.StatementTypes = SelectionValues.StatementTypes();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Claim Statement Generator: Trigger Generation")]
        public ActionResult Trigger(string StatementTypes = "ALL", int maximumBatchSize = 10)
        {
            if (maximumBatchSize > 0)
            {
                powerPlant.Activate(StatementTypes, maximumBatchSize);
            }

            TempData["Message"] = "Generation complete";

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Claim Statement Generator: View Claim Statements")]
        public ActionResult Sample(string statementType)
        {
            string filePath = statementType == "XB" ? sampleXBService.Sample() : sampleService.Sample();
            string fileName = string.Format("Sample Claim Statement - {0}.docx", statementType);            

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            System.IO.File.Delete(filePath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}