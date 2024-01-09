namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestFixtures;
    using Xunit;
    public class SuperAdminAccessibilityTests : AccessibilityTestsBase,
        IClassFixture<AuthenticatedAccessibilityTestsFixture<Startup>>
    {
        public SuperAdminAccessibilityTests(AuthenticatedAccessibilityTestsFixture<Startup> fixture) : base(
            fixture
        )
        { }
        [Theory]
        [InlineData("/SuperAdmin/Users", "User Accounts")]
        [InlineData("/SuperAdmin/Users/2251/EditUserDetails", "Edit user details")]
        [InlineData("/SuperAdmin/Users/2251/CentreAccounts#2251-name", "User centre roles")]
        [InlineData("/SuperAdmin/Users/SuperAdminUserSetPassword/2251", "Set user password")]

        [InlineData("/SuperAdmin/AdminAccounts", "Administrators")]
        [InlineData("/SuperAdmin/Admins/4256/ManageRoles", "Edit Administrator roles")]
        [InlineData("/SuperAdmin/AdminAccounts/4256/DeactivateAdmin?returnPageQuery=pageNumber%3D1", "Are you sure you would like to deactivate this admin account?")]

        [InlineData("/SuperAdmin/Delegates", "Delegates")]
        [InlineData("/SuperAdmin/Delegates/2/InactivateDelegateConfirmation", "Inactivate Delegate")]

        [InlineData("/SuperAdmin/Centres", "Centres")]
        [InlineData("/SuperAdmin/Centres/AddCentre", "Add new centre")]
        [InlineData("/SuperAdmin/Centres/409/Manage", "2gether NHS Foundation Trust IT/Clinical Systems (409)")]
        [InlineData("/SuperAdmin/Centres/409/EditCentreDetails", "Edit centre details")]
        [InlineData("/SuperAdmin/Centres/409/ManageCentreManager", "Edit centre manager details")]
        [InlineData("/SuperAdmin/Centres/409/EditContractInfo", "Edit contract info for 2gether NHS Foundation Trust IT/Clinical Systems")]
        [InlineData("/SuperAdmin/Centres/409/CentreRoleLimits", "Customise role limits for 2gether NHS Foundation Trust IT/Clinical Systems")]
        [InlineData("/SuperAdmin/Centres/374/Courses", "Courses - NHS Digital (374)")]
        [InlineData("/SuperAdmin/Centres/374/Courses/Add", "Add courses to centre - NHS Digital (374)")]
        [InlineData("/SuperAdmin/Centres/374/Courses/Add/Other?searchTerm=de", "Add other courses to centre - NHS Digital (374)")]
        [InlineData("/SuperAdmin/Centres/374/Courses/818/ConfirmRemove", "Are you sure you want to remove Assessment attempts from NHS Digital?")]

        [InlineData("/SuperAdmin/Reports", "Platform usage summary")]
        [InlineData("/SuperAdmin/Reports/CourseUsage", "Course usage")]
        [InlineData("/SuperAdmin/Reports/CourseUsage/EditFilters", "Edit report filters")]
        [InlineData("/SuperAdmin/Reports/SelfAssessments/Independent", "Independent self assessments")]
        [InlineData("/SuperAdmin/Reports/SelfAssessments/Independent/EditFilters", "Edit report filters")]
        [InlineData("/SuperAdmin/Reports/SelfAssessments/Supervised", "Supervised self assessments")]
        [InlineData("/SuperAdmin/Reports/SelfAssessments/Supervised/EditFilters", "Edit report filters")]
        [InlineData("/SuperAdmin/System/Faqs", "FAQs")]
        public void SuperAdmin_page_has_no_accessibility_errors(string url, string pageTitle)
        {
            // when
            Driver.Navigate().GoToUrl(BaseUrl + url);

            // then
            AnalyzePageHeadingAndAccessibility(pageTitle);
        }
    }
}
