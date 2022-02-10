namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using NUnit.Framework;

    public class LearningLogEntryViewModelTests
    {
        [TestCase(1, "1")]
        [TestCase(null, "N/A")]

        public void LearningLogEntryViewModel_formats_LearningTime_correctly(int? learningTime, string expectedValue)
        {
            // Given
            var learningLogEntry = Builder<LearningLogEntry>.CreateNew()
                .With(e => e.LearningTime = learningTime).Build();

            // When
            var viewModel = new LearningLogEntryViewModel(learningLogEntry);

            // Then
            viewModel.LearningTime.Should().Be(expectedValue);
        }

        [TestCase("Some assessment name", "Some assessment name")]
        [TestCase(null, "N/A")]

        public void LearningLogEntryViewModel_formats_AssessmentTaken_correctly(string? assessmentTaken, string expectedValue)
        {
            // Given
            var learningLogEntry = Builder<LearningLogEntry>.CreateNew()
                .With(e => e.AssessmentTaken = assessmentTaken).Build();

            // When
            var viewModel = new LearningLogEntryViewModel(learningLogEntry);

            // Then
            viewModel.AssessmentTaken.Should().Be(expectedValue);
        }

        [TestCase(1, "1")]
        [TestCase(null, "N/A")]

        public void LearningLogEntryViewModel_formats_AssessmentScore_correctly(int? assessmentScore, string expectedValue)
        {
            // Given
            var learningLogEntry = Builder<LearningLogEntry>.CreateNew()
                .With(e => e.AssessmentScore = assessmentScore).Build();

            // When
            var viewModel = new LearningLogEntryViewModel(learningLogEntry);

            // Then
            viewModel.AssessmentScore.Should().Be(expectedValue);
        }

        [TestCase(true, "Pass")]
        [TestCase(false, "Fail")]
        [TestCase(null, "N/A")]

        public void LearningLogEntryViewModel_formats_AssessmentStatus_correctly(bool? assessmentStatus, string expectedValue)
        {
            // Given
            var learningLogEntry = Builder<LearningLogEntry>.CreateNew()
                .With(e => e.AssessmentStatus = assessmentStatus).Build();

            // When
            var viewModel = new LearningLogEntryViewModel(learningLogEntry);

            // Then
            viewModel.AssessmentStatus.Should().Be(expectedValue);
        }
    }
}
