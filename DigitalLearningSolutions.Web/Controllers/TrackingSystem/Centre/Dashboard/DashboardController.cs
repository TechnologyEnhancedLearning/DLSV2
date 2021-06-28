namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Dashboard
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Dashboard;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("/TrackingSystem/Centre/Dashboard")]
    public class DashboardController : Controller
    {
        private readonly ICentresDataService centresDataService;
        private readonly ICentresService centresService;
        private readonly ICourseService courseService;
        private readonly ISupportTicketDataService ticketDataService;
        private readonly IUserDataService userDataService;

        public DashboardController(
            IUserDataService userDataService,
            ICentresDataService centresDataService,
            ICourseService courseService,
            ISupportTicketDataService ticketDataService,
            ICentresService centresService
        )
        {
            this.userDataService = userDataService;
            this.centresDataService = centresDataService;
            this.courseService = courseService;
            this.ticketDataService = ticketDataService;
            this.centresService = centresService;
        }

        public IActionResult Index()
        {
            var adminUser = userDataService.GetAdminUserById(User.GetAdminId()!.Value)!;
            var centreId = User.GetCentreId();
            var centre = centresDataService.GetCentreDetailsById(centreId)!;
            var delegateCount = userDataService.GetNumberOfApprovedDelegatesAtCentre(centreId);
            var courseCount = courseService.GetNumberOfActiveCoursesAtCentreForCategory(centreId, adminUser.CategoryId);
            var adminCount = userDataService.GetNumberOfActiveAdminsAtCentre(centreId);
            var supportTicketCount = ticketDataService.GetNumberOfUnarchivedTicketsForCentreId(centreId);
            var centreRank = centresService.GetCentreRankForCentre(centreId);

            var model = new CentreDashboardViewModel(
                centre,
                adminUser.FirstName,
                adminUser.CategoryName,
                Request.GetUserIpAddressFromRequest(),
                delegateCount,
                courseCount,
                adminCount,
                supportTicketCount,
                centreRank
            );

            return View(model);
        }
    }
}
