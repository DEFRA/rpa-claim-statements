// =============================================================================================
// EF Core context for the ClaimStatements database. Migrations live in Context\Migrations\; the
// model deliberately keeps the column types, constraint names and indexes the former EF6
// migrations created, so existing databases match it (see the developer guide, "Database schema and migrations").
// Deployed environments apply migrations with the EF migration bundle, never at startup; see
// SchemaMigrations for the local developer opt-in.
// =============================================================================================
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Unity;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Models.Entities.DAX;
using RPA.ClaimStatements.Generator.Models.Entities.FDMR;
using RPA.ClaimStatements.Generator.Models.Entities.SITI;
using RPA.ClaimStatements.Generator.Models.Entities.XB;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RPA.ClaimStatements.Generator.Context
{
    public class ClaimStatementsContext : DbContext, IClaimStatementsContext
    {
        /// <summary>
        /// Parameterless constructor used by the Unity container registration
        /// (<c>container.RegisterType&lt;IClaimStatementsContext, ClaimStatementsContext&gt;()</c>
        /// in Web/App_Start/UnityConfig.cs). Requires ConnectionStrings__ClaimStatementsContext, and preserves
        /// the EF6 context's <c>Database.CommandTimeout = 180</c> behavior via
        /// <see cref="Microsoft.EntityFrameworkCore.Infrastructure.SqlServerDbContextOptionsBuilder.CommandTimeout(int)"/>.
        /// </summary>
        [InjectionConstructor]
        public ClaimStatementsContext()
            : base(BuildOptionsFromConfiguration())
        {
        }


        /// <summary>
        /// DI-friendly constructor for callers (e.g. the model/query acceptance tests in
        /// Generator.Tests) that want to supply their own <see cref="DbContextOptions{TContext}"/>
        /// instead of reading the environment. Deliberately NOT decorated with [InjectionConstructor] so
        /// Unity's container (Program.cs / Web/App_Start/UnityConfig.cs) always resolves the
        /// parameterless constructor above, never this one — verified against the real Unity
        /// 5.11.10 package (Unity.InjectionConstructorAttribute, AttributeTargets.Constructor) so
        /// this is not a guessed convention. Production code paths never call this overload.
        /// </summary>
        public ClaimStatementsContext(DbContextOptions<ClaimStatementsContext> options)
            : base(options)
        {
        }

        private static DbContextOptions<ClaimStatementsContext> BuildOptionsFromConfiguration()
        {
            string connectionString = ClaimStatementsConnectionString.Resolve();

            var optionsBuilder = new DbContextOptionsBuilder<ClaimStatementsContext>();
            optionsBuilder.UseSqlServer(connectionString, sql => sql.CommandTimeout(180));
            return optionsBuilder.Options;
        }

        public virtual DbSet<AP> AP { get; set; }
        public virtual DbSet<AR> AR { get; set; }
        public virtual DbSet<BPS> BPS { get; set; }
        public virtual DbSet<BPSPEN> BPSPEN { get; set; }
        public virtual DbSet<Claim> Claims { get; set; }
        public virtual DbSet<CLD> CLD { get; set; }
        public virtual DbSet<Configuration> Configurations { get; set; }
        public virtual DbSet<ConversionRate> ConversionRates { get; set; }
        public virtual DbSet<Error> Errors { get; set; }
        public virtual DbSet<Extract> Extract { get; set; }
        public virtual DbSet<Folder> Folders { get; set; }
        public virtual DbSet<GR> GR { get; set; }
        public virtual DbSet<GRPEN> GRPEN { get; set; }
        public virtual DbSet<PR> PR { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<Monitor> Monitor { get; set; }
        public virtual DbSet<MonitorProcess> MonitorProcesses { get; set; }
        public virtual DbSet<SchemeCode> SchemeCodes { get; set; }
        public virtual DbSet<SchemeYear> SchemeYears { get; set; }
        public virtual DbSet<SUM> SUM { get; set; }
        public virtual DbSet<SUM2> SUM2 { get; set; }
        public virtual DbSet<Suppression> Suppressions { get; set; }
        public virtual DbSet<Transformation> Transformations { get; set; }
        public virtual DbSet<XB> XB { get; set; }
        public virtual DbSet<XBData> XBData { get; set; }
        public virtual DbSet<YF> YF { get; set; }

        public void SetModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// Used by PowerPlant.EnsureSchemaContract(). Wraps EF Core's DatabaseFacade so the
        /// interface stays provider-neutral. Performs no migration or DDL.
        /// </summary>
        public bool CanConnect()
        {
            return Database.CanConnect();
        }

        public IEnumerable<string> GetPendingMigrations()
        {
            return Database.GetPendingMigrations();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ---------------------------------------------------------------------------------------
            // Decimal precision — carried over verbatim from the former EF6 context. Every other
            // decimal column is left at the SqlServer provider default (18,2), identical in EF6 and
            // EF Core, so no explicit configuration is added for those.
            // ---------------------------------------------------------------------------------------
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
            modelBuilder.Entity<XBData>().Property(x => x.ScotlandRegion3Entitlements).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.NIAreaOnApplication).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.NIAreaIneligible).HasPrecision(18, 4);
            modelBuilder.Entity<XBData>().Property(x => x.NIEntitlements).HasPrecision(18, 4);

            // ---------------------------------------------------------------------------------------
            // Explicit primary keys — all entities follow the <Type>ID convention (also honored
            // implicitly by both EF6 and EF Core), made explicit here per the task requirement to not
            // infer convention parity blindly. FDMR.Extract already has [Key]/[DatabaseGenerated]
            // data annotations, honored identically by EF Core, so it is not repeated here.
            // ---------------------------------------------------------------------------------------
            modelBuilder.Entity<AP>().HasKey(x => x.APID);
            modelBuilder.Entity<AR>().HasKey(x => x.ARID);
            modelBuilder.Entity<BPS>().HasKey(x => x.BPSID);
            modelBuilder.Entity<BPSPEN>().HasKey(x => x.BPSPENID);
            modelBuilder.Entity<Claim>().HasKey(x => x.ClaimID);
            modelBuilder.Entity<CLD>().HasKey(x => x.CLDID);
            modelBuilder.Entity<Configuration>().HasKey(x => x.ConfigurationID);
            modelBuilder.Entity<ConversionRate>().HasKey(x => x.ConversionRateID);
            modelBuilder.Entity<Error>().HasKey(x => x.ErrorID);
            modelBuilder.Entity<Folder>().HasKey(x => x.FolderID);
            modelBuilder.Entity<GR>().HasKey(x => x.GRID);
            modelBuilder.Entity<GRPEN>().HasKey(x => x.GRPENID);
            modelBuilder.Entity<PR>().HasKey(x => x.PRID);
            modelBuilder.Entity<Log>().HasKey(x => x.LogID);
            modelBuilder.Entity<Monitor>().HasKey(x => x.MonitorID);
            modelBuilder.Entity<MonitorProcess>().HasKey(x => x.MonitorProcessID);
            modelBuilder.Entity<SchemeCode>().HasKey(x => x.SchemeCodeID);
            modelBuilder.Entity<SchemeYear>().HasKey(x => x.SchemeYearID);
            modelBuilder.Entity<SUM>().HasKey(x => x.SUMID);
            modelBuilder.Entity<SUM2>().HasKey(x => x.SUM2ID);
            modelBuilder.Entity<Suppression>().HasKey(x => x.SuppressionID);
            modelBuilder.Entity<Transformation>().HasKey(x => x.TransformationID);
            modelBuilder.Entity<XB>().HasKey(x => x.XBID);
            modelBuilder.Entity<XBData>().HasKey(x => x.XBDataID);
            modelBuilder.Entity<YF>().HasKey(x => x.YFID);

            // ---------------------------------------------------------------------------------------
            // Explicit FK/cascade configuration, taken from the former EF6 migrations (see git history).
            // SITI "part" entities
            // (BPS, BPSPEN, CLD, GR, GRPEN, PR, SUM, SUM2, YF) each also carry their own ClaimID
            // scalar column with NO navigation and NO enforced FK back to Claims — that is the known
            // historic discrepancy, preserved as a plain scalar column below, NOT modeled as a
            // relationship (§11.3).
            // ---------------------------------------------------------------------------------------
            modelBuilder.Entity<Claim>().HasOne(c => c.SUM).WithOne().HasForeignKey<Claim>(c => c.SUMID).IsRequired().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Claim>().HasOne(c => c.SUM2).WithOne().HasForeignKey<Claim>(c => c.SUM2ID).IsRequired().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Claim>().HasOne(c => c.BPS).WithOne().HasForeignKey<Claim>(c => c.BPSID).IsRequired().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Claim>().HasOne(c => c.BPSPEN).WithOne().HasForeignKey<Claim>(c => c.BPSPENID).IsRequired().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Claim>().HasOne(c => c.GR).WithOne().HasForeignKey<Claim>(c => c.GRID).IsRequired().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Claim>().HasOne(c => c.GRPEN).WithOne().HasForeignKey<Claim>(c => c.GRPENID).IsRequired().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Claim>().HasOne(c => c.CLD).WithOne().HasForeignKey<Claim>(c => c.CLDID).IsRequired().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Claim>().HasOne(c => c.YF).WithOne().HasForeignKey<Claim>(c => c.YFID).IsRequired(false).OnDelete(DeleteBehavior.ClientSetNull);
            modelBuilder.Entity<Claim>().HasOne(c => c.PR).WithOne().HasForeignKey<Claim>(c => c.PRID).IsRequired(false).OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Log>().HasOne(l => l.Claim).WithMany().HasForeignKey(l => l.ClaimID).IsRequired(false).OnDelete(DeleteBehavior.ClientSetNull);
            modelBuilder.Entity<AP>().HasOne(a => a.Log).WithMany().HasForeignKey(a => a.LogID).IsRequired(false).OnDelete(DeleteBehavior.ClientSetNull);
            modelBuilder.Entity<AR>().HasOne(a => a.Log).WithMany().HasForeignKey(a => a.LogID).IsRequired(false).OnDelete(DeleteBehavior.ClientSetNull);
            modelBuilder.Entity<Monitor>().HasOne(m => m.MonitorProcess).WithMany(mp => mp.Monitors).HasForeignKey(m => m.MonitorProcessID).IsRequired().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<XB>().HasOne(x => x.XBData).WithMany().HasForeignKey(x => x.XBDataID).IsRequired().OnDelete(DeleteBehavior.Cascade);

            // ---------------------------------------------------------------------------------------
            // Explicit Guid PK generation — every Guid primary key is always client-supplied
            // (Guid.NewGuid() in each entity's parameterless constructor), never DB-generated, which
            // is EF6's DatabaseGeneratedOption.None behavior for these columns. EF Core's default
            // convention differs subtly (it conditionally skips generation only when a non-default
            // value is already present). To guarantee 1:1 parity rather than rely on that heuristic,
            // every Guid PK is explicitly set to ValueGenerated.Never here (§11.3). FDMR.Extract's
            // int identity PK is untouched (DatabaseGeneratedOption.Identity via data annotation).
            // ---------------------------------------------------------------------------------------
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var primaryKey = entityType.FindPrimaryKey();

                if (primaryKey == null || primaryKey.Properties.Count != 1)
                {
                    continue;
                }

                var keyProperty = primaryKey.Properties[0];

                if (keyProperty.ClrType == typeof(Guid))
                {
                    keyProperty.ValueGenerated = ValueGenerated.Never;
                }
            }

            // Keep the names (and non-unique FK indexes) EF6 created, so databases built by the EF6
            // migrations match this model and later migrations can drop or alter them by name.
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                string tableName = entityType.GetTableName();
                string schemaName = entityType.GetSchema();
                var table = StoreObjectIdentifier.Table(tableName, schemaName);
                string Columns(System.Collections.Generic.IEnumerable<IMutableProperty> properties) =>
                    string.Join("_", properties.Select(p => p.GetColumnName(table)));

                entityType.FindPrimaryKey()?.SetName($"PK_{schemaName}.{tableName}");

                foreach (var foreignKey in entityType.GetForeignKeys())
                {
                    var principal = foreignKey.PrincipalEntityType;
                    foreignKey.SetConstraintName(
                        $"FK_{schemaName}.{tableName}_{principal.GetSchema()}.{principal.GetTableName()}_{Columns(foreignKey.Properties)}");
                }

                foreach (var index in entityType.GetIndexes())
                {
                    index.SetDatabaseName($"IX_{Columns(index.Properties)}");
                    index.IsUnique = false;
                }
            }

            base.OnModelCreating(modelBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // EF6 mapped DateTime to datetime; EF Core defaults to datetime2.
            configurationBuilder.Properties<DateTime>().HaveColumnType("datetime");
        }
    }
}
