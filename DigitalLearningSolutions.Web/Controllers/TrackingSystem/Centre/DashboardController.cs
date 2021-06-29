namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre
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
        private readonly IUserDataService userDataService;
        private readonly ICourseDataService courseDataService;
        private readonly ISupportTicketDataService ticketDataService;

        public DashboardController
        (
            IUserDataService userDataService,
            ICentresDataService centresDataService,
            ICourseDataService courseDataService,
            ISupportTicketDataService ticketDataService
        )
        {
            this.userDataService = userDataService;
            this.centresDataService = centresDataService;
            this.courseDataService = courseDataService;
            this.ticketDataService = ticketDataService;
        }

        public IActionResult Index()
        {
            var adminUser = userDataService.GetAdminUserById(User.GetAdminId()!.Value)!;
            var centreId = User.GetCentreId();
            var centre = centresDataService.GetCentreDetailsById(centreId)!;
            var delegateCount = userDataService.GetNumberOfApprovedDelegatesAtCentre(centreId);
            var courseCount = courseDataService.GetNumberOfActiveCoursesAtCentreForCategory(centreId, adminUser.CategoryId);
            var adminCount = userDataService.GetNumberOfActiveAdminsAtCentre(centreId);
            var supportTicketCount = ticketDataService.GetNumberOfUnarchivedTicketsForCentreId(centreId);
            
            var model = new CentreDashboardViewModel(
                centre,
                adminUser.FirstName,
                adminUser.CategoryName,
                Request.GetUserIpAddressFromRequest(),
                delegateCount,
                courseCount,
                adminCount,
                supportTicketCount
            );

            return View(model);
        }
    }
}
