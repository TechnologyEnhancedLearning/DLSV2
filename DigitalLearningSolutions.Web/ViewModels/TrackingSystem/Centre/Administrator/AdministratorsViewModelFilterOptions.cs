namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public static class AdministratorsViewModelFilterOptions
    {
        public static readonly IEnumerable<FilterOptionViewModel> RoleOptions = new[]
        {
            AdminRoleFilterOptions.CentreAdministrator,
            AdminRoleFilterOptions.Supervisor,
            AdminRoleFilterOptions.NominatedSupervisor,
            AdminRoleFilterOptions.Trainer,
            AdminRoleFilterOptions.ContentCreatorLicense,
            AdminRoleFilterOptions.CmsAdministrator,
            AdminRoleFilterOptions.CmsManager,
        };

        public static readonly IEnumerable<FilterOptionViewModel> AccountStatusOptions = new[]
        {
            AdminAccountStatusFilterOptions.IsLocked,
            AdminAccountStatusFilterOptions.IsNotLocked,
        };

        public static IEnumerable<FilterOptionViewModel> GetCategoryOptions(IEnumerable<string> categories)
        {
            return categories.Select(
                c => new FilterOptionViewModel(
                    c,
                    nameof(AdminUser.CategoryName) + FilteringHelper.Separator + nameof(AdminUser.CategoryName) +
                    FilteringHelper.Separator + c,
                    FilterStatus.Default
                )
            );
        }
    }
}
