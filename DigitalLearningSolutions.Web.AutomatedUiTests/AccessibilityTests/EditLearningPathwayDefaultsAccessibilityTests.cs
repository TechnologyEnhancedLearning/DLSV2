namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestFixtures;
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers;
    using FluentAssertions;
    using Selenium.Axe;
    using Xunit;

    public class EditLearningPathwayDefaultsAccessibilityTests : AccessibilityTestsBase,
        IClassFixture<AccessibilityTestsFixture<Startup>>
    {
        public EditLearningPathwayDefaultsAccessibilityTests(AccessibilityTestsFixture<Startup> fixture) :
            base(fixture) { }

        [Fact]
        public void EditLearningPathwayDefaults_journey_has_no_accessibility_errors()
        {
            // Given
            Driver.LogUserInAsAdminAndDelegate(BaseUrl);
            const string startUrl = "/TrackingSystem/CourseSetup/100/Manage/LearningPathwayDefaults";

            // When
            Driver.Navigate().GoToUrl(BaseUrl + startUrl);
            ValidatePageHeading("Edit Learning Pathway defaults");
            var editPageResult = new AxeBuilder(Driver).Analyze();

            Driver.SetCheckboxState("AutoRefresh", true);

            Driver.ClickButtonByText("Save");
            ValidatePageHeading("Edit auto-refresh options");
            var autoRefreshPageResult = new AxeBuilder(Driver).Analyze();

            editPageResult.Violations.Should().BeEmpty();
            autoRefreshPageResult.Violations.Should().BeEmpty();
        }
    }
}
