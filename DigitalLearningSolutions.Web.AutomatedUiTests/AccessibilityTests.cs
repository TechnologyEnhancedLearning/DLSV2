namespace DigitalLearningSolutions.Web.AutomatedUiTests
{
    using System;
    using FluentAssertions;
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
        [InlineData("/Home/Welcome")]
        [InlineData("/Home/Products")]
        [InlineData("/Home/LearningContent")]
        [InlineData("/Login")]
        [InlineData("/ForgotPassword")]
        [InlineData("/ResetPassword/Error")]
        public void Page_has_no_accessibility_errors(string url)
        {
            // when
            driver.Navigate().GoToUrl(baseUrl + url);
            var axeResult = new AxeBuilder(driver).Analyze();

            // then
            axeResult.Violations.Should().BeEmpty();
        }

        [Theory]
        [InlineData("/MyAccount")]
        [InlineData("/MyAccount/EditDetails")]
        [InlineData("/TrackingSystem/CentreConfiguration")]
        [InlineData("/TrackingSystem/CentreConfiguration/EditCentreManagerDetails")]
        [InlineData("/TrackingSystem/Delegates/Approve")]
        public void Authenticated_page_has_no_accessibility_errors(string url)
        {
            // when
            LogUserIn();
            driver.Navigate().GoToUrl(baseUrl + url);
            var axeResult = new AxeBuilder(driver).Analyze();

            // then
            axeResult.Violations.Should().BeEmpty();
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
