﻿namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
{
    using DigitalLearningSolutions.Web.AutomatedUiTests.TestHelpers;
    using FluentAssertions;
    using Selenium.Axe;
    using Xunit;

    public class AddCentreCustomPromptsAccessibilityTests : AccessibilityTestsBase
    {
        public AddCentreCustomPromptsAccessibilityTests(SeleniumServerFactory<Startup> factory) : base(factory) { }

        [Fact]
        public void AddRegistrationPrompt_journey_has_no_accessibility_errors()
        {
            // given
            Driver.LogUserInAsAdminAndDelegate(BaseUrl);
            const string startUrl = "/TrackingSystem/CentreConfiguration/RegistrationPrompts/Add/New";

            // when
            Driver.Navigate().GoToUrl(BaseUrl + startUrl);
            ValidatePageHeading("Add delegate registration prompt");
            var selectPromptResult = new AxeBuilder(Driver).Analyze();
            Driver.SelectDropdownItemValue("CustomPromptId", "1");
            Driver.SubmitForm();

            ValidatePageHeading("Configure answers");
            var configureAnswerInitialResult = new AxeBuilder(Driver).Analyze();

            AddAnswer("Answer 1");
            AddAnswer("Answer 2");
            ValidatePageHeading("Configure answers");
            var configureAnswerWithAnswersResult = new AxeBuilder(Driver).Analyze();

            Driver.ClickButtonByText("Bulk edit");
            ValidatePageHeading("Configure answers in bulk");
            var bulkAdditionResult = new AxeBuilder(Driver).Analyze();

            Driver.ClickButtonByText("Next");
            Driver.ClickButtonByText("Next");

            ValidatePageHeading("Summary");
            var summaryResult = new AxeBuilder(Driver).Analyze();

            // then
            selectPromptResult.Violations.Should().BeEmpty();
            configureAnswerInitialResult.Violations.Should().BeEmpty();
            configureAnswerWithAnswersResult.Violations.Should().BeEmpty();
            bulkAdditionResult.Violations.Should().BeEmpty();
            summaryResult.Violations.Should().BeEmpty();
        }

        private void AddAnswer(string answerString)
        {
            Driver.FillTextInput("Answer", answerString);
            Driver.ClickButtonByText("Add");
        }
    }
}
