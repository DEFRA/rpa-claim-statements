using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPA.ClaimStatements.Generator.Models.Entities.CS
{
    [Table("SchemeCodes", Schema="CS")]
    public class SchemeCode
    {
        public Guid SchemeCodeID { get; set; }

        public string SchemeCodeName { get; set; }
    }
}