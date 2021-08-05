﻿namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers;
    using FluentAssertions;
    using Selenium.Axe;
    using Xunit;

    public class EditCourseAdminFieldsAccessibilityTests : AccessibilityTestsBase
    {
        public EditCourseAdminFieldsAccessibilityTests(SeleniumServerFactory<Startup> factory) : base(factory) { }

        [Fact]
        public void EditAdminField_journey_has_no_accessibility_errors()
        {
            // Given
            Driver.LogUserInAsAdminAndDelegate(BaseUrl);
            const string startUrl = "/TrackingSystem/CourseSetup/100/AdminFields/1/Edit";

            // When
            Driver.Navigate().GoToUrl(BaseUrl + startUrl);
            ValidatePageHeading("Edit course admin field");
            var editPageResult = new AxeBuilder(Driver).Analyze();

            Driver.ClickButtonByText("Bulk edit");
            ValidatePageHeading("Configure answers in bulk");
            var bulkAdditionResult = new AxeBuilder(Driver).Analyze();

            Driver.ClickButtonByText("Next");
            ValidatePageHeading("Edit delegate registration prompt");

            editPageResult.Violations.Should().BeEmpty();
            bulkAdditionResult.Violations.Should().BeEmpty();
        }
    }
}
