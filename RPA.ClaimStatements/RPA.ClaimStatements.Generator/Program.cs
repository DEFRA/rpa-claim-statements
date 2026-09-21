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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace RPA.ClaimStatements.Generator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SetCulture();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Claim Statement Generator");
            Console.WriteLine("DEFRA Digital, Data and Technology Services (DDTS)");
            Console.WriteLine("Development Team {0}", DateTime.Now.Year);
            Console.ResetColor();

            var container = new UnityContainer();
            container.RegisterType<IClaimStatementsContext, ClaimStatementsContext>();
            container.RegisterType<IPumpingStation, PumpingStation>();
            container.RegisterType<IFuelTank, FuelTank>();
            container.RegisterType<ITurbine, Turbine>();
            container.RegisterType<ITrackingStation, TrackingStation>();
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
            container.RegisterType<IEmailService, EmailService>();
            container.RegisterType<ILogService, LogService>();
            container.RegisterType<IDateService, DateService>();
            container.RegisterType<IBuildService, BuildService>();
            var powerPlant = container.Resolve<PowerPlant>();

            string statementType = null;
            int? maxBatchSize = null;

            if (args.Length > 0)
            {
                statementType = ValidateStatementType(args[0]) ? args[0] : null;
                maxBatchSize = args.Length > 1 && ValidateBatchSize(args[1]) ? int.Parse(args[1]) : (int?)null;
            }
            else
            {
                bool awaitingStatementType = true;

                while (awaitingStatementType)
                {
                    Console.WriteLine("Enter Statement code BPS, XB or All to begin generation or I to run imports");
                    string input = Console.ReadLine().ToUpper();

                    if (ValidateStatementType(input))
                    {
                        statementType = input;
                        awaitingStatementType = false;
                    }
                }

                if (statementType != "I")
                {
                    bool awaitingBatchSize = true;

                    while (awaitingBatchSize)
                    {
                        Console.WriteLine("Enter maximum batch size for generation");
                        string input = Console.ReadLine();

                        if (ValidateBatchSize(input))
                        {
                            maxBatchSize = int.Parse(input);
                            awaitingBatchSize = false;
                        }
                    }
                }
            }

            if (statementType == "I")
            {
                powerPlant.Activate(statementType);
            }
            else if (statementType != null && maxBatchSize != null)
            {
                Console.WriteLine("Generation requested for {0} with a maximum batch size of {1}", statementType, maxBatchSize);
                powerPlant.Activate(statementType, maxBatchSize.Value);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid Statement type and/or batch size supplied");
                Console.ResetColor();
            }
        }

        private static void SetCulture()
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-GB");
        }

        private static bool ValidateStatementType(string input)
        {
            return input == "BPS" || input == "XB" || input == "ALL" || input == "I" ? true : false;            
        }
        private static bool ValidateBatchSize(string input)
        {
            int value;

            return int.TryParse(input, out value);            
        }
    }
}
