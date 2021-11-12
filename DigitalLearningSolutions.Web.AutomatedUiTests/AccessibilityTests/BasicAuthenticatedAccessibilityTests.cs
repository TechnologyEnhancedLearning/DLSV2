namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestFixtures;
    using Xunit;

    public class BasicAuthenticatedAccessibilityTests : AccessibilityTestsBase,
        IClassFixture<AuthenticatedAccessibilityTestsFixture<Startup>>
    {
        public BasicAuthenticatedAccessibilityTests(AuthenticatedAccessibilityTestsFixture<Startup> fixture) : base(
            fixture
        ) { }

        [Theory]
        [InlineData("/MyAccount", "My account")]
        [InlineData("/MyAccount/EditDetails", "Edit details")]
        [InlineData("/TrackingSystem/Centre/Administrators", "Centre administrators")]
        [InlineData("/TrackingSystem/Centre/Administrators/1/EditAdminRoles", "Edit administrator roles")]
        [InlineData("/TrackingSystem/Centre/Administrators/1/DeactivateAdmin", "Are you sure you would like to deactivate this admin account?")]
        [InlineData("/TrackingSystem/Centre/Dashboard", "Centre dashboard")]
        [InlineData("/TrackingSystem/Centre/Ranking", "Centre ranking")]
        [InlineData("/TrackingSystem/Centre/ContractDetails", "Contract details")]
        [InlineData("/TrackingSystem/Centre/Configuration", "Centre configuration")]
        [InlineData("/TrackingSystem/Centre/Configuration/EditCentreDetails", "Edit centre details")]
        [InlineData("/TrackingSystem/Centre/Configuration/EditCentreManagerDetails", "Edit centre manager details")]
        [InlineData(
            "/TrackingSystem/Centre/Configuration/EditCentreWebsiteDetails",
            "Edit centre content on DLS website"
        )]
        [InlineData("/TrackingSystem/Centre/Configuration/RegistrationPrompts", "Manage delegate registration prompts")]
        [InlineData(
            "/TrackingSystem/Centre/Configuration/RegistrationPrompts/1/Remove",
            "Remove delegate registration prompt"
        )]
        [InlineData("/TrackingSystem/Centre/Reports", "Centre reports")]
        [InlineData("/TrackingSystem/Centre/SystemNotifications", "New system notifications")]
        [InlineData("/TrackingSystem/Centre/TopCourses", "Top courses")]
        [InlineData("/TrackingSystem/CourseSetup", "Centre course setup")]
        [InlineData("/TrackingSystem/CourseSetup/10716/AdminFields", "Manage course admin fields")]
        [InlineData(
            "/TrackingSystem/CourseSetup/100/AdminFields/1/Remove",
            "Delete course admin field"
        )]
        [InlineData("/TrackingSystem/CourseSetup/10716/Content", "Course content")]
        [InlineData("/TrackingSystem/CourseSetup/10716/Content/EditSection/203", "Edit section content")]
        [InlineData("/TrackingSystem/CourseSetup/10716/Manage", "Level 1 - Microsoft Excel 2010 - Inductions")]
        [InlineData(
            "/TrackingSystem/CourseSetup/10716/Manage/LearningPathwayDefaults",
            "Edit Learning Pathway defaults"
        )]
        [InlineData("/TrackingSystem/CourseSetup/10716/Manage/EditCourseOptions", "Edit course options")]
        [InlineData("/TrackingSystem/Delegates/All", "Delegates")]
        [InlineData("/TrackingSystem/Delegates/Groups", "Groups")]
        [InlineData("/TrackingSystem/Delegates/Groups/5/Delegates", "Group delegates")]
        [InlineData("/TrackingSystem/Delegates/Groups/5/EditDescription", "Edit description for Activities worker or coordinator group (optional)")]
        [InlineData("/TrackingSystem/Delegates/Groups/5/EditGroupName", "Edit group name")]

        [InlineData(
            "/TrackingSystem/Delegates/Groups/5/Delegates/Remove/245969",
            "Are you sure you would like to remove xxxxx xxxx from this group?"
        )]
        [InlineData("/TrackingSystem/Delegates/Groups/5/Courses", "Group courses")]
        [InlineData("/TrackingSystem/Delegates/Groups/Add", "Add new delegate group")]
        [InlineData("/TrackingSystem/Delegates/3/View", "xxxx xxxxxx")]
        [InlineData("/TrackingSystem/Delegates/3/Edit", "Edit delegate details")]
        [InlineData("/TrackingSystem/Delegates/3/SetPassword", "Set delegate user password")]
        [InlineData(
            "/TrackingSystem/Delegates/3/View/100/Remove",
            "Remove delegate enrolment from Entry Level - Win XP, Office 2003/07 OLD - Standard"
        )]
        [InlineData("/TrackingSystem/Delegates/Approve", "Approve delegate registrations")]
        [InlineData("/TrackingSystem/Delegates/BulkUpload", "Bulk upload/update delegates")]
        [InlineData("/TrackingSystem/Delegates/Email", "Send welcome messages")]
        [InlineData("/TrackingSystem/Delegates/CourseDelegates", "Course delegates")]
        [InlineData("/TrackingSystem/Delegates/CourseDelegates/DelegateProgress/243104", "Delegate progress")]
        [InlineData(
            "/TrackingSystem/Delegates/CourseDelegates/DelegateProgress/243104/EditSupervisor",
            "Edit supervisor for Digital Literacy for the Workplace - CC Test"
        )]
        [InlineData("/NotificationPreferences", "Notification preferences")]
        [InlineData("/NotificationPreferences/Edit/AdminUser", "Update notification preferences")]
        [InlineData("/NotificationPreferences/Edit/DelegateUser", "Update notification preferences")]
        [InlineData("/ChangePassword", "Change password")]
        [InlineData("/TrackingSystem/Support", "Support")]
        public void Authenticated_page_has_no_accessibility_errors(string url, string pageTitle)
        {
            // when
            Driver.Navigate().GoToUrl(BaseUrl + url);

            // then
            AnalyzePageHeadingAndAccessibility(pageTitle);
        }
    }
}
