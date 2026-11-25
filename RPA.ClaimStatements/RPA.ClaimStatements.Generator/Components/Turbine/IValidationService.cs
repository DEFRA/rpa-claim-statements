using RPA.ClaimStatements.Generator.Models.ClaimStatements;

namespace RPA.ClaimStatements.Generator.Components.Turbine
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
