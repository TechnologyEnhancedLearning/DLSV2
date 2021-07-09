namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AllAdminsViewModel : BaseJavascriptFilterableViewModel
    {
        private static readonly IEnumerable<FilterOptionViewModel> RoleOptions = new[]
        {
            AdminRoleFilterOptions.CentreAdministrator,
            AdminRoleFilterOptions.Supervisor,
            AdminRoleFilterOptions.Trainer,
            AdminRoleFilterOptions.ContentCreatorLicense,
            AdminRoleFilterOptions.CmsAdministrator,
            AdminRoleFilterOptions.CmsManager
        };

        private static readonly IEnumerable<FilterOptionViewModel> AccountStatusOptions = new[]
        {
            AdminAccountStatusFilterOptions.IsLocked,
            AdminAccountStatusFilterOptions.IsNotLocked
        };

        public readonly IEnumerable<SearchableAdminViewModel> Admins;

        public AllAdminsViewModel(IEnumerable<AdminUser> adminUsers, IEnumerable<string> categories)
        {
            Admins = adminUsers.Select(au => new SearchableAdminViewModel(au));

            IEnumerable<FilterOptionViewModel> categoryOptions =
                categories.Select(
                    c => new FilterOptionViewModel(
                        c,
                        $"{nameof(AdminUser.CategoryName)}|{nameof(AdminUser.CategoryName)}|{c}",
                        FilterStatus.Default
                    )
                );
            Filters = new[]
            {
                new FilterViewModel("Role", "Role", RoleOptions),
                new FilterViewModel("CategoryName", "Category", categoryOptions),
                new FilterViewModel("AccountStatus", "Account Status", AccountStatusOptions)
            }.Select(
                f => f.FilterOptions.Select(
                    fo => new AppliedFilterViewModel(fo.DisplayText, f.FilterName, fo.FilterValue)
                )
            ).SelectMany(af => af).Distinct();
        }
    }
}
