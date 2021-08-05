namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class DelegateGroupsViewModelFilterOptions
    {
        public static readonly IEnumerable<FilterOptionViewModel> BaseLinkedFieldOptions = new[]
        {
            DelegateGroupLinkedFieldFilterOptions.None,
            DelegateGroupLinkedFieldFilterOptions.JobGroup
        };

        public static IEnumerable<FilterOptionViewModel> GetLinkedFieldOptions(
            IEnumerable<(int promptNumber, string promptText)> registrationPrompts
        )
        {
            var promptOptions = registrationPrompts.Select(
                a => new FilterOptionViewModel(
                    a.promptText,
                    nameof(Group.LinkedToField) + FilteringHelper.Separator + nameof(Group.LinkedToField) +
                    FilteringHelper.Separator + a.promptNumber,
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
                a => new FilterOptionViewModel(
                    a.adminName,
                    nameof(Group.AddedByAdminId) + FilteringHelper.Separator + nameof(Group.AddedByAdminId) +
                    FilteringHelper.Separator + a.adminId,
                    FilterStatus.Default
                )
            );
        }
    }
}
