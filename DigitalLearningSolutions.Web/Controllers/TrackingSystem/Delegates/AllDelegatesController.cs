﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AllDelegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using Microsoft.Extensions.Configuration;
    using DigitalLearningSolutions.Data.Extensions;
    using Microsoft.AspNetCore.Hosting;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("TrackingSystem/Delegates/All")]
    public class AllDelegatesController : Controller
    {
        private string DelegateFilterCookieName = "DelegateFilter";
        private readonly IDelegateDownloadFileService delegateDownloadFileService;
        private readonly IJobGroupsService jobGroupsService;
        private readonly PromptsService promptsService;
        private readonly IPaginateService paginateService;
        private readonly IUserService userService;
        private readonly IGroupsService groupsService;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment env;

        public AllDelegatesController(
            IDelegateDownloadFileService delegateDownloadFileService,
            IUserService userDataService,
            PromptsService promptsService,
            IJobGroupsService jobGroupsDataService,
            IPaginateService paginateService,
            IGroupsService groupsService,
            IConfiguration configuration,
            IWebHostEnvironment env
        )
        {
            this.delegateDownloadFileService = delegateDownloadFileService;
            this.userService = userDataService;
            this.promptsService = promptsService;
            this.jobGroupsService = jobGroupsDataService;
            this.paginateService = paginateService;
            this.groupsService = groupsService;
            this.configuration = configuration;
            this.env = env;
        }

        [NoCaching]
        [Route("{page=1:int}")]
        public IActionResult Index(
            int page = 1,
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            string? existingFilterString = null,
            string? newFilterToAdd = null,
            bool clearFilters = false,
            int? itemsPerPage = 10
        )
        {
            searchString = searchString == null ? string.Empty : searchString.Trim();
            var loggedInSuperAdmin = userService.GetAdminById(User.GetAdminId()!.Value);
            if (loggedInSuperAdmin.AdminAccount.Active == false)
            {
                return NotFound();
            }

            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            sortDirection ??= GenericSortingHelper.Ascending;

            DelegateFilterCookieName += env.EnvironmentName;

            int offSet = ((page - 1) * itemsPerPage) ?? 0;

            var centreId = User.GetCentreIdKnownNotNull();
            var jobGroups = jobGroupsService.GetJobGroupsAlphabetical();
            var customPrompts = promptsService.GetCentreRegistrationPrompts(centreId).ToList();
            var groups = groupsService.GetActiveGroups(centreId);

            var promptsWithOptions = customPrompts.Where(customPrompt => customPrompt.Options.Count > 0);
            var availableFilters = AllDelegatesViewModelFilterOptions.GetAllDelegatesFilterViewModels(jobGroups, promptsWithOptions, groups);

            var filterString = FilteringHelper.GetFilterString(
                existingFilterString,
                newFilterToAdd,
                clearFilters,
                Request,
                DelegateFilterCookieName,
                DelegateActiveStatusFilterOptions.IsActive.FilterValue,
                availableFilters
            );

            string isActive, isPasswordSet, isAdmin, isUnclaimed, isEmailVerified, registrationType, answer1, answer2, answer3, answer4, answer5, answer6;
            isActive = isPasswordSet = isAdmin = isUnclaimed = isEmailVerified = registrationType = answer1 = answer2 = answer3 = answer4 = answer5 = answer6 = "Any";
            int jobGroupId = 0;
            int? groupId = null;

            if (!string.IsNullOrEmpty(filterString))
            {
                var selectedFilters = filterString.Split(FilteringHelper.FilterSeparator).ToList();

                if (selectedFilters.Count > 0)
                {
                    foreach (var filter in selectedFilters)
                    {
                        var filterArr = filter.Split(FilteringHelper.Separator);
                        var filterValue = filterArr[2];
                        if (filterValue == FilteringHelper.EmptyValue) filterValue = "No option selected";

                        if (filter.Contains("IsPasswordSet"))
                            isPasswordSet = filterValue;

                        if (filter.Contains("IsAdmin"))
                            isAdmin = filterValue;

                        if (filter.Contains("Active"))
                            isActive = filterValue;

                        if (filter.Contains("RegistrationType"))
                            registrationType = filterValue;

                        if (filter.Contains("IsYetToBeClaimed"))
                            isUnclaimed = filterValue;

                        if (filter.Contains("IsEmailVerified"))
                            isEmailVerified = filterValue;

                        if (filter.Contains("JobGroupId"))
                            jobGroupId = Convert.ToInt32(filterValue);

                        if (filter.Contains("DelegateGroupId"))
                        {
                            groupId = Convert.ToInt32(filterValue);
                        }

                        if (filter.Contains("Answer1"))
                            answer1 = filterValue;

                        if (filter.Contains("Answer2"))
                            answer2 = filterValue;

                        if (filter.Contains("Answer3"))
                            answer3 = filterValue;

                        if (filter.Contains("Answer4"))
                            answer4 = filterValue;

                        if (filter.Contains("Answer5"))
                            answer5 = filterValue;

                        if (filter.Contains("Answer6"))
                            answer6 = filterValue;
                    }
                }
            }

            (var delegates, var resultCount) = this.userService.GetDelegateUserCards(searchString ?? string.Empty, offSet, itemsPerPage ?? 0, sortBy, sortDirection, centreId,
                                                isActive, isPasswordSet, isAdmin, isUnclaimed, isEmailVerified, registrationType, jobGroupId, groupId,
                                                answer1, answer2, answer3, answer4, answer5, answer6);

            if (delegates.Count() == 0 && resultCount > 0)
            {
                page = 1; offSet = 0;
                (delegates, resultCount) = this.userService.GetDelegateUserCards(searchString ?? string.Empty, offSet, itemsPerPage ?? 0, sortBy, sortDirection, centreId,
                                                isActive, isPasswordSet, isAdmin, isUnclaimed, isEmailVerified, registrationType, jobGroupId, groupId,
                                                answer1, answer2, answer3, answer4, answer5, answer6);
            }

            var result = paginateService.Paginate(
                delegates,
                resultCount,
                new PaginationOptions(page, itemsPerPage),
                new FilterOptions(filterString, availableFilters, DelegateActiveStatusFilterOptions.IsActive.FilterValue),
                searchString,
                sortBy,
                sortDirection
            );

            result.Page = page;
            TempData["Page"] = result.Page;

            var model = new AllDelegatesViewModel(
                result,
                customPrompts,
                availableFilters
            );

            model.TotalPages = (int)(resultCount / itemsPerPage) + ((resultCount % itemsPerPage) > 0 ? 1 : 0);
            model.MatchingSearchResults = resultCount;

            Response.UpdateFilterCookie(DelegateFilterCookieName, filterString);
            TempData.Remove("delegateRegistered");
            return View(model);
        }

        [Route("Export")]
        public IActionResult Export(
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            string? existingFilterString = null
        )
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var content = delegateDownloadFileService.GetAllDelegatesFileForCentre(
                centreId,
                searchString,
                sortBy,
                sortDirection,
                existingFilterString
            );

            const string fileName = "Digital Learning Solutions Delegates.xlsx";
            return File(
                content,
                FileHelper.GetContentTypeFromFileName(fileName),
                fileName
            );
        }
    }
}
