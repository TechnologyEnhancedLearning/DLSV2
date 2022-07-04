namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestFixtures;
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers;
    using FluentAssertions;
    using Selenium.Axe;
    using Xunit;

    public class RegistrationJourneyAccessibilityTests : AccessibilityTestsBase,
        IClassFixture<AccessibilityTestsFixture<Startup>>
    {
        public RegistrationJourneyAccessibilityTests(AccessibilityTestsFixture<Startup> fixture) : base(fixture) { }

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
            Driver.FillTextInput("PrimaryEmail", "candidate@test.com");
            Driver.SubmitForm();

            var learnerInformationResult = new AxeBuilder(Driver).Analyze();
            Driver.SelectDropdownItemValue("Answer1", "Principal Relationship Manager");
            Driver.FillTextInput("Answer2", "A Person");
            Driver.SelectDropdownItemValue("JobGroup", "1");
            Driver.SelectRadioOptionById("HasProfessionalRegistrationNumber_No");
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
        public void Registration_by_centre_journey_has_no_accessibility_errors()
        {
            // given
            Driver.LogUserInAsAdminAndDelegate(BaseUrl);
            const string registerUrl = "/TrackingSystem/Delegates/Register";

            // when
            Driver.Navigate().GoToUrl(BaseUrl + registerUrl);
            var registerResult = new AxeBuilder(Driver).Analyze();
            Driver.FillTextInput("FirstName", "Test");
            Driver.FillTextInput("LastName", "User");
            Driver.FillTextInput("PrimaryEmail", "candidate@test.com");
            Driver.SubmitForm();

            var learnerInformationResult = new AxeBuilder(Driver).Analyze();
            Driver.SelectDropdownItemValue("Answer1", "Principal Relationship Manager");
            Driver.FillTextInput("Answer2", "A Person");
            Driver.SelectDropdownItemValue("JobGroup", "1");
            Driver.SelectRadioOptionById("HasProfessionalRegistrationNumber_No");
            Driver.SubmitForm();

            var welcomeEmailResult = new AxeBuilder(Driver).Analyze();
            Driver.SetCheckboxState("ShouldSendEmail", false);
            Driver.SubmitForm();

            var passwordResult = new AxeBuilder(Driver).Analyze();
            Driver.FillTextInput("Password", "password!1");
            Driver.SubmitForm();

            var summaryResult = new AxeBuilder(Driver).Analyze();
            Driver.LogOutUser(BaseUrl);

            // then
            registerResult.Violations.Should().BeEmpty();
            learnerInformationResult.Violations.Should().BeEmpty();
            CheckWelcomeEmailViolations(welcomeEmailResult);
            passwordResult.Violations.Should().BeEmpty();
            summaryResult.Violations.Should().BeEmpty();
        }

        [Fact]
        public void Register_at_new_centre_journey_has_no_accessibility_errors()
        {
            // given
            var registerUrl = "/RegisterAtNewCentre";
            Driver.LogUserInAsAdminAndDelegate(BaseUrl);

            // when
            Driver.Navigate().GoToUrl(BaseUrl + registerUrl);
            var registerResult = new AxeBuilder(Driver).Analyze();
            Driver.SelectDropdownItemValue("Centre", "2");
            Driver.SubmitForm();

            var learnerInformationResult = new AxeBuilder(Driver).Analyze();
            Driver.SubmitForm();
            Driver.LogOutUser(BaseUrl);

            var summaryResult = new AxeBuilder(Driver).Analyze();

            // then
            registerResult.Violations.Should().BeEmpty();
            learnerInformationResult.Violations.Should().BeEmpty();
            summaryResult.Violations.Should().BeEmpty();
        }

        private static void CheckWelcomeEmailViolations(AxeResult welcomeEmailResult)
        {
            // Expect an axe violation caused by having an aria-expanded attribute on an input
            // The target #ShouldSendEmail is an nhs-tested component so ignore this violation
            welcomeEmailResult.Violations.Should().HaveCount(1);
            var violation = welcomeEmailResult.Violations[0];

            violation.Id.Should().Be("aria-allowed-attr");
            violation.Nodes.Should().HaveCount(1);
            violation.Nodes[0].Target.Should().HaveCount(1);
            violation.Nodes[0].Target[0].Selector.Should().Be("#ShouldSendEmail");
        }
    }
}
