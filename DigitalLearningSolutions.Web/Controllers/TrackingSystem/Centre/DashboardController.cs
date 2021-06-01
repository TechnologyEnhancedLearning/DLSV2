namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Dashboard;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = CustomPolicies.UserCentreAdminOnly)]
    [Route("/TrackingSystem/Centre/Dashboard")]
    public class DashboardController : Controller
    {
        private readonly IUserDataService userDataService;
        private readonly ICentresDataService centresDataService;

        public DashboardController(
            IUserDataService userDataService,
            ICentresDataService centresDataService
        )
        {
            this.userDataService = userDataService;
            this.centresDataService = centresDataService;
        }

        public IActionResult Index()
        {
            var adminUser = userDataService.GetAdminUserById(User.GetAdminId()!.Value);
            var centre = centresDataService.GetCentreDetailsById(User.GetCentreId());

            var model = new CentreDashboardViewModel(centre!, adminUser!.FirstName, adminUser!.CategoryName, Request.GetUserIpAddressFromRequest())
            {
                Delegates = 50,
                Courses = 12,
                Admins = 2,
                HelpTickets = 11
            };
            
            return View(model);
        }
    }
}
