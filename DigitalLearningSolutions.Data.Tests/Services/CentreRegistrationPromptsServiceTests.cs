namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class CentreRegistrationPromptsServiceTests
    {
        private ICentreRegistrationPromptsDataService centreRegistrationPromptsDataService = null!;
        private ICentreRegistrationPromptsService centreRegistrationPromptsService = null!;
        private ILogger<CentreRegistrationPromptsService> logger = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void Setup()
        {
            centreRegistrationPromptsDataService = A.Fake<ICentreRegistrationPromptsDataService>();
            logger = A.Fake<ILogger<CentreRegistrationPromptsService>>();
            userDataService = A.Fake<IUserDataService>();
            centreRegistrationPromptsService = new CentreRegistrationPromptsService(
                centreRegistrationPromptsDataService,
                logger,
                userDataService
            );
        }

        [Test]
        public void GetCentreRegistrationPromptsByCentreId_Returns_Populated_CentreRegistrationPrompts()
        {
            // Given
            var expectedPrompt1 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(1, options: null, mandatory: true);
            var expectedPrompt2 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(2, "Department / team", null, true, 2);
            var centreRegistrationPrompts = new List<CentreRegistrationPrompt> { expectedPrompt1, expectedPrompt2 };
            var expectedPrompts = PromptsTestHelper.GetDefaultCentreRegistrationPrompts(centreRegistrationPrompts);
            A.CallTo(() => centreRegistrationPromptsDataService.GetCentreRegistrationPromptsByCentreId(29))
                .Returns
                (
                    PromptsTestHelper.GetDefaultCentreRegistrationPromptsResult(
                        centreRegistrationPrompt1Prompt: "Custom Prompt",
                        centreRegistrationPrompt1Options: null
                    )
                );

            // When
            var result = centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(29);

            // Then
            result.Should().BeEquivalentTo(expectedPrompts);
        }

        [Test]
        public void GetCentreRegistrationPromptsThatHaveOptionsByCentreId_only_returns_prompts_with_options()
        {
            // Given
            const int centreId = 29;
            var expectedPrompt = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(
                1,
                "Group",
                "Clinical\r\nNon-Clinical",
                true
            );
            var expectedPrompts = new List<CentreRegistrationPrompt> { expectedPrompt };

            A.CallTo(() => centreRegistrationPromptsDataService.GetCentreRegistrationPromptsByCentreId(centreId))
                .Returns(PromptsTestHelper.GetDefaultCentreRegistrationPromptsResult());

            // When
            var result = centreRegistrationPromptsService.GetCentreRegistrationPromptsThatHaveOptionsByCentreId(centreId);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => centreRegistrationPromptsDataService.GetCentreRegistrationPromptsByCentreId(centreId))
                    .MustHaveHappenedOnceExactly();
                result.Should().BeEquivalentTo(expectedPrompts);
            }
        }

        [Test]
        public void GetCentreRegistrationPromptsWithAnswersByCentreIdAndDelegateUser_Returns_Populated_CentreRegistrationPrompts()
        {
            // Given
            var answer1 = "Answer 1";
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(answer1: answer1);
            var expectedPrompt1 =
                PromptsTestHelper.GetDefaultCentreRegistrationPromptWithAnswer(
                    1,
                    options: null,
                    mandatory: true,
                    answer: answer1
                );
            var expectedPrompt2 =
                PromptsTestHelper.GetDefaultCentreRegistrationPromptWithAnswer(2, "Department / team", null, true, promptId: 2);
            var centreRegistrationPrompts = new List<CentreRegistrationPromptWithAnswer> { expectedPrompt1, expectedPrompt2 };
            var expectedCustomerPrompts =
                PromptsTestHelper.GetDefaultCentreRegistrationPromptsWithAnswers(centreRegistrationPrompts);
            A.CallTo(() => centreRegistrationPromptsDataService.GetCentreRegistrationPromptsByCentreId(29))
                .Returns
                (
                    PromptsTestHelper.GetDefaultCentreRegistrationPromptsResult(
                        centreRegistrationPrompt1Prompt: "Custom Prompt",
                        centreRegistrationPrompt1Options: null
                    )
                );

            // When
            var result =
                centreRegistrationPromptsService.GetCentreRegistrationPromptsWithAnswersByCentreIdAndDelegateUser(29, delegateUser);

            // Then
            result.Should().BeEquivalentTo(expectedCustomerPrompts);
        }

        [Test]
        public void GetCentreRegistrationPromptsWithAnswersByCentreIdForDelegateUsers_Returns_Populated_Tuple()
        {
            // Given
            const string answer1 = "Answer 1";
            const string answer2 = "Answer 2";
            var delegateUser1 = UserTestHelper.GetDefaultDelegateUser(answer1: answer1);
            var delegateUser2 = UserTestHelper.GetDefaultDelegateUser(answer1: answer2);
            var expectedPrompt1 = PromptsTestHelper.GetDefaultCentreRegistrationPromptWithAnswer(
                1,
                options: null,
                mandatory: true,
                answer: answer1
            );
            var expectedPrompt2 =
                PromptsTestHelper.GetDefaultCentreRegistrationPromptWithAnswer(2, "Department / team", null, true, promptId: 2);
            var expectedPrompt3 = PromptsTestHelper.GetDefaultCentreRegistrationPromptWithAnswer(
                1,
                options: null,
                mandatory: true,
                answer: answer2
            );
            A.CallTo(() => centreRegistrationPromptsDataService.GetCentreRegistrationPromptsByCentreId(29))
                .Returns(
                    PromptsTestHelper.GetDefaultCentreRegistrationPromptsResult(
                        centreRegistrationPrompt1Prompt: "Custom Prompt",
                        centreRegistrationPrompt1Options: null
                    )
                );

            // When
            var result = centreRegistrationPromptsService.GetCentreRegistrationPromptsWithAnswersByCentreIdForDelegateUsers(
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
        public void GetCentreRegistrationPromptsByCentreId_with_options_splits_correctly()
        {
            // Given
            A.CallTo(() => centreRegistrationPromptsDataService.GetCentreRegistrationPromptsByCentreId(29))
                .Returns(PromptsTestHelper.GetDefaultCentreRegistrationPromptsResult());

            // When
            var result = centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(29);

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
        public void UpdateCentreRegistrationPrompt_calls_data_service()
        {
            // Given
            A.CallTo(() => centreRegistrationPromptsDataService.UpdateCentreRegistrationPrompt(1, 1, true, null)).DoesNothing();

            // When
            centreRegistrationPromptsService.UpdateCentreRegistrationPrompt(1, 1, true, null);

            // Then
            A.CallTo(() => centreRegistrationPromptsDataService.UpdateCentreRegistrationPrompt(1, 1, true, null))
                .MustHaveHappened();
        }

        [Test]
        public void GetCentreRegistrationPromptsAlphabeticalList_calls_data_service()
        {
            // Given
            const string promptName = "Department / team";
            A.CallTo(() => centreRegistrationPromptsDataService.GetCustomPromptsAlphabetical()).Returns
                (new List<(int, string)> { (1, promptName) });

            // When
            var result = centreRegistrationPromptsService.GetCentreRegistrationPromptsAlphabeticalList();

            // Then
            A.CallTo(() => centreRegistrationPromptsDataService.GetCustomPromptsAlphabetical()).MustHaveHappened();
            result.Contains((1, promptName)).Should().BeTrue();
        }

        [Test]
        public void AddCentreRegistrationPrompt_add_prompt_at_lowest_possible_prompt_number()
        {
            // Given
            A.CallTo
            (
                () => centreRegistrationPromptsDataService.UpdateCentreRegistrationPrompt(1, A<int>._, 1, true, null)
            ).DoesNothing();
            A.CallTo(() => centreRegistrationPromptsDataService.GetCentreRegistrationPromptsByCentreId(1))
                .Returns
                (
                    PromptsTestHelper.GetDefaultCentreRegistrationPromptsResult(
                        centreRegistrationPrompt1Prompt: "Prompt",
                        centreRegistrationPrompt2Prompt: "Prompt",
                        centreRegistrationPrompt3Prompt: null,
                        centreRegistrationPrompt4Prompt: null,
                        centreRegistrationPrompt5Prompt: null,
                        centreRegistrationPrompt6Prompt: null
                    )
                );

            // When
            var result = centreRegistrationPromptsService.AddCentreRegistrationPrompt(1, 1, true, null);

            // Then
            A.CallTo
            (
                () => centreRegistrationPromptsDataService.UpdateCentreRegistrationPrompt(1, 3, 1, true, null)
            ).MustHaveHappened();
            result.Should().BeTrue();
        }

        [Test]
        public void AddCentreRegistrationPrompt_adds_prompt_at_lowest_possible_prompt_number_with_gaps_in_prompt_numbers()
        {
            // Given
            A.CallTo
            (
                () => centreRegistrationPromptsDataService.UpdateCentreRegistrationPrompt(1, A<int>._, 1, true, null)
            ).DoesNothing();
            A.CallTo(() => centreRegistrationPromptsDataService.GetCentreRegistrationPromptsByCentreId(1))
                .Returns
                (
                    PromptsTestHelper.GetDefaultCentreRegistrationPromptsResult(
                        centreRegistrationPrompt1Prompt: "Prompt",
                        centreRegistrationPrompt2Prompt: null,
                        centreRegistrationPrompt3Prompt: "Prompt",
                        centreRegistrationPrompt4Prompt: "Prompt",
                        centreRegistrationPrompt5Prompt: "Prompt",
                        centreRegistrationPrompt6Prompt: null
                    )
                );

            // When
            var result = centreRegistrationPromptsService.AddCentreRegistrationPrompt(1, 1, true, null);

            // Then
            A.CallTo
            (
                () => centreRegistrationPromptsDataService.UpdateCentreRegistrationPrompt(1, 2, 1, true, null)
            ).MustHaveHappened();
            result.Should().BeTrue();
        }

        [Test]
        public void AddCentreRegistrationPrompt_does_not_add_prompt_if_centre_has_all_prompts_defined()
        {
            // Given
            A.CallTo
            (
                () => centreRegistrationPromptsDataService.UpdateCentreRegistrationPrompt(1, A<int>._, 1, true, null)
            ).DoesNothing();
            A.CallTo(() => centreRegistrationPromptsDataService.GetCentreRegistrationPromptsByCentreId(1))
                .Returns
                (
                    PromptsTestHelper.GetDefaultCentreRegistrationPromptsResult(
                        centreRegistrationPrompt1Prompt: "Prompt",
                        centreRegistrationPrompt2Prompt: "Prompt",
                        centreRegistrationPrompt3Prompt: "Prompt",
                        centreRegistrationPrompt4Prompt: "Prompt",
                        centreRegistrationPrompt5Prompt: "Prompt",
                        centreRegistrationPrompt6Prompt: "Prompt"
                    )
                );

            // When
            var result = centreRegistrationPromptsService.AddCentreRegistrationPrompt(1, 1, true, null);

            // Then
            A.CallTo(() => centreRegistrationPromptsDataService.UpdateCentreRegistrationPrompt(1, A<int>._, 1, true, null))
                .MustNotHaveHappened();
            result.Should().BeFalse();
        }

        [Test]
        public void RemoveCentreRegistrationPrompt_calls_data_service_with_correct_values()
        {
            // Given
            A.CallTo(() => centreRegistrationPromptsDataService.UpdateCentreRegistrationPrompt(1, 1, 0, false, null))
                .DoesNothing();
            A.CallTo(() => userDataService.DeleteAllAnswersForPrompt(1, 1)).DoesNothing();

            // When
            centreRegistrationPromptsService.RemoveCentreRegistrationPrompt(1, 1);

            // Then
            A.CallTo(
                () => centreRegistrationPromptsDataService.UpdateCentreRegistrationPrompt(1, 1, 0, false, null)
            ).MustHaveHappened();
            A.CallTo(() => userDataService.DeleteAllAnswersForPrompt(1, 1)).MustHaveHappened();
        }
    }
}
