namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using System;
    using System.Linq;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("TrackingSystem/Delegates/CourseDelegates")]
    public class CourseDelegatesController : Controller
    {
        private const string CourseDelegatesFilterCookieName = "CourseDelegatesFilter";
        private readonly ICourseDelegatesDownloadFileService courseDelegatesDownloadFileService;
        private readonly ICourseDelegatesService courseDelegatesService;
        private readonly IPaginateService paginateService;

        public CourseDelegatesController(
            ICourseDelegatesService courseDelegatesService,
            ICourseDelegatesDownloadFileService courseDelegatesDownloadFileService,
            IPaginateService paginateService
        )
        {
            this.courseDelegatesService = courseDelegatesService;
            this.courseDelegatesDownloadFileService = courseDelegatesDownloadFileService;
            this.paginateService = paginateService;
        }

        [NoCaching]
        [Route("{page:int=1}")]
        public IActionResult Index(
            int? customisationId = null,
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            string? existingFilterString = null,
            string? newFilterToAdd = null,
            bool clearFilters = false,
            int page = 1,
            int? itemsPerPage = 10
        )
        {
            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            sortDirection ??= GenericSortingHelper.Ascending;

            existingFilterString = FilteringHelper.GetFilterString(
                existingFilterString,
                newFilterToAdd,
                clearFilters,
                Request,
                CourseDelegatesFilterCookieName,
                CourseDelegateAccountStatusFilterOptions.Active.FilterValue
            );

            if (TempData["customisationId"] != null && TempData["customisationId"].ToString() != customisationId.ToString()
                    && existingFilterString != null && existingFilterString.Contains("Answer"))
            {
                var selectedFilters = existingFilterString!.Split(FilteringHelper.FilterSeparator).Where(filter=> !filter.Contains("Answer")).ToList();
                existingFilterString = selectedFilters.Any() ? string.Join(FilteringHelper.FilterSeparator, selectedFilters) : null;
            }

            int offSet = ((page - 1) * itemsPerPage) ?? 0;

            var centreId = User.GetCentreIdKnownNotNull();
            var adminCategoryId = User.GetAdminCategoryId();

            bool? isDelegateActive, isProgressLocked, removed, hasCompleted;            
            isDelegateActive = isProgressLocked = removed = hasCompleted = null;

            string? answer1, answer2, answer3;
            answer1 = answer2 = answer3 = null;

            if (!string.IsNullOrEmpty(existingFilterString))
            {
                var selectedFilters = existingFilterString.Split(FilteringHelper.FilterSeparator).ToList();

                if (!string.IsNullOrEmpty(newFilterToAdd))
                {
                    var filterHeader = newFilterToAdd.Split(FilteringHelper.Separator)[0];
                    var dupfilters = selectedFilters.Where(x => x.Contains(filterHeader));
                    if (dupfilters.Count() > 1)
                    {
                        foreach (var filter in selectedFilters)
                        {
                            if (filter.Contains(filterHeader))
                            {
                                selectedFilters.Remove(filter);
                                existingFilterString = string.Join(FilteringHelper.FilterSeparator, selectedFilters);
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
                        dynamic filterValue = filterArr[2];
                        switch (filterValue)
                        {
                            case FilteringHelper.EmptyValue:
                                filterValue = "No option selected"; break;
                            case "true":
                                filterValue = true; break;
                            case "false":
                                filterValue = false; break;
                        }

                        if (filter.Contains("AccountStatus"))
                            isDelegateActive = filterValue;

                        if (filter.Contains("ProgressLocked"))
                            isProgressLocked = filterValue;

                        if (filter.Contains("ProgressRemoved"))
                            removed = filterValue;

                        if (filter.Contains("CompletionStatus"))
                            hasCompleted = filterValue;

                        if (filter.Contains("Answer1"))
                            answer1 = filterValue;

                        if (filter.Contains("Answer2"))
                            answer2 = filterValue;

                        if (filter.Contains("Answer3"))
                            answer3 = filterValue;
                    }
                }
            }

            try
            {
                var (courseDelegatesData, resultCount) = courseDelegatesService.GetCoursesAndCourseDelegatesPerPageForCentre(searchString ?? string.Empty, offSet, itemsPerPage ?? 0, sortBy, sortDirection,
                customisationId, centreId, adminCategoryId, isDelegateActive, isProgressLocked, removed, hasCompleted, answer1, answer2, answer3);

                if (courseDelegatesData.Delegates.Count() == 0 && resultCount > 0)
                {
                    page = 1;
                    offSet = 0;
                    (courseDelegatesData, resultCount) = courseDelegatesService.GetCoursesAndCourseDelegatesPerPageForCentre(searchString ?? string.Empty, offSet, itemsPerPage ?? 0, sortBy, sortDirection,
                        customisationId, centreId, adminCategoryId, isDelegateActive, isProgressLocked, removed, hasCompleted, answer1, answer2, answer3);

                }

                var availableFilters = CourseDelegateViewModelFilterOptions.GetAllCourseDelegatesFilterViewModels(
                    courseDelegatesData.CourseAdminFields
                );

                var result = paginateService.Paginate(
                    courseDelegatesData.Delegates,
                    resultCount,
                    new PaginationOptions(page, itemsPerPage),
                    new FilterOptions(existingFilterString, availableFilters, CourseDelegateAccountStatusFilterOptions.Active.FilterValue),
                    searchString,
                    sortBy,
                    sortDirection
                );

                result.Page = page;
                TempData["Page"] = result.Page;

                var model = new CourseDelegatesViewModel(
                    courseDelegatesData,
                    result,
                    availableFilters,
                    "customisationId"
                );

                Response.UpdateFilterCookie(CourseDelegatesFilterCookieName, result.FilterString);
                TempData["customisationId"] = customisationId;
                return View(model);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [ServiceFilter(typeof(VerifyAdminUserCanViewCourse))]
        [Route("DownloadCurrent/{customisationId:int}")]
        public IActionResult DownloadCurrent(
            int customisationId,
             string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            string? existingFilterString = null
        )
        {
            var centreId = User.GetCentreIdKnownNotNull();

            bool? isDelegateActive, isProgressLocked, removed, hasCompleted;
            isDelegateActive = isProgressLocked = removed = hasCompleted = null;

            string? answer1, answer2, answer3;
            answer1 = answer2 = answer3 = null;

            if (!string.IsNullOrEmpty(existingFilterString))
            {
                var selectedFilters = existingFilterString.Split(FilteringHelper.FilterSeparator).ToList();

                if (selectedFilters.Count > 0)
                {
                    foreach (var filter in selectedFilters)
                    {
                        var filterArr = filter.Split(FilteringHelper.Separator);
                        dynamic filterValue = filterArr[2];
                        switch (filterValue)
                        {
                            case FilteringHelper.EmptyValue:
                                filterValue = "No option selected"; break;
                            case "true":
                                filterValue = true; break;
                            case "false":
                                filterValue = false; break;
                        }

                        if (filter.Contains("AccountStatus"))
                            isDelegateActive = filterValue;

                        if (filter.Contains("ProgressLocked"))
                            isProgressLocked = filterValue;

                        if (filter.Contains("ProgressRemoved"))
                            removed = filterValue;

                        if (filter.Contains("CompletionStatus"))
                            hasCompleted = filterValue;

                        if (filter.Contains("Answer1"))
                            answer1 = filterValue;

                        if (filter.Contains("Answer2"))
                            answer2 = filterValue;

                        if (filter.Contains("Answer3"))
                            answer3 = filterValue;
                    }
                }
            }

            var content = courseDelegatesDownloadFileService.GetCourseDelegateDownloadFileForCourse(searchString ?? string.Empty, sortBy, sortDirection,
                    customisationId, centreId, isDelegateActive, isProgressLocked, removed, hasCompleted, answer1, answer2, answer3
            );

            const string fileName = "Digital Learning Solutions Course Delegates.xlsx";
            return File(
                content,
            FileHelper.GetContentTypeFromFileName(fileName),
            fileName
            );
        }
    }
}
