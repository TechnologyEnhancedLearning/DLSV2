namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;

    public static class DelegateGroupsViewModelFilterOptions
    {
        public static readonly IEnumerable<FilterOptionModel> BaseLinkedFieldOptions = new[]
        {
            DelegateGroupLinkedFieldFilterOptions.None,
            DelegateGroupLinkedFieldFilterOptions.JobGroup,
        };

        public static IEnumerable<FilterOptionModel> GetLinkedFieldOptions(
            IEnumerable<CentreRegistrationPrompt> registrationPrompts
        )
        {
            var promptOptions = registrationPrompts.Select(
                prompt => new FilterOptionModel(
                    prompt.PromptText,
                    nameof(Group.LinkedToField) + FilteringHelper.Separator + prompt.PromptText +
                    FilteringHelper.Separator + prompt.RegistrationField.LinkedToFieldId,
                    FilterStatus.Default
                )
            );

            return BaseLinkedFieldOptions.Concat(promptOptions);
        }

        public static IEnumerable<FilterOptionModel> GetAddedByOptions(
            IEnumerable<GroupDelegateAdmin> admins)
        {
            return admins.Select(
                admin => new FilterOptionModel(
                    admin.FullName,
                    nameof(Group.AddedByAdminId) + FilteringHelper.Separator + nameof(Group.AddedByAdminId) +
                    FilteringHelper.Separator + admin.AdminId,
                    FilterStatus.Default
                )
            );
        }

        public static IEnumerable<FilterModel> GetDelegateGroupFilterModels(IEnumerable<GroupDelegateAdmin> addedByAdmins, IEnumerable<CentreRegistrationPrompt> registrationPrompts)
        {
            return new[]
            {
                new FilterModel(
                    nameof(Group.AddedByAdminId),
                    "Added by",
                    GetAddedByOptions(addedByAdmins)
                    ),
                new FilterModel(
                    nameof(Group.LinkedToField),
                    "Linked field",
                    GetLinkedFieldOptions(registrationPrompts)
                ),
            };
        }
    }
}
