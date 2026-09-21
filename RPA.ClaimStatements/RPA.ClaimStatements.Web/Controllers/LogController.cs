using PagedList;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RPA.ClaimStatements.Web.Controllers
{
    namespace RPA.ClaimStatements.Controllers
    {
        [Authorize(Roles = "Claim Statement Generator: View Claim Statements")]
        public class LogController : Controller
        {
            IClaimStatementsContext db;
            IConfigurationService configurationService;
            
            public LogController(IClaimStatementsContext context, IConfigurationService configurationService)
            {
                this.db = context;
                this.configurationService = configurationService;
            }

            [Authorize(Roles = "Claim Statement Generator: Download Data")]
            public void Download(int count)
            {
                var logs = db.Log
                    .AsNoTracking()
                    .Select(l => new { l.SBI, l.FRN, l.SchemeYear, l.ClaimProduced, l.PDFFile })
                    .OrderByDescending(x => x.ClaimProduced).Take(count);

                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=Log.csv");
                Response.Charset = "";
                Response.ContentType = "application/text";
                Response.Output.Write(logs.ToCsv());
                Response.Flush();
                Response.End();
            }

            public ActionResult Index(string searchString = null, int page = 1, int pageSize = 15)
            {
                PagedList<Log> logs;

                if (string.IsNullOrEmpty(searchString))
                {
                    logs = new PagedList<Log>(db.Log.AsNoTracking().OrderByDescending(x => x.ClaimProduced), page, pageSize);
                }
                else
                {
                    logs = new PagedList<Log>(db.Log.AsNoTracking().Where(x => x.FRN.ToString() == searchString.Trim() || x.SBI.ToString() == searchString.Trim()).OrderByDescending(x => x.ClaimProduced), page, pageSize);                    
                }
                

                ViewBag.searchData = searchString;
                ViewBag.uploadActive = configurationService.IsActive("Manual Upload");

                return View(logs);
            }
        }
    }
}