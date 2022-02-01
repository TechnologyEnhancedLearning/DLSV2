namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
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
        private readonly ICourseDelegatesDownloadFileService courseDelegatesDownloadFileService;
        private readonly ICourseDelegatesService courseDelegatesService;

        public CourseDelegatesController(
            ICourseDelegatesService courseDelegatesService,
            ICourseDelegatesDownloadFileService courseDelegatesDownloadFileService
        )
        {
            this.courseDelegatesService = courseDelegatesService;
            this.courseDelegatesDownloadFileService = courseDelegatesDownloadFileService;
        }

        [Route("{page:int=1}")]
        public IActionResult Index(
            int? customisationId = null,
            string? sortBy = null,
            string sortDirection = BaseSearchablePageViewModel.Ascending,
            string? filterBy = null,
            string? filterValue = null,
            int page = 1
        )
        {
            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            var newFilterBy = FilteringHelper.GetFilterBy(
                filterBy,
                filterValue,
                Request,
                CourseDelegatesFilterCookieName
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
            catch (CourseNotFoundException)
            {
                return NotFound();
            }

            var model = new CourseDelegatesViewModel(
                courseDelegatesData,
                "customisationId",
                sortBy,
                sortDirection,
                newFilterBy,
                page
            );

            Response.UpdateOrDeleteFilterCookie(CourseDelegatesFilterCookieName, newFilterBy);
            return View(model);
        }

        [Route("AllCourseDelegates/{customisationId:int}")]
        public IActionResult AllCourseDelegates(int customisationId)
        {
            var centreId = User.GetCentreId();
            var courseDelegates = courseDelegatesService.GetCourseDelegatesForCentre(customisationId, centreId);

            var model = new AllCourseDelegatesViewModel(courseDelegates);

            return View(model);
        }

        [ServiceFilter(typeof(VerifyAdminUserCanViewCourse))]
        [Route("DownloadCurrent/{customisationId:int}")]
        public IActionResult DownloadCurrent(int customisationId)
        {
            var centreId = User.GetCentreId();
            var adminCategoryId = User.GetAdminCourseCategoryFilter();
            var content = courseDelegatesDownloadFileService.GetCourseDelegateDownloadFileForCourse(
                customisationId,
                centreId,
                adminCategoryId
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
