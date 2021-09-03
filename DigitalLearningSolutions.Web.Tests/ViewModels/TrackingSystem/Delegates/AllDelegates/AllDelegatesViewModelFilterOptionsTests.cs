﻿namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.AllDelegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
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
                AllDelegatesViewModelFilterOptions.GetAllDelegatesFilterViewModels(jobGroups, new List<CustomPrompt>());

            // Then
            result.Should().ContainEquivalentOf(expectedFilter);
        }

        [Test]
        public void GetAllDelegatesFilterViewModels_should_return_correct_custom_prompt_filters()
        {
            // Given
            var (customPrompts, expectedFilters) = GetSampleCustomPromptsAndFilters();

            // When
            var result = AllDelegatesViewModelFilterOptions.GetAllDelegatesFilterViewModels(
                new List<(int, string)>(),
                customPrompts
            );

            // Then
            expectedFilters.ForEach(expectedFilter => result.Should().ContainEquivalentOf(expectedFilter));
        }

        private (IEnumerable<(int id, string name)> jobGroups, FilterViewModel filter) GetSampleJobGroupsAndFilter()
        {
            var jobGroups = new List<(int id, string name)> { (1, "J 1"), (2, "J 2") };

            var jobGroupOptions = new[]
            {
                new FilterOptionViewModel(
                    "J 1",
                    "JobGroupId" + FilteringHelper.Separator +
                    "JobGroupId" + FilteringHelper.Separator + 1,
                    FilterStatus.Default
                ),
                new FilterOptionViewModel(
                    "J 2",
                    "JobGroupId" + FilteringHelper.Separator +
                    "JobGroupId" + FilteringHelper.Separator + 2,
                    FilterStatus.Default
                )
            };
            var jobGroupFilter = new FilterViewModel("JobGroupId", "Job Group", jobGroupOptions);

            return (jobGroups, jobGroupFilter);
        }

        private (List<CustomPrompt> customPrompts, List<FilterViewModel> filters) GetSampleCustomPromptsAndFilters()
        {
            var customPrompt1 = CustomPromptsTestHelper.GetDefaultCustomPrompt(
                1,
                "First prompt",
                "Clinical\r\nNon-Clinical"
            );
            var customPrompt3 = CustomPromptsTestHelper.GetDefaultCustomPrompt(3);
            var customPrompt4 = CustomPromptsTestHelper.GetDefaultCustomPrompt(4, "Fourth prompt", "C 1\r\nC 2\r\nC 3");
            var customPrompts = new List<CustomPrompt> { customPrompt1, customPrompt3, customPrompt4 };

            var customPrompt1Options = new[]
            {
                new FilterOptionViewModel(
                    "Clinical",
                    "Answer1" + FilteringHelper.Separator +
                    "Answer1" + FilteringHelper.Separator + "Clinical",
                    FilterStatus.Default
                ),
                new FilterOptionViewModel(
                    "Non-Clinical",
                    "Answer1" + FilteringHelper.Separator +
                    "Answer1" + FilteringHelper.Separator + "Non-Clinical",
                    FilterStatus.Default
                ),
                new FilterOptionViewModel(
                    "No option selected",
                    "Answer1" + FilteringHelper.Separator +
                    "Answer1" + FilteringHelper.Separator + FilteringHelper.EmptyValue,
                    FilterStatus.Default
                )
            };
            var customPrompt4Options = new[]
            {
                new FilterOptionViewModel(
                    "C 1",
                    "Answer4" + FilteringHelper.Separator +
                    "Answer4" + FilteringHelper.Separator + "C 1",
                    FilterStatus.Default
                ),
                new FilterOptionViewModel(
                    "C 2",
                    "Answer4" + FilteringHelper.Separator +
                    "Answer4" + FilteringHelper.Separator + "C 2",
                    FilterStatus.Default
                ),
                new FilterOptionViewModel(
                    "C 3",
                    "Answer4" + FilteringHelper.Separator +
                    "Answer4" + FilteringHelper.Separator + "C 3",
                    FilterStatus.Default
                ),
                new FilterOptionViewModel(
                    "No option selected",
                    "Answer4" + FilteringHelper.Separator +
                    "Answer4" + FilteringHelper.Separator + FilteringHelper.EmptyValue,
                    FilterStatus.Default
                )
            };
            var customPromptFilters = new List<FilterViewModel>
            {
                new FilterViewModel("CustomPrompt1", "First prompt", customPrompt1Options),
                new FilterViewModel("CustomPrompt4", "Fourth prompt", customPrompt4Options)
            };

            return (customPrompts, customPromptFilters);
        }
    }
}
