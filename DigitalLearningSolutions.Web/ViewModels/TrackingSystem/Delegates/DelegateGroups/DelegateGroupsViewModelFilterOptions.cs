﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

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
                    nameof(Group.LinkedToField) + FilteringHelper.Separator + nameof(Group.LinkedToField) +
                    FilteringHelper.Separator + prompt.RegistrationField.LinkedToFieldId,
                    FilterStatus.Default
                )
            );

            return BaseLinkedFieldOptions.Concat(promptOptions);
        }

        public static IEnumerable<FilterOptionModel> GetAddedByOptions(
            IEnumerable<(int adminId, string adminName)> admins
        )
        {
            return admins.Select(
                admin => new FilterOptionModel(
                    admin.adminName,
                    nameof(Group.AddedByAdminId) + FilteringHelper.Separator + nameof(Group.AddedByAdminId) +
                    FilteringHelper.Separator + admin.adminId,
                    FilterStatus.Default
                )
            );
        }


        //TODO: This doesn't work because the groups collection doesn't have all the admins
        // Has to be done in the calling method. See DelegateCoursesViewModelFilterOptions

        public static IEnumerable<FilterModel> GetDelegateGroupFilterModels(List<Group> groups, IEnumerable<CentreRegistrationPrompt> registrationPrompts)
        //public static IEnumerable<FilterModel> GetDelegateGroupFilterModels(IEnumerable<int, string> addedByAdminIds, IEnumerable<CentreRegistrationPrompt> registrationPrompts)
        {
            var admins = groups.Select(
                g => (g.AddedByAdminId, DisplayStringHelper.GetPotentiallyInactiveAdminName(
                    g.AddedByFirstName,
                    g.AddedByLastName,
                    g.AddedByAdminActive
                ))
            ).Distinct();
            return new[]
            {
                new FilterModel(
                    nameof(Group.AddedByAdminId),
                    "Added by",
                    //GetAddedByOptions(addedByAdminIds)
                    GetAddedByOptions(admins)
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
