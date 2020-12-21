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
            initialMenuViewModel.Title.Should().Be($"{applicationName} - {customisationName}");
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

        [Test]
        public void Initial_menu_should_have_averageDuration_for_0_minutes()
        {
            // Given
            const int averageDuration = 0;
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(
                averageDuration: averageDuration
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.AverageDuration.Should().Be(" 0 minutes");
        }

        [Test]
        public void Initial_menu_should_have_averageDuration_for_1_minute()
        {
            // Given
            const int averageDuration = 1;
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(
                averageDuration: averageDuration
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.AverageDuration.Should().Be(" 1 minute");
        }

        [Test]
        public void Initial_menu_should_have_averageDuration_for_under_an_hour()
        {
            // Given
            const int averageDuration = 30;
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(
                averageDuration: averageDuration
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.AverageDuration.Should().Be(" 30 minutes");
        }

        [Test]
        public void Initial_menu_should_have_averageDuration_for_whole_number_of_hours()
        {
            // Given
            const int averageDuration = 120;
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(
                averageDuration: averageDuration
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.AverageDuration.Should().Be(" 2 hours");
        }

        [Test]
        public void Initial_menu_should_have_averageDuration_for_one_hour_one_minute()
        {
            // Given
            const int averageDuration = 61;
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(
                averageDuration: averageDuration
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.AverageDuration.Should().Be(" 1 hour 1 minute");
        }


        [Test]
        public void Initial_menu_should_have_averageDuration_for_multiple_hours()
        {
            // Given
            const int averageDuration = 195;
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(
                averageDuration: averageDuration
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.AverageDuration.Should().Be(" 3 hours 15 minutes");
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
        public void Initial_menu_show_completion_summary_should_be_include_certification_when_true()
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
        public void Initial_menu_show_completion_summary_should_be_include_certification_when_false()
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
            var courseContent = CourseContentHelper.CreateDefaultCourseContent(customisationId: customisationId);
            var section = CourseSectionHelper.CreateDefaultCourseSection(sectionName: sectionName, hasLearning: hasLearning, percentComplete: percentComplete);
            courseContent.Sections.Add(section);
            var expectedSection = new SectionCardViewModel(section, customisationId);
            var expectedSectionList = new List<SectionCardViewModel>
            {
                expectedSection
            };

            // When
            var initialMenuViewModel = new InitialMenuViewModel(courseContent);

            // Then
            initialMenuViewModel.Sections.Should().BeEquivalentTo(expectedSectionList);
        }

        [Test]
        public void Get_completion_status_for_completed_should_return_complete()
        {
            // Given
            var courseContent = CourseContentHelper.CreateDefaultCourseContent(
                completed: DateTime.Now
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(courseContent);

            // Then
            initialMenuViewModel.CompletionStatus.Should().Be("Complete");
        }

        [Test]
        public void Get_completion_status_for_null_completed_should_return_incomplete()
        {
            // Given
            var courseContent = CourseContentHelper.CreateDefaultCourseContent(
                completed: null
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(courseContent);

            // Then
            initialMenuViewModel.CompletionStatus.Should().Be("Incomplete");
        }

        [Test]
        public void Get_completion_styling_for_completed_should_return_complete()
        {
            // Given
            var courseContent = CourseContentHelper.CreateDefaultCourseContent(
                completed: DateTime.Now
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(courseContent);

            // Then
            initialMenuViewModel.CompletionStyling.Should().Be("complete");
        }

        [Test]
        public void Get_completion_styling_for_null_completed_should_return_incomplete()
        {
            // Given
            var courseContent = CourseContentHelper.CreateDefaultCourseContent(
                completed: null
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(courseContent);

            // Then
            initialMenuViewModel.CompletionStyling.Should().Be("incomplete");
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
        public void TutorialVideo_should_parse_path(
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
            var courseContent = CourseContentHelper.CreateDefaultCourseContent(
                completed: completed != null ? DateTime.Parse(completed) : (DateTime?) null,
                maxPostLearningAssessmentAttempts: maxPostLearningAssessmentAttempts,
                isAssessed: isAssessed,
                postLearningAssessmentPassThreshold: postLearningAssessmentPassThreshold,
                diagnosticAssessmentCompletionThreshold: diagnosticAssessmentCompletionThreshold,
                tutorialsCompletionThreshold: tutorialsCompletionThreshold
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(courseContent);

            // Then
            initialMenuViewModel.CompletionSummary.Should().Be(expectedSummary);
        }
    }
}
