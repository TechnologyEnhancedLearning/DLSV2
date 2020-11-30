namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.CourseContent;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FluentAssertions;
    using NUnit.Framework;

    public class InitialMenuViewModelTests
    {
        [Test]
        public void Initial_menu_should_have_values_for_all_courseContent_fields()
        {
            // Given
            const int customisationId = 12;
            const string customisationName = "Custom";
            const string applicationName = "My course";
            const string averageDuration = "5h 33m";
            const string centreName = "Central";
            const string bannerText = "Centre Banner";
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(
                customisationId,
                customisationName,
                applicationName,
                averageDuration,
                centreName,
                bannerText
            );
            const string sectionName = "TestName";
            const bool hasLearning = true;
            const double percentComplete = 12.00;
            var section = new CourseSection(sectionName, hasLearning, percentComplete);
            expectedCourseContent.Sections.Add(section);

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.Title.Should().Be($"{applicationName} - {customisationName}");
            initialMenuViewModel.Id.Should().Be(customisationId);
            initialMenuViewModel.AverageDuration.Should().Be(averageDuration);
            initialMenuViewModel.CentreName.Should().Be(centreName);
            initialMenuViewModel.BannerText.Should().Be(bannerText);
        }
    }
}
