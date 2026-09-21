using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Models.ClaimStatements;
using RPA.ClaimStatements.Generator.Models.Exceptions;
using RPA.ClaimStatements.Generator.Models.Generation;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;
using System.Collections.Concurrent;

namespace RPA.ClaimStatements.Generator.Components.Turbine
{
    public class Turbine : ITurbine
    {
        IMonitorService monitorService;
        IValidationService validationService;
        ITransformService transformService;
        IErrorService errorService;
        IConfigurationService configurationService;
        IBuildService buildService;
        bool validate;

        public Turbine(IMonitorService monitorService, IValidationService validationService, ITransformService transformService, IErrorService errorService, IConfigurationService configurationService, IBuildService buildService)
        {
            this.monitorService = monitorService;
            this.validationService = validationService;
            this.transformService = transformService;
            this.errorService = errorService;
            this.configurationService = configurationService;
            this.buildService = buildService;
            validate = configurationService.IsActive("Validation");
        }

        public void Generate(Request[] requests, string statementType)
        {
            if (requests.Length > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Turbines fuelled and activated");
                Console.WriteLine($"Generating Claim {requests.Length} Statements");
                Console.ResetColor();

                Monitor generationMonitor = new Monitor(monitorService.GetMonitorProcessId("Claim Statement Generation"), string.Format("Batch Generation {0} - {1}", statementType, requests.Length));
                monitorService.CreateMonitor(generationMonitor);

                Stopwatch sw = new Stopwatch();
                sw.Start();

                Dictionary<Request, ClaimStatement> requestStatements = new Dictionary<Request, ClaimStatement>();
 
                foreach (Request r in requests)
                {
                    if (r != null)
                    {
                        try
                        {
                            ClaimStatement statement = statementType == "BPS" ? buildService.Build(r) : buildService.BuildXB(r);

                            if (validate)
                            {
                                validationService.Validate(statement, statementType);
                            }

                            requestStatements.Add(r, statement);
                        }
                        catch (ClaimStatementValidationException ex)
                        {
                            errorService.Log(r.Claim.SUM.FRN, r.Claim.SUM.SchemeYear, string.Format("Claim Statement validation failed for {0} - {1}", r.Claim.SUM.FRN, ex.Message));
                        }
                        catch (Exception ex)
                        {
                            errorService.Log(r.Claim.SUM.FRN, r.Claim.SUM.SchemeYear, string.Format("Claim Statement generation failed for - {0} - {1}", r.Claim.SUM.FRN, ex.Message));
                        }
                    }
                }

                try
                {
                    Parallel.ForEach(requestStatements, new ParallelOptions { MaxDegreeOfParallelism = 16 }, r =>
                    {
                        transformService.Transform(r.Key, r.Value, statementType);
                    });
                }
                catch (AggregateException ex)
                {
                    foreach (Exception innerException in ex.InnerExceptions)
                    {
                        if (innerException is ClaimStatementGenerationException)
                        {
                            ClaimStatementGenerationException generationException = ex.InnerException as ClaimStatementGenerationException;
                            errorService.Log(generationException.FRN, generationException.SchemeYear, string.Format("Claim Statement generation failed for - {0} - {1}", generationException.FRN, ex.Message));
                        }
                        else
                        {
                            const byte statementUnknown = 0;
                            errorService.Log(statementUnknown, statementUnknown, string.Format("Claim Statement generation failed - {0}", ex.Message));
                        }
                    }
                }

                sw.Stop();
                Console.WriteLine($"Total requests: {requests.Length} Duration: {sw.Elapsed}");

                monitorService.EndMonitor(generationMonitor);                
            }
        }    
    }
}
