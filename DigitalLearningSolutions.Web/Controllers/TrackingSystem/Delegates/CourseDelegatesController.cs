namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/CourseDelegates")]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    public class CourseDelegatesController : Controller
    {
        private const string CourseDelegatesFilterCookieName = "CourseDelegatesFilter";
        private readonly ICourseDelegatesService courseDelegatesService;

        public CourseDelegatesController(
            ICourseDelegatesService courseDelegatesService
        )
        {
            this.courseDelegatesService = courseDelegatesService;
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
            filterBy = FilteringHelper.GetFilterBy(
                filterBy,
                filterValue,
                Request,
                CourseDelegatesFilterCookieName
            );

            var centreId = User.GetCentreId();
            int? categoryId = User.GetAdminCategoryId()!.Value;
            categoryId = categoryId == 0 ? null : categoryId;
            var courseDelegatesData =
                courseDelegatesService.GetCoursesAndCourseDelegatesForCentre(centreId, categoryId, customisationId);

            if (courseDelegatesData == null)
            {
                return NotFound();
            }

            var model = new CourseDelegatesViewModel(courseDelegatesData, sortBy, sortDirection, filterBy, page);

            Response.UpdateOrDeleteFilterCookie(CourseDelegatesFilterCookieName, filterBy);
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
    }
}
