using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Models.Entities.DAX;
using RPA.ClaimStatements.Generator.Models.Entities.FDMR;
using RPA.ClaimStatements.Generator.Models.Entities.SITI;
using RPA.ClaimStatements.Generator.Models.Entities.XB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Context
{
    public class ClaimStatementsContext : DbContext, IClaimStatementsContext
    {
        public ClaimStatementsContext():base("ClaimStatementsContext")
        {
            Database.CommandTimeout = 180;
        }

        public virtual DbSet<AP> AP { get; set; }
        public virtual DbSet<AR> AR { get; set; }
        public virtual DbSet<BPS> BPS { get; set; }
        public virtual DbSet<BPSPEN> BPSPEN { get; set; }
        public virtual DbSet<Claim> Claims { get; set; }
        public virtual DbSet<CLD> CLD { get; set; }
        public virtual DbSet<ConversionRate> ConversionRates { get; set; }
        public virtual DbSet<Folder> Folders { get; set; }
        public virtual DbSet<GR> GR { get; set; }
        public virtual DbSet<GRPEN> GRPEN { get; set; }
        public virtual DbSet<PR> PR { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<Monitor> Monitor { get; set; }
        public virtual DbSet<MonitorProcess> MonitorProcesses { get; set; }
        public virtual DbSet<SUM> SUM { get; set; }
        public virtual DbSet<SUM2> SUM2 { get; set; }
        public virtual DbSet<Suppression> Suppressions { get; set; }
        public virtual DbSet<YF> YF { get; set; }
        public virtual DbSet<SchemeCode> SchemeCodes { get; set; }
        public virtual DbSet<SchemeYear> SchemeYears { get; set; }
        public virtual DbSet<Error> Errors { get; set; }
        public virtual DbSet<Configuration> Configurations { get; set; }        
        public virtual DbSet<XB> XB { get; set; }
        public virtual DbSet<XBData> XBData { get; set; }        
        public virtual DbSet<Extract> Extract { get; set; }
        public virtual DbSet<Transformation> Transformations { get; set; }
        
        public void SetModified(object entity)
        {
            this.Entry(entity).State = System.Data.Entity.EntityState.Modified;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BPS>().Property(x => x.NonSDANumber).HasPrecision(18, 4);
            modelBuilder.Entity<BPS>().Property(x => x.NonSDATotal).HasPrecision(18, 4);
            modelBuilder.Entity<BPS>().Property(x => x.SDANumber).HasPrecision(18, 4);
            modelBuilder.Entity<BPS>().Property(x => x.SDATotal).HasPrecision(18, 4);
            modelBuilder.Entity<BPS>().Property(x => x.MoorlandNumber).HasPrecision(18, 4);
            modelBuilder.Entity<BPS>().Property(x => x.MoorlandSDATotal).HasPrecision(18, 4);

            modelBuilder.Entity<BPSPEN>().Property(x => x.OverDeclarationHectares).HasPrecision(18, 4);

            modelBuilder.Entity<GR>().Property(x => x.NonSDANumber).HasPrecision(18, 4);
            modelBuilder.Entity<GR>().Property(x => x.NonSDATotal).HasPrecision(18, 4);
            modelBuilder.Entity<GR>().Property(x => x.SDANumber).HasPrecision(18, 4);
            modelBuilder.Entity<GR>().Property(x => x.SDATotal).HasPrecision(18, 4);
            modelBuilder.Entity<GR>().Property(x => x.MoorlandNumber).HasPrecision(18, 4);
            modelBuilder.Entity<GR>().Property(x => x.MoorlandTotal).HasPrecision(18, 4);

            modelBuilder.Entity<GRPEN>().Property(x => x.EFANumber).HasPrecision(18, 4);
            modelBuilder.Entity<GRPEN>().Property(x => x.CropDiversificationNumber).HasPrecision(18, 4);
            modelBuilder.Entity<GRPEN>().Property(x => x.PermanentGrasslandNumber).HasPrecision(18, 4);
            modelBuilder.Entity<GRPEN>().Property(x => x.AdministrativeArea).HasPrecision(18, 4);
            modelBuilder.Entity<GRPEN>().Property(x => x.CropDiversificationNumberAdditional).HasPrecision(18, 4);
            modelBuilder.Entity<GRPEN>().Property(x => x.EFANumberAdditional).HasPrecision(18, 4);

            modelBuilder.Entity<PR>().Property(x => x.PRB1MaxBand).HasPrecision(18, 4);
            modelBuilder.Entity<PR>().Property(x => x.PRB1Percent).HasPrecision(18, 4);
            modelBuilder.Entity<PR>().Property(x => x.PRB1Result).HasPrecision(18, 4);
            modelBuilder.Entity<PR>().Property(x => x.PRB2MaxBand).HasPrecision(18, 4);
            modelBuilder.Entity<PR>().Property(x => x.PRB2Percent).HasPrecision(18, 4);
            modelBuilder.Entity<PR>().Property(x => x.PRB2Result).HasPrecision(18, 4);
            modelBuilder.Entity<PR>().Property(x => x.PRB3MaxBand).HasPrecision(18, 4);
            modelBuilder.Entity<PR>().Property(x => x.PRB3Percent).HasPrecision(18, 4);
            modelBuilder.Entity<PR>().Property(x => x.PRB3Result).HasPrecision(18, 4);
            modelBuilder.Entity<PR>().Property(x => x.PRB4MaxBand).HasPrecision(18, 4);
            modelBuilder.Entity<PR>().Property(x => x.PRB4Percent).HasPrecision(18, 4);
            modelBuilder.Entity<PR>().Property(x => x.PRB4Result).HasPrecision(18, 4);
            modelBuilder.Entity<PR>().Property(x => x.PRBTotalResult).HasPrecision(18, 4);

            modelBuilder.Entity<YF>().Property(x => x.OverDeclarationArea).HasPrecision(18, 4);
            modelBuilder.Entity<YF>().Property(x => x.EntitlementsUsedToClaim).HasPrecision(18, 4);

            modelBuilder.Entity<CLD>().Property(x => x.NonSDAAreaOnApplication).HasPrecision(18, 4);
            modelBuilder.Entity<CLD>().Property(x => x.NonSDAAreaEligible).HasPrecision(18, 4);
            modelBuilder.Entity<CLD>().Property(x => x.NonSDAEntitlements).HasPrecision(18, 4);
            modelBuilder.Entity<CLD>().Property(x => x.SDAAreaOnApplication).HasPrecision(18, 4);
            modelBuilder.Entity<CLD>().Property(x => x.SDAAreaEligible).HasPrecision(18, 4);
            modelBuilder.Entity<CLD>().Property(x => x.SDAEntitlements).HasPrecision(18, 4);
            modelBuilder.Entity<CLD>().Property(x => x.MoorlandAreaOnApplication).HasPrecision(18, 4);
            modelBuilder.Entity<CLD>().Property(x => x.MoorlandAreaEligible).HasPrecision(18, 4);
            modelBuilder.Entity<CLD>().Property(x => x.MoorlandEntitlements).HasPrecision(18, 4);
            
            modelBuilder.Entity<ConversionRate>().Property(x => x.Rate).HasPrecision(18, 6);

            modelBuilder.Entity<XBData>().Property(x => x.EnglandBPSNonSDANumber).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.EnglandBPSNonSDATotal).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.EnglandBPSSDANumber).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.EnglandBPSSDATotal).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.EnglandBPSMoorlandNumber).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.EnglandBPSMoorlandTotal).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.WalesBPSRegion1Number).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.WalesBPSRegion1Total).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandBPSRegion1Number).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandBPSRegion1Total).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandBPSRegion2Number).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandBPSRegion2Total).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandBPSRegion3Number).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandBPSRegion3Total).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.NIBPSRegion1Number).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.NIBPSRegion1Total).HasPrecision(18, 4);

            modelBuilder.Entity<XBData>().Property(x => x.EnglandBPSOverDeclarationHectares).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.WalesBPSOverDeclarationHectares).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandBPSOverDeclarationHectares).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.NIBPSOverDeclarationHectares).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.EnglandBPSAverageEntitlementValue2015Value).HasPrecision(18, 4);

            modelBuilder.Entity<XBData>().Property(x => x.EnglandGreeningNonSDANumber).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.EnglandGreeningNonSDATotal).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.EnglandGreeningSDANumber).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.EnglandGreeningSDATotal).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.EnglandGreeningMoorlandNumber).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.EnglandGreeningMoorlandTotal).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.WalesGreeningRegion1Number).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.WalesGreeningRegion1Total).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandGreeningRegion1Number).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandGreeningRegion1Total).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandGreeningRegion2Number).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandGreeningRegion2Total).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandGreeningRegion3Number).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandGreeningRegion3Total).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.NIGreeningRegion1Number).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.NIGreeningRegion1Total).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.EnglandGreeningAverageGreeningValue2015Value).HasPrecision(18, 4);

            modelBuilder.Entity<XBData>().Property(x => x.EnglandGreeningEFANumber).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.EnglandGreeningCropDiversificationNumber).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.EnglandGreeningPermanentGrasslandNumber).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.EnglandGreeningAdministrativeArea).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.WalesGreeningEFANumber).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.WalesGreeningCropDiversificationNumber).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.WalesGreeningPermanentGrasslandNumber).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.WalesGreeningAdministrativeArea).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandGreeningEFANumber).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandGreeningCropDiversificationNumber).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandGreeningPermanentGrasslandNumber).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandGreeningAdministrativeArea).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.NIGreeningEFANumber).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.NIGreeningCropDiversificationNumber).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.NIGreeningPermanentGrasslandNumber).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.NIGreeningAdministrativeArea).HasPrecision(18, 4);

            modelBuilder.Entity<XBData>().Property(x => x.WalesRedLandUsedToClaim).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.WalesRedOverDeclarationArea).HasPrecision(18, 4);

            modelBuilder.Entity<XBData>().Property(x => x.EnglandNonSDAAreaOnApplication).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.EnglandNonSDAAreaIneligible).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.EnglandNonSDAEntitlements).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.EnglandSDAAreaOnApplication).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.EnglandSDAAreaIneligible).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.EnglandSDAEntitlements).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.EnglandMoorlandAreaOnApplication).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.EnglandMoorlandAreaIneligible).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.EnglandMoorlandEntitlements).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.WalesAreaOnApplication).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.WalesAreaIneligible).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.WalesEntitlements).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandRegion1AreaOnApplication).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandRegion1AreaIneligible).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandRegion1Entitlements).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandRegion2AreaOnApplication).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandRegion2AreaIneligible).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandRegion2Entitlements).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandRegion1AreaOnApplication).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandRegion2AreaIneligible).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandRegion3Entitlements).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.NIAreaOnApplication).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.NIAreaIneligible).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.NIEntitlements).HasPrecision(18, 4);            

            base.OnModelCreating(modelBuilder);
        }
    }
}
