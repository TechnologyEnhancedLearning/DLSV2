namespace DigitalLearningSolutions.Web.AutomatedUiTests.TestFixtures
{
    using System;
    using System.Data.SqlClient;
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers;
    using OpenQA.Selenium;

    public class AccessibilityTestsFixture<TStartup> : IDisposable where TStartup : class
    {
        internal readonly string BaseUrl;
        internal readonly IWebDriver Driver;
        private readonly SeleniumServerFactory<TStartup> factory;

        public AccessibilityTestsFixture()
        {
            factory = new SeleniumServerFactory<TStartup>();
            BaseUrl = factory.RootUri;
            Driver = DriverHelper.CreateHeadlessChromeDriver();
        }

        public void Dispose()
        {
            Driver.Quit();
            Driver.Dispose();
            factory.Dispose();
        }
    }
}
