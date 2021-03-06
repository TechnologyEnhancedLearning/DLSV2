﻿namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers;
    using FluentAssertions;
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

        [Fact]
        public void Registration_by_centre_journey_with_send_email_has_no_accessibility_errors()
        {
            // given
            Driver.LogUserInAsAdminAndDelegate(BaseUrl);
            const string registerUrl = "/TrackingSystem/Delegates/Register";

            // when
            Driver.Navigate().GoToUrl(BaseUrl + registerUrl);
            var registerResult = new AxeBuilder(Driver).Analyze();
            Driver.FillTextInput("FirstName", "Test");
            Driver.FillTextInput("LastName", "User");
            Driver.FillTextInput("Email", "candidate@test.com");
            Driver.FillTextInput("Alias", "candid8");
            Driver.SubmitForm();

            var learnerInformationResult = new AxeBuilder(Driver).Analyze();
            Driver.SelectDropdownItemValue("Answer1", "Principal Relationship Manager");
            Driver.FillTextInput("Answer2", "A Person");
            Driver.SelectDropdownItemValue("JobGroup", "1");
            Driver.SubmitForm();

            var welcomeEmailResult = new AxeBuilder(Driver).Analyze();
            Driver.SetCheckboxState("ShouldSendEmail", true);
            Driver.FillTextInput("Day", "14");
            Driver.FillTextInput("Month", "7");
            Driver.FillTextInput("Year", "2222");
            Driver.SubmitForm();

            var summaryResult = new AxeBuilder(Driver).Analyze();

            // then
            registerResult.Violations.Should().BeEmpty();
            learnerInformationResult.Violations.Should().BeEmpty();
            welcomeEmailResult.Violations.Should().BeEmpty();
            summaryResult.Violations.Should().BeEmpty();
        }

        [Fact]
        public void Registration_by_centre_journey_with_set_password_has_no_accessibility_errors()
        {
            // given
            Driver.LogUserInAsAdminAndDelegate(BaseUrl);
            const string registerUrl = "/TrackingSystem/Delegates/Register";

            // when
            Driver.Navigate().GoToUrl(BaseUrl + registerUrl);
            var registerResult = new AxeBuilder(Driver).Analyze();
            Driver.FillTextInput("FirstName", "Test");
            Driver.FillTextInput("LastName", "User");
            Driver.FillTextInput("Email", "candidate@test.com");
            Driver.FillTextInput("Alias", "candid8");
            Driver.SubmitForm();

            var learnerInformationResult = new AxeBuilder(Driver).Analyze();
            Driver.SelectDropdownItemValue("Answer1", "Principal Relationship Manager");
            Driver.FillTextInput("Answer2", "A Person");
            Driver.SelectDropdownItemValue("JobGroup", "1");
            Driver.SubmitForm();

            var welcomeEmailResult = new AxeBuilder(Driver).Analyze();
            Driver.SetCheckboxState("ShouldSendEmail", false);
            Driver.SubmitForm();

            var passwordResult = new AxeBuilder(Driver).Analyze();
            Driver.FillTextInput("Password", "password!1");
            Driver.SubmitForm();

            var summaryResult = new AxeBuilder(Driver).Analyze();

            // then
            registerResult.Violations.Should().BeEmpty();
            learnerInformationResult.Violations.Should().BeEmpty();
            welcomeEmailResult.Violations.Should().BeEmpty();
            passwordResult.Violations.Should().BeEmpty();
            summaryResult.Violations.Should().BeEmpty();
        }
    }
}
