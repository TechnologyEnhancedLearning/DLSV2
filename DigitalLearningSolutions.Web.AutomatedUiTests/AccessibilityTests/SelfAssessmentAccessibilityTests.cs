namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestFixtures;
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers;
    using FluentAssertions;
    using Selenium.Axe;
    using Xunit;
    public class SelfAssessmentAccessibilityTests : AccessibilityTestsBase,
        IClassFixture<AccessibilityTestsFixture<Startup>>
    {
        public SelfAssessmentAccessibilityTests(AccessibilityTestsFixture<Startup> fixture) : base(fixture) { }

        [Fact]
        public void SelfAssessment_journey_has_no_accessibility_errors()
        {
            // Given
            Driver.LogUserInAsAdminAndDelegate(BaseUrl);
            const string startUrl = "/LearningPortal/SelfAssessment/1";
            const string completeByUrl = "/LearningPortal/SelfAssessment/3/CompleteBy";

            // When
            Driver.Navigate().GoToUrl(BaseUrl + startUrl);
            ValidatePageHeading("Digital Capability Self Assessment");
            var startPageResult = new AxeBuilder(Driver).Analyze();

            Driver.ClickLinkContainingText("Go to Capability 1");
            ValidatePageHeading("Data, Information and Content");
            var firstCapabilityResult = new AxeBuilder(Driver).Analyze();

            Driver.ClickLinkContainingText("Return to Capabilities");
            ValidatePageHeading("Digital Capability Self Assessment - Capabilities");
            var capabilitiesResult = new AxeBuilder(Driver).Analyze();

            Driver.Navigate().GoToUrl(BaseUrl + completeByUrl);
            ValidatePageHeading("Enter a completion date for Digital Capability Self Assessment");
            var completeByPageResult = new AxeBuilder(Driver).Analyze();

            //Then
            startPageResult.Violations.Should().BeEmpty();
            firstCapabilityResult.Violations.Should().BeEmpty();
            capabilitiesResult.Violations.Should().BeEmpty();
            completeByPageResult.Violations.Should().BeEmpty();
        }
    }
}
