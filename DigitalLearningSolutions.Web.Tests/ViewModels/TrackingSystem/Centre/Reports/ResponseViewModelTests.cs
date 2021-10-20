namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Centre.Reports
{
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports;
    using FluentAssertions;
    using NUnit.Framework;

    public class ResponseViewModelTests
    {
        [Test]
        public void EvaluationResponses_sets_expected_values()
        {
            // Given
            const string question = "Test?";
            var evaluationResponse = new EvaluationResponses(question, 20, 100);

            // When
            var result = new ResponseViewModel(evaluationResponse);

            // Then
            result.Count.Should().Be(20);
            result.Response.Should().Be(question);
            result.Percentage.Should().Be("20.0%");
        }

        [Test]
        public void EvaluationResponses_rounds_percentages_as_expected()
        {
            // Given
            const string question = "Test?";
            var evaluationResponse = new EvaluationResponses(question, 20, 66);

            // When
            var result = new ResponseViewModel(evaluationResponse);

            // Then
            result.Count.Should().Be(20);
            result.Response.Should().Be(question);
            result.Percentage.Should().Be("30.3%");
        }
    }
}
