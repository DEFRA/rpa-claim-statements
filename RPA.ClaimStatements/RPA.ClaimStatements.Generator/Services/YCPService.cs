namespace RPA.ClaimStatements.Generator.Services
{
    public class YCPService : IYCPService
    {
        public string GetRate(int schemeYear, decimal overDeclarationPercent, decimal OverDeclarationHectares, bool previous)
        {
            if (schemeYear >= 2016)
            {
                if (overDeclarationPercent > 3 || OverDeclarationHectares > 2)
                {
                    if (previous || overDeclarationPercent > 10)
                    {
                        return "Standard Rate";
                    }

                    return "Reduced Rate";
                }
            }

            return "No Reduction";
        }
    }
}
