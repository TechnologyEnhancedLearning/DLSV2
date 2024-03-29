﻿namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestFixtures;
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers;
    using FluentAssertions;
    using Selenium.Axe;
    using Xunit;

    public class SelfAssessmentAccessibilityTests : AccessibilityTestsBase,
        IClassFixture<AccessibilityTestsFixture<Startup>>
    {
        public SelfAssessmentAccessibilityTests(AccessibilityTestsFixture<Startup> fixture) : base(fixture) { }

        [Fact]
        public void SelfAssessment_journey_has_no_accessibility_errors()
        {
            // Given
            Driver.LogUserInAsAdminAndDelegate(BaseUrl);
            const string startUrl = "/LearningPortal/SelfAssessment/1";
            const string firstCapabilityUrl = startUrl + "/1";
            const string capabilitiesUrl = startUrl + "/Capabilities";
            const string completeByUrl = startUrl + "/CompleteBy?returnPageQuery=pageNumber%3D1";

            // When
            Driver.Navigate().GoToUrl(BaseUrl + startUrl);
            ValidatePageHeading("Digital Skills Assessment Tool");
            var startPageResult = new AxeBuilder(Driver).Analyze();

            Driver.Navigate().GoToUrl(BaseUrl + firstCapabilityUrl);
            ValidatePageHeading("Data, Information and Content");
            var firstCapabilityResult = new AxeBuilder(Driver).Analyze();

            Driver.Navigate().GoToUrl(BaseUrl + capabilitiesUrl);
            ValidatePageHeading("Digital Skills Assessment Tool - Capabilities");
            var capabilitiesResult = new AxeBuilder(Driver).Analyze();

            Driver.Navigate().GoToUrl(BaseUrl + completeByUrl);
            ValidatePageHeading("Enter a complete by date for Digital Skills Assessment Tool");
            var completeByPageResult = new AxeBuilder(Driver).Analyze();

            //Then
            startPageResult.Violations.Should().BeEmpty();
            firstCapabilityResult.Violations.Should().BeEmpty();
            capabilitiesResult.Violations.Should().BeEmpty();
            completeByPageResult.Violations.Should().BeEmpty();
        }
    }
}
