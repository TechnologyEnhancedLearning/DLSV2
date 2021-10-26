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

            editResult.Violations.Should().HaveCount(3);

            // Expect an axe violation caused by having an aria-expanded attribute on an input
            // The target inputs are nhs-tested components so ignore these violation
            var ariaViolation = editResult.Violations[0];

            ariaViolation.Id.Should().Be("aria-allowed-attr");
            ariaViolation.Nodes.Should().HaveCount(3);
            ariaViolation.Nodes[0].Target.Should().HaveCount(1);
            ariaViolation.Nodes[0].Target[0].Selector.Should().Be("#PasswordProtected");
            ariaViolation.Nodes[1].Target.Should().HaveCount(1);
            ariaViolation.Nodes[1].Target[0].Selector.Should().Be("#ReceiveNotificationEmails");
            ariaViolation.Nodes[2].Target.Should().HaveCount(1);
            ariaViolation.Nodes[2].Target[0].Selector.Should().Be("#OtherCompletionCriteria");

            // Expect two axe violations caused by having no visible label on the CustomisationNameSuffix input
            // The target input is meant to have only a hidden label
            var visibleLabelViolation = editResult.Violations[1];

            visibleLabelViolation.Id.Should().Be("label-title-only");
            visibleLabelViolation.Nodes.Should().HaveCount(1);
            visibleLabelViolation.Nodes[0].Target.Should().HaveCount(1);
            visibleLabelViolation.Nodes[0].Target[0].Selector.Should().Be("#CustomisationNameSuffix");

            var labelViolation = editResult.Violations[2];

            labelViolation.Id.Should().Be("label");
            labelViolation.Nodes.Should().HaveCount(1);
            labelViolation.Nodes[0].Target.Should().HaveCount(1);
            labelViolation.Nodes[0].Target[0].Selector.Should().Be("#CustomisationNameSuffix");
        }
    }
}
