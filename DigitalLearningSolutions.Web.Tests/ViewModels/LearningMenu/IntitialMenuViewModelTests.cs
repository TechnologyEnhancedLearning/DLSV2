namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
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

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.CourseContent.Title.Should().Be($"{applicationName} - {customisationName}");
            initialMenuViewModel.CourseContent.AverageDuration.Should().Be(averageDuration);
            initialMenuViewModel.CourseContent.CentreName.Should().Be(centreName);
            initialMenuViewModel.CourseContent.BannerText.Should().Be(bannerText);
        }
    }
}
