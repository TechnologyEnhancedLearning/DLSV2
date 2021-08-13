namespace DigitalLearningSolutions.Web.AutomatedUiTests.TestFixtures
{
    using System;
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers;
    using OpenQA.Selenium;

    public class AccessibilityTestsFixture<TStartup> : IDisposable where TStartup : class
    {
        internal readonly string BaseUrl;
        internal readonly IWebDriver Driver;

        public AccessibilityTestsFixture()
        {
            var factory = new SeleniumServerFactory<TStartup>();
            BaseUrl = factory.RootUri;
            Driver = DriverHelper.CreateHeadlessChromeDriver();
        }

        public void Dispose()
        {
            Driver.Quit();
            Driver.Dispose();
        }
    }
}
