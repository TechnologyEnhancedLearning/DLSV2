namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Centre.Reports
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports;
    using FluentAssertions;
    using NUnit.Framework;

    public class EvaluationSummaryViewModelTests
    {
        [Test]
        public void EvaluationSummaryViewModel_sets_percentage_values_correctly()
        {
            // Given
            var expectedData = new Dictionary<string, float>
            {
                { "Yes", 20f }, { "No", 50f }, { "No response", 30f }
            };

            // When
            var model = new EvaluationSummaryViewModel(
                "Increased productivity?",
                new Dictionary<string, int>
                    { { "Yes", 10 }, { "No", 25 }, { "No response", 15 } },
                15
            );

            // Then
            model.Question.Should().Be("Increased productivity?");
            model.Data.Should().BeEquivalentTo(expectedData);
        }

        [Test]
        public void EvaluationSummaryViewModel_sets_data_to_null_when_no_responses_at_all()
        {
            // When
            var model = new EvaluationSummaryViewModel(
                "This question has never been seen!",
                new Dictionary<string, int>
                    { { "Yes", 0 }, { "No", 0 }, { "No response", 0 } },
                0
            );

            // Then
            model.Question.Should().Be("This question has never been seen!");
            model.Data.Should().BeNull();
        }

        [Test]
        public void EvaluationSummaryViewModel_sets_data_to_null_when_all_responses_are_noResponse()
        {
            // When
            var model = new EvaluationSummaryViewModel(
                "Don't answer this question!",
                new Dictionary<string, int>
                    { { "Yes", 0 }, { "No", 0 }, { "No response", 25 } },
                25
            );

            // Then
            model.Question.Should().Be("Don't answer this question!");
            model.Data.Should().BeNull();
        }
    }
}
