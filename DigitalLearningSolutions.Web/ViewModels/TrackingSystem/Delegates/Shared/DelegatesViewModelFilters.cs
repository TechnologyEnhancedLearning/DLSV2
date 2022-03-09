namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

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

        public static IEnumerable<FilterOptionModel> GetPromptOptions(Prompt prompt)
        {
            var filterValueName = prompt is CentreRegistrationPrompt registrationPrompt
                ? @$"{DelegateUserCard.GetPropertyNameForDelegateRegistrationPromptAnswer(
                    registrationPrompt.RegistrationField.Id
                )}({prompt.PromptText})"
                : $"{CourseDelegate.GetPropertyNameForAdminFieldAnswer(((CourseAdminField)prompt).PromptNumber)}({prompt.PromptText})";

            var options = prompt.Options.Select(
                option => new FilterOptionModel(
                    option,
                    FilteringHelper.BuildFilterValueString(filterValueName, filterValueName, option),
                    FilterStatus.Default
                )
            ).ToList();
            options.Add(
                new FilterOptionModel(
                    "No option selected",
                    FilteringHelper.BuildFilterValueString(
                        filterValueName,
                        filterValueName,
                        FilteringHelper.EmptyValue
                    ),
                    FilterStatus.Default
                )
            );
            return options;
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
                        GetFilterValueForRegistrationPrompt(delegateRegistrationPrompt)
                    )
                ).ToDictionary(x => x.Key, x => x.Value);
        }

        private static string GetFilterValueForRegistrationPrompt(DelegatePrompt delegatePrompt)
        {
            var filterValueName =
                $"{DelegateUserCard.GetPropertyNameForDelegateRegistrationPromptAnswer(delegatePrompt.PromptNumber)}({delegatePrompt.Prompt})";
            var propertyValue = string.IsNullOrEmpty(delegatePrompt.Answer)
                ? FilteringHelper.EmptyValue
                : delegatePrompt.Answer;
            return FilteringHelper.BuildFilterValueString(filterValueName, filterValueName, propertyValue);
        }
    }
}
