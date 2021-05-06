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
        [InlineData("/Register")]
        [InlineData("/Register/LearnerInformation")]
        [InlineData("/Register/Password")]
        [InlineData("/Register/Summary")]
        [InlineData("/Register/Confirmation")]
        public void Page_has_no_accessibility_errors(string url)
        {
            // when
            driver.Navigate().GoToUrl(baseUrl + url);
            var axeResult = new AxeBuilder(driver).Analyze();

            // then
            axeResult.Violations.Should().BeEmpty();
        }

        private ChromeDriver CreateHeadlessChromeDriver()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless");
            return new ChromeDriver(chromeOptions);
        }
    }
}
