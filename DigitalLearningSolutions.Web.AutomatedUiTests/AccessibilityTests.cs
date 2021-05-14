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
        [InlineData("/Register")]
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
        public void Authenticated_page_has_no_accessibility_errors(string url)
        {
            // when
            LogUserIn();
            driver.Navigate().GoToUrl(baseUrl + url);
            var axeResult = new AxeBuilder(driver).Analyze();

            // then
            axeResult.Violations.Should().BeEmpty();
        }

        private static ChromeDriver CreateHeadlessChromeDriver()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless");
            return new ChromeDriver(chromeOptions);
        }
        
        // TODO HEEDLS-396 Add automated UI tests for registration journey

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
