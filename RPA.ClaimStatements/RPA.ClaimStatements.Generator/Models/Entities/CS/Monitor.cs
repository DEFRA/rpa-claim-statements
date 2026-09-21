using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.Entities.CS
{
    [Table("Monitor", Schema="Monitor")]
    public class Monitor
    {
        public Guid MonitorID { get; set; }

        public Guid MonitorProcessID { get; set; }

        public string Reference { get; set; }

        public DateTime StartProcess { get; set; }

        public DateTime? EndProcess { get; set; }

        public virtual MonitorProcess MonitorProcess { get; set; }

        public void Start()
        {
            StartProcess = DateTime.Now;
        }

        public void Stop()
        {
            EndProcess = DateTime.Now;
        }

        public Monitor()
        {
            MonitorID = Guid.NewGuid();
        }

        public Monitor(string reference):this()
        {            
            Reference = reference;            
        }

        public Monitor(Guid monitorProcessId, string reference):this(reference)
        {
            MonitorProcessID = monitorProcessId;
        }
    }
}