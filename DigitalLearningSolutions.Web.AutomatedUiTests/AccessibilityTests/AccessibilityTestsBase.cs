namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestFixtures;
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
            // Exclude conditional radios, see: https://github.com/alphagov/govuk-frontend/issues/979#issuecomment-872300557
            var axeResult = new AxeBuilder(Driver).Exclude("div.nhsuk-radios--conditional div.nhsuk-radios__item input.nhsuk-radios__input").Analyze();
            axeResult.Violations.Should().BeEmpty();
        }

        public void ValidatePageHeading(string pageTitle)
        {
            var h1Element = Driver.FindElement(By.TagName("h1"));
            h1Element.Text.Should().BeEquivalentTo(pageTitle);
        }
    }
}
