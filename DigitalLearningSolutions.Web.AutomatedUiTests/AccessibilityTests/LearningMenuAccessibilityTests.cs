namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestFixtures;
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers;
    using FluentAssertions;
    using Selenium.Axe;
    using Xunit;
    public class LearningMenuAccessibilityTests : AccessibilityTestsBase,
        IClassFixture<AccessibilityTestsFixture<Startup>>
    {
        public LearningMenuAccessibilityTests(AccessibilityTestsFixture<Startup> fixture) : base(fixture) { }

        [Fact]
        public void LearningMenu_interface_has_no_accessibility_errors()
        {
            {
                // Given
                Driver.LogUserInAsAdminAndDelegate(BaseUrl);
                const string startUrl = "/LearningMenu/19262";
                const string submenuUrl = startUrl + "/1010";
                const string diagnosticUrl = submenuUrl + "/Diagnostic";
                const string tutorialUrl = submenuUrl + "/4448";
                const string postLearningUrl = submenuUrl + "/PostLearning";
                const string completionSummaryUrl = startUrl + "/CompletionSummary";


                // When
                Driver.Navigate().GoToUrl(BaseUrl + startUrl);
                ValidatePageHeading("Excel 2013 for the Workplace - Testing");
                var courseMenuResult = new AxeBuilder(Driver).Analyze();

                Driver.Navigate().GoToUrl(BaseUrl + submenuUrl);
                ValidatePageHeading("Workbooks and worksheets");
                var submenuResult = new AxeBuilder(Driver).Analyze();

                Driver.Navigate().GoToUrl(BaseUrl + diagnosticUrl);
                ValidatePageHeading("Diagnostic assessment Workbooks and worksheets");
                var diagnosticResult = new AxeBuilder(Driver).Analyze();

                Driver.Navigate().GoToUrl(BaseUrl + tutorialUrl);
                ValidatePageHeading("Working with Workbooks");
                var tutorialResult = new AxeBuilder(Driver).Analyze();

                Driver.Navigate().GoToUrl(BaseUrl + postLearningUrl);
                ValidatePageHeading("Post learning assessment Workbooks and worksheets");
                var postLearningResult = new AxeBuilder(Driver).Analyze();

                Driver.Navigate().GoToUrl(BaseUrl + completionSummaryUrl);
                ValidatePageHeading("Course incomplete");
                var completionSummaryResult = new AxeBuilder(Driver).Analyze();

                // Then
                courseMenuResult.Violations.Should().BeEmpty();
                submenuResult.Violations.Should().BeEmpty();
                diagnosticResult.Violations.Should().BeEmpty();
                tutorialResult.Violations.Should().BeEmpty();
                postLearningResult.Violations.Should().BeEmpty();
                completionSummaryResult.Violations.Should().BeEmpty();
            }
        }
    }
}
