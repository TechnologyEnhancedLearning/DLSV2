namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin.Administrators
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Administrators;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [FeatureGate(FeatureFlags.RefactoredSuperAdminInterface)]
    [Authorize(Policy = CustomPolicies.UserSuperAdmin)]

    [SetDlsSubApplication(nameof(DlsSubApplication.SuperAdmin))]
    [SetSelectedTab(nameof(NavMenuTab.Admins))]
    public class AdminAccountsController : Controller
    {
        private readonly IUserDataService userDataService;
        private readonly IAdminDownloadFileService adminDownloadFileService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly ICentresDataService centresDataService;
        public AdminAccountsController(IUserDataService userDataService,
            ICentresDataService centresDataService, 
            ISearchSortFilterPaginateService searchSortFilterPaginateService,
            IAdminDownloadFileService adminDownloadFileService
            )
        {
            this.userDataService = userDataService;
            this.centresDataService = centresDataService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
            this.adminDownloadFileService = adminDownloadFileService;
        }

        [Route("SuperAdmin/AdminAccounts/{page=0:int}")]
        public IActionResult Index(
          int page=1,
          string? Search = "",
          int AdminId = 0,
          string? UserStatus = "",
          string? Role = "",
          int? CentreId = 0,
          int? itemsPerPage = 10,
          string? SearchString = "",
          string? ExistingFilterString = ""
        )
        {
            var loggedInSuperAdmin = userDataService.GetAdminById(User.GetAdminId()!.Value);
            if(loggedInSuperAdmin.AdminAccount.Active == false)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(SearchString) || string.IsNullOrEmpty(ExistingFilterString))
            {
                page = 1;
            }

            int offSet = ((page - 1) * itemsPerPage) ?? 0;
            UserStatus = (string.IsNullOrEmpty(UserStatus) ? "Any" : UserStatus);
            Role = (string.IsNullOrEmpty(Role) ? "Any" : Role);

            if (!string.IsNullOrEmpty(SearchString))
            {
                List<string> searchFilters = SearchString.Split("-").ToList();
                if (searchFilters.Count == 2)
                {
                    string searchFilter = searchFilters[0];
                    if (searchFilter.Contains("SearchQuery|"))
                    {
                        Search = searchFilter.Split("|")[1];
                    }

                    string userIdFilter = searchFilters[1];
                    if (userIdFilter.Contains("AdminID|"))
                    {
                        AdminId = Convert.ToInt16(userIdFilter.Split("|")[1]);
                    }
                }
            }

            if (!string.IsNullOrEmpty(ExistingFilterString))
            {
                List<string> selectedFilters = ExistingFilterString.Split("-").ToList();
                if (selectedFilters.Count == 3)
                {
                    string adminStatusFilter = selectedFilters[0];
                    if (adminStatusFilter.Contains("UserStatus|"))
                    {
                        UserStatus = adminStatusFilter.Split("|")[1];
                    }

                    string roleFilter = selectedFilters[1];
                    if (roleFilter.Contains("Role|"))
                    {
                        Role = roleFilter.Split("|")[1];
                    }

                    string centreFilter = selectedFilters[2];
                    if (centreFilter.Contains("CentreID|"))
                    {
                        CentreId = Convert.ToInt16(centreFilter.Split("|")[1]);
                    }
                }
            }

            (var Admins, var ResultCount) = this.userDataService.GetAllAdmins(Search ?? string.Empty, offSet, itemsPerPage ?? 0, AdminId, UserStatus, Role, CentreId, AuthHelper.FailedLoginThreshold);

            var centres = centresDataService.GetAllCentres().ToList();
            centres.Insert(0, (0, "Any"));

            //var loggedInSuperAdmin = userDataService.GetAdminById(User.GetAdminId()!.Value);

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                null,
                new SortOptions(GenericSortingHelper.DefaultSortOption, GenericSortingHelper.Ascending),
                null,
                new PaginationOptions(page, itemsPerPage)
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                Admins,
                searchSortPaginationOptions
            );

            result.Page = page;
            if (
                !string.IsNullOrEmpty(Search) ||
                AdminId != 0 ||
                !string.IsNullOrEmpty(UserStatus) ||
                !string.IsNullOrEmpty(Role) ||
                CentreId != 0
            )
            {
                result.SearchString = "SearchQuery|" + Search + "-AdminID|" + AdminId;
                result.FilterString = "UserStatus|" + UserStatus + "-Role|" + Role + "-CentreID|" + CentreId;
            }

            var model = new AdminAccountsViewModel(
                result,
                loggedInSuperAdmin!.AdminAccount
            );

            ViewBag.Roles = SelectListHelper.MapOptionsToSelectListItems(
                GetRoles(), Role
            );

            ViewBag.UserStatus = SelectListHelper.MapOptionsToSelectListItems(
                GetUserStatus(), UserStatus
            );

            ViewBag.Centres = SelectListHelper.MapOptionsToSelectListItems(
                centres, CentreId
            );

            model.TotalPages = (int)(ResultCount / itemsPerPage) + ((ResultCount % itemsPerPage) > 0 ? 1 : 0);
            model.MatchingSearchResults = ResultCount;
            model.AdminID = AdminId == 0 ? null : AdminId;
            model.UserStatus = UserStatus;
            model.Role = Role;
            model.CentreID = CentreId == 0 ? null : CentreId;
            model.Search = Search;


            model.JavascriptSearchSortFilterPaginateEnabled = false;
            ModelState.ClearAllErrors();
            return View(model);
        }

        [Route("Export")]
        public IActionResult Export(
            string? searchString = null,
            string? existingFilterString = null
        )
        {
            var content = adminDownloadFileService.GetAllAdminsFile(
                searchString,
                existingFilterString
            );

            const string fileName = "Digital Learning Solutions Administrators.xlsx";
            return File(
                content,
                FileHelper.GetContentTypeFromFileName(fileName),
                fileName
            );
        }

        public List<string> GetUserStatus()
        {
            return new List<string>(new string[] { "Any", "Active", "Inactive" });
        }

        public List<string> GetRoles()
        {
            var roles = new List<string>(new string[]{"Any"});
            
            foreach (var role in AdminAccountsViewModelFilterOptions.RoleOptions.Select(r=>r.DisplayText))
            {
                roles.Add(role);
            }

            return roles;
        }

        
    }
}
