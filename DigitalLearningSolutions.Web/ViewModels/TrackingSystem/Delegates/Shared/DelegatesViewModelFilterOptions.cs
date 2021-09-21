namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class DelegatesViewModelFilterOptions
    {
        public static IEnumerable<FilterOptionViewModel> GetJobGroupOptions(
            IEnumerable<(int id, string name)> jobGroups
        )
        {
            return jobGroups.Select(
                jobGroup => new FilterOptionViewModel(
                    jobGroup.name,
                    FilteringHelper.BuildFilterValueString(
                        nameof(DelegateUserCard.JobGroupId),
                        nameof(DelegateUserCard.JobGroupId),
                        jobGroup.id.ToString()
                    ),
                    FilterStatus.Default
                )
            );
        }

        public static IEnumerable<FilterOptionViewModel> GetCustomPromptOptions(CustomPrompt customPrompt)
        {
            string filterValueName =
                CentreCustomPromptHelper.GetDelegateCustomPromptAnswerName(customPrompt.CustomPromptNumber);

            var options = customPrompt.Options.Select(
                option => new FilterOptionViewModel(
                    option,
                    FilteringHelper.BuildFilterValueString(filterValueName, filterValueName, option),
                    FilterStatus.Default
                )
            ).ToList();
            options.Add(
                new FilterOptionViewModel(
                    "No option selected",
                    FilteringHelper.BuildFilterValueString(
                        filterValueName,
                        filterValueName,
                        FilteringHelper.EmptyValue.ToString()
                    ),
                    FilterStatus.Default
                )
            );
            return options;
        }
    }
}
