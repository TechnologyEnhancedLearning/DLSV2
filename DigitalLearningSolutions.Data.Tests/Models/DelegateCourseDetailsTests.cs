namespace DigitalLearningSolutions.Data.Tests.Models
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using FluentAssertions;
    using NUnit.Framework;

    public class DelegateCourseDetailsTests
    {
        [Test]
        [TestCase(0, 0, 0)]
        [TestCase(5, 5, 100)]
        [TestCase(10, 5, 50)]
        [TestCase(1000, 994, 99)]
        [TestCase(1000, 995, 100)]
        public void ViewModel_calculates_pass_rate_correctly(
            int attemptsMade,
            int attemptsPassed,
            double expectedPassRate
        )
        {
            // Given
            var details = new DelegateCourseDetails(
                new DelegateCourseInfo(),
                new List<CustomPromptWithAnswer>(),
                (attemptsMade, attemptsPassed)
            );

            // Then
            details.AttemptStats.Should().Be((attemptsMade, attemptsPassed, expectedPassRate));
        }
    }
}
