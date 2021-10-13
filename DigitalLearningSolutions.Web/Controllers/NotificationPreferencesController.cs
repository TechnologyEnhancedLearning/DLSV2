namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [ValidateAllowedApplicationType]
    [SetApplicationType(determiningRouteParameter: "application")]
    [SetSelectedTab(nameof(NavMenuTab.MyAccount))]
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

        [Route("/{application}/NotificationPreferences")]
        [Route("/NotificationPreferences", Order = 1)]
        public IActionResult Index(ApplicationType application)
        {
            var adminId = User.GetCustomClaimAsInt(CustomClaimTypes.UserAdminId);
            var adminNotifications =
                notificationPreferencesService.GetNotificationPreferencesForUser(UserType.AdminUser, adminId);

            var delegateId = User.GetCandidateId();
            var delegateNotifications =
                notificationPreferencesService.GetNotificationPreferencesForUser(UserType.DelegateUser, delegateId);

            var model = new NotificationPreferencesViewModel(adminNotifications, delegateNotifications, application);

            return View(model);
        }

        [Authorize]
        [HttpGet]
        [Route("/{application}/NotificationPreferences/Edit/{userType}")]
        [Route("/NotificationPreferences/Edit/{userType}", Order = 1)]
        public IActionResult UpdateNotificationPreferences(UserType? userType, ApplicationType application)
        {
            var userReference = GetUserReference(userType);
            if (userReference == null)
            {
                return NotFound();
            }

            var notifications =
                notificationPreferencesService.GetNotificationPreferencesForUser(
                    userReference.UserType,
                    userReference.Id
                );

            var model = new UpdateNotificationPreferencesViewModel(notifications, userType!, application);

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [Route("/{application}/NotificationPreferences/Edit/{userType}")]
        [Route("/NotificationPreferences/Edit/{userType}", Order = 1)]
        public IActionResult SaveNotificationPreferences(
            UserType? userType,
            IEnumerable<int> notificationIds,
            ApplicationType application
        )
        {
            var userReference = GetUserReference(userType);
            if (userReference == null)
            {
                return NotFound();
            }

            notificationPreferencesService.SetNotificationPreferencesForUser(
                userReference.UserType,
                userReference.Id,
                notificationIds
            );

            return RedirectToAction("Index", "NotificationPreferences", new { application = application.UrlSegment });
        }

        private UserReference? GetUserReference(UserType? userType)
        {
            int? userId = null;

            if (Equals(userType, UserType.AdminUser))
            {
                userId = User.GetAdminId();
            }
            else if (Equals(userType, UserType.DelegateUser))
            {
                userId = User.GetCandidateId();
            }

            return userId != null ? new UserReference(userId.Value, userType!) : null;
        }
    }
}
