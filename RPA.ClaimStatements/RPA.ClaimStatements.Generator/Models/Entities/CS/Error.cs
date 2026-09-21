using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPA.ClaimStatements.Generator.Models.Entities.CS
{
    [Table("Errors", Schema="CS")]
    public class Error
    {
        public Guid ErrorID { get; set; }

        [Display(Name="Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime ErrorDate { get; set; }

        public Int64 FRN { get; set; }

        [Display(Name = "Scheme Year")]
        public int? SchemeYear { get; set; }

        [Display(Name="Error")]
        public string ErrorMessage { get; set; }

        [Display(Name="Claim Statement")]
        public string XML { get; set; }

        public bool Resolved { get; set; }

        public Error()
        {
            ErrorID = Guid.NewGuid();
            ErrorDate = DateTime.Now;
            Resolved = false;
        }

        public Error(long frn, int schemeYear, string message = null, string filePath = null) :this()
        {
            FRN = frn;
            SchemeYear = schemeYear;
            ErrorMessage = message;
            XML = filePath;
        }
    }
}