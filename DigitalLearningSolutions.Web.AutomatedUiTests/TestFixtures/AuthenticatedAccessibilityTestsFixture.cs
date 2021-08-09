namespace DigitalLearningSolutions.Web.AutomatedUiTests.TestFixtures
{
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers;

    public class AuthenticatedAccessibilityTestsFixture<TStartup> : AccessibilityTestsFixture<TStartup>
        where TStartup : class
    {
        public AuthenticatedAccessibilityTestsFixture()
        {
            Driver.LogUserInAsAdminAndDelegate(BaseUrl);
        }

        public new void Dispose()
        {
            Driver.LogoutUser(BaseUrl);
            base.Dispose();
        }
    }
}
