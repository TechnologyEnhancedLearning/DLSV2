namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SearchableDelegateViewModel : BaseFilterableViewModel
    {
        public SearchableDelegateViewModel(
            DelegateUserCard delegateUser,
            IEnumerable<CustomFieldViewModel> customFields,
            IEnumerable<CustomPrompt> closedCustomPrompts
        )
        {
            DelegateInfo = new DelegateInfoViewModel(delegateUser, customFields.ToList());
            Tags = FilterableTagHelper.GetCurrentTagsForDelegateUser(delegateUser);

            var closedCustomPromptIds =
                closedCustomPrompts.Select(customPrompt => customPrompt.CustomPromptNumber).ToList();
            CustomPromptFilters = DelegateInfo.CustomFields
                .Where(customField => closedCustomPromptIds.Contains(customField.CustomFieldId)).Select(
                    customField =>
                    {
                        string filterValueName =
                            CentreCustomPromptHelper.GetDelegateCustomPromptAnswerName(customField.CustomFieldId);

                        return new KeyValuePair<int, string>(
                            customField.CustomFieldId,
                            filterValueName + FilteringHelper.Separator + filterValueName + FilteringHelper.Separator +
                            (string.IsNullOrEmpty(customField.Answer)
                                ? FilteringHelper.EmptyValue.ToString()
                                : customField.Answer)
                        );
                    }
                ).ToDictionary(x => x.Key, x => x.Value);
        }

        public DelegateInfoViewModel DelegateInfo { get; set; }

        public string JobGroupFilter => nameof(DelegateUserCard.JobGroupId) + FilteringHelper.Separator +
                                        nameof(DelegateUserCard.JobGroupId) + FilteringHelper.Separator +
                                        DelegateInfo.JobGroupId;

        public Dictionary<int, string> CustomPromptFilters { get; set; }
    }
}
