namespace DigitalLearningSolutions.Web.AutomatedUiTests.AccessibilityTests
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
            const string startUrl = "/TrackingSystem/CentreConfiguration/RegistrationPrompts/Add/SelectPrompt";

            // when
            Driver.Navigate().GoToUrl(BaseUrl + startUrl);
            AnalyzePageHeading("Add delegate registration prompt");
            var selectPromptResult = new AxeBuilder(Driver).Analyze();
            Driver.SelectDropdownItemValue("CustomPromptId", "1");
            Driver.SubmitForm();

            AnalyzePageHeading("Configure answers");
            var configureAnswerInitialResult = new AxeBuilder(Driver).Analyze();

            AddAnswer("Answer 1");
            AddAnswer("Answer 2");
            AnalyzePageHeading("Configure answers");
            var configureAnswerWithAnswersResult = new AxeBuilder(Driver).Analyze();

            Driver.ClickButtonByText("Next");

            AnalyzePageHeading("Summary");
            var summaryResult = new AxeBuilder(Driver).Analyze();
            Driver.SubmitForm();

            // then
            selectPromptResult.Violations.Should().BeEmpty();
            configureAnswerInitialResult.Violations.Should().BeEmpty();
            configureAnswerWithAnswersResult.Violations.Should().BeEmpty();
            summaryResult.Violations.Should().BeEmpty();
        }

        private void AddAnswer(string answerString)
        {
            Driver.FillTextInput(answerString, "Answer");
            Driver.ClickButtonByText("Add");
        }
    }
}
