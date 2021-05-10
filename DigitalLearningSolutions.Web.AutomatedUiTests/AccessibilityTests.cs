namespace DigitalLearningSolutions.Web.AutomatedUiTests
{
    using System;
    using FluentAssertions;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using Selenium.Axe;
    using Xunit;

    public class AccessibilityTests : IClassFixture<SeleniumServerFactory<Startup>>, IDisposable
    {
        private readonly string baseUrl;
        private readonly IWebDriver driver;

        public AccessibilityTests(SeleniumServerFactory<Startup> factory)
        {
            factory.CreateClient();
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
        public void Page_has_no_accessibility_errors(string url)
        {
            // when
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
            var learnerInformationUrl = "/Register/LearnerInformation";
            var passwordUrl = "/Register/Password";
            var summaryUrl = "/Register/Summary";
            var confirmationUrl = "/Register/Confirmation";

            // when
            driver.Navigate().GoToUrl(baseUrl + registerUrl);
            var registerResult = new AxeBuilder(driver).Analyze();
            var firstName = driver.FindElement(By.Id("FirstName"));
            firstName.SendKeys("Test");
            var lastName = driver.FindElement(By.Id("LastName"));
            lastName.SendKeys("User");
            var email = driver.FindElement(By.Id("Email"));
            email.SendKeys("email@test.com");
            var registerForm = driver.FindElement(By.TagName("form"));
            registerForm.Submit();

            var learnerInformationResult = new AxeBuilder(driver).Analyze();

            // then
            registerResult.Violations.Should().BeEmpty();
        }

        private ChromeDriver CreateHeadlessChromeDriver()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless");
            return new ChromeDriver(chromeOptions);
        }
    }
}
