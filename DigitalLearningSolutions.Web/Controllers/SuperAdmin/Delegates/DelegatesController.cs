
namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin.Delegates
{
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Delegates;
    using DigitalLearningSolutions.Web.Extensions;
    [FeatureGate(FeatureFlags.RefactoredSuperAdminInterface)]
    [Authorize(Policy = CustomPolicies.UserSuperAdmin)]

    [SetDlsSubApplication(nameof(DlsSubApplication.SuperAdmin))]
    [SetSelectedTab(nameof(NavMenuTab.Admins))]
    public class DelegatesController : Controller
    {
        private readonly IUserDataService userDataService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly ICentresDataService centresDataService;
        private readonly IUserService userService;
        public DelegatesController(IUserDataService userDataService,
            ICentresDataService centresDataService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService,
            IUserService userService
            )
        {
            this.userDataService = userDataService;
            this.centresDataService = centresDataService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
            this.userService = userService;
        }

        [Route("SuperAdmin/Delegates/{page=0:int}")]
        public IActionResult Index(
          int page = 1,
          string? Search = "",
          int DelegateId = 0,
          string? AccountStatus = "",
          string? LHLinkStatus = "",
          int? CentreId = 0,
          int? itemsPerPage = 10,
          string? SearchString = "",
          string? ExistingFilterString = "")
        {
            var loggedInSuperAdmin = userDataService.GetDelegateById(User.GetAdminId()!.Value);
            if (loggedInSuperAdmin.DelegateAccount.Active == false)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(SearchString) || string.IsNullOrEmpty(ExistingFilterString))
            {
                page = 1;
            }

            int offSet = ((page - 1) * itemsPerPage) ?? 0;
            AccountStatus = (string.IsNullOrEmpty(AccountStatus) ? "Any" : AccountStatus);
            LHLinkStatus = (string.IsNullOrEmpty(LHLinkStatus) ? "Any" : LHLinkStatus);
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
                    if (userIdFilter.Contains("DelegateId|"))
                    {
                        DelegateId = Convert.ToInt16(userIdFilter.Split("|")[1]);
                    }
                }
            }
            if (!string.IsNullOrEmpty(ExistingFilterString))
            {
                List<string> selectedFilters = ExistingFilterString.Split("-").ToList();
                if (selectedFilters.Count == 3)
                {
                    string accountStatusFilter = selectedFilters[0];
                    if (accountStatusFilter.Contains("AccountStatus|"))
                    {
                        AccountStatus = accountStatusFilter.Split("|")[1];
                    }

                    string LHLinkStatusFilter = selectedFilters[1];
                    if (LHLinkStatusFilter.Contains("Role|"))
                    {
                        LHLinkStatus = LHLinkStatusFilter.Split("|")[1];
                    }

                    string centreFilter = selectedFilters[2];
                    if (centreFilter.Contains("CentreID|"))
                    {
                        CentreId = Convert.ToInt16(centreFilter.Split("|")[1]);
                    }
                }
            }
            (var Delegates, var ResultCount) = this.userDataService.GetAllDelegates(Search ?? string.Empty, offSet, itemsPerPage ?? 0, DelegateId, AccountStatus, LHLinkStatus, CentreId, AuthHelper.FailedLoginThreshold);

            var centres = centresDataService.GetAllCentres().ToList();
            centres.Insert(0, (0, "Any"));

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                null,
                new SortOptions(GenericSortingHelper.DefaultSortOption, GenericSortingHelper.Ascending),
                null,
                new PaginationOptions(page, itemsPerPage)
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                Delegates,
                searchSortPaginationOptions
            );

            result.Page = page;
            if (
                !string.IsNullOrEmpty(Search) ||
                DelegateId != 0 ||
                !string.IsNullOrEmpty(AccountStatus) ||
                !string.IsNullOrEmpty(LHLinkStatus) ||
                CentreId != 0
            )
            {
                result.SearchString = "SearchQuery|" + Search + "-DelegateId|" + DelegateId;
                result.FilterString = "AccountStatus|" + AccountStatus + "-LHLinkStatus|" + LHLinkStatus + "-CentreID|" + CentreId;
                TempData["SearchString"] = result.SearchString;
                TempData["FilterString"] = result.FilterString;
            }
            TempData["Page"] = result.Page;
            var model = new DelegatesViewModel(
                result,
                loggedInSuperAdmin!.DelegateAccount
            );

            ViewBag.LHLinkStatus = SelectListHelper.MapOptionsToSelectListItems(
                GetLHLinkStatus(), LHLinkStatus
            );

            ViewBag.AccountStatus = SelectListHelper.MapOptionsToSelectListItems(
                GetAccoutntStatus(), AccountStatus
            );

            ViewBag.Centres = SelectListHelper.MapOptionsToSelectListItems(
                centres, CentreId
            );

            model.TotalPages = (int)(ResultCount / itemsPerPage) + ((ResultCount % itemsPerPage) > 0 ? 1 : 0);
            model.MatchingSearchResults = ResultCount;
            model.DelegateID = DelegateId == 0 ? null : DelegateId;
            model.AccountStatus = AccountStatus;
            model.LHLinkStatus  = LHLinkStatus;
            model.CentreID = CentreId == 0 ? null : CentreId;
            model.Search = Search;


            model.JavascriptSearchSortFilterPaginateEnabled = false;
            ViewBag.DelegateId = TempData["DelegateId"];
            ModelState.ClearAllErrors();
            return View(model);
        }
        public List<string> GetAccoutntStatus()
        {
            return new List<string>(new string[] { "Any", "Active", "Inactive", "Approved", "Unapproved", "Claimed", "Unclaimed" });
        }
        public List<string> GetLHLinkStatus()
        {
            return new List<string>(new string[] { "Any", "Linked", "Not linked"});
        }
    }
}
