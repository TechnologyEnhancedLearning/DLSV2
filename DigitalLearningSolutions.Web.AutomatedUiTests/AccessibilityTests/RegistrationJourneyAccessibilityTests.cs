namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers;
    using FluentAssertions;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;
    using Selenium.Axe;
    using Xunit;

    public class RegistrationJourneyAccessibilityTests : AccessibilityTestsBase
    {
        public RegistrationJourneyAccessibilityTests(SeleniumServerFactory<Startup> factory) : base(factory) { }

        [Fact]
        public void Registration_journey_has_no_accessibility_errors()
        {
            // given
            var registerUrl = "/Register";

            // when
            Driver.Navigate().GoToUrl(BaseUrl + registerUrl);
            var registerResult = new AxeBuilder(Driver).Analyze();
            Driver.SelectDropdownItemValue("Centre", "101");
            Driver.FillTextInput("FirstName", "Test");
            Driver.FillTextInput("LastName", "User");
            Driver.FillTextInput("Email", "candidate@test.com");
            Driver.SubmitForm();

            var learnerInformationResult = new AxeBuilder(Driver).Analyze();
            Driver.SelectDropdownItemValue("Answer1", "Principal Relationship Manager");
            Driver.FillTextInput("Answer2", "A Person");
            Driver.SelectDropdownItemValue("JobGroup", "1");
            Driver.SubmitForm();

            var passwordResult = new AxeBuilder(Driver).Analyze();
            Driver.FillTextInput("Password", "password!1");
            Driver.FillTextInput("ConfirmPassword", "password!1");
            Driver.SubmitForm();

            var summaryResult = new AxeBuilder(Driver).Analyze();

            // then
            registerResult.Violations.Should().BeEmpty();
            learnerInformationResult.Violations.Should().BeEmpty();
            passwordResult.Violations.Should().BeEmpty();
            summaryResult.Violations.Should().BeEmpty();
        }
    }
}
