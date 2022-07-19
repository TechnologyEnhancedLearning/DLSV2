namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
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

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("TrackingSystem/Delegates/CourseDelegates")]
    public class CourseDelegatesController : Controller
    {
        private const string CourseDelegatesFilterCookieName = "CourseDelegatesFilter";
        private readonly ICourseAdminFieldsService courseAdminFieldsService;
        private readonly ICourseDelegatesDownloadFileService courseDelegatesDownloadFileService;
        private readonly ICourseDelegatesService courseDelegatesService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;

        public CourseDelegatesController(
            ICourseAdminFieldsService courseAdminFieldsService,
            ICourseDelegatesService courseDelegatesService,
            ICourseDelegatesDownloadFileService courseDelegatesDownloadFileService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService
        )
        {
            this.courseAdminFieldsService = courseAdminFieldsService;
            this.courseDelegatesService = courseDelegatesService;
            this.courseDelegatesDownloadFileService = courseDelegatesDownloadFileService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
        }

        [Route("{page:int=1}")]
        public IActionResult Index(
            int? customisationId = null,
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            string? existingFilterString = null,
            string? newFilterToAdd = null,
            bool clearFilters = false,
            int page = 1
        )
        {
            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            var newFilterString = FilteringHelper.GetFilterString(
                existingFilterString,
                newFilterToAdd,
                clearFilters,
                Request,
                CourseDelegatesFilterCookieName,
                CourseDelegateAccountStatusFilterOptions.Active.FilterValue
            );

            var centreId = User.GetCentreIdKnownNotNull();
            var adminCategoryId = User.GetAdminCategoryId();

            try
            {
                var courseDelegatesData = courseDelegatesService.GetCoursesAndCourseDelegatesForCentre(
                    centreId,
                    adminCategoryId,
                    customisationId
                );

                var availableFilters = CourseDelegateViewModelFilterOptions.GetAllCourseDelegatesFilterViewModels(
                    courseDelegatesData.CourseAdminFields
                );

                var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                    new SearchOptions(searchString),
                    new SortOptions(sortBy, sortDirection),
                    new FilterOptions(
                        newFilterString,
                        availableFilters,
                        CourseDelegateAccountStatusFilterOptions.Active.FilterValue
                    ),
                    new PaginationOptions(page)
                );

                var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                    courseDelegatesData.Delegates,
                    searchSortPaginationOptions
                );

                var model = new CourseDelegatesViewModel(
                    courseDelegatesData,
                    result,
                    availableFilters,
                    "customisationId"
                );

                Response.UpdateFilterCookie(CourseDelegatesFilterCookieName, result.FilterString);
                return View(model);
            }
            catch (CourseAccessDeniedException)
            {
                return NotFound();
            }
        }

        [NoCaching]
        [Route("AllCourseDelegates/{customisationId:int}")]
        public IActionResult AllCourseDelegates(int customisationId)
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var courseDelegates = courseDelegatesService.GetCourseDelegatesForCentre(customisationId, centreId);
            var adminFields = courseAdminFieldsService.GetCourseAdminFieldsForCourse(customisationId);
            var model = new AllCourseDelegatesViewModel(courseDelegates, adminFields.AdminFields);

            return View(model);
        }

        [ServiceFilter(typeof(VerifyAdminUserCanViewCourse))]
        [Route("DownloadCurrent/{customisationId:int}")]
        public IActionResult DownloadCurrent(
            int customisationId,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            string? existingFilterString = null
        )
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var content = courseDelegatesDownloadFileService.GetCourseDelegateDownloadFileForCourse(
                customisationId,
                centreId,
                sortBy,
                existingFilterString,
                sortDirection
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
