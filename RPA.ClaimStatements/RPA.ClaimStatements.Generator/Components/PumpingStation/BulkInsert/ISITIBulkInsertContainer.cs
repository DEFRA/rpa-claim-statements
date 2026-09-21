using RPA.ClaimStatements.Generator.Models.Entities.SITI;

namespace RPA.ClaimStatements.Generator.Components.PumpingStation.BulkInsert
{
    public interface ISITIBulkInsertContainer
    {
        IBulkInsert<BPS> BPSBulkInsert { get; }
        IBulkInsert<BPSPEN> BPSPENBulkInsert { get; }
        IBulkInsert<Claim> ClaimBulkInsert { get; }
        IBulkInsert<CLD> CLDBulkInsert { get; }
        IBulkInsert<GR> GRBulkInsert { get; }
        IBulkInsert<GRPEN> GRPENBulkInsert { get; }
        IBulkInsert<SUM2> SUM2BulkInsert { get; }
        IBulkInsert<SUM> SUMBulkInsert { get; }
        IBulkInsert<YF> YFBulkInsert { get; }
        IBulkInsert<PR> PRBulkInsert { get; }
    }
}