namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Dashboard
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
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
        private readonly ICentresDataService centresDataService;
        private readonly ICourseDataService courseDataService;
        private readonly IUserDataService userDataService;

        public ContractDetailsController(
            ICentresDataService centresDataService,
            IUserDataService userDataService,
            ICourseDataService courseDataService
        )
        {
            this.centresDataService = centresDataService;
            this.userDataService = userDataService;
            this.courseDataService = courseDataService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var centreDetails = centresDataService.GetCentreDetailsById(centreId)!;
            var adminUsersAtCentre = userDataService.GetAdminUsersByCentreId(centreId);
            var numberOfCourses = courseDataService.GetNumberOfActiveCoursesAtCentreFilteredByCategory(centreId, null);

            var model = new ContractDetailsViewModel(adminUsersAtCentre, centreDetails, numberOfCourses);

            return View(model);
        }
    }
}
