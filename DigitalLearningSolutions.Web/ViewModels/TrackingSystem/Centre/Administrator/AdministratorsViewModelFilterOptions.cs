﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;

    public static class AdministratorsViewModelFilterOptions
    {
        public static readonly IEnumerable<FilterOptionModel> RoleOptions = new[]
        {
            AdminRoleFilterOptions.CentreManager,
            AdminRoleFilterOptions.CentreAdministrator,
            AdminRoleFilterOptions.Supervisor,
            AdminRoleFilterOptions.NominatedSupervisor,
            AdminRoleFilterOptions.Trainer,
            AdminRoleFilterOptions.ContentCreatorLicense,
            AdminRoleFilterOptions.CmsAdministrator,
            AdminRoleFilterOptions.CmsManager,
        };

        public static readonly IEnumerable<FilterOptionModel> AccountStatusOptions = new[]
        {
            AdminAccountStatusFilterOptions.IsLocked,
            AdminAccountStatusFilterOptions.IsNotLocked,
        };

        public static readonly IEnumerable<FilterOptionModel> UserStatusOptions = new[]
        {
            UserAccountStatusFilterOptions.Active,
            UserAccountStatusFilterOptions.Inactive,
        };

        public static IEnumerable<FilterOptionModel> GetCategoryOptions(IEnumerable<string> categories)
        {
            return categories.Select(
                c => new FilterOptionModel(
                    c,
                    nameof(AdminEntity.CategoryName) + FilteringHelper.Separator + nameof(AdminEntity.CategoryName) +
                    FilteringHelper.Separator + c,
                    FilterStatus.Default
                )
            );
        }

        public static List<FilterModel> GetAllAdministratorsFilterModels(IEnumerable<Category> categories)
        {
            var categoryStrings = categories.Select(c => c.CategoryName.Trim());
            categoryStrings = categoryStrings.Prepend("All");
            var filters = new List<FilterModel>
            {
                new FilterModel("Role", "Role", RoleOptions),
                new FilterModel(
                    "CategoryName",
                    "Category",
                    GetCategoryOptions(categoryStrings)
                ),
                new FilterModel(
                    "AccountStatus",
                    "Account Status",
                    AccountStatusOptions
                ),
                new FilterModel(
                    "UserStatus",
                    "User Status",
                    UserStatusOptions
                ),
            };
            return filters;
        }
    }
}
