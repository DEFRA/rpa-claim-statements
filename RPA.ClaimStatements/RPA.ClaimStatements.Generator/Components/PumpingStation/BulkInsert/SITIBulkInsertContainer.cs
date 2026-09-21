using RPA.ClaimStatements.Generator.Models.Entities.SITI;
using RPA.ClaimStatements.Generator.Components.PumpingStation.BulkInsert;
using RPA.ClaimStatements.Generator.Components.PumpingStation.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Components.PumpingStation.BulkInsert
{
    public class SITIBulkInsertContainer : ISITIBulkInsertContainer
    {
        public IBulkInsert<Claim> ClaimBulkInsert { get; private set; }
        public IBulkInsert<SUM> SUMBulkInsert { get; private set; }
        public IBulkInsert<SUM2> SUM2BulkInsert { get; private set; }
        public IBulkInsert<BPS> BPSBulkInsert { get; private set; }
        public IBulkInsert<BPSPEN> BPSPENBulkInsert { get; private set; }
        public IBulkInsert<GR> GRBulkInsert { get; private set; }
        public IBulkInsert<GRPEN> GRPENBulkInsert { get; private set; }
        public IBulkInsert<YF> YFBulkInsert { get; private set; }
        public IBulkInsert<CLD> CLDBulkInsert { get; private set; }
        public IBulkInsert<PR> PRBulkInsert { get; private set; }

        public SITIBulkInsertContainer(IBulkInsert<Claim> claimBulkInsert, IBulkInsert<SUM> sumBulkInsert, IBulkInsert<SUM2> sum2BulkInsert, IBulkInsert<BPS> bpsBulkInsert, IBulkInsert<BPSPEN> bpspenBulkInsert, IBulkInsert<GR> grBulkInsert, IBulkInsert<GRPEN> grpenBulkInsert,
            IBulkInsert<YF> yfBulkInsert, IBulkInsert<CLD> cldBulkInsert, IBulkInsert<PR> prBulkInsert)
        {
            this.ClaimBulkInsert = claimBulkInsert;
            this.SUMBulkInsert = sumBulkInsert;
            this.SUM2BulkInsert = sum2BulkInsert;
            this.BPSBulkInsert = bpsBulkInsert;
            this.BPSPENBulkInsert = bpspenBulkInsert;
            this.GRBulkInsert = grBulkInsert;
            this.GRPENBulkInsert = grpenBulkInsert;
            this.YFBulkInsert = yfBulkInsert;
            this.CLDBulkInsert = cldBulkInsert;
            this.PRBulkInsert = prBulkInsert;
        }


    }
}
