namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class CustomPromptsServiceTests
    {
        private ICustomPromptsDataService customPromptsDataService = null!;
        private ICustomPromptsService customPromptsService = null!;

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
                .Returns
                (
                    CustomPromptsTestHelper.GetDefaultCentreCustomPromptsResult
                    (
                        customField1Prompt: "Custom Prompt",
                        customField1Options: null
                    )
                );

            // When
            var result = customPromptsService.GetCustomPromptsForCentreByCentreId(29);

            // Then
            result.Should().BeEquivalentTo(expectedCustomerPrompts);
        }

        [Test]
        public void GetCentreCustomPromptsWithAnswersByCentreIdAndDelegateUser_Returns_Populated_CentreCustomPrompts()
        {
            // Given#
            var answer1 = "Answer 1";
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(answer1: answer1);
            var expectedPrompt1 =
                CustomPromptsTestHelper.GetDefaultCustomPromptWithAnswer
                (
                    1,
                    options: null,
                    mandatory: true,
                    answer: answer1
                );
            var expectedPrompt2 =
                CustomPromptsTestHelper.GetDefaultCustomPromptWithAnswer(2, "Department / team", null, true);
            var customPrompts = new List<CustomPromptWithAnswer> { expectedPrompt1, expectedPrompt2 };
            var expectedCustomerPrompts =
                CustomPromptsTestHelper.GetDefaultCentreCustomPromptsWithAnswers(customPrompts);
            A.CallTo(() => customPromptsDataService.GetCentreCustomPromptsByCentreId(29))
                .Returns
                (
                    CustomPromptsTestHelper.GetDefaultCentreCustomPromptsResult
                    (
                        customField1Prompt: "Custom Prompt",
                        customField1Options: null
                    )
                );

            // When
            var result =
                customPromptsService.GetCentreCustomPromptsWithAnswersByCentreIdAndDelegateUser(29, delegateUser);

            // Then
            result.Should().BeEquivalentTo(expectedCustomerPrompts);
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

        [Test]
        public void UpdateCustomPromptForCentre_call_data_service()
        {
            // Given
            A.CallTo(() => customPromptsDataService.UpdateCustomPromptForCentre(1, 1, true, null)).DoesNothing();

            // When
            customPromptsService.UpdateCustomPromptForCentre(1, 1, true, null);

            // Then
            A.CallTo(() => customPromptsDataService.UpdateCustomPromptForCentre(1, 1, true, null)).MustHaveHappened();
        }

        [Test]
        public void GetCustomPromptsAlphabeticalList_calls_data_service()
        {
            // Given
            const string promptName = "Department / team";
            A.CallTo(() => customPromptsDataService.GetCustomPromptsAlphabetical()).Returns
                (new List<(int, string)> { (1, promptName) });

            // When
            var result = customPromptsService.GetCustomPromptsAlphabeticalList();

            // Then
            A.CallTo(() => customPromptsDataService.GetCustomPromptsAlphabetical()).MustHaveHappened();
            result.Contains((1, promptName)).Should().BeTrue();
        }
    }
}
