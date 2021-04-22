namespace DigitalLearningSolutions.Web.AutomatedUiTests
{
    using System;
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
        [InlineData("/Home")]
        [InlineData("/Login")]
        [InlineData("/ForgotPassword")]
        [InlineData("/Register")]
        public void Page_has_no_accessibility_errors(string url)
        {
            // when
            driver.Navigate().GoToUrl(baseUrl + url);
            var axeResult = new AxeBuilder(driver).Analyze();

            // then
            Assert.Empty(axeResult.Violations);
        }

        private ChromeDriver CreateHeadlessChromeDriver()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless");
            return new ChromeDriver(chromeOptions);
        }
    }
}
