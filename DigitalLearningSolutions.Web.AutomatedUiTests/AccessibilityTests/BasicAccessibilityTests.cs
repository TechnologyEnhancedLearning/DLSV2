namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers;
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
        [InlineData("/TrackingSystem/Centre/Dashboard", "Centre dashboard")]
        [InlineData("/TrackingSystem/Centre/Ranking", "Centre ranking")]
        [InlineData("/TrackingSystem/Centre/ContractDetails", "Contract details")]
        [InlineData("/TrackingSystem/CentreConfiguration", "Centre configuration")]
        [InlineData("/TrackingSystem/CentreConfiguration/EditCentreManagerDetails", "Edit centre manager details")]
        [InlineData("/TrackingSystem/CentreConfiguration/EditCentreWebsiteDetails", "Edit centre content on DLS website")]
        [InlineData("/TrackingSystem/CentreConfiguration/RegistrationPrompts", "Manage delegate registration prompts")]
        [InlineData("/TrackingSystem/CentreConfiguration/RegistrationPrompts/1/Remove", "Remove delegate registration prompt")]
        [InlineData("/TrackingSystem/Centre/Reports", "Centre reports")]
        [InlineData("/TrackingSystem/Centre/TopCourses", "Top courses")]
        [InlineData("/TrackingSystem/CourseSetup", "Centre course setup")]
        [InlineData("/TrackingSystem/Delegates/All", "Delegates")]
        [InlineData("/TrackingSystem/Delegates/View/1", "xxxxx xxxxxxxxx")]
        [InlineData("/TrackingSystem/Delegates/Approve", "Approve delegate registrations")]
        [InlineData("/TrackingSystem/Delegates/BulkUpload", "Bulk upload/update delegates")]
        [InlineData("/NotificationPreferences", "Notification preferences")]
        [InlineData("/NotificationPreferences/Edit/AdminUser", "Update notification preferences")]
        [InlineData("/NotificationPreferences/Edit/DelegateUser", "Update notification preferences")]
        [InlineData("/ChangePassword", "Change password")]
        [InlineData("/TrackingSystem/Support", "Support")]
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
