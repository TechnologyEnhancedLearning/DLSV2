namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public class NotificationPreferencesController : Controller
    {
        private readonly ILogger<NotificationPreferencesController> logger;
        private readonly INotificationPreferencesService notificationPreferencesService;

        public NotificationPreferencesController(
            INotificationPreferencesService notificationPreferencesService,
            ILogger<NotificationPreferencesController> logger
        )
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

            var delegateId = User.GetCandidateId();
            var delegateNotifications =
                notificationPreferencesService.GetNotificationPreferencesForUser(UserType.DelegateUser, delegateId);

            var model = new NotificationPreferencesViewModel(adminNotifications, delegateNotifications);

            return View(model);
        }

        [Authorize]
        [HttpGet]
        [Route("/NotificationPreferences/Edit/{userType}")]
        public IActionResult UpdateNotificationPreferences(UserType userType)
        {
            var userId = GetUserId(userType);
            if (userId == null)
            {
                return NotFound();
            }

            var notifications = notificationPreferencesService.GetNotificationPreferencesForUser(userType, userId);

            var model = new UpdateNotificationPreferencesViewModel(notifications, userType);

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [Route("/NotificationPreferences/Edit/{userType}")]
        public IActionResult SaveNotificationPreferences(UserType userType, IEnumerable<int> notificationIds)
        {
            var userId = GetUserId(userType);
            if (userId == null)
            {
                return NotFound();
            }

            notificationPreferencesService.SetNotificationPreferencesForUser(userType, userId, notificationIds);

            return RedirectToAction("Index", "NotificationPreferences");
        }

        private int? GetUserId(UserType userType)
        {
            if (Equals(userType, UserType.AdminUser))
            {
                return User.GetAdminId();
            }
            if (Equals(userType, UserType.DelegateUser))
            {
                return User.GetCandidateId();
            }
            return null;
        }
    }
}
