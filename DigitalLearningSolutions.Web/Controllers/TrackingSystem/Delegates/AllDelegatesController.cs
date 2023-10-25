﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
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

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("TrackingSystem/Delegates/All")]
    public class AllDelegatesController : Controller
    {
        private const string DelegateFilterCookieName = "DelegateFilter";
        private readonly IDelegateDownloadFileService delegateDownloadFileService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly PromptsService promptsService;
        private readonly IPaginateService paginateService;
        private readonly IUserDataService userDataService;

        public AllDelegatesController(
            IDelegateDownloadFileService delegateDownloadFileService,
            IUserDataService userDataService,
            PromptsService promptsService,
            IJobGroupsDataService jobGroupsDataService,
            IPaginateService paginateService
        )
        {
            this.delegateDownloadFileService = delegateDownloadFileService;
            this.userDataService = userDataService;
            this.promptsService = promptsService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.paginateService = paginateService;
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
            var loggedInSuperAdmin = userDataService.GetAdminById(User.GetAdminId()!.Value);
            if (loggedInSuperAdmin.AdminAccount.Active == false)
            {
                return NotFound();
            }

            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            sortDirection ??= GenericSortingHelper.Ascending;
            existingFilterString = FilteringHelper.GetFilterString(
                existingFilterString,
                newFilterToAdd,
                clearFilters,
                Request,
                DelegateFilterCookieName,
                DelegateActiveStatusFilterOptions.IsActive.FilterValue
            );

            int offSet = ((page - 1) * itemsPerPage) ?? 0;            

            var centreId = User.GetCentreIdKnownNotNull();
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical();
            var customPrompts = promptsService.GetCentreRegistrationPrompts(centreId).ToList();

            string isActive = "Any";
            string isPasswordSet = "Any";
            string isAdmin = "Any";
            string isUnclaimed = "Any";
            string isEmailVerified = "Any";
            string registrationType = "Any";
            int jobGroupId = 0;
            string answer1 = "Any";
            string answer2 = "Any";
            string answer3 = "Any";
            string answer4 = "Any";
            string answer5 = "Any";
            string answer6 = "Any";

            if (!string.IsNullOrEmpty(existingFilterString))
            {
                var selectedFilters = existingFilterString.Split(FilteringHelper.FilterSeparator).ToList();

                if(!string.IsNullOrEmpty(newFilterToAdd))
                {
                    var filterHeader = newFilterToAdd.Split(FilteringHelper.Separator)[1];
                    var dupfilters = selectedFilters.Where(x => x.Contains(filterHeader));
                    if (dupfilters.Count() > 1)
                    {
                        foreach (var filter in selectedFilters)
                        {
                            if (filter.Contains(filterHeader))
                            {
                                selectedFilters.Remove(filter);
                                existingFilterString= string.Join(FilteringHelper.FilterSeparator,selectedFilters);
                                break;
                            }
                        }
                    }
                }

                if (selectedFilters.Count > 0)
                {
                    foreach (var filter in selectedFilters)
                    {
                        var filterArr = filter.Split(FilteringHelper.Separator);
                        var filterValue = filterArr[2];
                        if (filterValue == "╳") filterValue = "No option selected";

                        if (filter.Contains("IsPasswordSet"))
                            isPasswordSet= filterValue;

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
            

            (var delegates, var resultCount) = this.userDataService.GetDelegateUserCards(searchString ?? string.Empty, offSet, itemsPerPage ?? 0, sortBy, sortDirection, centreId,
                                                isActive, isPasswordSet, isAdmin, isUnclaimed, isEmailVerified, registrationType, jobGroupId,
                                                answer1, answer2, answer3, answer4, answer5, answer6);

            if (delegates.Count() == 0 && resultCount > 0)
            {
                page = 1;
                offSet = 0;
                (delegates, resultCount) = this.userDataService.GetDelegateUserCards(searchString ?? string.Empty, offSet, itemsPerPage ?? 0, sortBy, sortDirection, centreId,
                                                isActive, isPasswordSet, isAdmin, isUnclaimed, isEmailVerified, registrationType, jobGroupId,
                                                answer1, answer2, answer3, answer4, answer5, answer6);
            }

            var promptsWithOptions = customPrompts.Where(customPrompt => customPrompt.Options.Count > 0);
            var availableFilters = AllDelegatesViewModelFilterOptions.GetAllDelegatesFilterViewModels(
            jobGroups,
            promptsWithOptions
            );

            var result = paginateService.Paginate(
                delegates,
                resultCount,
                new PaginationOptions(page, itemsPerPage),
                new FilterOptions(existingFilterString, availableFilters, DelegateActiveStatusFilterOptions.IsActive.FilterValue),
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

            Response.UpdateFilterCookie(DelegateFilterCookieName, existingFilterString);
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
