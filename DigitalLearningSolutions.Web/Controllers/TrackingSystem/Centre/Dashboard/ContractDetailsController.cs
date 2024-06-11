namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Dashboard
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.ContractDetails;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Centre))]
    [Route("/TrackingSystem/Centre/ContractDetails")]
    public class ContractDetailsController : Controller
    {
        private readonly ICentresService centresService;
        private readonly ICourseService courseService;
        private readonly IUserService userService;

        public ContractDetailsController(
            ICentresService centresService,
            IUserService userService,
            ICourseService courseService
        )
        {
            this.centresService = centresService;
            this.userService = userService;
            this.courseService = courseService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var centreDetails = centresService.GetCentreDetailsById(centreId)!;
            var adminUsersAtCentre = userService.GetAdminUsersByCentreId(centreId);
            var numberOfCourses = courseService.GetNumberOfActiveCoursesAtCentreFilteredByCategory(centreId, null);

            var model = new ContractDetailsViewModel(adminUsersAtCentre, centreDetails, numberOfCourses);

            return View(model);
        }
    }
}
