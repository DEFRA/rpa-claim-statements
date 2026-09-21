using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Services
{
    public class DateService : IDateService
    {
        public string CurrentDateTimeString()
        {
            DateTime current = DateTime.Now;

            return string.Format("_{0}{1}{2}{3}{4}{5}", current.Day.ToString(), current.Month.ToString(), current.Year.ToString(), current.Hour.ToString(), current.Minute.ToString(), current.Second.ToString());            
        }
    }
}
