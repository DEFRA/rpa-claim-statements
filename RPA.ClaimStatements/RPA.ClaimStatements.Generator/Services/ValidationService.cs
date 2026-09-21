using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPA.ClaimStatements.Generator.Models.ClaimStatements;
using RPA.ClaimStatements.Generator.Models.Exceptions;

namespace RPA.ClaimStatements.Generator.Services
{
    public class ValidationService : IValidationService
    {
        public void Validate(ClaimStatement statement, string statementType)
        {
            ValidateCurrency(statement);
            ValidateInvoices(statement);
            ValidateClaimHistory(statement, statementType);
            ValidateReconciliation(statement, statementType);
        }

        public void ValidateClaimHistory(ClaimStatement statement, string statementType)
        {
            decimal lastSettlement = statement.PartC.Invoices.OrderByDescending(x => x.DateDate).ThenByDescending(x => x.Reference).Select(x => x.ClaimValueEuro).FirstOrDefault();

            switch (statementType)
            {
                case "BPS":

                    if (lastSettlement != statement.PartA.TotalEuro)
                    {
                        throw new ClaimStatementValidationException(string.Format("Validation Error: Last settlement ({0}) in Part C does not reconcile with Part A ({1}).", lastSettlement, statement.PartA.TotalEuro));
                    }
                    break;
                case "XB":
                    ClaimStatementXB csXB = (ClaimStatementXB)statement;

                    if (lastSettlement != csXB.PartAXB.TotalTotalEuro)
                    {
                        throw new ClaimStatementValidationException(string.Format("Validation Error - Cross Border: Last settlement (€{0}) in Part C does not reconcile with Part A (€{1}).", lastSettlement, csXB.PartAXB.TotalEuro));
                    }
                    break;
                default:
                    break;
            }
        }

        public void ValidateCurrency(ClaimStatement statement)
        {
            if (statement.Currency == null)
            {
                throw new ClaimStatementValidationException("Validation Error: Currency not populated.");
            }
        }

        public void ValidateInvoices(ClaimStatement statement)
        {
            if (statement.PartC.Invoices == null || statement.PartC.Invoices.Count == 0)
            {
                throw new ClaimStatementValidationException("Validation Error: No invoices populated in Part C.");
            }
        }

        public void ValidateReconciliation(ClaimStatement statement, string statementType)
        {
            switch (statementType)
            {
                case "BPS":

                    if (statement.Currency == "Euros")
                    {
                        if (statement.PartA.TotalEuro != statement.PartC.TotalPayments)
                        {
                            throw new ClaimStatementValidationException(string.Format("Validation Error: Part C total (€{0}) does not reconcile with Part A (€{1}).", statement.PartC.TotalPayments, statement.PartA.TotalEuro));
                        }
                    }
                    else
                    {
                        if (statement.PartA.TotalSterling != statement.PartC.TotalPayments)
                        {
                            throw new ClaimStatementValidationException(string.Format("Validation Error: Part C total (£{0}) does not reconcile with Part A (£{1}).", statement.PartC.TotalPayments, statement.PartA.TotalSterling));
                        }
                    }
                    break;
                case "XB":
                    ClaimStatementXB csXB = (ClaimStatementXB)statement;

                    if (csXB.Currency == "Euros")
                    {
                        if (csXB.PartAXB.TotalTotalEuro != csXB.PartC.TotalPayments)
                        {
                            throw new ClaimStatementValidationException(string.Format("Validation Error - Cross Border: Part C total (€{0}) does not reconcile with Part A (€{1}).", csXB.PartC.TotalPayments, csXB.PartAXB.TotalTotalEuro));
                        }
                    }
                    else
                    {
                        if (csXB.PartAXB.TotalTotalSterling != csXB.PartC.TotalPayments)
                        {
                            throw new ClaimStatementValidationException(string.Format("Validation Error - Cross Border: Part C total (£{0}) does not reconcile with Part A (£{1}).", csXB.PartC.TotalPayments, csXB.PartAXB.TotalTotalSterling));
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
