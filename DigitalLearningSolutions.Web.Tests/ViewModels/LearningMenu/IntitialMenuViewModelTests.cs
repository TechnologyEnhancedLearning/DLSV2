namespace DigitalLearningSolutions.Web.Tests.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FluentAssertions;
    using NUnit.Framework;

    public class InitialMenuViewModelTests
    {
        [Test]
        public void Initial_menu_should_have_id()
        {
            // Given
            var customisationId = 12;

            // When
            var initialMenuViewModel = new InitialMenuViewModel(customisationId);

            // Then
            initialMenuViewModel.CustomisationId.Should().Be(customisationId);
        }
    }
}
