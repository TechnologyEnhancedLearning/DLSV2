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
        private readonly ICentresDataService centresDataService;
        private readonly IUserDataService userDataService;

        public DashboardController
        (
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

            // TODO: HEEDLS-473 populate these numbers from the database
            var model = new CentreDashboardViewModel(
                centre!,
                adminUser!.FirstName,
                adminUser!.CategoryName,
                Request.GetUserIpAddressFromRequest(),
                50,
                12,
                2,
                11
            );

            return View(model);
        }
    }
}
