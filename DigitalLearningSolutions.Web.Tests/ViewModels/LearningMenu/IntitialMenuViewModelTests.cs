namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FluentAssertions;
    using NUnit.Framework;

    public class InitialMenuViewModelTests
    {
        [Test]
        public void Initial_menu_should_have_name()
        {
            // Given
            const string customisationName = "Custom";
            const string applicationName = "My course";
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(
                customisationName: customisationName,
                applicationName: applicationName
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.Title.Should().Be(!String.IsNullOrEmpty(customisationName) ? $"{applicationName} - {customisationName}" : applicationName);
        }

        [Test]
        public void Initial_menu_should_have_id()
        {
            // Given
            const int customisationId = 12;
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(
                customisationId: customisationId
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.Id.Should().Be(customisationId);
        }

        [TestCase(0, "0 minutes")]
        [TestCase(1, "1 minute")]
        [TestCase(30, "30 minutes")]
        [TestCase(120, "2 hours")]
        [TestCase(61, "1 hour 1 minute")]
        [TestCase(195, "3 hours 15 minutes")]
        [TestCase(null, null)]
        public void Initial_menu_formats_average_duration(int? averageDuration, string? expectedFormattedTime)
        {
            // Given
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(averageDuration: averageDuration);

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.AverageDuration.Should().Be(expectedFormattedTime);
        }

        [Test]
        public void Initial_menu_should_have_centre_name()
        {
            // Given
            const string centreName = "CentreName";
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(
                centreName: centreName
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.CentreName.Should().Be(centreName);
        }

        [Test]
        public void Initial_menu_should_have_true_show_time()
        {
            // Given
            const string courseSettings = "{\"lm.st\":true}"; // ShowTime = true
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(
                courseSettings: courseSettings
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.ShowTime.Should().BeTrue();
        }

        [Test]
        public void Initial_menu_should_have_false_show_time_if_setting_is_false()
        {
            // Given
            const string courseSettings = "{\"lm.st\":false}"; // ShowTime = false
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(
                courseSettings: courseSettings
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.ShowTime.Should().BeFalse();
        }

        [Test]
        public void Initial_menu_should_have_false_show_time_if_time_is_null()
        {
            // Given
            const string courseSettings = "{\"lm.st\":true}"; // ShowTime = true
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(
                averageDuration: null,
                courseSettings: courseSettings
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.ShowTime.Should().BeFalse();
        }

        [Test]
        public void Initial_menu_can_have_banner_text()
        {
            // Given
            const string bannerText = "BannerText";
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(
                bannerText: bannerText
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.BannerText.Should().Be(bannerText);
        }

        [Test]
        public void Initial_menu_banner_text_can_be_null()
        {
            // Given
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(
                bannerText: null
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.BannerText.Should().BeNullOrEmpty();
        }

        [Test]
        public void Initial_menu_should_show_completion_summary_when_include_certification_is_true()
        {
            // Given
            const bool includeCertification = true;
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(
                includeCertification: includeCertification
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.ShouldShowCompletionSummary.Should().Be(includeCertification);
        }

        [Test]
        public void Initial_menu_should_not_show_completion_summary_when_include_certification_is_false()
        {
            // Given
            const bool includeCertification = false;
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(
                includeCertification: includeCertification
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.ShouldShowCompletionSummary.Should().Be(includeCertification);
        }

        [Test]
        public void Initial_menu_should_have_list_of_section_card_view_model()
        {
            // Given
            const string sectionName = "TestSectionName";
            const bool hasLearning = true;
            const double percentComplete = 12.00;
            const int customisationId = 1;
            const bool showPercentage = true;
            const string courseSettings = "{\"lm.sp\":true}"; // ShowPercentage = true

            var courseContent = CourseContentHelper.CreateDefaultCourseContent(
                customisationId: customisationId,
                courseSettings: courseSettings
            );
            var section = CourseSectionHelper.CreateDefaultCourseSection(
                sectionName: sectionName,
                hasLearning: hasLearning,
                percentComplete: percentComplete
            );
            courseContent.Sections.Add(section);
            var expectedSection = new SectionCardViewModel(section, customisationId, showPercentage);
            var expectedSectionList = new List<SectionCardViewModel>
            {
                expectedSection
            };

            // When
            var initialMenuViewModel = new InitialMenuViewModel(courseContent);

            // Then
            initialMenuViewModel.Sections.Should().BeEquivalentTo(expectedSectionList);
        }

        [TestCase(2, "2020-12-25T15:00:00Z", 1, true, 75, 80, 85)]
        [TestCase(3, null, 0, true, 75, 80, 85)]
        [TestCase(4, null, 3, true, 75, 80, 85)]
        [TestCase(5, null, 3, false, 75, 80, 85)]
        [TestCase(6, null, 3, false, 75, 80, 0)]
        [TestCase(7, null, 3, false, 75, 0, 85)]
        [TestCase(8, null, 3, false, 75, 0, 0)]
        public void Initial_menu_should_have_completion_summary_card_view_model(
            int customisationId,
            string? completed,
            int maxPostLearningAssessmentAttempts,
            bool isAssessed,
            int postLearningAssessmentPassThreshold,
            int diagnosticAssessmentCompletionThreshold,
            int tutorialsCompletionThreshold
        )
        {
            // Given
            var completedDateTime = completed != null ? DateTime.Parse(completed) : (DateTime?)null;

            var courseContent = CourseContentHelper.CreateDefaultCourseContent(
                customisationId: customisationId,
                completed: completedDateTime,
                maxPostLearningAssessmentAttempts: maxPostLearningAssessmentAttempts,
                isAssessed: isAssessed,
                postLearningAssessmentPassThreshold: postLearningAssessmentPassThreshold,
                diagnosticAssessmentCompletionThreshold: diagnosticAssessmentCompletionThreshold,
                tutorialsCompletionThreshold: tutorialsCompletionThreshold
            );

            var expectedCompletionSummaryViewModel = new CompletionSummaryCardViewModel(
                customisationId,
                completedDateTime,
                maxPostLearningAssessmentAttempts,
                isAssessed,
                postLearningAssessmentPassThreshold,
                diagnosticAssessmentCompletionThreshold,
                tutorialsCompletionThreshold
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(courseContent);

            // Then
            initialMenuViewModel.CompletionSummaryCardViewModel
                .Should().BeEquivalentTo(expectedCompletionSummaryViewModel);
        }
    }
}
