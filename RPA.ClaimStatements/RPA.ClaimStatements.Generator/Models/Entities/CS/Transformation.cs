using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Models.Entities.CS
{
    [Table("Transformations", Schema = "CS")]
    public class Transformation
    {
        public Guid TransformationID { get; set; }

        public string StatementType { get; set; }

        public string XSLT { get; set; }

        public string Template { get; set; }

        public Transformation()
        {
            TransformationID = Guid.NewGuid();
        }

        public Transformation(string statementType, string xslt, string template):this()
        {
            StatementType = statementType;
            XSLT = xslt;
            Template = template;
        }
    }
}
