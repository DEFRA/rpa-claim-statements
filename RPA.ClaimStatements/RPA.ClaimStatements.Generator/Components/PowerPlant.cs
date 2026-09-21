using RPA.ClaimStatements.Generator.Components.FuelTank;
using RPA.ClaimStatements.Generator.Components.PumpingStation;
using RPA.ClaimStatements.Generator.Components.TrackingStation;
using RPA.ClaimStatements.Generator.Components.Turbine;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Components
{
    public class PowerPlant : IPowerPlant
    {
        IClaimStatementsContext db;
        IPumpingStation pumpingStation;
        IFuelTank fuelTank;
        ITurbine turbine;
        ITrackingStation trackingStation;
        IConfigurationService configurationService;
        IFolderService folderService;
        const string csgMutexId = "2F56AE60-350C-4AA2-B69D-9C661B04AE1F";

        public PowerPlant(IClaimStatementsContext context, IPumpingStation pumpingStation, IFuelTank fuelTank, ITurbine turbine, ITrackingStation trackingStation,
            IConfigurationService configurationService, IFolderService folderService)
        {
            this.db = context;
            this.pumpingStation = pumpingStation;
            this.fuelTank = fuelTank;
            this.turbine = turbine;
            this.trackingStation = trackingStation;
            this.configurationService = configurationService;
            this.folderService = folderService;
        }

        public void Activate(string statementType, int maximumBatchSize = 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Power Plant activated");
            Console.ResetColor();

            RunMigrations();

            pumpingStation.Activate();

            if (maximumBatchSize > 0 && configurationService.IsActive("Generation"))
            {
                Mutex mutex = new Mutex(false, string.Format(@"Global\CSG_{0}_{1}", statementType, csgMutexId));

                try
                {
                    try
                    {
                        mutex.WaitOne();
                    }
                    catch (AbandonedMutexException) { }

                    if (statementType == "ALL")
                    {
                        folderService.CheckCreateWorking();

                        List<string> statements = db.Transformations.AsNoTracking().Select(x => x.StatementType).ToList();

                        foreach (string statement in statements)
                        {
                            var fuel = fuelTank.Fill(statement, maximumBatchSize);
                            turbine.Generate(fuel, statement);
                        }
                    }
                    else
                    {
                        var fuel = fuelTank.Fill(statementType, maximumBatchSize);
                        turbine.Generate(fuel, statementType);
                    }
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }

            trackingStation.Report();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Power Plant shutting down");
            Console.ResetColor();
        }

        private void RunMigrations()
        {
            var configuration = new Migrations.Configuration();
            var migrator = new DbMigrator(configuration);
            migrator.Update();
        }
    }
}
