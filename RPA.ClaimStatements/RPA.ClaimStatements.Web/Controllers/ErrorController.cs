using PagedList;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RPA.ClaimStatements.Web.Controllers
{
    public static class StreamExtensions
    {
        public static byte[] ToBytes(this string value, Encoding encoding)
        {
            using (var stream = new MemoryStream())
            using (var sw = new StreamWriter(stream, encoding))
            {
                sw.Write(value);
                sw.Flush();
                return stream.ToArray();
            }
        }
    }

    [Authorize(Roles = "Claim Statement Generator: View Claim Statements")]
    public class ErrorController : Controller
    {
        IClaimStatementsContext db;
        IConfigurationService configurationService;
        
        public ErrorController(IClaimStatementsContext context, IConfigurationService configurationService)
        {
            this.db = context;
            this.configurationService = configurationService;
        }

        public ActionResult Index(string searchString = null, int page = 1, int pageSize = 10)
        {
            PagedList<Error> errors;

            if (string.IsNullOrEmpty(searchString))
            {
                errors = new PagedList<Error>(db.Errors.AsNoTracking().Where(x=>!x.Resolved).OrderByDescending(x => x.ErrorDate), page, pageSize);
            }
            else
            {
                errors = new PagedList<Error>(db.Errors.AsNoTracking().Where(x => x.FRN.ToString() == searchString.Trim() && !x.Resolved).OrderByDescending(x => x.ErrorDate), page, pageSize);
            }
            
            ViewBag.ForceGeneration = configurationService.IsActive("Force Generation");
            ViewBag.searchData = searchString;

            return View(errors);
        }

        [Authorize(Roles = "Claim Statement Generator: Download Data")]
        public void Download()
        {
            var errors = db.Errors
                .AsNoTracking()
                .Select(e => new { e.ErrorDate, e.FRN, e.SchemeYear, e.ErrorMessage, e.Resolved })
                .Where(x => !x.Resolved)
                .OrderByDescending(x => x.ErrorDate);

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=Errors.csv");
            Response.ContentType = "application/text;";
            Response.BinaryWrite(StreamExtensions.ToBytes(errors.ToCsv(), Encoding.UTF8));
            Response.Flush();
            Response.End();
        }

        [HttpPost]
        [Authorize(Roles = "Claim Statement Generator: Manage Exceptions")]
        public ActionResult ResolveError(Guid id)
        {
            Error error = db.Errors.Find(id);

            if (error != null)
            {
                error.Resolved = true;
                db.SaveChanges();
            }

            return Content("");
        }
    }
}