namespace RPA.ClaimStatements.Generator.Services
{
    public interface ICurrencyService
    {
        decimal Convert(decimal value, decimal rate);
        
        bool CheckTolerance(decimal claimValue, decimal transactionValue, decimal tolerance = 0.05M);
    }
}
