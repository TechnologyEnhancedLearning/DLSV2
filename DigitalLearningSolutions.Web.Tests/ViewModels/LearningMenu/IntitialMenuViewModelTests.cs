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
        public void Initial_menu_should_have_averageDuration()
        {
            // Given
            const string averageDuration = "3h 20m";
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(
                averageDuration: averageDuration
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.AverageDuration.Should().Be(averageDuration);
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
        public void Initial_menu_include_certification_can_be_true()
        {
            // Given
            const bool includeCertification = true;
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(
                includeCertification: includeCertification
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.IncludeCertification.Should().Be(includeCertification);
        }

        [Test]
        public void Initial_menu_include_certification_can_be_false()
        {
            // Given
            const bool includeCertification = false;
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(
                includeCertification: includeCertification
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.IncludeCertification.Should().Be(includeCertification);
        }

        [Test]
        public void Initial_menu_can_have_completed()
        {
            // Given
            var completed = DateTime.Today;
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(
                completed: completed
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.Completed.Should().Be(completed);
        }

        [Test]
        public void Initial_menu_completed_can_be_null()
        {
            // Given
            var completed = DateTime.Today;
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(
                completed: null
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.Completed.Should().BeNull();
        }

        [Test]
        public void Initial_menu_should_have_list_of_section_card_view_model()
        {
            // Given
            const string sectionName = "TestSectionName";
            const bool hasLearning = true;
            const double percentComplete = 12.00;
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent();
            var section = CourseSectionHelper.CreateDefaultCourseSection(sectionName, hasLearning, percentComplete);
            expectedCourseContent.Sections.Add(section);
            var expectedSection = new SectionCardViewModel(section);
            var expectedSectionList = new List<SectionCardViewModel>
            {
                expectedSection
            };

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

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
            initialMenuViewModel.GetCompletionStatus().Should().Be("Complete");
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
            initialMenuViewModel.GetCompletionStatus().Should().Be("Incomplete");
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
            initialMenuViewModel.GetCompletionStyling().Should().Be("complete");
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
            initialMenuViewModel.GetCompletionStyling().Should().Be("incomplete");
        }
    }
}
