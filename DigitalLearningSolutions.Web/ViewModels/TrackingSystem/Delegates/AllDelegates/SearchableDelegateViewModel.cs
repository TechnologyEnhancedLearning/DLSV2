namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AllDelegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Helpers;
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
            IEnumerable<CustomPrompt> promptsWithOptions,
            int page
        )
        {
            DelegateInfo = new DelegateInfoViewModel(delegateUser, customFields);
            Tags = FilterableTagHelper.GetCurrentTagsForDelegateUser(delegateUser);
            CustomPromptFilters = DelegatesViewModelFilters.GetCustomPromptFilters(
                DelegateInfo.CustomFields,
                promptsWithOptions
            );
            Page = page;
        }

        public DelegateInfoViewModel DelegateInfo { get; set; }

        public string JobGroupFilter => FilteringHelper.BuildFilterValueString(
            nameof(DelegateUserCard.JobGroupId),
            nameof(DelegateUserCard.JobGroupId),
            DelegateInfo.JobGroupId.ToString()
        );

        public Dictionary<int, string> CustomPromptFilters { get; set; }

        public int Page { get; set; }
    }
}
