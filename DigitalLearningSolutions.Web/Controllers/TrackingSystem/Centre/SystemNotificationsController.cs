namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.SystemNotifications;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Centre))]
    [Route("/TrackingSystem/Centre/SystemNotifications")]
    public class SystemNotificationsController : Controller
    {
        private readonly IClockService clockService;
        private readonly ISystemNotificationsDataService systemNotificationsDataService;

        public SystemNotificationsController(
            ISystemNotificationsDataService systemNotificationsDataService,
            IClockService clockService
        )
        {
            this.systemNotificationsDataService = systemNotificationsDataService;
            this.clockService = clockService;
        }

        [HttpGet]
        [Route("{page:int=1}")]
        public IActionResult Index(int page = 1)
        {
            var adminId = User.GetAdminId()!.Value;
            var unacknowledgedNotifications =
                systemNotificationsDataService.GetUnacknowledgedSystemNotifications(adminId).ToList();

            if (unacknowledgedNotifications.Count > 0)
            {
                Response.Cookies.SetSkipSystemNotificationCookie(adminId, clockService.UtcNow);
            }
            else if (Request.Cookies.HasSkippedNotificationsCookie(adminId))
            {
                Response.Cookies.DeleteSkipSystemNotificationCookie();
            }

            var model = new SystemNotificationsViewModel(unacknowledgedNotifications, page);
            return View(model);
        }

        [HttpPost]
        [Route("{page:int}")]
        public IActionResult AcknowledgeNotification(int systemNotificationId, int page)
        {
            var adminId = User.GetAdminId()!.Value;
            systemNotificationsDataService.AcknowledgeNotification(systemNotificationId, adminId);

            return RedirectToAction("Index", "SystemNotifications", new { page });
        }
    }
}
