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
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(customisationId, customisationName, applicationName);

            // When
            var initialMenuViewModel = new InitialMenuViewModel(expectedCourseContent);

            // Then
            initialMenuViewModel.Name.Should().Be($"{applicationName} - {customisationName}");
        }
    }
}
