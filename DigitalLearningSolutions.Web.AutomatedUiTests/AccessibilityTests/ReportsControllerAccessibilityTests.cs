namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestFixtures;
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers;
    using FluentAssertions;
    using Selenium.Axe;
    using Xunit;

    public class ReportsControllerAccessibilityTests : AccessibilityTestsBase,
        IClassFixture<AccessibilityTestsFixture<Startup>>
    {
        public ReportsControllerAccessibilityTests(AccessibilityTestsFixture<Startup> fixture) : base(fixture) { }

        [Fact]
        public void Edit_reports_filters_page_has_no_accessibility_errors()
        {
            // given
            Driver.LogUserInAsAdminAndDelegate(BaseUrl);
            const string reportsEditFiltersUrl = "/TrackingSystem/Centre/Reports/Courses/EditFilters";

            // when
            Driver.Navigate().GoToUrl(BaseUrl + reportsEditFiltersUrl);
            //Exclude conditional radios, see: https://github.com/alphagov/govuk-frontend/issues/979#issuecomment-872300557
            var result = new AxeBuilder(Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();

            // then
            CheckAccessibilityResult(result);
        }

        private static void CheckAccessibilityResult(AxeResult result)
        {
            result.Violations.Should().BeEmpty();
        }
    }
}
