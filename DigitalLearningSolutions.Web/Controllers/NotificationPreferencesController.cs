namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
    using System.Collections.Generic;
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

        [HttpGet]
        [Route("/NotificationPreferences/Edit/{userType}")]
        public IActionResult UpdateNotificationPreferences(string userType)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login");
            }

            IEnumerable<NotificationPreference> notifications;

            if (userType == UserTypes.Admin)
            {
                var adminId = User.GetCustomClaimAsInt(CustomClaimTypes.UserAdminId);
                notifications = notificationPreferencesDataService.GetNotificationPreferencesForAdmin(adminId);
            }
            else if (userType == UserTypes.Delegate)
            {
                var delegateId = User.GetCustomClaimAsInt(CustomClaimTypes.LearnCandidateId);
                notifications = notificationPreferencesDataService.GetNotificationPreferencesForDelegate(delegateId);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new UpdateNotificationPreferencesViewModel(notifications, userType);

            return View(model);
        }

        [HttpPost]
        [Route("/NotificationPreferences/Edit/{userType}")]
        public IActionResult SaveNotificationPreferences(IEnumerable<int> notifications)
        {
            throw new NotImplementedException();
        }
    }
}
