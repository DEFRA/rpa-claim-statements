using System;
using RPA.ClaimStatements.Generator.Models.Entities.CS;

namespace RPA.ClaimStatements.Generator.Services
{
    public interface IMonitorService
    {
        void CreateMonitor(Monitor monitor);
        void EndMonitor(Monitor monitor);
        Guid GetMonitorProcessId(string processName);
    }
}