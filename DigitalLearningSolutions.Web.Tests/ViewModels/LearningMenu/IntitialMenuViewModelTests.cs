namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CourseContent;
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
            initialMenuViewModel.Id.Should().Be(12);
        }

        [Test]
        public void Initial_menu_should_have_averageDuration()
        {
            // Given
            const int customisationId = 12;
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(
                customisationId: customisationId
            );

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.Id.Should().Be(12);
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
        public void Initial_menu_should_have_list_of_section_card_view_model()
        {
            // Given
            const string sectionName = "TestSectionName";
            const bool hasLearning = true;
            const double percentComplete = 12.00;
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent();
            var section = CourseSectionHelper.CreateDefaultCourseSection(sectionName, hasLearning, percentComplete);
            var expectedSection = new SectionCardViewModel(section);
            expectedCourseContent.Sections.Add(section);

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.Sections.First().Should().BeEquivalentTo(expectedSection);
        }
    }
}
