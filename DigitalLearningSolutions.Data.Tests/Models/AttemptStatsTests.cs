namespace DigitalLearningSolutions.Data.Tests.Models
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using FluentAssertions;
    using NUnit.Framework;

    public class AttemptStatsTests
    {
        [Test]
        [TestCase(0, 0, 0)]
        [TestCase(5, 5, 100)]
        [TestCase(10, 5, 50)]
        [TestCase(1000, 994, 99)]
        [TestCase(1000, 995, 100)]
        public void AttemptStats_calculates_pass_rate_correctly(
            int attemptsMade,
            int attemptsPassed,
            double expectedPassRate
        )
        {
            // Given
            var stats = new AttemptStats(attemptsMade, attemptsPassed);

            // Then
            stats.TotalAttempts.Should().Be(attemptsMade);
            stats.AttemptsPassed.Should().Be(attemptsPassed);
            stats.PassRate.Should().Be(expectedPassRate);
        }
    }
}
