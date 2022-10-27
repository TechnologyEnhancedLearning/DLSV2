namespace DigitalLearningSolutions.Data.Tests.Utilities
{
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class ClockUtilityTests
    {
        private readonly string repoRootDir = RunCommandAndReturnOutput(
            "/C git rev-parse --show-toplevel",
            Directory.GetCurrentDirectory()
        ).Trim();

        private readonly string[] directoriesToTest =
        {
            "DigitalLearningSolutions.Data",
            "DigitalLearningSolutions.Data.Migrations",
            "DigitalLearningSolutions.Web",
        };

        [Test]
        [TestCase("DateTime.UtcNow", "ClockUtility.UtcNow")]
        [TestCase("DateTime.Now", "ClockUtility.UtcNow")]
        [TestCase("DateTime.Today", "ClockUtility.UtcToday")]
        public void ClockUtility_should_be_used_instead_of_DateTime_functions(string disallowed, string useInstead)
        {
            var filenames = RunCommandAndReturnOutput(
                $"/C git grep -l {disallowed} {string.Join(" ", directoriesToTest)}",
                repoRootDir
            ).Trim().Split("\n");


            //TODO: Currently fails on:
            //DigitalLearningSolutions.Web/Controllers/TrackingSystem/Delegates/EnrolController.cs
            ////DigitalLearningSolutions.Web/Views/TrackingSystem/Delegates/Enrol/EnrolCompleteBy.cshtml


            using (var _ = new AssertionScope())
            {
                filenames
                    .Where(
                        filename =>
                            filename != string.Empty &&
                            filename != "DigitalLearningSolutions.Data/Utilities/ClockUtility.cs" &&
                            filename != "DigitalLearningSolutions.Data.Tests/Utilities/ClockUtilityTests.cs"
                    )
                    .Should().BeEmpty($"Use {useInstead} instead");
            }
        }

        private static string RunCommandAndReturnOutput(string args, string workingDirectory)
        {
            var process = new System.Diagnostics.Process();

            process.StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                Arguments = args,
                WorkingDirectory = workingDirectory,
            };

            process.Start();

            var output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            return output;
        }
    }
}
