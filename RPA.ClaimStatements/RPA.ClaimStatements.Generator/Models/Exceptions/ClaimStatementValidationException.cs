using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Models.Exceptions
{
    [Serializable]
    public class ClaimStatementValidationException:Exception
    {
        public ClaimStatementValidationException():base()
        {
        }

        public ClaimStatementValidationException(string message):base(message)
        {
        }

        public ClaimStatementValidationException(string message, Exception inner):base(message, inner)
        {
        }

        protected ClaimStatementValidationException(SerializationInfo info, StreamingContext context):base(info, context)
        {
        }
    }
}
