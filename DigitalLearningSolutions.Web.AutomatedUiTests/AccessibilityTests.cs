namespace DigitalLearningSolutions.Web.AutomatedUiTests
{
    using System;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Support.UI;
    using Selenium.Axe;
    using Xunit;

    public class AccessibilityTests : IClassFixture<SeleniumServerFactory<Startup>>, IDisposable
    {
        private readonly string baseUrl;
        private readonly IWebDriver driver;

        public AccessibilityTests(SeleniumServerFactory<Startup> factory)
        {
            baseUrl = factory.RootUri;
            driver = CreateHeadlessChromeDriver();
        }

        public void Dispose()
        {
            driver.Quit();
            driver.Dispose();
        }

        [Theory]
        [InlineData("/Home/Welcome", "Welcome - Digital Learning Solutions")]
        [InlineData("/Home/Products", "Products - Digital Learning Solutions")]
        [InlineData("/Home/LearningContent", "Learning Content - Digital Learning Solutions")]
        [InlineData("/Login", "Log in")]
        [InlineData("/ForgotPassword", "Reset your password")]
        [InlineData("/ResetPassword/Error", "Something went wrong...")]
        public void Page_has_no_accessibility_errors(string url, string pageTitle)
        {
            // when
            driver.Navigate().GoToUrl(baseUrl + url);
            var axeResult = new AxeBuilder(driver).Analyze();
            var h1Element = driver.FindElement(By.TagName("h1"));

            // then
            using (new AssertionScope())
            {
                axeResult.Violations.Should().BeEmpty();
                h1Element.Text.Should().BeEquivalentTo(pageTitle);
            }
        }

        [Theory]
        [InlineData("/MyAccount", "My account")]
        [InlineData("/MyAccount/EditDetails", "Edit details")]
        [InlineData("/TrackingSystem/CentreConfiguration", "Centre configuration")]
        [InlineData("/TrackingSystem/CentreConfiguration/EditCentreManagerDetails", "Edit centre manager details")]
        [InlineData("/TrackingSystem/CentreConfiguration/EditCentreWebsiteDetails", "Edit centre content on DLS website")]
        [InlineData("/TrackingSystem/Delegates/Approve", "Approve delegate registrations")]
        public void Authenticated_page_has_no_accessibility_errors(string url, string pageTitle)
        {
            // when
            LogUserIn();
            driver.Navigate().GoToUrl(baseUrl + url);
            var axeResult = new AxeBuilder(driver).Analyze();
            var h1Element = driver.FindElement(By.TagName("h1"));

            // then
            using (new AssertionScope())
            {
                axeResult.Violations.Should().BeEmpty();
                h1Element.Text.Should().BeEquivalentTo(pageTitle);
            }
        }

        [Fact]
        public void Registration_journey_has_no_accessibility_errors()
        {
            // given
            var registerUrl = "/Register";

            // when
            driver.Navigate().GoToUrl(baseUrl + registerUrl);
            var registerResult = new AxeBuilder(driver).Analyze();
            var firstName = driver.FindElement(By.Id("FirstName"));
            firstName.SendKeys("Test");
            var lastName = driver.FindElement(By.Id("LastName"));
            lastName.SendKeys("User");
            var email = driver.FindElement(By.Id("Email"));
            email.SendKeys("candidate@test.com");
            var registerForm = driver.FindElement(By.TagName("form"));
            registerForm.Submit();

            var learnerInformationResult = new AxeBuilder(driver).Analyze();
            var centre = new SelectElement(driver.FindElement(By.Id("Centre")));
            centre.SelectByValue("2");
            var jobGroup = new SelectElement(driver.FindElement(By.Id("JobGroup")));
            jobGroup.SelectByValue("1");
            var learnerInformationForm = driver.FindElement(By.TagName("form"));
            learnerInformationForm.Submit();

            var passwordResult = new AxeBuilder(driver).Analyze();
            var password = driver.FindElement(By.Id("Password"));
            password.SendKeys("password!1");
            var confirmPassword = driver.FindElement(By.Id("ConfirmPassword"));
            confirmPassword.SendKeys("password!1");
            var passwordForm = driver.FindElement(By.TagName("form"));
            passwordForm.Submit();

            var summaryResult = new AxeBuilder(driver).Analyze();

            // then
            registerResult.Violations.Should().BeEmpty();
            learnerInformationResult.Violations.Should().BeEmpty();
            passwordResult.Violations.Should().BeEmpty();
            summaryResult.Violations.Should().BeEmpty();
        }

        private static ChromeDriver CreateHeadlessChromeDriver()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless");
            return new ChromeDriver(chromeOptions);
        }

        private void LogUserIn()
        {
            driver.Navigate().GoToUrl(baseUrl + "/Login");
            var username = driver.FindElement(By.Id("Username"));
            username.SendKeys("admin");

            var password = driver.FindElement(By.Id("Password"));
            password.SendKeys("password-1");

            var submitButton = driver.FindElement(By.TagName("form"));
            submitButton.Submit();
        }
    }
}
