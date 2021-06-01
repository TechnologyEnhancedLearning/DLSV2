namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using Xunit;

    [CollectionDefinition("Accessibility collection")]
    public class AccessibilityCollection : ICollectionFixture<SeleniumServerFactory<Startup>>
    {
    }
}
