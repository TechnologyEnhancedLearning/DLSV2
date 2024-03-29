namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [TypeFilter(typeof(ValidateAllowedDlsSubApplication))]
    [SetDlsSubApplication]
    [SetSelectedTab(nameof(NavMenuTab.MyAccount))]
    [Authorize(Policy = CustomPolicies.CentreUser)]
    public class NotificationPreferencesController : Controller
    {
        private readonly INotificationPreferencesService notificationPreferencesService;

        public NotificationPreferencesController(
            INotificationPreferencesService notificationPreferencesService
        )
        {
            this.notificationPreferencesService = notificationPreferencesService;
        }

        [Route("/{dlsSubApplication}/NotificationPreferences")]
        [Route("/NotificationPreferences", Order = 1)]
        public IActionResult Index(DlsSubApplication dlsSubApplication)
        {
            var adminId = User.GetCustomClaimAsInt(CustomClaimTypes.UserAdminId);
            var adminNotifications =
                notificationPreferencesService.GetNotificationPreferencesForUser(UserType.AdminUser, adminId);

            var delegateId = User.GetCandidateId();
            var delegateNotifications =
                notificationPreferencesService.GetNotificationPreferencesForUser(UserType.DelegateUser, delegateId);

            var model = new NotificationPreferencesViewModel(
                adminNotifications,
                delegateNotifications,
                dlsSubApplication
            );

            return View(model);
        }

        [HttpGet]
        [Route("/{dlsSubApplication}/NotificationPreferences/Edit/{userType}")]
        [Route("/NotificationPreferences/Edit/{userType}", Order = 1)]
        public IActionResult UpdateNotificationPreferences(UserType? userType, DlsSubApplication dlsSubApplication)
        {
            var userReference = GetUserReference(userType);
            if (userReference == null)
            {
                return RedirectToAction("AccessDenied", "LearningSolutions");
            }

            var notifications =
                notificationPreferencesService.GetNotificationPreferencesForUser(
                    userReference.UserType,
                    userReference.Id
                );

            var model = new UpdateNotificationPreferencesViewModel(notifications, userType!, dlsSubApplication);

            return View(model);
        }

        [HttpPost]
        [Route("/{dlsSubApplication}/NotificationPreferences/Edit/{userType}")]
        [Route("/NotificationPreferences/Edit/{userType}", Order = 1)]
        public IActionResult SaveNotificationPreferences(
            UserType? userType,
            IEnumerable<int> notificationIds,
            DlsSubApplication dlsSubApplication
        )
        {
            var userReference = GetUserReference(userType);
            if (userReference == null)
            {
                return RedirectToAction("AccessDenied", "LearningSolutions");
            }

            notificationPreferencesService.SetNotificationPreferencesForUser(
                userReference.UserType,
                userReference.Id,
                notificationIds
            );

            return RedirectToAction(
                "Index",
                "NotificationPreferences",
                new { dlsSubApplication = dlsSubApplication.UrlSegment }
            );
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
