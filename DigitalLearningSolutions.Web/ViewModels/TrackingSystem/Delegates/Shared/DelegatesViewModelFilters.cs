namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public static class DelegatesViewModelFilters
    {
        public static IEnumerable<FilterOptionModel> GetJobGroupOptions(
            IEnumerable<(int id, string name)> jobGroups
        )
        {
            return jobGroups.Select(
                jobGroup => new FilterOptionModel(
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

        public static Dictionary<int, string> GetRegistrationPromptFilters(
            IEnumerable<DelegateRegistrationPrompt> delegateRegistrationPrompts,
            IEnumerable<CentreRegistrationPrompt> promptsWithOptions
        )
        {
            var promptsWithOptionsIds = promptsWithOptions.Select(c => c.RegistrationField.Id);
            var delegateRegistrationPromptsWithOptions =
                delegateRegistrationPrompts.Where(
                    delegateRegistrationPrompt =>
                        promptsWithOptionsIds.Contains(delegateRegistrationPrompt.PromptNumber)
                );
            return delegateRegistrationPromptsWithOptions
                .Select(
                    delegateRegistrationPrompt => new KeyValuePair<int, string>(
                        delegateRegistrationPrompt.PromptNumber,
                        FilteringHelper.GetFilterValueForRegistrationPrompt(
                            delegateRegistrationPrompt.PromptNumber,
                            delegateRegistrationPrompt.Answer,
                            delegateRegistrationPrompt.Prompt
                        )
                    )
                ).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
