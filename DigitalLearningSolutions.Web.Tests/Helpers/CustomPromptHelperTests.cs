﻿namespace DigitalLearningSolutions.Web.Tests.Helpers
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

    public class CustomPromptHelperTests
    {
        private const string Answer1 = "Answer1";
        private const string Answer2 = "Answer2";
        private const string Answer3 = "Answer3";
        private ICentreCustomPromptsService centreCustomPromptsService = null!;
        private CustomPromptHelper customPromptHelper = null!;

        [SetUp]
        public void Setup()
        {
            centreCustomPromptsService = A.Fake<ICentreCustomPromptsService>();
            customPromptHelper = new CustomPromptHelper(centreCustomPromptsService);
        }

        [Test]
        public void GetEditCustomFieldViewModelsForCentre_returns_populated_list()
        {
            // Given
            var customPrompt1 = CustomPromptsTestHelper.GetDefaultCustomPrompt(1, options: "Clinical\r\nNon-Clinical");
            var customPrompt2 = CustomPromptsTestHelper.GetDefaultCustomPrompt(2);
            var centreCustomPrompts = CustomPromptsTestHelper.GetDefaultCentreCustomPrompts(
                new List<CustomPrompt> { customPrompt1, customPrompt2 },
                1
            );
            A.CallTo(() => centreCustomPromptsService.GetCustomPromptsForCentreByCentreId(1))
                .Returns(centreCustomPrompts);

            // When
            var result =
                customPromptHelper.GetEditCustomFieldViewModelsForCentre(1, Answer1, Answer2, null, null, null, null);

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(2);

                result[0].CustomFieldId.Should().Be(1);
                result[0].Options.Count().Should().Be(2);
                result[0].Options.First().Value.Should().BeEquivalentTo("Clinical");

                result[1].CustomFieldId.Should().Be(2);
                result[1].Options.Count().Should().Be(0);
            }
        }

        [Test]
        public void GetCustomFieldViewModelsForCentre_returns_populated_list()
        {
            // Given
            var customPrompt1 = CustomPromptsTestHelper.GetDefaultCustomPrompt(1, options: "Clinical\r\nNon-Clinical");
            var customPrompt2 = CustomPromptsTestHelper.GetDefaultCustomPrompt(2);
            var centreCustomPrompts = CustomPromptsTestHelper.GetDefaultCentreCustomPrompts(
                new List<CustomPrompt> { customPrompt1, customPrompt2 },
                1
            );
            A.CallTo(() => centreCustomPromptsService.GetCustomPromptsForCentreByCentreId(1))
                .Returns(centreCustomPrompts);

            // When
            var result =
                customPromptHelper.GetCustomFieldViewModelsForCentre(1, Answer1, Answer2, null, null, null, null);

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(2);
                result[0].CustomFieldId.Should().Be(1);
                result[1].CustomFieldId.Should().Be(2);
            }
        }

        [Test]
        public void ValidateCustomPrompts_adds_error_for_missing_mandatory_answer()
        {
            // Given
            var customPrompt1 = CustomPromptsTestHelper.GetDefaultCustomPrompt(1, mandatory: true);
            var customPrompt2 = CustomPromptsTestHelper.GetDefaultCustomPrompt(2, mandatory: true);
            var centreCustomPrompts = CustomPromptsTestHelper.GetDefaultCentreCustomPrompts(
                new List<CustomPrompt> { customPrompt1, customPrompt2 },
                1
            );
            var modelState = new ModelStateDictionary();
            A.CallTo(() => centreCustomPromptsService.GetCustomPromptsForCentreByCentreId(1))
                .Returns(centreCustomPrompts);

            // When
            customPromptHelper.ValidateCustomPrompts(1, null, Answer2, null, null, null, null, modelState);

            // Then
            modelState["Answer1"].Errors.Count.Should().Be(1);
            modelState["Answer2"].Should().BeNull();
        }

        [Test]
        public void ValidateCustomPrompts_adds_error_for_too_long_answer()
        {
            // Given
            const string? longAnswer2 =
                "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
            var customPrompt1 = CustomPromptsTestHelper.GetDefaultCustomPrompt(1);
            var customPrompt2 = CustomPromptsTestHelper.GetDefaultCustomPrompt(2);
            var centreCustomPrompts = CustomPromptsTestHelper.GetDefaultCentreCustomPrompts(
                new List<CustomPrompt> { customPrompt1, customPrompt2 },
                1
            );
            var modelState = new ModelStateDictionary();
            A.CallTo(() => centreCustomPromptsService.GetCustomPromptsForCentreByCentreId(1))
                .Returns(centreCustomPrompts);

            // When
            customPromptHelper.ValidateCustomPrompts(1, Answer1, longAnswer2, null, null, null, null, modelState);

            // Then
            modelState[Answer1].Should().BeNull();
            modelState[Answer2].Errors.Count.Should().Be(1);
        }

        [Test]
        public void GetEditCustomFieldViewModelsForCentre_returns_correctly_mapped_answers_with_gap_in_prompts()
        {
            // Given
            var customPrompt1 = CustomPromptsTestHelper.GetDefaultCustomPrompt(1, options: "Clinical\r\nNon-Clinical");
            var customPrompt3 = CustomPromptsTestHelper.GetDefaultCustomPrompt(3);
            var centreCustomPrompts = CustomPromptsTestHelper.GetDefaultCentreCustomPrompts(
                new List<CustomPrompt> { customPrompt1, customPrompt3 },
                1
            );
            A.CallTo(() => centreCustomPromptsService.GetCustomPromptsForCentreByCentreId(1))
                .Returns(centreCustomPrompts);

            // When
            var result =
                customPromptHelper.GetEditCustomFieldViewModelsForCentre(
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

                result[0].CustomFieldId.Should().Be(1);
                result[0].Options.Count().Should().Be(2);
                result[0].Options.First().Value.Should().BeEquivalentTo("Clinical");
                result[0].Answer.Should().BeEquivalentTo(Answer1);

                result[1].CustomFieldId.Should().Be(3);
                result[1].Options.Count().Should().Be(0);
                result[1].Answer.Should().BeEquivalentTo(Answer3);
            }
        }

        [Test]
        public void GetCustomFieldViewModelsForCentre_returns_correctly_mapped_answers_with_gap_in_prompts()
        {
            // Given
            var customPrompt1 = CustomPromptsTestHelper.GetDefaultCustomPrompt(1, options: "Clinical\r\nNon-Clinical");
            var customPrompt3 = CustomPromptsTestHelper.GetDefaultCustomPrompt(3);
            var centreCustomPrompts = CustomPromptsTestHelper.GetDefaultCentreCustomPrompts(
                new List<CustomPrompt> { customPrompt1, customPrompt3 },
                1
            );
            A.CallTo(() => centreCustomPromptsService.GetCustomPromptsForCentreByCentreId(1))
                .Returns(centreCustomPrompts);

            // When
            var result =
                customPromptHelper.GetCustomFieldViewModelsForCentre(1, Answer1, Answer2, Answer3, null, null, null);

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(2);

                result[0].CustomFieldId.Should().Be(1);
                result[0].Answer.Should().BeEquivalentTo(Answer1);

                result[1].CustomFieldId.Should().Be(3);
                result[1].Answer.Should().BeEquivalentTo(Answer3);
            }
        }
    }
}
