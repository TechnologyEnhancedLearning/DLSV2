namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class DelegateGroupsViewModelFilterOptions
    {
        public static readonly IEnumerable<FilterOptionViewModel> BaseLinkedFieldOptions = new[]
        {
            DelegateGroupLinkedFieldFilterOptions.None,
            DelegateGroupLinkedFieldFilterOptions.JobGroup,
        };

        public static IEnumerable<FilterOptionViewModel> GetLinkedFieldOptions(
            IEnumerable<CustomPrompt> registrationPrompts
        )
        {
            var promptOptions = registrationPrompts.Select(
                prompt => new FilterOptionViewModel(
                    prompt.CustomPromptText,
                    nameof(Group.LinkedToField) + FilteringHelper.Separator + nameof(Group.LinkedToField) +
                    FilteringHelper.Separator + GetLinkedFieldIdFromRegistrationPromptNumber(prompt.RegistrationField.Id),
                    FilterStatus.Default
                )
            );

            return BaseLinkedFieldOptions.Concat(promptOptions);
        }

        public static IEnumerable<FilterOptionViewModel> GetAddedByOptions(
            IEnumerable<(int adminId, string adminName)> admins
        )
        {
            return admins.Select(
                admin => new FilterOptionViewModel(
                    admin.adminName,
                    nameof(Group.AddedByAdminId) + FilteringHelper.Separator + nameof(Group.AddedByAdminId) +
                    FilteringHelper.Separator + admin.adminId,
                    FilterStatus.Default
                )
            );
        }

        // Centre registration prompts correspond to Groups.LinkedToField
        // 4 is reserved for Job Group so we must skip it
        private static int GetLinkedFieldIdFromRegistrationPromptNumber(int promptNumber)
        {
            return promptNumber > 3 ? promptNumber + 1 : promptNumber;
        }
    }
}
