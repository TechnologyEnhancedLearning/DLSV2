namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
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
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly ISystemNotificationsDataService systemNotificationsDataService;

        public SystemNotificationsController(
            ISystemNotificationsDataService systemNotificationsDataService,
            IClockService clockService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService
        )
        {
            this.systemNotificationsDataService = systemNotificationsDataService;
            this.clockService = clockService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
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

            const int numberOfNotificationsPerPage = 1;
            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                null,
                null,
                null,
                new PaginationOptions(page, numberOfNotificationsPerPage)
            );
            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                unacknowledgedNotifications,
                searchSortPaginationOptions
            );

            var model = new SystemNotificationsViewModel(result);
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
