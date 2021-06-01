namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
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
            var firstName = Driver.FindElement(By.Id("FirstName"));
            firstName.SendKeys("Test");
            var lastName = Driver.FindElement(By.Id("LastName"));
            lastName.SendKeys("User");
            var email = Driver.FindElement(By.Id("Email"));
            email.SendKeys("candidate@test.com");
            var registerForm = Driver.FindElement(By.TagName("form"));
            registerForm.Submit();

            var learnerInformationResult = new AxeBuilder(Driver).Analyze();
            var centre = new SelectElement(Driver.FindElement(By.Id("Centre")));
            centre.SelectByValue("2");
            var jobGroup = new SelectElement(Driver.FindElement(By.Id("JobGroup")));
            jobGroup.SelectByValue("1");
            var learnerInformationForm = Driver.FindElement(By.TagName("form"));
            learnerInformationForm.Submit();

            var passwordResult = new AxeBuilder(Driver).Analyze();
            var password = Driver.FindElement(By.Id("Password"));
            password.SendKeys("password!1");
            var confirmPassword = Driver.FindElement(By.Id("ConfirmPassword"));
            confirmPassword.SendKeys("password!1");
            var passwordForm = Driver.FindElement(By.TagName("form"));
            passwordForm.Submit();

            var summaryResult = new AxeBuilder(Driver).Analyze();

            // then
            registerResult.Violations.Should().BeEmpty();
            learnerInformationResult.Violations.Should().BeEmpty();
            passwordResult.Violations.Should().BeEmpty();
            summaryResult.Violations.Should().BeEmpty();
        }
    }
}
