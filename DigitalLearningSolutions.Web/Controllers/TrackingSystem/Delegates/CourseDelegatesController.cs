namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
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

        public CourseDelegatesController(
            ICourseAdminFieldsService courseAdminFieldsService,
            ICourseDelegatesService courseDelegatesService,
            ICourseDelegatesDownloadFileService courseDelegatesDownloadFileService
        )
        {
            this.courseAdminFieldsService = courseAdminFieldsService;
            this.courseDelegatesService = courseDelegatesService;
            this.courseDelegatesDownloadFileService = courseDelegatesDownloadFileService;
        }

        [Route("{page:int=1}")]
        public IActionResult Index(
            int? customisationId = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            string? existingFilterString = null,
            string? newFilterToAdd = null,
            int page = 1
        )
        {
            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            var newFilterString = FilteringHelper.GetFilterString(
                existingFilterString,
                newFilterToAdd,
                Request,
                CourseDelegatesFilterCookieName,
                CourseDelegateAccountStatusFilterOptions.Active.FilterValue
            );

            var centreId = User.GetCentreId();
            var adminCategoryId = User.GetAdminCourseCategoryFilter();
            CourseDelegatesData courseDelegatesData;

            try
            {
                courseDelegatesData =
                    courseDelegatesService.GetCoursesAndCourseDelegatesForCentre(
                        centreId,
                        adminCategoryId,
                        customisationId
                    );
            }
            catch (CourseAccessDeniedException)
            {
                return NotFound();
            }

            var model = new CourseDelegatesViewModel(
                courseDelegatesData,
                "customisationId",
                sortBy,
                sortDirection,
                newFilterString,
                page
            );

            Response.UpdateOrDeleteFilterCookie(CourseDelegatesFilterCookieName, newFilterString);
            return View(model);
        }

        [Route("AllCourseDelegates/{customisationId:int}")]
        public IActionResult AllCourseDelegates(int customisationId)
        {
            var centreId = User.GetCentreId();
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
            var centreId = User.GetCentreId();
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
