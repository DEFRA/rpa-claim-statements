using Microsoft.EntityFrameworkCore;
using RPA.ClaimStatements.Generator.Components.FuelTank;
using RPA.ClaimStatements.Generator.Components.PumpingStation;
using RPA.ClaimStatements.Generator.Components.TrackingStation;
using RPA.ClaimStatements.Generator.Components.Turbine;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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
            using var activity = ApplicationTelemetry.Source.StartActivity("ClaimStatements.Run");
            activity?.SetTag("statement.type", statementType);
            activity?.SetTag("batch.maximum", maximumBatchSize);
            try
            {
                ActivateCore(statementType, maximumBatchSize);
                activity?.SetStatus(ActivityStatusCode.Ok);
            }
            catch (Exception error)
            {
                activity?.SetTag("error.type", error.GetType().FullName);
                activity?.SetStatus(ActivityStatusCode.Error);
                throw;
            }
        }

        private void ActivateCore(string statementType, int maximumBatchSize)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Power Plant activated");
            Console.ResetColor();

            EnsureSchemaContract();

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

        /// <summary>
        /// Fails visibly, before any work starts, when the database is unreachable or its schema is
        /// behind the migrations in this build. It never creates or migrates anything: deployed
        /// environments apply the EF migration bundle, and a developer machine can opt in through
        /// SchemaMigrations.
        /// </summary>
        private void EnsureSchemaContract()
        {
            if (!db.CanConnect())
            {
                throw new InvalidOperationException(
                    "ClaimStatementsContext could not connect to the configured database. Verify the " +
                    "'ClaimStatementsContext' connection string/managed identity and that the database exists.");
            }

            var pending = db.GetPendingMigrations()?.ToList();
            if (pending != null && pending.Count > 0)
            {
                throw new InvalidOperationException(
                    "The database schema is behind this version of the application. Pending migrations: " +
                    string.Join(", ", pending) + ". Apply the EF migration bundle before starting the application.");
            }
        }
    }
}
