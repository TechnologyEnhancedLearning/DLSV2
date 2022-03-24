namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateCourses;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("TrackingSystem/Delegates/Courses")]
    public class DelegateCoursesController : Controller
    {
        private const string CourseFilterCookieName = "DelegateCoursesFilter";
        private readonly ICourseDelegatesDownloadFileService courseDelegatesDownloadFileService;
        private readonly ICourseService courseService;

        public DelegateCoursesController(
            ICourseService courseService,
            ICourseDelegatesDownloadFileService courseDelegatesDownloadFileService
        )
        {
            this.courseService = courseService;
            this.courseDelegatesDownloadFileService = courseDelegatesDownloadFileService;
        }

        [Route("{page=1:int}")]
        public IActionResult Index(
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            string? existingFilterString = null,
            string? newFilterToAdd = null,
            int page = 1,
            int? itemsPerPage = null
        )
        {
            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            existingFilterString = FilteringHelper.GetFilterString(
                existingFilterString,
                newFilterToAdd,
                Request,
                CourseFilterCookieName,
                CourseStatusFilterOptions.IsActive.FilterValue
            );

            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCourseCategoryFilter();

            var details = courseService.GetDelegateCourseDetails(centreId, categoryId);

            var model = new DelegateCoursesViewModel(
                details,
                searchString,
                sortBy,
                sortDirection,
                existingFilterString,
                page,
                itemsPerPage
            );

            Response.UpdateOrDeleteFilterCookie(CourseFilterCookieName, existingFilterString);

            return View(model);
        }

        [Route("AllCourseStatistics")]
        public IActionResult AllCourseStatistics()
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCourseCategoryFilter();
            var details = courseService.GetDelegateCourseDetails(centreId, categoryId);

            var model = new AllDelegateCourseStatisticsViewModel(details);

            return View(model);
        }

        [Route("DownloadAll")]
        public IActionResult DownloadAll(
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            string? existingFilterString = null
        )
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCourseCategoryFilter();
            var content = courseDelegatesDownloadFileService.GetCourseDelegateDownloadFile(
                centreId,
                categoryId,
                searchString,
                sortBy,
                existingFilterString,
                sortDirection
            );

            const string fileName = "Digital Learning Solutions Delegate Courses.xlsx";
            return File(
                content,
                FileHelper.GetContentTypeFromFileName(fileName),
                fileName
            );
        }
    }
}
