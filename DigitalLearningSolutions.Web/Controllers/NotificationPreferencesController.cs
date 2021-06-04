namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public class NotificationPreferencesController : Controller
    {
        private readonly INotificationPreferencesService notificationPreferencesService;
        private readonly ILogger<NotificationPreferencesController> logger;

        public NotificationPreferencesController(
            INotificationPreferencesService notificationPreferencesService,
            ILogger<NotificationPreferencesController> logger)
        {
            this.notificationPreferencesService = notificationPreferencesService;
            this.logger = logger;
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login");
            }

            var adminId = User.GetCustomClaimAsInt(CustomClaimTypes.UserAdminId);
            var adminNotifications =
                notificationPreferencesService.GetNotificationPreferencesForUser(UserType.AdminUser, adminId);

            var delegateId = User.GetCustomClaimAsInt(CustomClaimTypes.LearnCandidateId);
            var delegateNotifications =
                 notificationPreferencesService.GetNotificationPreferencesForUser(UserType.DelegateUser, delegateId);

            var model = new NotificationPreferencesViewModel(adminNotifications, delegateNotifications);

            return View(model);
        }

        [Authorize]
        [HttpGet]
        [Route("/NotificationPreferences/Edit/{userType}")]
        public IActionResult UpdateNotificationPreferences(string userType)
        {
            var userId = ((UserType)userType).Equals(UserType.AdminUser)
                ? User.GetAdminId()
                : User.GetCandidateIdKnownNotNull();
            var notifications = notificationPreferencesService.GetNotificationPreferencesForUser(userType, userId);

            var model = new UpdateNotificationPreferencesViewModel(notifications, userType);

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [Route("/NotificationPreferences/Edit/{userType}")]
        public IActionResult SaveNotificationPreferences(string userType, IEnumerable<int> notificationIds)
        {
            var userId = ((UserType)userType).Equals(UserType.AdminUser)
                ? User.GetAdminId()
                : User.GetCandidateIdKnownNotNull();

            notificationPreferencesService.SetNotificationPreferencesForUser(userType, userId, notificationIds);

            return RedirectToAction("Index", "NotificationPreferences");
        }
    }
}
