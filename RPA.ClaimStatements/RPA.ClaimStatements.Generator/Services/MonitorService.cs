using Microsoft.EntityFrameworkCore;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using System;
using System.Linq;

namespace RPA.ClaimStatements.Generator.Services
{
    public class MonitorService : IMonitorService
    {
        IClaimStatementsContext db;
                
        public MonitorService(IClaimStatementsContext context)
        {
            db = context;
        }

        public Guid GetMonitorProcessId(string processName)
        {
            return db.MonitorProcesses.AsNoTracking().Where(x => x.ProcessName == processName).Select(x => x.MonitorProcessID).FirstOrDefault();
        }

        public void CreateMonitor(Monitor monitor)
        {
            monitor.Start();
            db.Monitor.Add(monitor);
            db.SaveChanges();
        }

        public void EndMonitor(Monitor monitor)
        {
            monitor.Stop();
            db.SaveChanges();
        }
    }
}
