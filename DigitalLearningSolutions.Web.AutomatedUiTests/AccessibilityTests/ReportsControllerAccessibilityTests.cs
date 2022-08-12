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
            var result = new AxeBuilder(Driver).Analyze();

            // then
            CheckAccessibilityResult(result);
        }

        private static void CheckAccessibilityResult(AxeResult result)
        {
            // Expect axe violations caused by having an aria-expanded attribute on two
            // radio inputs and one checkbox input.
            // The targets #course-filter-type-1, #course-filter-type-2 and #EndDate are
            // nhs-tested components so ignore this violation.
            result.Violations.Should().HaveCount(1);

            var violation = result.Violations[0];

            violation.Id.Should().Be("aria-allowed-attr");
            violation.Nodes.Should().HaveCount(3);
            violation.Nodes[0].Target.Should().HaveCount(1);
            violation.Nodes[0].Target[0].Selector.Should().Be("#course-filter-type-1");
            violation.Nodes[1].Target.Should().HaveCount(1);
            violation.Nodes[1].Target[0].Selector.Should().Be("#course-filter-type-2");
            violation.Nodes[2].Target.Should().HaveCount(1);
            violation.Nodes[2].Target[0].Selector.Should().Be("#EndDate");
        }
    }
}
