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
                { "Yes", (float)100 * 107 / 216 }, { "No", (float)100 * 102 / 216 },
                { "No response", (float)100 * 7 / 216 }
            };

            // When
            var model = new EvaluationSummaryViewModel(
                "Increased productivity?",
                new Dictionary<string, int>
                    { { "Yes", 107 }, { "No", 102 }, { "No response", 7 } }
            );

            // Then
            model.Question.Should().Be("Increased productivity?");
            model.Data.Should().BeEquivalentTo(expectedData);
        }

        [Test]
        public void EvaluationSummaryViewModel_sets_data_to_null_when_no_responses()
        {
            // When
            var model = new EvaluationSummaryViewModel(
                "Don't answer this question!",
                new Dictionary<string, int>
                    { { "Yes", 0 }, { "No", 0 }, { "No response", 0 } }
            );

            // Then
            model.Question.Should().Be("Don't answer this question!");
            model.Data.Should().BeNull();
        }
    }
}
