namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers;
    using FluentAssertions;
    using OpenQA.Selenium;
    using Selenium.Axe;
    using Xunit;

    public class BasicAccessibilityTests : AccessibilityTestsBase
    {
        public BasicAccessibilityTests(SeleniumServerFactory<Startup> factory) : base(factory) { }

        [Theory]
        [InlineData("/Home/Welcome", "Welcome - Digital Learning Solutions")]
        [InlineData("/Home/Products", "Products - Digital Learning Solutions")]
        [InlineData("/Home/LearningContent", "Learning Content - Digital Learning Solutions")]
        [InlineData("/Login", "Log in")]
        [InlineData("/ForgotPassword", "Reset your password")]
        [InlineData("/ResetPassword/Error", "Something went wrong...")]
        public void Page_has_no_accessibility_errors(string url, string pageTitle)
        {
            // when
            Driver.Navigate().GoToUrl(BaseUrl + url);

            // then
            AnalyzePageHeadingAndAccessibility(pageTitle);
        }

        [Theory]
        [InlineData("/MyAccount", "My account")]
        [InlineData("/MyAccount/EditDetails", "Edit details")]
        [InlineData("/TrackingSystem/Centre/Administrators", "Centre administrators")]
        [InlineData("/TrackingSystem/CentreConfiguration", "Centre configuration")]
        [InlineData("/TrackingSystem/CentreConfiguration/EditCentreManagerDetails", "Edit centre manager details")]
        [InlineData("/TrackingSystem/CentreConfiguration/EditCentreWebsiteDetails", "Edit centre content on DLS website")]
        [InlineData("/TrackingSystem/CentreConfiguration/RegistrationPrompts", "Manage delegate registration prompts")]
        [InlineData("/TrackingSystem/CentreConfiguration/RegistrationPrompts/1/Edit", "Edit delegate registration prompt")]
        [InlineData("/TrackingSystem/Centre/Reports", "Reports")]
        [InlineData("/TrackingSystem/Delegates/Approve", "Approve delegate registrations")]
        [InlineData("/NotificationPreferences", "Notification preferences")]
        [InlineData("/NotificationPreferences/Edit/AdminUser", "Update notification preferences")]
        [InlineData("/NotificationPreferences/Edit/DelegateUser", "Update notification preferences")]
        public void Authenticated_page_has_no_accessibility_errors(string url, string pageTitle)
        {
            // when
            Driver.LogUserInAsAdminAndDelegate(BaseUrl);
            Driver.Navigate().GoToUrl(BaseUrl + url);

            // then
            AnalyzePageHeadingAndAccessibility(pageTitle);
        }
    }
}
