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
        private readonly INotificationPreferencesDataService notificationPreferencesDataService;

        public NotificationPreferencesController(INotificationPreferencesDataService notificationPreferencesDataService)
        {
            this.notificationPreferencesDataService = notificationPreferencesDataService;
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login");
            }

            var adminId = User.GetCustomClaimAsInt(CustomClaimTypes.UserAdminId);
            var adminNotifications = notificationPreferencesDataService.GetNotificationPreferencesForAdmin(adminId);

            var delegateId = User.GetCustomClaimAsInt(CustomClaimTypes.LearnCandidateId);
            var delegateNotifications = notificationPreferencesDataService.GetNotificationPreferencesForDelegate(delegateId);

            var model = new NotificationPreferencesViewModel(adminNotifications, delegateNotifications);

            return View(model);
        }
    }
}
