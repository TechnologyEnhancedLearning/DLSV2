namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Dashboard
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Dashboard;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Centre))]
    [Route("/TrackingSystem/Centre/Dashboard")]
    public class DashboardController : Controller
    {
        private readonly IDashboardInformationService dashboardInformationService;
        private readonly ISystemNotificationsDataService systemNotificationsDataService;

        public DashboardController(
            IDashboardInformationService dashboardInformationService,
            ISystemNotificationsDataService systemNotificationsDataService
        )
        {
            this.dashboardInformationService = dashboardInformationService;
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

            var centreId = User.GetCentreId();

            var dashboardInformation = dashboardInformationService.GetDashboardInformationForCentre(centreId, adminId);

            if (dashboardInformation == null)
            {
                return NotFound();
            }

            var model = new CentreDashboardViewModel(
                dashboardInformation,
                Request.GetUserIpAddressFromRequest(),
                unacknowledgedNotifications.Count
            );

            return View(model);
        }
    }
}
