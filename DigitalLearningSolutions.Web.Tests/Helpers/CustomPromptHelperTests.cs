namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using NUnit.Framework;

    public class CustomPromptHelperTests
    {
        private ICustomPromptsService customPromptsService;
        private CustomPromptHelper customPromptHelper;

        [SetUp]
        public void Setup()
        {
            customPromptsService = A.Fake<ICustomPromptsService>();
            customPromptHelper = new CustomPromptHelper(customPromptsService);
        }

        [Test]
        public void GetCustomFieldViewModelsForCentre_returns_populated_list()
        {
            // Given
            var answer1 = "Answer1";
            var answer2 = "Answer2";
            var customPrompt1 = CustomPromptsTestHelper.GetDefaultCustomPrompt(1, options: "Clinical\r\nNon-Clinical");
            var customPrompt2 = CustomPromptsTestHelper.GetDefaultCustomPrompt(2);
            var centreCustomPrompts = CustomPromptsTestHelper.GetDefaultCentreCustomPrompts(
                new List<CustomPrompt> { customPrompt1, customPrompt2 },
                1
            );
            A.CallTo(() => customPromptsService.GetCustomPromptsForCentreByCentreId(1)).Returns(centreCustomPrompts);

            // When
            var result =
                customPromptHelper.GetCustomFieldViewModelsForCentre(1, answer1, answer2, null, null, null, null);

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
        public void ValidateCustomPrompts_adds_errors_to_invalid_answers_only()
        {
            // Given
            var answer2 = "Answer2";
            var customPrompt1 = CustomPromptsTestHelper.GetDefaultCustomPrompt(1, mandatory: true);
            var customPrompt2 = CustomPromptsTestHelper.GetDefaultCustomPrompt(2, mandatory: true);
            var centreCustomPrompts = CustomPromptsTestHelper.GetDefaultCentreCustomPrompts(
                new List<CustomPrompt> { customPrompt1, customPrompt2 },
                1
            );
            var modelState = new ModelStateDictionary();
            A.CallTo(() => customPromptsService.GetCustomPromptsForCentreByCentreId(1)).Returns(centreCustomPrompts);

            // When
            customPromptHelper.ValidateCustomPrompts(1, null, answer2, null, null, null, null, modelState);

            // Then
            modelState["Answer1"].Errors.Count.Should().Be(1);
            modelState["Answer2"].Should().BeNull();
        }
    }
}
