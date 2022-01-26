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
                // Given
                Driver.LogUserInAsAdminAndDelegate(BaseUrl);
                const string startUrl = "/LearningPortal/Current";
                const string completedUrl = "/LearningPortal/Completed";
                const string availableUrl = "/LearningPortal/Available";
                const string completeByUrl = "/LearningPortal/Current/CompleteBy/19262";

                // When
                Driver.Navigate().GoToUrl(BaseUrl + startUrl);
                ValidatePageHeading("My Current Activities");
                var currentActivitiesResult = new AxeBuilder(Driver).Analyze();

                Driver.Navigate().GoToUrl(BaseUrl + completedUrl);
                ValidatePageHeading("My Completed Activities");
                var completedActivitiesResult = new AxeBuilder(Driver).Analyze();

                Driver.Navigate().GoToUrl(BaseUrl + availableUrl);
                ValidatePageHeading("Available Activities");
                var availableActivitiesResult = new AxeBuilder(Driver).Analyze();

                Driver.Navigate().GoToUrl(BaseUrl + completeByUrl);
                ValidatePageHeading("Enter a complete by date for Excel 2013 for the Workplace - Testing");
                var completeByResult = new AxeBuilder(Driver).Analyze();

                //Then
                currentActivitiesResult.Violations.Should().BeEmpty();
                completedActivitiesResult.Violations.Should().BeEmpty();
                availableActivitiesResult.Violations.Should().BeEmpty();
                completeByResult.Violations.Should().BeEmpty();
            }
        }
    }
}
