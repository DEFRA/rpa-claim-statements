using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.Entities.CS
{
    [Table("SchemeCodes", Schema="CS")]
    public class SchemeCode
    {
        public Guid SchemeCodeID { get; set; }

        public string SchemeCodeName { get; set; }
    }
}