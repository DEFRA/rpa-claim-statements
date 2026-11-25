namespace RPA.ClaimStatements.Generator.Services
{
    public interface IConversionService
    {
        string TransformInvoice(string invoice, string to);
    }
}
