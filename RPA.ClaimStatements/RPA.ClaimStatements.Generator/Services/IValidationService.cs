using RPA.ClaimStatements.Generator.Models.ClaimStatements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Services
{
    public interface IValidationService
    {
        void Validate(ClaimStatement statement, string statementType);

        void ValidateCurrency(ClaimStatement statement);

        void ValidateInvoices(ClaimStatement statement);

        void ValidateClaimHistory(ClaimStatement statement, string statementType);

        void ValidateReconciliation(ClaimStatement statement, string statementType);
    }
}
