namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestFixtures;
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers;
    using FluentAssertions;
    using Selenium.Axe;
    using Xunit;

    public class EditCourseDetailsAccessibilityTests : AccessibilityTestsBase,
    IClassFixture<AccessibilityTestsFixture<Startup>>
    {
        public EditCourseDetailsAccessibilityTests(AccessibilityTestsFixture<Startup> fixture) : base(fixture) { }

        [Fact]
        public void Edit_course_details_page_has_no_accessibility_errors_except_expected_ones()
        {
            // given
            Driver.LogUserInAsAdminAndDelegate(BaseUrl);
            const string editCourseDetailsUrl = "/TrackingSystem/CourseSetup/10716/Manage/CourseDetails";

            // when
            Driver.Navigate().GoToUrl(BaseUrl + editCourseDetailsUrl);
            ValidatePageHeading("Edit course details");
            var editResult = new AxeBuilder(Driver).Analyze();

            // Expect an axe violation caused by having an aria-expanded attribute on an input
            // The target inputs are nhs-tested components so ignore these violation
            editResult.Violations.Should().HaveCount(1);
            var violation = editResult.Violations[0];

            violation.Id.Should().Be("aria-allowed-attr");
            violation.Nodes.Should().HaveCount(3);
            violation.Nodes[0].Target.Should().HaveCount(1);
            violation.Nodes[0].Target[0].Selector.Should().Be("#PasswordProtected");
            violation.Nodes[1].Target.Should().HaveCount(1);
            violation.Nodes[1].Target[0].Selector.Should().Be("#ReceiveNotificationEmails");
            violation.Nodes[2].Target.Should().HaveCount(1);
            violation.Nodes[2].Target[0].Selector.Should().Be("#OtherCompletionCriteria");
        }
    }
}
