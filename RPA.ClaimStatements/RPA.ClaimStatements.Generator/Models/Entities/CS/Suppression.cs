using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.Entities.CS
{
    [Table("Suppressions", Schema="CS")]
    public class Suppression
    {
        public Guid SuppressionID { get; set; }

        [Required]
        [Range(1000000000 , 9999999999, ErrorMessage="Invalid FRN")]
        public Int64 FRN { get; set; }

        [Display(Name="Start")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime SuppressionStart { get; set; }

        [Display(Name = "End")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? SuppressionEnd { get; set; }

        [Display(Name = "Scheme Year")]
        [Range(2015, 2099, ErrorMessage = "Invalid Scheme Year")]
        public int SchemeYear { get; set; }

        public void Start()
        {
            SuppressionStart = DateTime.Now;
        }

        public void End()
        {
            SuppressionEnd = DateTime.Now;
        }

        public Suppression()
        {
            SuppressionID = Guid.NewGuid();            
            SuppressionEnd = null;
        }

        public Suppression(long frn, int schemeYear):this()
        {
            FRN = frn;
            SchemeYear = schemeYear;            
        }

    }
}