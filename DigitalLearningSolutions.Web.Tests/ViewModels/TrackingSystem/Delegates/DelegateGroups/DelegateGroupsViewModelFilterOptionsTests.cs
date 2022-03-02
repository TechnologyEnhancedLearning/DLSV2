namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class DelegateGroupsViewModelFilterOptionsTests
    {
        [Test]
        public void GetLinkedFieldOptions_returns_expected_filter_options()
        {
            // Given
            var prompt1 = CustomPromptsTestHelper.GetDefaultCustomPrompt(1, "Prompt 1");
            var prompt2 = CustomPromptsTestHelper.GetDefaultCustomPrompt(2, "Prompt 2");
            var prompt3 = CustomPromptsTestHelper.GetDefaultCustomPrompt(3, "Prompt 3");
            var prompt4 = CustomPromptsTestHelper.GetDefaultCustomPrompt(4, "Prompt 4");
            var prompt5 = CustomPromptsTestHelper.GetDefaultCustomPrompt(5, "Prompt 5");
            var prompt6 = CustomPromptsTestHelper.GetDefaultCustomPrompt(6, "Prompt 6");
            var prompts = new[] { prompt1, prompt2, prompt3, prompt4, prompt5, prompt6 };

            // When
            var result = DelegateGroupsViewModelFilterOptions.GetLinkedFieldOptions(prompts).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(8);
                result.Single(f => f.DisplayText == "Prompt 1").NewFilterToAdd.Should()
                    .Be("LinkedToField|LinkedToField|1");
                result.Single(f => f.DisplayText == "Prompt 2").NewFilterToAdd.Should()
                    .Be("LinkedToField|LinkedToField|2");
                result.Single(f => f.DisplayText == "Prompt 3").NewFilterToAdd.Should()
                    .Be("LinkedToField|LinkedToField|3");
                result.Single(f => f.DisplayText == "Prompt 4").NewFilterToAdd.Should()
                    .Be("LinkedToField|LinkedToField|5");
                result.Single(f => f.DisplayText == "Prompt 5").NewFilterToAdd.Should()
                    .Be("LinkedToField|LinkedToField|6");
                result.Single(f => f.DisplayText == "Prompt 6").NewFilterToAdd.Should()
                    .Be("LinkedToField|LinkedToField|7");
            }
        }

        [Test]
        public void GetAddedByOptions_returns_expected_filter_options()
        {
            // Given
            var admins = new[] { (1, "Test Admin"), (2, "Test Person") };

            // When
            var result = DelegateGroupsViewModelFilterOptions.GetAddedByOptions(admins).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(2);
                result.First().DisplayText.Should().Be("Test Admin");
                result.First().NewFilterToAdd.Should().Be("AddedByAdminId|AddedByAdminId|1");
            }
        }
    }
}
