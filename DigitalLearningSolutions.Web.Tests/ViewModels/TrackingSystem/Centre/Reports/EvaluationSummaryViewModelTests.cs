namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Centre.Reports
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports;
    using FluentAssertions;
    using NUnit.Framework;

    public class EvaluationSummaryViewModelTests
    {
        [Test]
        public void EvaluationSummaryViewModel_sets_percentage_values_correctly()
        {
            // Given
            const string question = "Increased productivity?";
            var responseCounts = new List<(string, int)> { ("Yes", 10), ("No", 25), ("No response", 15) };
            var expectedPercentages = new List<(string, float)> { ("Yes", 20f), ("No", 50f), ("No response", 30f) };
            var model = new EvaluationSummaryModel(question, responseCounts);

            // When
            var viewModel = new EvaluationSummaryViewModel(model);

            // Then
            viewModel.Question.Should().Be(question);
            viewModel.ResponsePercentages.Should().BeEquivalentTo(expectedPercentages);
        }

        [Test]
        public void EvaluationSummaryViewModel_sets_ResponsePercentages_to_null_when_no_responses()
        {
            // Given
            const string question = "Don't answer this question!";
            var responseCounts = new List<(string, int)> { ("Yes", 0), ("No", 0), ("No response", 0) };
            var model = new EvaluationSummaryModel(question, responseCounts);

            // When
            var viewModel = new EvaluationSummaryViewModel(model);

            // Then
            viewModel.Question.Should().Be(question);
            viewModel.ResponsePercentages.Should().BeNull();
        }
    }
}
