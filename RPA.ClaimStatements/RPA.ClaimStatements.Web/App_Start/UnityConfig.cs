using RPA.ClaimStatements.Generator.Components;
using RPA.ClaimStatements.Generator.Components.FuelTank;
using RPA.ClaimStatements.Generator.Components.PumpingStation;
using RPA.ClaimStatements.Generator.Components.PumpingStation.BulkInsert;
using RPA.ClaimStatements.Generator.Components.PumpingStation.Serializer;
using RPA.ClaimStatements.Generator.Components.TrackingStation;
using RPA.ClaimStatements.Generator.Components.Turbine;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.DAX;
using RPA.ClaimStatements.Generator.Models.Entities.SITI;
using RPA.ClaimStatements.Generator.Models.Entities.XB;
using RPA.ClaimStatements.Generator.Services;
using RPA.ClaimStatements.Web.Services;
using System;

using Unity;

namespace RPA.ClaimStatements.Web
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();
       
            container.RegisterType<IClaimStatementsContext, ClaimStatementsContext>();
            container.RegisterType<IPumpingStation, PumpingStation>();
            container.RegisterType<IFuelTank, FuelTank>();
            container.RegisterType<ITurbine, Turbine>();
            container.RegisterType<ITrackingStation, TrackingStation>();
            container.RegisterType<IPowerPlant, PowerPlant>();
            container.RegisterType<ISITIImportService, SITIImportService>();
            container.RegisterType<IXBImportService, XBImportService>();
            container.RegisterType<IAPImportService, APImportService>();
            container.RegisterType<IARImportService, ARImportService>();
            container.RegisterType<ISerializer<Claim>, SITISerializer>();
            container.RegisterType<ISerializer<XBData>, Serializer<XBData>>();
            container.RegisterType<ISerializer<AP>, Serializer<AP>>();
            container.RegisterType<ISerializer<AR>, Serializer<AR>>();
            container.RegisterType<IBulkInsert<Claim>, SQLBulkInsert<Claim>>();
            container.RegisterType<IBulkInsert<SUM>, SQLBulkInsert<SUM>>();
            container.RegisterType<IBulkInsert<SUM2>, SQLBulkInsert<SUM2>>();
            container.RegisterType<IBulkInsert<BPS>, SQLBulkInsert<BPS>>();
            container.RegisterType<IBulkInsert<BPSPEN>, SQLBulkInsert<BPSPEN>>();
            container.RegisterType<IBulkInsert<GR>, SQLBulkInsert<GR>>();
            container.RegisterType<IBulkInsert<GRPEN>, SQLBulkInsert<GRPEN>>();
            container.RegisterType<IBulkInsert<PR>, SQLBulkInsert<PR>>();
            container.RegisterType<IBulkInsert<YF>, SQLBulkInsert<YF>>();
            container.RegisterType<IBulkInsert<CLD>, SQLBulkInsert<CLD>>();
            container.RegisterType<IBulkInsert<XB>, SQLBulkInsert<XB>>();
            container.RegisterType<IBulkInsert<XBData>, SQLBulkInsert<XBData>>();
            container.RegisterType<IBulkInsert<AP>, SQLBulkInsert<AP>>();
            container.RegisterType<IBulkInsert<AR>, SQLBulkInsert<AR>>();
            container.RegisterType<ISITIBulkInsertContainer, SITIBulkInsertContainer>();
            container.RegisterType<IConfigurationService, ConfigurationService>();
            container.RegisterType<IFTPService, FTPService>();
            container.RegisterType<IFolderService, FolderService>();
            container.RegisterType<IErrorService, ErrorService>();
            container.RegisterType<IFileService, FileService>();
            container.RegisterType<IMonitorService, MonitorService>();
            container.RegisterType<IConversionService, ConversionService>();
            container.RegisterType<ICurrencyService, CurrencyService>();
            container.RegisterType<IValidationService, ValidationService>();
            container.RegisterType<ITransformService, TransformService>();
            container.RegisterType<ILogService, LogService>();
            container.RegisterType<IDateService, DateService>();
            container.RegisterType<IBuildService, BuildService>();
            container.RegisterType<ISuppressionService, SuppressionService>();
            container.RegisterType<IEmailService, EmailService>();
            container.RegisterType<ISampleService, SampleService>();
            container.RegisterType<ISampleXBService, SampleXBService>();
        }
    }
}