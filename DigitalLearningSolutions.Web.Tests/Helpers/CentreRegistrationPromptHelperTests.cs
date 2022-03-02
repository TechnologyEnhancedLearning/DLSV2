namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using NUnit.Framework;

    public class CentreRegistrationPromptHelperTests
    {
        private const string Answer1 = "Answer1";
        private const string Answer2 = "Answer2";
        private const string Answer3 = "Answer3";
        private PromptHelper promptHelper = null!;
        private ICentreRegistrationPromptsService centreRegistrationPromptsService = null!;

        [SetUp]
        public void Setup()
        {
            centreRegistrationPromptsService = A.Fake<ICentreRegistrationPromptsService>();
            promptHelper = new PromptHelper(centreRegistrationPromptsService);
        }

        [Test]
        public void GetEditDelegateRegistrationPromptViewModelsForCentre_returns_populated_list()
        {
            // Given
            var registrationPrompt1 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(1, options: "Clinical\r\nNon-Clinical");
            var registrationPrompt2 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(2);
            var centreRegistrationPrompts = PromptsTestHelper.GetDefaultCentreRegistrationPrompts(
                new List<CentreRegistrationPrompt> { registrationPrompt1, registrationPrompt2 },
                1
            );
            A.CallTo(() => centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(1))
                .Returns(centreRegistrationPrompts);

            // When
            var result =
                promptHelper.GetEditDelegateRegistrationPromptViewModelsForCentre(
                    1,
                    Answer1,
                    Answer2,
                    null,
                    null,
                    null,
                    null
                );

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(2);

                result[0].PromptNumber.Should().Be(1);
                result[0].Options.Count().Should().Be(2);
                result[0].Options.First().Value.Should().BeEquivalentTo("Clinical");

                result[1].PromptNumber.Should().Be(2);
                result[1].Options.Count().Should().Be(0);
            }
        }

        [Test]
        public void GetDelegateRegistrationPromptsForCentre_returns_populated_list()
        {
            // Given
            var registrationPrompt1 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(1, options: "Clinical\r\nNon-Clinical");
            var registrationPrompt2 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(2);
            var centreRegistrationPrompts = PromptsTestHelper.GetDefaultCentreRegistrationPrompts(
                new List<CentreRegistrationPrompt> { registrationPrompt1, registrationPrompt2 },
                1
            );
            A.CallTo(() => centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(1))
                .Returns(centreRegistrationPrompts);

            // When
            var result =
                promptHelper.GetDelegateRegistrationPromptsForCentre(1, Answer1, Answer2, null, null, null, null);

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(2);
                result[0].PromptNumber.Should().Be(1);
                result[1].PromptNumber.Should().Be(2);
            }
        }

        [Test]
        public void ValidateCentreRegistrationPrompts_adds_error_for_missing_mandatory_answer()
        {
            // Given
            var registrationPrompt1 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(1, mandatory: true);
            var registrationPrompt2 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(2, mandatory: true);
            var centreRegistrationPrompts = PromptsTestHelper.GetDefaultCentreRegistrationPrompts(
                new List<CentreRegistrationPrompt> { registrationPrompt1, registrationPrompt2 },
                1
            );
            var modelState = new ModelStateDictionary();
            A.CallTo(() => centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(1))
                .Returns(centreRegistrationPrompts);

            // When
            promptHelper.ValidateCentreRegistrationPrompts(1, null, Answer2, null, null, null, null, modelState);

            // Then
            modelState["Answer1"].Errors.Count.Should().Be(1);
            modelState["Answer2"].Should().BeNull();
        }

        [Test]
        public void ValidateCentreRegistrationPrompts_adds_error_for_too_long_answer()
        {
            // Given
            const string? longAnswer2 =
                "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
            var registrationPrompt1 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(1);
            var registrationPrompt2 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(2);
            var centreRegistrationPrompts = PromptsTestHelper.GetDefaultCentreRegistrationPrompts(
                new List<CentreRegistrationPrompt> { registrationPrompt1, registrationPrompt2 },
                1
            );
            var modelState = new ModelStateDictionary();
            A.CallTo(() => centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(1))
                .Returns(centreRegistrationPrompts);

            // When
            promptHelper.ValidateCentreRegistrationPrompts(1, Answer1, longAnswer2, null, null, null, null, modelState);

            // Then
            modelState[Answer1].Should().BeNull();
            modelState[Answer2].Errors.Count.Should().Be(1);
        }

        [Test]
        public void GetEditDelegateRegistrationPromptViewModelsForCentre_returns_correctly_mapped_answers_with_gap_in_prompts()
        {
            // Given
            var registrationPrompt1 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(1, options: "Clinical\r\nNon-Clinical");
            var registrationPrompt3 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(3);
            var centreRegistrationPrompts = PromptsTestHelper.GetDefaultCentreRegistrationPrompts(
                new List<CentreRegistrationPrompt> { registrationPrompt1, registrationPrompt3 },
                1
            );
            A.CallTo(() => centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(1))
                .Returns(centreRegistrationPrompts);

            // When
            var result =
                promptHelper.GetEditDelegateRegistrationPromptViewModelsForCentre(
                    1,
                    Answer1,
                    Answer2,
                    Answer3,
                    null,
                    null,
                    null
                );

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(2);

                result[0].PromptNumber.Should().Be(1);
                result[0].Options.Count().Should().Be(2);
                result[0].Options.First().Value.Should().BeEquivalentTo("Clinical");
                result[0].Answer.Should().BeEquivalentTo(Answer1);

                result[1].PromptNumber.Should().Be(3);
                result[1].Options.Count().Should().Be(0);
                result[1].Answer.Should().BeEquivalentTo(Answer3);
            }
        }

        [Test]
        public void GetDelegateRegistrationPromptsForCentre_returns_correctly_mapped_answers_with_gap_in_prompts()
        {
            // Given
            var registrationPrompt1 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(1, options: "Clinical\r\nNon-Clinical");
            var registrationPrompt3 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(3);
            var centreRegistrationPrompts = PromptsTestHelper.GetDefaultCentreRegistrationPrompts(
                new List<CentreRegistrationPrompt> { registrationPrompt1, registrationPrompt3 },
                1
            );
            A.CallTo(() => centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(1))
                .Returns(centreRegistrationPrompts);

            // When
            var result =
                promptHelper.GetDelegateRegistrationPromptsForCentre(
                    1,
                    Answer1,
                    Answer2,
                    Answer3,
                    null,
                    null,
                    null
                );

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(2);

                result[0].PromptNumber.Should().Be(1);
                result[0].Answer.Should().BeEquivalentTo(Answer1);

                result[1].PromptNumber.Should().Be(3);
                result[1].Answer.Should().BeEquivalentTo(Answer3);
            }
        }
    }
}
