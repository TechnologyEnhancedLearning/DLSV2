namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestFixtures;
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers;
    using FluentAssertions;
    using OpenQA.Selenium;
    using Selenium.Axe;
    using Xunit;

    public class AddNewCentreCourseAccessibilityTests : AccessibilityTestsBase,
        IClassFixture<AccessibilityTestsFixture<Startup>>
    {
        public AddNewCentreCourseAccessibilityTests(AccessibilityTestsFixture<Startup> fixture) : base(fixture) { }

        [Fact]
        public void AddNewCentreCourse_journey_has_no_accessibility_errors()
        {
            // Given
            Driver.LogUserInAsAdminAndDelegate(BaseUrl);
            const string startUrl = "/TrackingSystem/CourseSetup/AddCourseNew";

            // When
            Driver.Navigate().GoToUrl(BaseUrl + startUrl);
            ValidatePageHeading("Add new centre course");
            var selectCoursePageResult = new AxeBuilder(Driver).Analyze();
            Driver.SelectDropdownItemValue("searchable-elements", "206");
            Driver.ClickButtonByText("Next");

            ValidatePageHeading("Set course details");
            // Exclude conditional radios, see: https://github.com/alphagov/govuk-frontend/issues/979#issuecomment-872300557
            var setCourseDetailsPageResult = new AxeBuilder(Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();
            Driver.SubmitForm();

            ValidatePageHeading("Set course options");
            var setCourseOptionsPageResult = new AxeBuilder(Driver).Analyze();
            Driver.SubmitForm();

            ValidatePageHeading("Set course content");
            var setCourseContentPageResult = new AxeBuilder(Driver).Analyze();
            Driver.FindElement(By.Id("ChooseSections")).Click();
            Driver.SetCheckboxState("section-671", true);
            Driver.SubmitForm();

            ValidatePageHeading("Set section content");
            var setSectionContentPageResult = new AxeBuilder(Driver).Analyze();
            Driver.ClickButtonByText("Next");

            ValidatePageHeading("Summary");
            var summaryPageResult = new AxeBuilder(Driver).Analyze();

            selectCoursePageResult.Violations.Should().BeEmpty();
            setCourseDetailsPageResult.Violations.Should().BeEmpty();
            setCourseOptionsPageResult.Violations.Should().BeEmpty();
            setCourseContentPageResult.Violations.Should().BeEmpty();
            setSectionContentPageResult.Violations.Should().BeEmpty();
            summaryPageResult.Violations.Should().BeEmpty();
        }
    }
}
