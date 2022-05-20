namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestFixtures;
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers;
    using FluentAssertions;
    using Selenium.Axe;
    using Xunit;

    public class AddCourseAdminFieldsAccessibilityTests : AccessibilityTestsBase,
        IClassFixture<AccessibilityTestsFixture<Startup>>
    {
        public AddCourseAdminFieldsAccessibilityTests(AccessibilityTestsFixture<Startup> fixture) : base(fixture) { }

        [Fact]
        public void AddAdminField_journey_has_no_accessibility_errors()
        {
            // Given
            Driver.LogUserInAsAdminAndDelegate(BaseUrl);
            const string startUrl = "/TrackingSystem/CourseSetup/100/AdminFields/Add/New";

            // When
            Driver.Navigate().GoToUrl(BaseUrl + startUrl);
            ValidatePageHeading("Add course admin field");
            var addPageResult = new AxeBuilder(Driver).Analyze();
            Driver.SelectDropdownItemValue("AdminFieldId", "1");

            AddAnswer("Answer 1");
            AddAnswer("Answer 2");
            ValidatePageHeading("Add course admin field");
            var addAnswersResult = new AxeBuilder(Driver).Analyze();

            Driver.ClickButtonByText("Bulk edit");
            ValidatePageHeading("Configure responses in bulk");
            var bulkAdditionResult = new AxeBuilder(Driver).Analyze();

            Driver.ClickButtonByText("Submit");
            ValidatePageHeading("Add course admin field");

            addPageResult.Violations.Should().BeEmpty();
            bulkAdditionResult.Violations.Should().BeEmpty();
            addAnswersResult.Violations.Should().BeEmpty();
        }

        private void AddAnswer(string answerString)
        {
            Driver.FillTextInput("Answer", answerString);
            Driver.ClickButtonByText("Add");
        }
    }
}
