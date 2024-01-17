namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
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
            var prompt1 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(1, "Prompt 1");
            var prompt2 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(2, "Prompt 2");
            var prompt3 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(3, "Prompt 3");
            var prompt4 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(4, "Prompt 4");
            var prompt5 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(5, "Prompt 5");
            var prompt6 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(6, "Prompt 6");
            var prompts = new[] { prompt1, prompt2, prompt3, prompt4, prompt5, prompt6 };

            // When
            var result = DelegateGroupsViewModelFilterOptions.GetLinkedFieldOptions(prompts).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(8);
                result.Single(f => f.DisplayText == "Prompt 1").FilterValue.Should()
                    .Be("LinkedToField|LinkedToField|1");
                result.Single(f => f.DisplayText == "Prompt 2").FilterValue.Should()
                    .Be("LinkedToField|LinkedToField|2");
                result.Single(f => f.DisplayText == "Prompt 3").FilterValue.Should()
                    .Be("LinkedToField|LinkedToField|3");
                result.Single(f => f.DisplayText == "Prompt 4").FilterValue.Should()
                    .Be("LinkedToField|LinkedToField|5");
                result.Single(f => f.DisplayText == "Prompt 5").FilterValue.Should()
                    .Be("LinkedToField|LinkedToField|6");
                result.Single(f => f.DisplayText == "Prompt 6").FilterValue.Should()
                    .Be("LinkedToField|LinkedToField|7");
            }
        }

        [Test]
        public void GetAddedByOptions_returns_expected_filter_options()
        {
            // Given

            IEnumerable<GroupDelegateAdmin> admins = new List<GroupDelegateAdmin>();

            admins = admins.Append(new GroupDelegateAdmin { AdminId = 1, FullName = "Test Admin One" });
            admins = admins.Append(new GroupDelegateAdmin { AdminId = 2, FullName = "Test Admin Two" });
            admins = admins.Append(new GroupDelegateAdmin { AdminId = 3, FullName = "Test Admin Three" });

            // When
            var result = DelegateGroupsViewModelFilterOptions.GetAddedByOptions(admins).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(3);
                result.First().DisplayText.Should().Be("Test Admin One");
                result.First().FilterValue.Should().Be("AddedByAdminId|AddedByAdminId|1");
            }
        }
    }
}
