using System;
using System.Diagnostics;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace RPA.ClaimStatements.Generator.Tests.TestSupport
{
    // Skips a test, instead of failing it, when Python with WeasyPrint is not installed on the machine.
    // The container image always has them; a developer machine may not.
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class RequiresWeasyPrintAttribute : NUnitAttribute, IApplyToTest
    {
        private static readonly Lazy<bool> Available = new Lazy<bool>(Probe);

        public void ApplyToTest(Test test)
        {
            if (!Available.Value)
            {
                test.RunState = RunState.Skipped;
                test.Properties.Set(PropertyNames.SkipReason, "Python with WeasyPrint 70.0 is not installed.");
            }
        }

        private static bool Probe()
        {
            try
            {
                var start = new ProcessStartInfo(OperatingSystem.IsWindows() ? "python" : "python3")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                start.ArgumentList.Add("-c");
                start.ArgumentList.Add("import weasyprint");
                using var process = Process.Start(start);
                return process.WaitForExit(30000) && process.ExitCode == 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
