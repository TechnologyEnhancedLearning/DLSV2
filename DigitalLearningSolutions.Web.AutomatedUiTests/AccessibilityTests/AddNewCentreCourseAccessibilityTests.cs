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
            Driver.SelectDropdownItemValue("ApplicationId", "731");
            Driver.ClickButtonByText("Next");

            ValidatePageHeading("Set course details");
            var setCourseDetailsPageResult = new AxeBuilder(Driver).Analyze();
            Driver.SubmitForm();

            ValidatePageHeading("Set course options");
            var setCourseOptionsPageResult = new AxeBuilder(Driver).Analyze();
            Driver.SubmitForm();

            ValidatePageHeading("Set course content");
            var setCourseContentPageResult = new AxeBuilder(Driver).Analyze();
            Driver.FindElement(By.Id("ChooseSections")).Click();
            Driver.SetCheckboxState("section-2596", true);
            Driver.SubmitForm();

            ValidatePageHeading("Set section content");
            var setSectionContentPageResult = new AxeBuilder(Driver).Analyze();
            Driver.ClickButtonByText("Next");

            ValidatePageHeading("Summary");
            var summaryPageResult = new AxeBuilder(Driver).Analyze();

            selectCoursePageResult.Violations.Should().BeEmpty();

            // Expect an axe violation caused by having an aria-expanded attribute on an input
            // The target inputs are nhs-tested components so ignore these violation
            setCourseDetailsPageResult.Violations.Should().HaveCount(1);
            var setCourseDetailsViolation = setCourseDetailsPageResult.Violations[0];
            setCourseDetailsViolation.Id.Should().Be("aria-allowed-attr");
            setCourseDetailsViolation.Nodes.Should().HaveCount(3);
            setCourseDetailsViolation.Nodes[0].Target.Should().HaveCount(1);
            setCourseDetailsViolation.Nodes[0].Target[0].Selector.Should().Be("#PasswordProtected");
            setCourseDetailsViolation.Nodes[1].Target.Should().HaveCount(1);
            setCourseDetailsViolation.Nodes[1].Target[0].Selector.Should().Be("#ReceiveNotificationEmails");
            setCourseDetailsViolation.Nodes[2].Target.Should().HaveCount(1);
            setCourseDetailsViolation.Nodes[2].Target[0].Selector.Should().Be("#OtherCompletionCriteria");

            setCourseOptionsPageResult.Violations.Should().BeEmpty();
            setCourseContentPageResult.Violations.Should().BeEmpty();
            setSectionContentPageResult.Violations.Should().BeEmpty();
            summaryPageResult.Violations.Should().BeEmpty();
        }
    }
}
