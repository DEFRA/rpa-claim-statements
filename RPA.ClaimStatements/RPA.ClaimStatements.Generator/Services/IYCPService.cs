using RPA.ClaimStatements.Generator.Models.Enums;
using System.Collections.Generic;

namespace RPA.ClaimStatements.Generator.Services
{
    public interface IYCPService
    {
        string GetRate(int schemeYear, decimal overDeclarationPercent, decimal OverDeclarationHectares, bool previous);
    }
}