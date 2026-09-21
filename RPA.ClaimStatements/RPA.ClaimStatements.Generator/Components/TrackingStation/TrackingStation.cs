using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Data.Entity;

namespace RPA.ClaimStatements.Generator.Components.TrackingStation
{
    public class TrackingStation : ITrackingStation
    {
        IClaimStatementsContext db;
        IEmailService emailService;
        DateTime start;
        DateTime end;

        public TrackingStation(IClaimStatementsContext context, IEmailService emailService)
        {
            db = context;
            this.emailService = emailService;
            start = DateTime.Now;
        }

        public void Report()
        {
            end = DateTime.Now;

            try
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Tracking Station reporting");
                Console.ResetColor();

                var monitors = db.Monitor.AsNoTracking().Where(x => x.StartProcess > start).OrderBy(x => x.StartProcess).ToList();

                if (monitors.Where(x=> !x.Reference.StartsWith("Load Claim Statement Data")).ToList().Count > 0)
                {
                    emailService.Send(Build(monitors));
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Tracking Station offline - {0}", ex.Message);
                Console.ResetColor();
            }
        }

        private string Build(List<Monitor> monitors)
        {
            monitors.ForEach(x => x.EndProcess = x.EndProcess.HasValue ? x.EndProcess : end);
            
            StringWriter stringWriter = new StringWriter();

            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                writer.AddStyleAttribute("font-family", "Arial,'Segoe UI', Verdana, Helvetica, Sans-Serif");
                writer.AddStyleAttribute("font-size", "12");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.RenderBeginTag(HtmlTextWriterTag.H2);
                writer.Write("Claim Statement Generator - Tracking Station Report");
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.P);
                writer.Write("Tracking Station has identified the following Claim Statement Generator activity.");
                writer.RenderEndTag();
                foreach (var monitor in monitors)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.H4);
                    writer.Write(monitor.Reference);
                    writer.RenderEndTag();
                    writer.RenderBeginTag(HtmlTextWriterTag.P);
                    writer.Write("Start: {0}", monitor.StartProcess.ToString());
                    writer.RenderEndTag();
                    writer.RenderBeginTag(HtmlTextWriterTag.P);
                    writer.Write("End: {0}", monitor.EndProcess.ToString());
                    writer.RenderEndTag();

                    var duration = monitor.EndProcess - monitor.StartProcess;
                    var durationString = Math.Truncate(duration.Value.TotalHours).ToString().PadLeft(2, '0') + ":" + duration.Value.Minutes.ToString().PadLeft(2, '0') + ":" + duration.Value.Seconds.ToString().PadLeft(2, '0');

                    writer.RenderBeginTag(HtmlTextWriterTag.P);
                    writer.Write("Duration: {0}", durationString);
                    writer.RenderEndTag();

                    if (monitor.Reference.StartsWith("Batch Generation"))
                    {
                        var logs = db.Log.AsNoTracking().Where(x => x.ClaimProduced >= monitor.StartProcess && x.ClaimProduced <= monitor.EndProcess).Count();
                        writer.RenderBeginTag(HtmlTextWriterTag.P);
                        writer.Write("{0} statements successfully generated.", logs);
                        writer.RenderEndTag();
                    }

                    var errors = db.Errors.AsNoTracking().Where(x => x.ErrorDate >= monitor.StartProcess && x.ErrorDate <= monitor.EndProcess).OrderBy(x => x.ErrorDate).ToList();
                    
                    if(errors.Count > 0)
                    {
                        writer.RenderBeginTag(HtmlTextWriterTag.P);
                        writer.Write("{0} errors were logged during the process.", errors.Count);
                        writer.RenderEndTag();
                        foreach (var error in errors)
                        {
                            writer.RenderBeginTag(HtmlTextWriterTag.P);
                            writer.Write("{0} - {1} - {2}", error.ErrorDate.ToString(), error.FRN, error.ErrorMessage);
                            writer.RenderEndTag();
                        }
                    }
                }                
                writer.RenderEndTag();
            }        

            return stringWriter.ToString();
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    db.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
