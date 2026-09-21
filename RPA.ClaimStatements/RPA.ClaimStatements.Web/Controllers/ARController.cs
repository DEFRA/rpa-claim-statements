using RPA.ClaimStatements.Data.UnitOfWork;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RPA.ClaimStatements.Web.Controllers
{
    [Authorize(Roles = "Claim Statement Generator: Trigger Generation")]
    public class ARController : Controller
    {
        IUnitOfWork uow = null;
        IImportService importService = null;

        public ARController()
        {
            this.uow = new UnitOfWork();
            this.importService = new ImportService(uow);
        }

        public ARController(IUnitOfWork uow)
        {
            this.uow = uow;
            this.importService = new ImportService(uow);
        }

        public ARController(IUnitOfWork uow, IImportService importService)
        {
            this.uow = uow;
            this.importService = importService;
        }
        
        public ActionResult Import()
        {
            importService.ImportAR();

            TempData["MessageAR"] = "AR Load complete";

            return RedirectToAction("Index", "Home");
        }
    }
}