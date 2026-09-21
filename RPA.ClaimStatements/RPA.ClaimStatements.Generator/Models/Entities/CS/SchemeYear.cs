using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.Entities.CS
{
    [Table("SchemeYears", Schema = "CS")]
    public class SchemeYear
    {
        public Guid SchemeYearID { get; set; }

        [Display(Name = "Scheme Year")]
        public int SchemeYearNumber { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

        public SchemeYear()
        {
            SchemeYearID = Guid.NewGuid();
            Active = true;
        }

        public SchemeYear(int schemeYearNumber):this()
        {
            SchemeYearNumber = schemeYearNumber;
        }

        public SchemeYear(int schemeYearNumber, bool active) : this(schemeYearNumber)
        {
            Active = active;
        }
    }
}