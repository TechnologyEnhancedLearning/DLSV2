namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EmailDelegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared;
    using DateHelper = DigitalLearningSolutions.Web.Helpers.DateHelper;

    public class EmailDelegatesItemViewModel
    {
        public EmailDelegatesItemViewModel(
            DelegateUser delegateUser,
            bool isDelegateSelected
        )
        {
            Id = delegateUser.Id;
            Name = delegateUser.SearchableName;
            Email = delegateUser.EmailAddress;
            if (delegateUser.DateRegistered.HasValue)
            {
                RegistrationDate = delegateUser.DateRegistered.Value.ToString(DateHelper.StandardDateFormat);
            }

            IsDelegateSelected = isDelegateSelected;
            RegistrationPromptFilters = new Dictionary<int, string>();
        }

        public EmailDelegatesItemViewModel(
            DelegateUser delegateUser,
            bool isDelegateSelected,
            IEnumerable<DelegateRegistrationPrompt> delegateRegistrationPrompts,
            IEnumerable<CentreRegistrationPrompt> promptsWithOptions
        )
        {
            Id = delegateUser.Id;
            Name = delegateUser.SearchableName;
            Email = delegateUser.EmailAddress;
            if (delegateUser.DateRegistered.HasValue)
            {
                RegistrationDate = delegateUser.DateRegistered.Value.ToString(DateHelper.StandardDateFormat);
            }

            IsDelegateSelected = isDelegateSelected;
            JobGroupId = delegateUser.JobGroupId;
            RegistrationPromptFilters = DelegatesViewModelFilters.GetRegistrationPromptFilters(delegateRegistrationPrompts, promptsWithOptions);
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? RegistrationDate { get; set; }
        public bool IsDelegateSelected { get; set; }
        private int JobGroupId { get; }

        public string JobGroupFilter => FilteringHelper.BuildFilterValueString(
            nameof(DelegateUserCard.JobGroupId),
            nameof(DelegateUserCard.JobGroupId),
            JobGroupId.ToString()
        );

        public Dictionary<int, string> RegistrationPromptFilters { get; set; }
    }
}
