namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using Xunit;

    [CollectionDefinition("Selenium test collection")]
    public class AccessibilityCollection : ICollectionFixture<SeleniumServerFactory<Startup>>
    {
    }
}
