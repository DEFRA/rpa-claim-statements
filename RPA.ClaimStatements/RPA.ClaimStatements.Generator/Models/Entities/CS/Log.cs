using RPA.ClaimStatements.Generator.Models.Entities.SITI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.Entities.CS
{
    [Table("Log", Schema="Log")]
    public class Log
    {
        public Guid LogID { get; set; }

        public Guid? ClaimID { get; set; }
        
        public int SBI { get; set; }

        public Int64 FRN { get; set; }

        [Display(Name="Scheme Year")]
        public int SchemeYear { get; set; }

        [Display(Name = "Generated")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? ClaimProduced { get; set; }

        [Display(Name = "")]
        public string PDFFile { get; set; }

        public DateTime? Uploaded { get; set; }        

        public string UploadedBy { get; set; }

        public virtual Claim Claim { get; set; }

        public string UploadedShort
        {
            get
            {
                if(Uploaded.HasValue)
                {
                    return Uploaded.Value.ToShortDateString();
                }
                else
                {
                    return null;
                }
            }
        }

        public Log()
        {
            LogID = Guid.NewGuid();
            ClaimProduced = DateTime.Now;
        }

        public Log(long frn, int sbi, int schemeYear, string @filePath, Guid claimId):this()
        {
            FRN = frn;
            SBI = sbi;
            SchemeYear = schemeYear;
            PDFFile = @filePath;
            ClaimID = claimId;
        }
    }
}