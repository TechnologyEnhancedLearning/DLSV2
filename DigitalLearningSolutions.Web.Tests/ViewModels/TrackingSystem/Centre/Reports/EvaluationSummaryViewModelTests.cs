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
            var responseCounts = new List<(string, int)> { ("Yes", 4), ("No", 76), ("No response", 80) };
            var expectedPercentages = new List<ResponseViewModel>
            {
                new ResponseViewModel("Yes", 4, "2.5%"),
                new ResponseViewModel("No", 76, "47.5%"),
                new ResponseViewModel("No response", 80, "50.0%")
            };
            var model = new EvaluationResponseBreakdown(question, responseCounts);

            // When
            var viewModel = new EvaluationSummaryViewModel(model);

            // Then
            viewModel.Question.Should().Be(question);
            viewModel.Responses.Should().BeEquivalentTo(expectedPercentages);
        }

        [Test]
        public void EvaluationSummaryViewModel_sets_ResponsePercentages_to_empty_when_no_responses()
        {
            // Given
            const string question = "Don't answer this question!";
            var responseCounts = new List<(string, int)> { ("Yes", 0), ("No", 0), ("No response", 0) };
            var model = new EvaluationResponseBreakdown(question, responseCounts);

            // When
            var viewModel = new EvaluationSummaryViewModel(model);

            // Then
            viewModel.Question.Should().Be(question);
            viewModel.Responses.Should().BeEmpty();
        }
    }
}
