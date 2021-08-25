﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Dashboard
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Dashboard;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("/TrackingSystem/Centre/Dashboard")]
    public class DashboardController : Controller
    {
        private readonly ICentresDataService centresDataService;
        private readonly ICentresService centresService;
        private readonly ICourseDataService courseDataService;
        private readonly ISystemNotificationsDataService systemNotificationsDataService;
        private readonly ISupportTicketDataService ticketDataService;
        private readonly IUserDataService userDataService;

        public DashboardController(
            IUserDataService userDataService,
            ICentresDataService centresDataService,
            ICourseDataService courseDataService,
            ISupportTicketDataService ticketDataService,
            ICentresService centresService,
            ISystemNotificationsDataService systemNotificationsDataService
        )
        {
            this.userDataService = userDataService;
            this.centresDataService = centresDataService;
            this.courseDataService = courseDataService;
            this.ticketDataService = ticketDataService;
            this.centresService = centresService;
            this.systemNotificationsDataService = systemNotificationsDataService;
        }

        public IActionResult Index()
        {
            var adminId = User.GetAdminId()!.Value;
            var unacknowledgedNotifications =
                systemNotificationsDataService.GetUnacknowledgedSystemNotifications(adminId).ToList();

            if (!Request.Cookies.HasSkippedNotificationsCookie(adminId) && unacknowledgedNotifications.Any())
            {
                return RedirectToAction("Index", "SystemNotifications");
            }

            var adminUser = userDataService.GetAdminUserById(adminId)!;
            var centreId = User.GetCentreId();
            var centre = centresDataService.GetCentreDetailsById(centreId)!;
            var delegateCount = userDataService.GetNumberOfApprovedDelegatesAtCentre(centreId);
            var courseCount =
                courseDataService.GetNumberOfActiveCoursesAtCentreForCategory(centreId, adminUser.CategoryId);
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
                centreRank,
                unacknowledgedNotifications.Count
            );

            return View(model);
        }
    }
}
