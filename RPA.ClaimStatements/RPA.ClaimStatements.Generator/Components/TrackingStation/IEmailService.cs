using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Components.TrackingStation
{
    public interface IEmailService
    {
        void Send(string content);
    }
}
