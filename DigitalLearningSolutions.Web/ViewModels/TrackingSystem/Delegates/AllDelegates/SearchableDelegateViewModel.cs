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
            IEnumerable<DelegateRegistrationPrompt> delegateRegistrationPrompts,
            IEnumerable<CentreRegistrationPrompt> promptsWithOptions,
            int page
        )
        {
            DelegateInfo = new DelegateInfoViewModel(delegateUser, delegateRegistrationPrompts);
            Tags = FilterableTagHelper.GetCurrentTagsForDelegateUser(delegateUser);
            RegistrationPromptFilters = DelegatesViewModelFilters.GetRegistrationPromptFilters(
                DelegateInfo.DelegateRegistrationPrompts,
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

        public Dictionary<int, string> RegistrationPromptFilters { get; set; }

        public int Page { get; set; }
    }
}
