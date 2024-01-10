namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestFixtures;
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers;
    using FluentAssertions;
    using Selenium.Axe;
    using Xunit;

    public class EditCourseDetailsAccessibilityTests : AccessibilityTestsBase,
    IClassFixture<AccessibilityTestsFixture<Startup>>
    {
        public EditCourseDetailsAccessibilityTests(AccessibilityTestsFixture<Startup> fixture) : base(fixture) { }

        [Fact]
        public void Edit_course_details_page_has_no_accessibility_errors_except_expected_ones()
        {
            // given
            Driver.LogUserInAsAdminAndDelegate(BaseUrl);
            const string editCourseDetailsUrl = "/TrackingSystem/CourseSetup/10716/Manage/CourseDetails";

            // when
            Driver.Navigate().GoToUrl(BaseUrl + editCourseDetailsUrl);
            ValidatePageHeading("Edit course details");
            // Exclude conditional radios, see: https://github.com/alphagov/govuk-frontend/issues/979#issuecomment-872300557
            var editResult = new AxeBuilder(Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();

            // then
            editResult.Violations.Should().BeEmpty();
        }
    }
}
