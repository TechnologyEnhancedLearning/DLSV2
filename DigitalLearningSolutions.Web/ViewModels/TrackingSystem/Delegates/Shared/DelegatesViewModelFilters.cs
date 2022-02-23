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

        public static IEnumerable<FilterOptionViewModel> GetCustomPromptOptions(CustomPrompt customPrompt)
        {
            var filterValueName =
                CentreCustomPromptHelper.GetDelegateCustomPromptAnswerName(customPrompt.RegistrationField.Id);

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

        public static Dictionary<int, string> GetCustomPromptFilters(
            IEnumerable<CustomFieldViewModel> customFields,
            IEnumerable<CustomPrompt> promptsWithOptions
        )
        {
            var promptsWithOptionsIds = promptsWithOptions.Select(c => c.RegistrationField);
            var customFieldsWithOptions =
                customFields.Where(customField => promptsWithOptionsIds.Contains(customField.CustomFieldId));
            return customFieldsWithOptions
                .Select(
                    customField => new KeyValuePair<int, string>(
                        customField.CustomFieldId,
                        GetFilterValueForCustomField(customField)
                    )
                ).ToDictionary(x => x.Key, x => x.Value);
        }

        private static string GetFilterValueForCustomField(CustomFieldViewModel customField)
        {
            var filterValueName =
                CentreCustomPromptHelper.GetDelegateCustomPromptAnswerName(customField.CustomFieldId);
            var propertyValue = string.IsNullOrEmpty(customField.Answer)
                ? FilteringHelper.EmptyValue.ToString()
                : customField.Answer;
            return FilteringHelper.BuildFilterValueString(filterValueName, filterValueName, propertyValue);
        }
    }
}
