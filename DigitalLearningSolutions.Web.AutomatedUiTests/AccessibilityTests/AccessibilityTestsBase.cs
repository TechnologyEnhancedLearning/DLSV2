namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using System;
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers;
    using OpenQA.Selenium;
    using Xunit;

    [Collection("Accessibility collection")]
    public class AccessibilityTestsBase : IDisposable
    {
        internal readonly string BaseUrl;
        internal readonly IWebDriver Driver;

        public AccessibilityTestsBase(SeleniumServerFactory<Startup> factory)
        {
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
