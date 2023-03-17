namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin.Users
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
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Users;
    using DigitalLearningSolutions.Web.ViewModels.UserCentreAccounts;
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
    public class UsersController : Controller
    {
        private readonly IUserDataService userDataService;
        private readonly ICentreRegistrationPromptsDataService centreRegistrationPromptsDataService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private const string UserAccountFilterCookieName = "UserAccountFilter";
        private readonly IUserService userService;
        private readonly IUserCentreAccountsService userCentreAccountsService;
        public UsersController(IUserDataService userDataService, ICentreRegistrationPromptsDataService centreRegistrationPromptsDataService, ISearchSortFilterPaginateService searchSortFilterPaginateService, IJobGroupsDataService jobGroupsDataService,IUserCentreAccountsService userCentreAccountsService, IUserService userService)
        {
            this.userDataService = userDataService;
            this.centreRegistrationPromptsDataService = centreRegistrationPromptsDataService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.userService = userService;
            this.userCentreAccountsService = userCentreAccountsService;
        }

        [Route("SuperAdmin/Users/{page=0:int}")]
        public IActionResult Index(
          int page=1,
          string? Search = "",
          int UserId = 0,
          string? UserStatus = "",
          string? EmailStatus = "",
          int JobGroupId = 0,
          int? itemsPerPage = 10,
          string? SearchString = "",
          string? ExistingFilterString = ""
        )
        {
            int offSet = ((page - 1) * itemsPerPage) ?? 0;
            UserStatus = (string.IsNullOrEmpty(UserStatus) ? "Any" : UserStatus);
            EmailStatus = (string.IsNullOrEmpty(EmailStatus) ? "Any" : EmailStatus);

            if(string.IsNullOrEmpty(SearchString) || string.IsNullOrEmpty(ExistingFilterString))
            {
                page = 1;
            }

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
                    if (userIdFilter.Contains("UserId|"))
                    {
                        UserId = Convert.ToInt16(userIdFilter.Split("|")[1]);
                    }
                }
            }

            if (!string.IsNullOrEmpty(ExistingFilterString))
            {
                List<string> selectedFilters = ExistingFilterString.Split("-").ToList();
                if (selectedFilters.Count == 3)
                {
                    string userStatusFilter = selectedFilters[0];
                    if (userStatusFilter.Contains("UserStatus|"))
                    {
                        UserStatus = userStatusFilter.Split("|")[1];
                    }

                    string emailStatusFilter = selectedFilters[1];
                    if (emailStatusFilter.Contains("EmailStatus|"))
                    {
                        EmailStatus = emailStatusFilter.Split("|")[1];
                    }

                    string jobGroupFilter = selectedFilters[2];
                    if (jobGroupFilter.Contains("JobGroup|"))
                    {
                        JobGroupId = Convert.ToInt16(jobGroupFilter.Split("|")[1]);
                    }
                }
            }

            (var UserAccounts, var ResultCount) = this.userDataService.GetUserAccounts(Search ?? string.Empty, offSet, itemsPerPage ?? 0, JobGroupId, UserStatus, EmailStatus, UserId, AuthHelper.FailedLoginThreshold);

            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();
            jobGroups.Insert(0, (0, "Any"));

            var loggedInUser = userDataService.GetUserAccountById(User.GetUserId()!.Value);

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                null,
                new SortOptions(GenericSortingHelper.DefaultSortOption, GenericSortingHelper.Ascending),
                null,
                new PaginationOptions(page, itemsPerPage)
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                UserAccounts,
                searchSortPaginationOptions
            );

            result.Page = page;
            if (
                !string.IsNullOrEmpty(Search) ||
                UserId != 0 ||
                !string.IsNullOrEmpty(UserStatus) ||
                !string.IsNullOrEmpty(EmailStatus) ||
                JobGroupId != 0
            )
            {
                result.SearchString = "SearchQuery|" + Search + "-UserId|" + UserId;
                result.FilterString = "UserStatus|" + UserStatus + "-EmailStatus|" + EmailStatus + "-JobGroup|" + JobGroupId;
            }


            var model = new UserAccountsViewModel(
                result
            );

            ViewBag.JobGroups = SelectListHelper.MapOptionsToSelectListItems(
                jobGroups, JobGroupId
            );

            ViewBag.UserStatus = SelectListHelper.MapOptionsToSelectListItems(
                GetUserStatus(), UserStatus
            );

            ViewBag.EmailStatus = SelectListHelper.MapOptionsToSelectListItems(
                GetEmailStatus(), EmailStatus
            );

            model.TotalPages = (int)(ResultCount / itemsPerPage) + ((ResultCount % itemsPerPage) > 0 ? 1 : 0);
            model.MatchingSearchResults = ResultCount;
            model.UserId = UserId == 0 ? null : UserId;
            model.UserStatus = UserStatus;
            model.EmailStatus = EmailStatus;
            model.JobGroupId = JobGroupId;
            model.Search = Search;


            model.JavascriptSearchSortFilterPaginateEnabled = false;
            ModelState.ClearAllErrors();
            return View(model);
        }

        public List<string> GetUserStatus()
        {
            return new List<string>(new string[] { "Any", "Active", "Locked", "Inactive" });
        }

        public List<string> GetEmailStatus()
        {
            return new List<string>(new string[] { "Any", "Verified", "Unverified" });
        }

        [Route("SuperAdmin/Users/Administrators")]
        public IActionResult Administrators()
        {
            var model = new AdministratorsViewModel();
            return View(model);
        }
        [Route("SuperAdmin/Users/{userId:int}/CentreAccounts")]
        public IActionResult CentreAccounts(int userId)
        {
            TempData["UserID"] = userId;
            var userEntity = userService.GetUserById(userId);
            var UserCentreAccountsRoleViewModel =
                userCentreAccountsService.GetUserCentreAccountsRoleViewModel(userEntity);
            var model = new UserCentreAccountRoleViewModel(
                     UserCentreAccountsRoleViewModel.OrderByDescending(account => account.IsActiveAdmin)
                         .ThenBy(account => account.CentreName).ToList(),
                     userEntity
                 );
            return View("UserCentreAccounts", model);
        }
    }
}
