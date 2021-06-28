namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.ContractDetails
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.ContractDetails;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("/TrackingSystem/Centre/ContractDetails")]
    public class ContractDetailsController : Controller
    {
        private readonly ICentresDataService centresDataService;
        private readonly ICourseService courseService;
        private readonly IUserDataService userDataService;

        public ContractDetailsController(
            ICentresDataService centresDataService,
            IUserDataService userDataService,
            ICourseService courseService
        )
        {
            this.centresDataService = centresDataService;
            this.userDataService = userDataService;
            this.courseService = courseService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreId();
            var centreDetails = centresDataService.GetCentreDetailsById(centreId)!;
            var adminUsersAtCentre = userDataService.GetAdminUsersByCentreId(centreId);
            var numberOfCourses = courseService.GetNumberOfActiveCoursesAtCentreForCategory(centreId, 0);

            var model = new ContractDetailsViewModel(adminUsersAtCentre, centreDetails, numberOfCourses);

            return View(model);
        }
    }
}
