using System.Data.Entity;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Models.Entities.DAX;
using RPA.ClaimStatements.Generator.Models.Entities.FDMR;
using RPA.ClaimStatements.Generator.Models.Entities.SITI;
using RPA.ClaimStatements.Generator.Models.Entities.XB;

namespace RPA.ClaimStatements.Generator.Context
{
    public interface IClaimStatementsContext
    {
        DbSet<AP> AP { get; set; }
        DbSet<AR> AR { get; set; }
        DbSet<BPS> BPS { get; set; }
        DbSet<BPSPEN> BPSPEN { get; set; }
        DbSet<Claim> Claims { get; set; }
        DbSet<CLD> CLD { get; set; }
        DbSet<Configuration> Configurations { get; set; }
        DbSet<ConversionRate> ConversionRates { get; set; }
        DbSet<Error> Errors { get; set; }
        DbSet<Extract> Extract { get; set; }
        DbSet<Folder> Folders { get; set; }
        DbSet<GR> GR { get; set; }
        DbSet<GRPEN> GRPEN { get; set; }
        DbSet<PR> PR { get; set; }
        DbSet<Log> Log { get; set; }
        DbSet<Monitor> Monitor { get; set; }
        DbSet<MonitorProcess> MonitorProcesses { get; set; }
        DbSet<SchemeCode> SchemeCodes { get; set; }
        DbSet<SchemeYear> SchemeYears { get; set; }
        DbSet<SUM> SUM { get; set; }
        DbSet<SUM2> SUM2 { get; set; }
        DbSet<Suppression> Suppressions { get; set; }
        DbSet<Transformation> Transformations { get; set; }
        DbSet<XB> XB { get; set; }
        DbSet<XBData> XBData { get; set; }
        DbSet<YF> YF { get; set; }
        int SaveChanges();
        void Dispose();
        void SetModified(object entity);
    }
}