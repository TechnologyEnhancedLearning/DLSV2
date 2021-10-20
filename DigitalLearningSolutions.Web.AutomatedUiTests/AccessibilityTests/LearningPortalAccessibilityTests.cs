namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestFixtures;
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers;
    using FluentAssertions;
    using Selenium.Axe;
    using Xunit;
    public class LearningPortalAccessibilityTests : AccessibilityTestsBase,
        IClassFixture<AccessibilityTestsFixture<Startup>>
    {
        public LearningPortalAccessibilityTests(AccessibilityTestsFixture<Startup> fixture) : base(fixture) { }

        [Fact]
        public void LearningPortal_interface_has_no_accessibility_errors()
        {
            {
                Driver.LogUserInAsAdminAndDelegate(BaseUrl);
                const string startUrl = "/LearningPortal/Current";

                // When
                Driver.Navigate().GoToUrl(BaseUrl + startUrl);
                ValidatePageHeading("My Current Activities");
                var currentActivitiesResult = new AxeBuilder(Driver).Analyze();

                Driver.ClickLinkContainingText("Completed activities");
                ValidatePageHeading("My Completed Activities");
                var completedActivitiesResult = new AxeBuilder(Driver).Analyze();

                Driver.ClickLinkContainingText("Available activities");
                ValidatePageHeading("Available Activities");
                var availableActivitiesResult = new AxeBuilder(Driver).Analyze();

                currentActivitiesResult.Violations.Should().BeEmpty();
                completedActivitiesResult.Violations.Should().BeEmpty();
                availableActivitiesResult.Violations.Should().BeEmpty();
            }
        }
    }
}
