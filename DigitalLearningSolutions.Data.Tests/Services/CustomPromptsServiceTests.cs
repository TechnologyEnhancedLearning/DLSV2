namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class CustomPromptsServiceTests
    {
        private ICustomPromptsService customPromptsService;
        private ICustomPromptsDataService customPromptsDataService;

        [SetUp]
        public void Setup()
        {
            customPromptsDataService = A.Fake<ICustomPromptsDataService>();
            customPromptsService = new CustomPromptsService(customPromptsDataService);
        }

        [Test]
        public void GetCustomPromptsForCentreByCentreId_Returns_Populated_CentreCustomPrompts()
        {
            // Given
            var expectedPrompt1 = CustomPromptsTestHelper.GetDefaultCustomPrompt(1,options: null, mandatory: true);
            var expectedPrompt2 = CustomPromptsTestHelper.GetDefaultCustomPrompt(2,text: "Department / team", options: null, mandatory: true);
            var expectedCustomerPrompts = CustomPromptsTestHelper.GetDefaultCentreCustomPrompts(customPrompt1: expectedPrompt1, customPrompt2: expectedPrompt2);
            A.CallTo(() => customPromptsDataService.GetCentreCustomPromptsByCentreId(29))
                .Returns(CustomPromptsTestHelper.GetDefaultCentreCustomPromptsResult(customField1Prompt: "Custom Prompt", customField1Options: null));

            // When
            var result = customPromptsService.GetCustomPromptsForCentreByCentreId(29);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeEquivalentTo(expectedCustomerPrompts);
                result.CustomPrompts.Count.Should().Be(2);
                result.CustomPrompts[0].Should().BeEquivalentTo(expectedPrompt1);
                result.CustomPrompts[1].Should().BeEquivalentTo(expectedPrompt2);
            }
        }

        [Test]
        public void GetCentreCustomPromptsWithAnswersByCentreIdAndDelegateUser_Returns_Populated_CentreCustomPrompts()
        {
            // Given#
            var answer1 = "Answer 1";
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(answer1: answer1);
            var expectedPrompt1 = CustomPromptsTestHelper.GetDefaultCustomPromptWithAnswer(1, options: null, mandatory: true, answer: answer1);
            var expectedPrompt2 = CustomPromptsTestHelper.GetDefaultCustomPromptWithAnswer(2, text: "Department / team", options: null, mandatory: true);
            var expectedCustomerPrompts = CustomPromptsTestHelper.GetDefaultCentreCustomPromptsWithAnswers(customPrompt1: expectedPrompt1, customPrompt2: expectedPrompt2);
            A.CallTo(() => customPromptsDataService.GetCentreCustomPromptsByCentreId(29))
                .Returns(CustomPromptsTestHelper.GetDefaultCentreCustomPromptsResult(customField1Prompt: "Custom Prompt", customField1Options: null));

            // When
            var result = customPromptsService.GetCentreCustomPromptsWithAnswersByCentreIdAndDelegateUser(29, delegateUser);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeEquivalentTo(expectedCustomerPrompts);
                result.CustomPrompts.Count.Should().Be(2);
                result.CustomPrompts[0].Should().BeEquivalentTo(expectedPrompt1);
                result.CustomPrompts[1].Should().BeEquivalentTo(expectedPrompt2);
            }
        }

        [Test]
        public void GetCustomPrompts_with_options_splits_correctly()
        {
            // Given
            A.CallTo(() => customPromptsDataService.GetCentreCustomPromptsByCentreId(29))
                .Returns(CustomPromptsTestHelper.GetDefaultCentreCustomPromptsResult());

            // When
            var result = customPromptsService.GetCustomPromptsForCentreByCentreId(29);

            // Then
            using (new AssertionScope())
            {
                result.CustomPrompts.Should().NotBeNull();
                result.CustomPrompts[0].Options.Count.Should().Be(2);
                result.CustomPrompts[0].Options[0].Should().BeEquivalentTo("Clinical");
                result.CustomPrompts[0].Options[1].Should().BeEquivalentTo("Non-Clinical");
            }
        }
    }
}
