namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
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
            var expectedPrompt1 = CustomPromptsTestHelper.GetDefaultCustomPrompt(1, options: null, mandatory: true);
            var expectedPrompt2 = CustomPromptsTestHelper.GetDefaultCustomPrompt(2, "Department / team", null, true);
            var customPromts = new List<CustomPrompt> { expectedPrompt1, expectedPrompt2 };
            var expectedCustomerPrompts = CustomPromptsTestHelper.GetDefaultCentreCustomPrompts(customPromts);
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
            // Given
            var answer1 = "Answer 1";
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(answer1: answer1);
            var expectedPrompt1 = CustomPromptsTestHelper.GetDefaultCustomPromptWithAnswer(1, options: null, mandatory: true, answer: answer1);
            var expectedPrompt2 = CustomPromptsTestHelper.GetDefaultCustomPromptWithAnswer(2, "Department / team", null, true);
            var customPrompts = new List<CustomPromptWithAnswer> { expectedPrompt1, expectedPrompt2 };
            var expectedCustomerPrompts = CustomPromptsTestHelper.GetDefaultCentreCustomPromptsWithAnswers(customPrompts);
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
        public void GetCentreCustomPromptsWithAnswersByCentreIdForDelegateUsers_Returns_Populated_Tuple()
        {
            // Given
            const string answer1 = "Answer 1";
            const string answer2 = "Answer 2";
            var delegateUser1 = UserTestHelper.GetDefaultDelegateUser(answer1: answer1);
            var delegateUser2 = UserTestHelper.GetDefaultDelegateUser(answer1: answer2);
            var expectedPrompt1 = CustomPromptsTestHelper.GetDefaultCustomPromptWithAnswer(1, options: null, mandatory: true, answer: answer1);
            var expectedPrompt2 = CustomPromptsTestHelper.GetDefaultCustomPromptWithAnswer(2, "Department / team", null, true);
            var expectedPrompt3 = CustomPromptsTestHelper.GetDefaultCustomPromptWithAnswer(1, options: null, mandatory: true, answer: answer2);
            A.CallTo(() => customPromptsDataService.GetCentreCustomPromptsByCentreId(29))
                .Returns(CustomPromptsTestHelper.GetDefaultCentreCustomPromptsResult(customField1Prompt: "Custom Prompt", customField1Options: null));

            // When
            var result = customPromptsService.GetCentreCustomPromptsWithAnswersByCentreIdForDelegateUsers(29, new [] {delegateUser1, delegateUser2});

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(2);

                var first = result.First();
                first.Item1.Should().BeEquivalentTo(delegateUser1);
                first.Item2.Count.Should().Be(2);
                first.Item2[0].Should().BeEquivalentTo(expectedPrompt1);
                first.Item2[1].Should().BeEquivalentTo(expectedPrompt2);

                var second = result.Last();
                second.Item1.Should().BeEquivalentTo(delegateUser2);
                second.Item2.Count.Should().Be(2);
                second.Item2[0].Should().BeEquivalentTo(expectedPrompt3);
                second.Item2[1].Should().BeEquivalentTo(expectedPrompt2);
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
