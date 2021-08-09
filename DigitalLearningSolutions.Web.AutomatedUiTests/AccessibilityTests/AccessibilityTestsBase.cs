namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using FluentAssertions;
    using OpenQA.Selenium;
    using Selenium.Axe;
    using Xunit;

    [Collection("Selenium test collection")]
    public class AccessibilityTestsBase
    {
        internal readonly string BaseUrl;
        internal readonly IWebDriver Driver;

        public AccessibilityTestsBase(AccessibilityTestsFixture<Startup> fixture)
        {
            BaseUrl = fixture.BaseUrl;
            Driver = fixture.Driver;
        }
        
        public void AnalyzePageHeadingAndAccessibility(string pageTitle)
        {
            ValidatePageHeading(pageTitle);

            // then
            var axeResult = new AxeBuilder(Driver).Analyze();
            axeResult.Violations.Should().BeEmpty();
        }

        public void ValidatePageHeading(string pageTitle)
        {
            var h1Element = Driver.FindElement(By.TagName("h1"));
            h1Element.Text.Should().BeEquivalentTo(pageTitle);
        }
    }
}
