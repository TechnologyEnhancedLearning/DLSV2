namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using Microsoft.AspNetCore.Mvc;

    public class NotificationPreferencesController : Controller
    {
        private readonly INotificationPreferenceService notificationPreferenceService;

        public NotificationPreferencesController(INotificationPreferenceService notificationPreferenceService)
        {
            this.notificationPreferenceService = notificationPreferenceService;
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login");
            }

            var adminId = User.GetCustomClaimAsInt(CustomClaimTypes.UserAdminId);
            var adminNotifications = new List<NotificationPreference>();
            if (adminId.HasValue)
            {
                adminNotifications = notificationPreferenceService.GetNotificationPreferencesForAdmin(adminId.Value).ToList();
            }

            var delegateId = User.GetCustomClaimAsInt(CustomClaimTypes.LearnCandidateId);
            var delegateNotifications = new List<NotificationPreference>();
            if (delegateId.HasValue)
            {
                delegateNotifications = notificationPreferenceService.GetNotificationPreferencesForDelegate(delegateId.Value).ToList();
            }

            var model = new NotificationPreferencesViewModel(adminId, delegateId, adminNotifications, delegateNotifications);

            return View(model);
        }
    }
}
