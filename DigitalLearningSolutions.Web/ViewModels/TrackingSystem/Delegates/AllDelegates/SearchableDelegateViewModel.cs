namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AllDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared;

    public class SearchableDelegateViewModel : BaseFilterableViewModel
    {
        public SearchableDelegateViewModel(
            DelegateUserCard delegateUser,
            IEnumerable<CustomFieldViewModel> customFields,
            IEnumerable<CustomPrompt> promptsWithOptions
        )
        {
            DelegateInfo = new DelegateInfoViewModel(delegateUser, customFields);
            Tags = FilterableTagHelper.GetCurrentTagsForDelegateUser(delegateUser);

            var closedCustomPromptIds = promptsWithOptions.Select(c => c.CustomPromptNumber);
            var closedCustomFields = DelegateInfo.CustomFields
                .Where(customField => closedCustomPromptIds.Contains(customField.CustomFieldId));
            CustomPromptFilters = closedCustomFields
                .Select(
                    customField => new KeyValuePair<int, string>(
                        customField.CustomFieldId,
                        GetFilterValueForCustomField(customField)
                    )
                ).ToDictionary(x => x.Key, x => x.Value);
        }

        public DelegateInfoViewModel DelegateInfo { get; set; }

        public string JobGroupFilter => nameof(DelegateUserCard.JobGroupId) + FilteringHelper.Separator +
                                        nameof(DelegateUserCard.JobGroupId) + FilteringHelper.Separator +
                                        DelegateInfo.JobGroupId;

        public Dictionary<int, string> CustomPromptFilters { get; set; }

        private string GetFilterValueForCustomField(CustomFieldViewModel customField)
        {
            string filterValueName =
                CentreCustomPromptHelper.GetDelegateCustomPromptAnswerName(customField.CustomFieldId);
            string propertyValue = string.IsNullOrEmpty(customField.Answer)
                ? FilteringHelper.EmptyValue.ToString()
                : customField.Answer;
            return FilteringHelper.BuildFilterValueString(
                filterValueName,
                filterValueName,
                propertyValue
            );
        }
    }
}
