using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.Entities.CS
{
    [Table("MonitorProcesses", Schema = "Monitor")]
    public class MonitorProcess
    {
        public Guid MonitorProcessID { get; set; }

        public string ProcessName { get; set; }

        public virtual ICollection<Monitor> Monitors { get; set; }

        public MonitorProcess()
        {
            MonitorProcessID = Guid.NewGuid();
        }

        public MonitorProcess(string processName):this()
        {
            ProcessName = processName;
        }
    }
}