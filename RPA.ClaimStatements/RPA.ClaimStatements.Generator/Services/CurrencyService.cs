using RPA.ClaimStatements.Generator.Models.Entities.CS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Services
{
    public class CurrencyService : ICurrencyService
    {
        public bool CheckTolerance(decimal claimValue, decimal transactionValue, decimal tolerance = 0.05M)
        {
            decimal check = Math.Abs(claimValue - transactionValue);

            if (check <= tolerance && check >= (tolerance * -1))
            {
                return true;
            }

            return false;
        }

        public decimal Convert(decimal value, decimal rate)
        {
            return Math.Round(value * rate, 2);
        }
    }
}
