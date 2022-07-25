namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestFixtures;
    using Xunit;

    public class BasicAccessibilityTests : AccessibilityTestsBase, IClassFixture<AccessibilityTestsFixture<Startup>>
    {
        public BasicAccessibilityTests(AccessibilityTestsFixture<Startup> fixture) : base(fixture) { }

        [Theory]
        [InlineData("/Home/Welcome", "Welcome - Digital Learning Solutions")]
        [InlineData("/Home/Products", "Products - Digital Learning Solutions")]
        [InlineData("/Home/LearningContent", "Learning Content - Digital Learning Solutions")]
        [InlineData("/Home/LearningContent/1/1", "IT Skills Pathway")]
        [InlineData("/Login", "Log in")]
        [InlineData("/ForgotPassword", "Reset your password")]
        [InlineData("/ResetPassword/Error", "Something went wrong...")]
        [InlineData("/ClaimAccount?email=claimable_user@email.com&code=code", "Complete registration")]
        [InlineData(
            "/ClaimAccount/CompleteRegistration?email=claimable_user@email.com&code=code",
            "Complete registration"
        )]
        [InlineData("/ClaimAccount/Confirmation", "Delegate registration complete")]
        public void Page_has_no_accessibility_errors(string url, string pageTitle)
        {
            // when
            Driver.Navigate().GoToUrl(BaseUrl + url);

            // then
            AnalyzePageHeadingAndAccessibility(pageTitle);
        }
    }
}
