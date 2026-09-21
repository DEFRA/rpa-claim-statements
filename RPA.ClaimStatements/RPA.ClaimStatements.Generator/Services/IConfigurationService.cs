using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Services
{
    public interface IConfigurationService
    {
        bool IsActive(string name);

        string GetValue(string name);        
    }
}
