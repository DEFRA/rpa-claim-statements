using RPA.ClaimStatements.Generator.Components;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Globalization;
using System.IO;
using Unity;

namespace RPA.ClaimStatements.Generator
{
    public class Program
    {
        public static int Main(string[] args)
        {
            using var telemetry = ApplicationTelemetry.CreateProvider("RPA.ClaimStatements.Generator");
            SetCulture();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Claim Statement Generator");
            Console.WriteLine("DEFRA Digital, Data and Technology Services (DDTS)");
            Console.WriteLine("Development Team {0}", DateTime.Now.Year);
            Console.ResetColor();

            try
            {
                return Run(args, Console.IsInputRedirected, Console.In, Console.Out, Console.Error, Execute);
            }
            catch (Exception error)
            {
                // Reported here, not left unhandled, so the failed run's telemetry is flushed when the provider is disposed.
                Console.Error.WriteLine(error);
                return 1;
            }
        }

        internal static int Run(string[] args, bool inputRedirected, TextReader input,
            TextWriter output, TextWriter error, Action<string, int> execute)
        {
            string statementType;
            int maxBatchSize = 0;

            if (args.Length > 0)
            {
                statementType = args[0];
                if (!ValidateStatementType(statementType))
                    return UsageError(error, "Invalid statement code.");

                if (args.Length != (statementType == "I" ? 1 : 2))
                    return UsageError(error, "Incorrect number of arguments.");

                if (statementType != "I" && !ValidateBatchSize(args[1], out maxBatchSize))
                    return UsageError(error, "Batch size must be a positive 32-bit integer.");
            }
            else
            {
                if (inputRedirected)
                    return UsageError(error, "Arguments are required when standard input is redirected.");

                while (true)
                {
                    output.WriteLine("Enter Statement code BPS, XB or All to begin generation or I to run imports");
                    string line = input.ReadLine();
                    if (line == null)
                        return UsageError(error, "End of input before a statement code was supplied.");

                    statementType = line.ToUpper();
                    if (ValidateStatementType(statementType))
                        break;
                }

                if (statementType != "I")
                {
                    while (true)
                    {
                        output.WriteLine("Enter maximum batch size for generation");
                        string line = input.ReadLine();
                        if (line == null)
                            return UsageError(error, "End of input before a batch size was supplied.");

                        if (ValidateBatchSize(line, out maxBatchSize))
                            break;
                    }
                }
            }

            if (statementType != "I")
                output.WriteLine("Generation requested for {0} with a maximum batch size of {1}", statementType, maxBatchSize);

            execute(statementType, maxBatchSize);
            return 0;
        }

        private static int UsageError(TextWriter error, string message)
        {
            error.WriteLine("Error: " + message);
            error.WriteLine("Usage: RPA.ClaimStatements.Generator BPS N | XB N | ALL N | I");
            error.WriteLine("N must be a positive 32-bit integer. I runs imports only.");
            return 2;
        }

        private static void Execute(string statementType, int maxBatchSize)
        {
            StartupChecks.EnsureReady();

            using var container = new UnityContainer();
            GeneratorServices.Register(container);
            var powerPlant = container.Resolve<IPowerPlant>();

            if (statementType == "I")
            {
                powerPlant.Activate(statementType);
            }
            else
            {
                powerPlant.Activate(statementType, maxBatchSize);
            }
        }

        private static void SetCulture()
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-GB");
        }

        private static bool ValidateStatementType(string input)
        {
            return input == "BPS" || input == "XB" || input == "ALL" || input == "I";
        }
        private static bool ValidateBatchSize(string input, out int value)
        {
            return int.TryParse(input, out value) && value > 0;
        }
    }
}
