﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AllAdminsViewModel : BaseJavaScriptFilterableViewModel
    {
        public readonly IEnumerable<SearchableAdminViewModel> Admins;

        public AllAdminsViewModel(IEnumerable<AdminUser> adminUsers, IEnumerable<string> categories)
        {
            Admins = adminUsers.Select(au => new SearchableAdminViewModel(au));

            Filters = new[]
            {
                new FilterViewModel("Role", "Role", AdministratorsViewModelFilterOptions.RoleOptions),
                new FilterViewModel(
                    "CategoryName",
                    "Category",
                    AdministratorsViewModelFilterOptions.GetCategoryOptions(categories)
                ),
                new FilterViewModel(
                    "AccountStatus",
                    "Account Status",
                    AdministratorsViewModelFilterOptions.AccountStatusOptions
                )
            }.Select(
                f => f.FilterOptions.Select(
                    fo => new AppliedFilterViewModel(fo.DisplayText, f.FilterName, fo.FilterValue)
                )
            ).SelectMany(af => af).Distinct();
        }
    }
}
