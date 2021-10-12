namespace DigitalLearningSolutions.Data.Tests.Models
{
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using FluentAssertions;
    using NUnit.Framework;

    public class EvaluationResponsesTests
    {
        [Test]
        public void EvaluationResponses_sets_expected_values()
        {
            // Given
            const string question = "Test?";

            // When
            var result = new EvaluationResponses(question, 20, 100);

            // Then
            result.Count.Should().Be(20);
            result.Response.Should().Be(question);
            result.Percentage.Should().Be((float)0.2);
        }
    }
}
