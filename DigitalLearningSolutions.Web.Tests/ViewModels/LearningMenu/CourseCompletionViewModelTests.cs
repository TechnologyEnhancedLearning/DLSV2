namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using System;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class CourseCompletionViewModelTests
    {
        private IConfiguration config = null!;

        private const string BaseUrl = "https://example.com";
        private const int ProgressId = 23;

        [SetUp]
        public void SetUp()
        {
            config = A.Fake<IConfiguration>();
            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns(BaseUrl);
        }

        [Test]
        public void CourseCompletion_should_have_customisationId()
        {
            // Given
            const int customisationId = 1234;
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(customisationId);

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.CustomisationId.Should().Be(customisationId);
        }

        [Test]
        public void CourseCompletion_should_have_courseTitle()
        {
            // Given
            const string applicationName = "Application Name";
            const string customisationName = "Customisation Name";
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(
                applicationName: applicationName,
                customisationName: customisationName
            );

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            var courseTitle = $"{applicationName} - {customisationName}";
            courseCompletionViewModel.CourseTitle.Should().BeEquivalentTo(courseTitle);
        }

        [Test]
        public void CourseCompletion_should_have_isAssessed()
        {
            // Given
            const bool isAssessed = true;
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(
                isAssessed: isAssessed
            );

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.IsAssessed.Should().Be(isAssessed);
        }

        [Test]
        public void CourseCompletion_should_have_diagnosticScore()
        {
            // Given
            const int diagnosticScore = 42;
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(
                diagnosticScore: diagnosticScore
            );

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.DiagnosticScore.Should().Be(diagnosticScore);
        }

        [Test]
        public void CourseCompletion_should_round_percentageTutorialsCompleted_down()
        {
            // Given
            const double percentageTutorialsCompleted = 44.4;
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(
                percentageTutorialsCompleted: percentageTutorialsCompleted
            );

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.PercentageTutorialsCompleted.Should().Be(44);
        }

        [Test]
        public void CourseCompletion_should_floor_round_percentageTutorialsCompleted()
        {
            // Given
            const double percentageTutorialsCompleted = 66.6;
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(
                percentageTutorialsCompleted: percentageTutorialsCompleted
            );

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.PercentageTutorialsCompleted.Should().Be(66);
        }

        [Test]
        public void CourseCompletion_should_not_show_DiagnosticScore_when_null()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(
                diagnosticScore: null
            );

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.ShowDiagnosticScore.Should().BeFalse();
        }

        [Test]
        public void CourseCompletion_should_not_show_DiagnosticScore_when_attempts_are_0()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(
                diagnosticScore: 10,
                diagnosticAttempts: 0
            );

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.ShowDiagnosticScore.Should().BeFalse();
        }

        [Test]
        public void CourseCompletion_should_show_DiagnosticScore_when_attempts_are_not_0()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(
                diagnosticScore: 10,
                diagnosticAttempts: 1
            );

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.ShowDiagnosticScore.Should().BeTrue();
        }

        [Test]
        public void CourseCompletion_should_not_show_PercentageTutorialsCompleted_when_0()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(
                percentageTutorialsCompleted: 0
            );

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.ShowPercentageTutorialsCompleted.Should().BeFalse();
        }

        [Test]
        public void CourseCompletion_should_show_PercentageTutorialsCompleted_when_not_0()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(
                percentageTutorialsCompleted: 10
            );

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.ShowPercentageTutorialsCompleted.Should().BeTrue();
        }

        [Test]
        public void CourseCompletion_should_have_postLearningPasses()
        {
            // Given
            const int postLearningPasses = 61;
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(
                postLearningPasses: postLearningPasses
            );

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.PostLearningPasses.Should().Be(postLearningPasses);
        }

        [Test]
        public void CourseCompletion_should_have_sectionCount()
        {
            // Given
            const int sectionCount = 62;
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(
                sectionCount: sectionCount
            );

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.SectionCount.Should().Be(sectionCount);
        }

        [Test]
        public void CourseCompletion_should_have_complete_status_for_nonnull_completed()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(
                completed: DateTime.UtcNow
            );

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.CompletionStatus.Should().Be("complete");
        }

        [Test]
        public void CourseCompletion_should_have_incomplete_status_for_null_completed()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(completed: null);

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.CompletionStatus.Should().Be("incomplete");
        }

        [Test]
        public void CourseCompletion_should_have_DownloadSummaryUrl()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion();
            var expectedDownloadSummaryUrl = $"{BaseUrl}/tracking/summary?ProgressID={ProgressId}";

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.DownloadSummaryUrl.Should().Be(expectedDownloadSummaryUrl);
        }

        [Test]
        public void CourseCompletion_should_have_FinaliseUrl()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion();
            var expectedFinaliseUrl = $"{BaseUrl}/tracking/finalise?ProgressID={ProgressId}";

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.FinaliseUrl.Should().Be(expectedFinaliseUrl);
        }

        [Test]
        public void CourseCompletion_FinaliseText_should_be_null_when_completed_is_null()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(completed: null);

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.FinaliseText.Should().BeNull();
        }

        [Test]
        public void CourseCompletion_FinaliseAriaLabel_should_be_null_when_completed_is_null()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(completed: null);

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.FinaliseAriaLabel.Should().BeNull();
        }

        [Test]
        public void CourseCompletion_FinaliseText_should_be_certificate_when_course_is_assessed_and_evaluated()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(
                completed: DateTime.UtcNow.AddDays(-1),
                evaluated: DateTime.UtcNow,
                isAssessed: true
            );

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.FinaliseText.Should().Be("Certificate");
        }

        [Test]
        public void CourseCompletion_FinaliseAriaLabel_should_be_certificate_when_course_is_assessed_and_evaluated()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(
                completed: DateTime.UtcNow.AddDays(-1),
                evaluated: DateTime.UtcNow,
                isAssessed: true
            );

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.FinaliseAriaLabel.Should().Be("View or print certificate");
        }

        [Test]
        public void CourseCompletion_FinaliseText_should_be_evaluate_when_course_is_assessed_and_not_evaluated()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(
                completed: DateTime.UtcNow,
                evaluated: null,
                isAssessed: true
            );

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.FinaliseText.Should().Be("Evaluate");
        }

        [Test]
        public void CourseCompletion_FinaliseAriaLabel_should_be_evaluate_when_course_is_assessed_and_not_evaluated()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(
                completed: DateTime.UtcNow,
                evaluated: null,
                isAssessed: true
            );

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.FinaliseAriaLabel.Should().Be("Evaluate course");
        }

        [Test]
        public void CourseCompletion_FinaliseText_should_be_evaluate_when_course_is_not_assessed_and_not_evaluated()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(
                completed: DateTime.UtcNow,
                evaluated: null,
                isAssessed: false
            );

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.FinaliseText.Should().Be("Evaluate");
        }

        [Test]
        public void CourseCompletion_FinaliseAriaLabel_should_be_evaluate_when_course_is_not_assessed_and_not_evaluated()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(
                completed: DateTime.UtcNow,
                evaluated: null,
                isAssessed: false
            );

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.FinaliseAriaLabel.Should().Be("Evaluate course");
        }

        [Test]
        public void CourseCompletion_FinaliseText_should_be_null_when_course_is_not_assessed_and_already_evaluated()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(
                completed: DateTime.UtcNow.AddDays(-1),
                evaluated: DateTime.UtcNow,
                isAssessed: false
            );

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.FinaliseText.Should().BeNull();
        }

        [Test]
        public void CourseCompletion_FinaliseAriaLabel_should_be_null_when_course_is_not_assessed_and_already_evaluated()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(
                completed: DateTime.UtcNow.AddDays(-1),
                evaluated: DateTime.UtcNow,
                isAssessed: false
            );

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.FinaliseAriaLabel.Should().BeNull();
        }

        [TestCase(
            "2020-12-25T15:00:00Z",
            1,
            true,
            75,
            80,
            85,
            "You completed this course on 25 December 2020."
        )]
        [TestCase(
            null,
            0,
            true,
            75,
            80,
            85,
            "To complete this course, you must pass all post learning assessments with a score of 75% or higher."
        )]
        [TestCase(
            null,
            3,
            true,
            75,
            80,
            85,
            "To complete this course, you must pass all post learning assessments with a score of 75% or higher. Failing an assessment 3 times will lock your progress."
        )]
        [TestCase(
            null,
            3,
            false,
            75,
            80,
            85,
            "To complete this course, you must achieve 80% in the diagnostic assessment and complete 85% of the learning material."
        )]
        [TestCase(
            null,
            3,
            false,
            75,
            80,
            0,
            "To complete this course, you must achieve 80% in the diagnostic assessment."
        )]
        [TestCase(
            null,
            3,
            false,
            75,
            0,
            85,
            "To complete this course, you must complete 85% of the learning material."
        )]
        [TestCase(
            null,
            3,
            false,
            75,
            0,
            0,
            "There are no requirements to complete this course."
        )]
        public void CourseCompletion_should_have_formatted_completion_summary(
            string? completed,
            int maxPostLearningAssessmentAttempts,
            bool isAssessed,
            int postLearningAssessmentPassThreshold,
            int diagnosticAssessmentCompletionThreshold,
            int tutorialsCompletionThreshold,
            string expectedSummary
        )
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(
                completed: completed != null ? DateTime.Parse(completed) : (DateTime?)null,
                maxPostLearningAssessmentAttempts: maxPostLearningAssessmentAttempts,
                isAssessed: isAssessed,
                postLearningAssessmentPassThreshold: postLearningAssessmentPassThreshold,
                diagnosticAssessmentCompletionThreshold: diagnosticAssessmentCompletionThreshold,
                tutorialsCompletionThreshold: tutorialsCompletionThreshold
            );

            // When
            var courseCompletionViewModel = new CourseCompletionViewModel(config, expectedCourseCompletion, ProgressId);

            // Then
            courseCompletionViewModel.SummaryText.Should().Be(expectedSummary);
        }
    }
}
