namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using System;
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers;
    using FluentAssertions;
    using OpenQA.Selenium;
    using Selenium.Axe;
    using Xunit;

    [Collection("Selenium test collection")]
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

        public void AnalyzePageHeadingAndAccessibility(string pageTitle)
        {
            AnalyzePageHeading(pageTitle);

            // then
            var axeResult = new AxeBuilder(Driver).Analyze();
            axeResult.Violations.Should().BeEmpty();
        }

        public void AnalyzePageHeading(string pageTitle)
        {
            var h1Element = Driver.FindElement(By.TagName("h1"));
            h1Element.Text.Should().BeEquivalentTo(pageTitle);
        }
    }
}
