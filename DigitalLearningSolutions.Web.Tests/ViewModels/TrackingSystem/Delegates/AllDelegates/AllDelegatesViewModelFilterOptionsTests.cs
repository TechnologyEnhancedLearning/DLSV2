namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.AllDelegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AllDelegates;
    using FluentAssertions;
    using NUnit.Framework;

    public class AllDelegatesViewModelFilterOptionsTests
    {
        [Test]
        public void GetAllDelegatesFilterViewModels_should_return_correct_job_group_filter()
        {
            // Given
            var (jobGroups, expectedFilter) = GetSampleJobGroupsAndFilter();

            // When
            var result =
                AllDelegatesViewModelFilterOptions.GetAllDelegatesFilterViewModels(
                    jobGroups,
                    new List<CentreRegistrationPrompt>()
                );

            // Then
            result.Should().ContainEquivalentOf(expectedFilter);
        }

        [Test]
        public void GetAllDelegatesFilterViewModels_should_return_correct_custom_prompt_filters()
        {
            // Given
            var (centreRegistrationPrompts, expectedFilters) = GetSampleCentreRegistrationPromptsAndFilters();

            // When
            var result = AllDelegatesViewModelFilterOptions.GetAllDelegatesFilterViewModels(
                new List<(int, string)>(),
                centreRegistrationPrompts
            );

            // Then
            expectedFilters.ForEach(expectedFilter => result.Should().ContainEquivalentOf(expectedFilter));
        }

        private (IEnumerable<(int id, string name)> jobGroups, FilterModel filter) GetSampleJobGroupsAndFilter()
        {
            var jobGroups = new List<(int id, string name)> { (1, "J 1"), (2, "J 2") };

            var jobGroupOptions = new[]
            {
                new FilterOptionModel(
                    "J 1",
                    "JobGroupId" + FilteringHelper.Separator +
                    "JobGroupId" + FilteringHelper.Separator + 1,
                    FilterStatus.Default
                ),
                new FilterOptionModel(
                    "J 2",
                    "JobGroupId" + FilteringHelper.Separator +
                    "JobGroupId" + FilteringHelper.Separator + 2,
                    FilterStatus.Default
                ),
            };
            var jobGroupFilter = new FilterModel("JobGroupId", "Job Group", jobGroupOptions);

            return (jobGroups, jobGroupFilter);
        }

        private (List<CentreRegistrationPrompt> centreRegistrationPrompts, List<FilterModel> filters)
            GetSampleCentreRegistrationPromptsAndFilters()
        {
            var prompt1 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(
                1,
                "First prompt",
                "Clinical\r\nNon-Clinical"
            );
            var prompt3 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(3);
            var prompt4 = PromptsTestHelper.GetDefaultCentreRegistrationPrompt(4, "Fourth prompt", "C 1\r\nC 2\r\nC 3");
            var prompts = new List<CentreRegistrationPrompt> { prompt1, prompt3, prompt4 };

            var prompt1Options = new[]
            {
                new FilterOptionModel(
                    "Clinical",
                    "Answer1(First prompt)" + FilteringHelper.Separator +
                    "Answer1" + FilteringHelper.Separator + "Clinical",
                    FilterStatus.Default
                ),
                new FilterOptionModel(
                    "Non-Clinical",
                    "Answer1(First prompt)" + FilteringHelper.Separator +
                    "Answer1" + FilteringHelper.Separator + "Non-Clinical",
                    FilterStatus.Default
                ),
                new FilterOptionModel(
                    "No option selected",
                    "Answer1(First prompt)" + FilteringHelper.Separator +
                    "Answer1" + FilteringHelper.Separator + FilteringHelper.EmptyValue,
                    FilterStatus.Default
                ),
            };
            var prompt4Options = new[]
            {
                new FilterOptionModel(
                    "C 1",
                    "Answer4(Fourth prompt)" + FilteringHelper.Separator +
                    "Answer4" + FilteringHelper.Separator + "C 1",
                    FilterStatus.Default
                ),
                new FilterOptionModel(
                    "C 2",
                    "Answer4(Fourth prompt)" + FilteringHelper.Separator +
                    "Answer4" + FilteringHelper.Separator + "C 2",
                    FilterStatus.Default
                ),
                new FilterOptionModel(
                    "C 3",
                    "Answer4(Fourth prompt)" + FilteringHelper.Separator +
                    "Answer4" + FilteringHelper.Separator + "C 3",
                    FilterStatus.Default
                ),
                new FilterOptionModel(
                    "No option selected",
                    "Answer4(Fourth prompt)" + FilteringHelper.Separator +
                    "Answer4" + FilteringHelper.Separator + FilteringHelper.EmptyValue,
                    FilterStatus.Default
                ),
            };
            var customPromptFilters = new List<FilterModel>
            {
                new FilterModel("CentreRegistrationPrompt1", "First prompt", prompt1Options),
                new FilterModel("CentreRegistrationPrompt4", "Fourth prompt", prompt4Options),
            };

            return (prompts, customPromptFilters);
        }
    }
}
