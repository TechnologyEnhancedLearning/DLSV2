namespace DigitalLearningSolutions.Data.Tests.Models
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using FluentAssertions;
    using NUnit.Framework;

    public class EvaluationSummaryModelTests
    {
        [Test]
        public void EvaluationSummaryModel_sets_values_correctly()
        {
            // Given
            const string question = "Increased productivity?";
            var responseCounts = new List<(string, int)> { ("Yes", 10), ("No", 25), ("No response", 15) };

            // When
            var model = new EvaluationSummaryModel(question, responseCounts);

            // Then
            model.Question.Should().Be(question);
            model.ResponseCounts.Should().BeEquivalentTo(responseCounts);
            model.TotalResponses.Should().Be(50);
        }

        [Test]
        public void EvaluationSummaryModel_sets_ResponseCounts_to_null_when_no_responses()
        {
            // Given
            const string question = "Don't answer this question!";
            var responseCounts = new List<(string, int)> { ("Yes", 0), ("No", 0), ("No response", 0) };

            // When
            var model = new EvaluationSummaryModel(question, responseCounts);

            // Then
            model.Question.Should().Be(question);
            model.ResponseCounts.Should().BeNull();
            model.TotalResponses.Should().Be(0);
        }
    }
}
