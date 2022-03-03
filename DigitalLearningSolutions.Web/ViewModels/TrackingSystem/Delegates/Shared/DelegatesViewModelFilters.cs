namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class DelegatesViewModelFilters
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

        public static IEnumerable<FilterOptionViewModel> GetPromptOptions(Prompt prompt)
        {
            var filterValueName = prompt is CentreRegistrationPrompt registrationPrompt ?
                PromptsService.GetDelegateRegistrationPromptAnswerName(registrationPrompt.RegistrationField.Id)
                : PromptsService.GetCourseAdminFieldAnswerName(((CourseAdminField)prompt).PromptNumber);

            var options = prompt.Options.Select(
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
                delegateRegistrationPrompts.Where(delegateRegistrationPrompt => promptsWithOptionsIds.Contains(delegateRegistrationPrompt.PromptNumber));
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
                PromptsService.GetDelegateRegistrationPromptAnswerName(delegatePrompt.PromptNumber);
            var propertyValue = string.IsNullOrEmpty(delegatePrompt.Answer)
                ? FilteringHelper.EmptyValue
                : delegatePrompt.Answer;
            return FilteringHelper.BuildFilterValueString(filterValueName, filterValueName, propertyValue);
        }
    }
}
