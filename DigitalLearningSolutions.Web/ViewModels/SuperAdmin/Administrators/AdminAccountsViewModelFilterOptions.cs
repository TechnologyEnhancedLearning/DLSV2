namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Administrators
{
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using System.Collections.Generic;

    public static class AdminAccountsViewModelFilterOptions
    {
        public static readonly IEnumerable<FilterOptionModel> RoleOptions = new[]
        {
            AdminRoleFilterOptions.SuperAdmin,
            AdminRoleFilterOptions.CentreManager,
            AdminRoleFilterOptions.CentreAdministrator,
            AdminRoleFilterOptions.Supervisor,
            AdminRoleFilterOptions.NominatedSupervisor,
            AdminRoleFilterOptions.Trainer,
            AdminRoleFilterOptions.ReportsViewer,
            AdminRoleFilterOptions.ContentCreatorLicense,
            AdminRoleFilterOptions.CmsAdministrator,
            AdminRoleFilterOptions.CmsManager,
            
        };
    }
}
