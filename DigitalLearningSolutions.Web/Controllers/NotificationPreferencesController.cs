namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using Microsoft.AspNetCore.Authorization;
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
            var adminNotifications = GetNotificationPreferencesForUser(UserTypes.Admin, adminId);

            var delegateId = User.GetCustomClaimAsInt(CustomClaimTypes.LearnCandidateId);
            var delegateNotifications = GetNotificationPreferencesForUser(UserTypes.Delegate, delegateId);

            var model = new NotificationPreferencesViewModel(adminNotifications, delegateNotifications);

            return View(model);
        }

        [Authorize]
        [HttpGet]
        [Route("/NotificationPreferences/Edit/{userType}")]
        public IActionResult UpdateNotificationPreferences(string userType)
        {
            var claimType = userType == UserTypes.Admin ? CustomClaimTypes.UserAdminId : CustomClaimTypes.LearnCandidateId;

            var userId = User.GetCustomClaimAsInt(claimType);
            var notifications = GetNotificationPreferencesForUser(userType, userId);

            var model = new UpdateNotificationPreferencesViewModel(notifications, userType);

            return View(model);
        }

        // TODO AIR-349 this should be moved into the repository once the user type enum is merged
        private IEnumerable<NotificationPreference> GetNotificationPreferencesForUser(string userType, int? userId)
        {
            if (userType == UserTypes.Admin)
            {
                return notificationPreferencesDataService.GetNotificationPreferencesForAdmin(userId);
            }
            if (userType == UserTypes.Delegate)
            {
                return notificationPreferencesDataService.GetNotificationPreferencesForDelegate(userId);
            }
            throw new Exception(); // switching to enum will allow a better specific handling of this
        }

        [HttpPost]
        [Route("/NotificationPreferences/Edit/{userType}")]
        public IActionResult SaveNotificationPreferences(IEnumerable<int> notifications)
        {
            throw new NotImplementedException();
        }
    }
}
