using NUnit.Framework;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace RPA.ClaimStatements.Generator.Tests
{
    [TestFixture]
    [NonParallelizable]
    public class ProgramTests
    {
        private CultureInfo originalCulture;

        [SetUp]
        public void SetUp()
        {
            originalCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");
        }

        [TearDown]
        public void TearDown()
        {
            CultureInfo.CurrentCulture = originalCulture;
        }

        [TestCase("BPS", "1", 1)]
        [TestCase("XB", "0005", 5)]
        [TestCase("ALL", "2147483647", int.MaxValue)]
        public void Run_ValidGeneration_DispatchesOnceWithoutReadingInput(string command, string size, int expectedSize)
        {
            using var input = new CountingReader("I\n");
            using var output = new StringWriter();
            using var error = new StringWriter();
            int calls = 0;

            int result = Program.Run(new[] { command, size }, true, input, output, error, (statement, batch) =>
            {
                calls++;
                Assert.That(statement, Is.EqualTo(command));
                Assert.That(batch, Is.EqualTo(expectedSize));
            });

            Assert.That(result, Is.Zero);
            Assert.That(calls, Is.EqualTo(1));
            Assert.That(input.Reads, Is.Zero);
            Assert.That(error.ToString(), Is.Empty);
            Assert.That(output.ToString(), Does.Contain("Generation requested for " + command));
        }

        [Test]
        public void Run_ImportsOnly_DispatchesOnceWithNoGenerationBatchOrPrompt()
        {
            using var input = new CountingReader("");
            using var output = new StringWriter();
            using var error = new StringWriter();
            int calls = 0;

            int result = Program.Run(new[] { "I" }, true, input, output, error, (statement, batch) =>
            {
                calls++;
                Assert.That(statement, Is.EqualTo("I"));
                Assert.That(batch, Is.Zero);
            });

            Assert.That(result, Is.Zero);
            Assert.That(calls, Is.EqualTo(1));
            Assert.That(input.Reads, Is.Zero);
            Assert.That(output.ToString(), Is.Empty);
            Assert.That(error.ToString(), Is.Empty);
        }

        private static IEnumerable<string[]> InvalidArguments()
        {
            yield return new[] { "synthetic-secret" };
            yield return new[] { "synthetic-secret", "10" };
            yield return new[] { "bps", "10" };
            yield return new[] { "BPS" };
            yield return new[] { "BPS", "1", "synthetic-secret" };
            foreach (string size in new[] { "", "synthetic-secret", "1.5", "0", "-1", "2147483648" })
                yield return new[] { "XB", size };
            yield return new[] { "I", "1" };
        }

        [TestCaseSource(nameof(InvalidArguments))]
        public void Run_InvalidArguments_ReturnsUsageErrorWithoutCreatingDependenciesOrExecuting(string[] args)
        {
            using var input = new CountingReader("I\n");
            using var output = new StringWriter();
            using var error = new StringWriter();

            int result = Program.Run(args, false, input, output, error,
                (_, _) => Assert.Fail("Dependency creation/execution must remain lazy until validation succeeds."));

            AssertUsageError(result, output.ToString(), error.ToString());
            Assert.That(input.Reads, Is.Zero);
            Assert.That(output.ToString(), Is.Empty);
        }

        [TestCase("BPS\n10\n")]
        public void Run_RedirectedInputWithoutArguments_RejectsEvenBufferedValidInputWithoutReading(string text)
        {
            using var input = new CountingReader(text);
            using var output = new StringWriter();
            using var error = new StringWriter();

            int result = Program.Run(Array.Empty<string>(), true, input, output, error,
                (_, _) => Assert.Fail("Redirected no-argument invocation must not construct dependencies."));

            AssertUsageError(result, output.ToString(), error.ToString());
            Assert.That(error.ToString(), Does.Contain("standard input is redirected"));
            Assert.That(input.Reads, Is.Zero);
            Assert.That(output.ToString(), Is.Empty);
        }

        [TestCase("bps\n3\n", "BPS", 3, 2)]
        [TestCase("i\n", "I", 0, 1)]
        [TestCase("synthetic-secret\n\n BPS\nbps\n0\n-1\n2147483648\n1.5\nsynthetic-secret\n5\n",
            "BPS", 5, 10)]
        public void Run_InteractiveInput_RepromptsAndDispatchesOnlyAfterCompleteValidInput(
            string text, string expectedCommand, int expectedBatch, int expectedReads)
        {
            using var input = new CountingReader(text);
            using var output = new StringWriter();
            using var error = new StringWriter();
            int calls = 0;

            int result = Program.Run(Array.Empty<string>(), false, input, output, error, (statement, batch) =>
            {
                calls++;
                Assert.That(input.Reads, Is.EqualTo(expectedReads), "No dependencies before both prompts are satisfied.");
                Assert.That(statement, Is.EqualTo(expectedCommand));
                Assert.That(batch, Is.EqualTo(expectedBatch));
            });

            Assert.That(result, Is.Zero);
            Assert.That(calls, Is.EqualTo(1));
            Assert.That(input.Reads, Is.EqualTo(expectedReads));
            Assert.That(error.ToString(), Is.Empty);
            Assert.That(output.ToString(), Does.Not.Contain("synthetic-secret"));
            Assert.That(output.ToString().Contains("Enter maximum batch size"), Is.EqualTo(expectedCommand != "I"));
        }

        [TestCase("", "statement code", 1)]
        [TestCase("BPS\n", "batch size", 2)]
        public void Run_InteractiveEndOfInput_FailsPromptlyWithoutExecution(string text, string missing, int expectedReads)
        {
            using var input = new CountingReader(text);
            using var output = new StringWriter();
            using var error = new StringWriter();

            int result = Program.Run(Array.Empty<string>(), false, input, output, error,
                (_, _) => Assert.Fail("EOF must not construct dependencies or start imports."));

            AssertUsageError(result, output.ToString(), error.ToString());
            Assert.That(error.ToString(), Does.Contain("End of input before a " + missing));
            Assert.That(input.Reads, Is.EqualTo(expectedReads));
        }

        [TestCase("I")]
        [TestCase("BPS")]
        public void Run_DependencyOrExecutionFailure_PropagatesWithoutSuccessFallback(string command)
        {
            using var input = new StringReader("");
            using var output = new StringWriter();
            using var error = new StringWriter();
            var failure = new InvalidOperationException("Synthetic failure");
            string[] args = command == "I" ? new[] { command } : new[] { command, "1" };

            Assert.That(Assert.Throws<InvalidOperationException>(() =>
                Program.Run(args, true, input, output, error, (_, _) => throw failure)), Is.SameAs(failure));
            Assert.That(error.ToString(), Is.Empty);
        }

        [TestCase("invalid")]
        [TestCase("zero")]
        [TestCase("redirected-closed")]
        public void Main_ChildProcess_UsageErrorsExitTwoWithoutResolvingServices(string scenario)
        {
            var start = CreateGeneratorStart();
            switch (scenario)
            {
                case "invalid":
                    start.ArgumentList.Add("synthetic-secret");
                    break;
                case "zero":
                    start.ArgumentList.Add("ALL");
                    start.ArgumentList.Add("0");
                    break;
            }
            // A real dependency resolution would fail configuration validation, before any database access.
            start.Environment[ClaimStatementsConnectionString.EnvironmentVariableName] = " ";
            using var process = Process.Start(start);
            var stdout = process.StandardOutput.ReadToEndAsync();
            var stderr = process.StandardError.ReadToEndAsync();
            process.StandardInput.Close();

            if (!process.WaitForExit(10000))
            {
                process.Kill(true);
                process.WaitForExit();
                Assert.Fail("Invalid invocation hung instead of returning a usage error.");
            }

            AssertUsageError(process.ExitCode, stdout.GetAwaiter().GetResult(), stderr.GetAwaiter().GetResult());
            Assert.That(stdout.GetAwaiter().GetResult(), Does.Contain("Claim Statement Generator"));
        }

        [Test]
        public void Main_ChildProcess_MissingConfigurationExitsOneWithTheSettingNamed()
        {
            var start = CreateGeneratorStart();
            start.ArgumentList.Add("I");
            start.Environment.Remove(ClaimStatementsConnectionString.EnvironmentVariableName);
            start.Environment.Remove(FolderService.RootEnvironmentVariableName);
            using var process = Process.Start(start);
            var stdout = process.StandardOutput.ReadToEndAsync();
            var stderr = process.StandardError.ReadToEndAsync();
            process.StandardInput.Close();

            if (!process.WaitForExit(20000))
            {
                process.Kill(true);
                process.WaitForExit();
                Assert.Fail("A run without configuration hung instead of failing.");
            }

            string error = stderr.GetAwaiter().GetResult();
            Assert.That(process.ExitCode, Is.EqualTo(1));
            Assert.That(error, Does.Contain(ClaimStatementsConnectionString.EnvironmentVariableName));
            Assert.That(error, Does.Not.Contain("ResolutionFailedException"));
            Assert.That(stdout.GetAwaiter().GetResult(), Does.Not.Contain("Power Plant activated"));
        }

        private static ProcessStartInfo CreateGeneratorStart()
        {
            var start = new ProcessStartInfo(Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet")
            {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            start.ArgumentList.Add("exec");
            start.ArgumentList.Add("--runtimeconfig");
            start.ArgumentList.Add(Path.ChangeExtension(typeof(ProgramTests).Assembly.Location, ".runtimeconfig.json"));
            start.ArgumentList.Add("--depsfile");
            start.ArgumentList.Add(Path.ChangeExtension(typeof(ProgramTests).Assembly.Location, ".deps.json"));
            start.ArgumentList.Add(typeof(Program).Assembly.Location);
            return start;
        }

        private static void AssertUsageError(int result, string output, string error)
        {
            Assert.That(result, Is.EqualTo(2));
            Assert.That(error, Does.Contain("Error:"));
            Assert.That(error, Does.Contain("Usage:"));
            Assert.That(error, Does.Contain("BPS N | XB N | ALL N | I"));
            Assert.That(error, Does.Contain("positive 32-bit integer"));
            Assert.That(error, Does.Not.Contain("synthetic-secret"));
            Assert.That(output, Does.Not.Contain("synthetic-secret"));
            Assert.That(output, Does.Not.Contain("Generation requested"));
            Assert.That(output, Does.Not.Contain("Power Plant activated"));
        }

        private sealed class CountingReader : StringReader
        {
            public CountingReader(string text) : base(text) { }

            public int Reads { get; private set; }

            public override string ReadLine()
            {
                Reads++;
                return base.ReadLine();
            }
        }
    }
}
