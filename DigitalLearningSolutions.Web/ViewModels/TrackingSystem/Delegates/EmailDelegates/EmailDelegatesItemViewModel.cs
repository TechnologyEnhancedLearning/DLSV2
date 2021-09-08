namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EmailDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class EmailDelegatesItemViewModel
    {
        public EmailDelegatesItemViewModel(
            DelegateUserCard delegateUser,
            bool preChecked
        )
        {
            Id = delegateUser.Id;
            Name = delegateUser.SearchableName;
            Email = delegateUser.EmailAddress;
            if (delegateUser.DateRegistered.HasValue)
            {
                RegistrationDate = delegateUser.DateRegistered.Value.ToString(DateHelper.StandardDateFormat);
            }

            PreChecked = preChecked;
            CustomPromptFilters = new Dictionary<int, string>();
        }

        public EmailDelegatesItemViewModel(
            DelegateUserCard delegateUser,
            bool preChecked,
            IEnumerable<CustomFieldViewModel> customFields,
            IEnumerable<CustomPrompt> promptsWithOptions
        )
        {
            Id = delegateUser.Id;
            Name = delegateUser.SearchableName;
            Email = delegateUser.EmailAddress;
            if (delegateUser.DateRegistered.HasValue)
            {
                RegistrationDate = delegateUser.DateRegistered.Value.ToString(DateHelper.StandardDateFormat);
            }

            PreChecked = preChecked;
            JobGroupId = delegateUser.JobGroupId;
            CustomPromptFilters = GetCustomPromptFilters(customFields, promptsWithOptions);
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? RegistrationDate { get; set; }
        public bool PreChecked { get; set; }
        private int JobGroupId { get; }

        public string JobGroupFilter => FilteringHelper.BuildFilterValueString(
            nameof(DelegateUserCard.JobGroupId),
            nameof(DelegateUserCard.JobGroupId),
            JobGroupId.ToString()
        );

        public Dictionary<int, string> CustomPromptFilters { get; set; }

        private Dictionary<int, string> GetCustomPromptFilters(
            IEnumerable<CustomFieldViewModel> customFields,
            IEnumerable<CustomPrompt> promptsWithOptions
        )
        {
            var closedCustomPromptIds = promptsWithOptions.Select(c => c.CustomPromptNumber);
            var closedCustomFields = customFields
                .Where(customField => closedCustomPromptIds.Contains(customField.CustomFieldId));
            return closedCustomFields
                .Select(
                    customField => new KeyValuePair<int, string>(
                        customField.CustomFieldId,
                        GetFilterValueForCustomField(customField)
                    )
                ).ToDictionary(x => x.Key, x => x.Value);
        }

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
