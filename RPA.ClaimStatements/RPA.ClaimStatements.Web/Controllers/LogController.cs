using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.PagedList;

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
            public ActionResult Download(int count)
            {
                var logs = db.Log
                    .AsNoTracking()
                    .Select(l => new { l.SBI, l.FRN, l.SchemeYear, l.ClaimProduced, l.PDFFile })
                    .OrderByDescending(x => x.ClaimProduced).Take(count);

                Response.Headers["Content-Disposition"] = "attachment;filename=Log.csv";

                // UTF-8 without a byte order mark and without a charset parameter, as before.
                return File(new UTF8Encoding(false).GetBytes(logs.ToCsv()), "application/text");
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