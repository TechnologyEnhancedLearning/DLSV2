﻿namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class CustomPromptsServiceTests
    {
        private ICustomPromptsDataService customPromptsDataService = null!;
        private ICustomPromptsService customPromptsService = null!;
        private ILogger<CustomPromptsService> logger = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void Setup()
        {
            customPromptsDataService = A.Fake<ICustomPromptsDataService>();
            logger = A.Fake<ILogger<CustomPromptsService>>();
            userDataService = A.Fake<IUserDataService>();
            customPromptsService = new CustomPromptsService(customPromptsDataService, logger, userDataService);
        }

        [Test]
        public void GetCustomPromptsForCentreByCentreId_Returns_Populated_CentreCustomPrompts()
        {
            // Given
            var expectedPrompt1 = CustomPromptsTestHelper.GetDefaultCustomPrompt(1, options: null, mandatory: true);
            var expectedPrompt2 = CustomPromptsTestHelper.GetDefaultCustomPrompt(2, "Department / team", null, true);
            var customPrompts = new List<CustomPrompt> { expectedPrompt1, expectedPrompt2 };
            var expectedCustomerPrompts = CustomPromptsTestHelper.GetDefaultCentreCustomPrompts(customPrompts);
            A.CallTo(() => customPromptsDataService.GetCentreCustomPromptsByCentreId(29))
                .Returns
                (
                    CustomPromptsTestHelper.GetDefaultCentreCustomPromptsResult(
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
            // Given
            var answer1 = "Answer 1";
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(answer1: answer1);
            var expectedPrompt1 =
                CustomPromptsTestHelper.GetDefaultCustomPromptWithAnswer(
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
                    CustomPromptsTestHelper.GetDefaultCentreCustomPromptsResult(
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
        public void GetCentreCustomPromptsWithAnswersByCentreIdForDelegateUsers_Returns_Populated_Tuple()
        {
            // Given
            const string answer1 = "Answer 1";
            const string answer2 = "Answer 2";
            var delegateUser1 = UserTestHelper.GetDefaultDelegateUser(answer1: answer1);
            var delegateUser2 = UserTestHelper.GetDefaultDelegateUser(answer1: answer2);
            var expectedPrompt1 = CustomPromptsTestHelper.GetDefaultCustomPromptWithAnswer(
                1,
                options: null,
                mandatory: true,
                answer: answer1
            );
            var expectedPrompt2 =
                CustomPromptsTestHelper.GetDefaultCustomPromptWithAnswer(2, "Department / team", null, true);
            var expectedPrompt3 = CustomPromptsTestHelper.GetDefaultCustomPromptWithAnswer(
                1,
                options: null,
                mandatory: true,
                answer: answer2
            );
            A.CallTo(() => customPromptsDataService.GetCentreCustomPromptsByCentreId(29))
                .Returns(
                    CustomPromptsTestHelper.GetDefaultCentreCustomPromptsResult(
                        customField1Prompt: "Custom Prompt",
                        customField1Options: null
                    )
                );

            // When
            var result = customPromptsService.GetCentreCustomPromptsWithAnswersByCentreIdForDelegateUsers(
                29,
                new[] { delegateUser1, delegateUser2 }
            );

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

        [Test]
        public void AddCustomPromptToCentre_add_prompt_at_lowest_possible_prompt_number()
        {
            // Given
            A.CallTo
            (
                () => customPromptsDataService.UpdateCustomPromptForCentre(1, A<int>._, 1, true, null)
            ).DoesNothing();
            A.CallTo(() => customPromptsDataService.GetCentreCustomPromptsByCentreId(1))
                .Returns
                (
                    CustomPromptsTestHelper.GetDefaultCentreCustomPromptsResult(
                        customField1Prompt: "Prompt",
                        customField2Prompt: "Prompt",
                        customField3Prompt: null,
                        customField4Prompt: null,
                        customField5Prompt: null,
                        customField6Prompt: null
                    )
                );

            // When
            var result = customPromptsService.AddCustomPromptToCentre(1, 1, true, null);

            // Then
            A.CallTo
            (
                () => customPromptsDataService.UpdateCustomPromptForCentre(1, 3, 1, true, null)
            ).MustHaveHappened();
            result.Should().BeTrue();
        }

        [Test]
        public void AddCustomPromptToCentre_add_prompt_at_lowest_possible_prompt_number_with_gaps_in_prompt_numbers()
        {
            // Given
            A.CallTo
            (
                () => customPromptsDataService.UpdateCustomPromptForCentre(1, A<int>._, 1, true, null)
            ).DoesNothing();
            A.CallTo(() => customPromptsDataService.GetCentreCustomPromptsByCentreId(1))
                .Returns
                (
                    CustomPromptsTestHelper.GetDefaultCentreCustomPromptsResult(
                        customField1Prompt: "Prompt",
                        customField2Prompt: null,
                        customField3Prompt: "Prompt",
                        customField4Prompt: "Prompt",
                        customField5Prompt: "Prompt",
                        customField6Prompt: null
                    )
                );

            // When
            var result = customPromptsService.AddCustomPromptToCentre(1, 1, true, null);

            // Then
            A.CallTo
            (
                () => customPromptsDataService.UpdateCustomPromptForCentre(1, 2, 1, true, null)
            ).MustHaveHappened();
            result.Should().BeTrue();
        }

        [Test]
        public void AddCustomPromptToCentre_does_not_add_prompt_if_centre_has_all_prompts_defined()
        {
            // Given
            A.CallTo
            (
                () => customPromptsDataService.UpdateCustomPromptForCentre(1, A<int>._, 1, true, null)
            ).DoesNothing();
            A.CallTo(() => customPromptsDataService.GetCentreCustomPromptsByCentreId(1))
                .Returns
                (
                    CustomPromptsTestHelper.GetDefaultCentreCustomPromptsResult(
                        customField1Prompt: "Prompt",
                        customField2Prompt: "Prompt",
                        customField3Prompt: "Prompt",
                        customField4Prompt: "Prompt",
                        customField5Prompt: "Prompt",
                        customField6Prompt: "Prompt"
                    )
                );

            // When
            var result = customPromptsService.AddCustomPromptToCentre(1, 1, true, null);

            // Then
            A.CallTo(() => customPromptsDataService.UpdateCustomPromptForCentre(1, A<int>._, 1, true, null))
                .MustNotHaveHappened();
            result.Should().BeFalse();
        }

        [Test]
        public void RemoveCustomPromptFromCentre_calls_data_service_with_correct_values()
        {
            // Given
            A.CallTo(() => customPromptsDataService.UpdateCustomPromptForCentre(1, 1, 0, false, null)).DoesNothing();
            A.CallTo(() => userDataService.DeleteAllAnswersForPrompt(1, 1)).DoesNothing();

            // When
            customPromptsService.RemoveCustomPromptFromCentre(1, 1);

            // Then
            A.CallTo(
                () => customPromptsDataService.UpdateCustomPromptForCentre(1, 1, 0, false, null)
            ).MustHaveHappened();
            A.CallTo(() => userDataService.DeleteAllAnswersForPrompt(1, 1)).MustHaveHappened();
        }

        [Test]
        public void GetCustomPromptsForCourse_Returns_Populated_CourseCustomPrompts()
        {
            // Given
            var expectedPrompt1 = CustomPromptsTestHelper.GetDefaultCustomPrompt(1, "System Access Granted", "Yes\r\nNo");
            var expectedPrompt2 = CustomPromptsTestHelper.GetDefaultCustomPrompt(2, "Access Permissions");
            var customPrompts = new List<CustomPrompt> { expectedPrompt1, expectedPrompt2 };
            var expectedCoursePrompts = CustomPromptsTestHelper.GetDefaultCourseCustomPrompts(customPrompts);
            A.CallTo(() => customPromptsDataService.GetCourseCustomPrompts(27920, 101, 0))
                .Returns(CustomPromptsTestHelper.GetDefaultCourseCustomPromptsResult());

            // When
            var result = customPromptsService.GetCustomPromptsForCourse(27920, 101, 0);

            // Then
            result.Should().BeEquivalentTo(expectedCoursePrompts);
        }
    }
}
