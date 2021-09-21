﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EmailDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared;

    public static class EmailDelegatesViewModelFilterOptions
    {
        public static List<FilterViewModel> GetEmailDelegatesFilterViewModels(
            IEnumerable<(int id, string name)> jobGroups,
            IEnumerable<CustomPrompt> promptsWithOptions
        )
        {
            var filters = new List<FilterViewModel>
            {
                new FilterViewModel(
                    "JobGroupId",
                    "Job Group",
                    DelegatesViewModelFilterOptions.GetJobGroupOptions(jobGroups)
                )
            };
            filters.AddRange(
                promptsWithOptions.Select(
                    customPrompt => new FilterViewModel(
                        $"CustomPrompt{customPrompt.CustomPromptNumber}",
                        customPrompt.CustomPromptText,
                        DelegatesViewModelFilterOptions.GetCustomPromptOptions(customPrompt)
                    )
                )
            );
            return filters;
        }
    }
}
