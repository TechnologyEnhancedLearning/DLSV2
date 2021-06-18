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
        private readonly IUserDataService userDataService;
        private readonly ICourseService courseService;

        public ContractDetailsController(ICentresDataService centresDataService, IUserDataService userDataService, ICourseService courseService)
        {
            this.centresDataService = centresDataService;
            this.userDataService = userDataService;
            this.courseService = courseService;
        }

        public IActionResult Index()
        {
            int centreId = User.GetCentreId();
            var centreDetails = centresDataService.GetCentreDetailsById(centreId)!;
            var adminUsersAtCentre = userDataService.GetAdminUsersByCentreId(centreId);
            var numberOfCourses = courseService.GetNumberOfActiveCoursesAtCentreForCategory(centreId, 0);

            return View(new ContractDetailsViewModel(adminUsersAtCentre, centreDetails, numberOfCourses));
        }
    }
}
