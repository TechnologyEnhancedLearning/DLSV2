namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.SystemNotifications
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SystemNotificationsViewModel : BasePaginatedViewModel
    {
        private const int NumberOfNotificationsPerPage = 1;

        public SystemNotificationsViewModel(
            IList<SystemNotification> unacknowledgedNotifications,
            int page
        ) : base(
            page,
            NumberOfNotificationsPerPage
        )
        {
            MatchingSearchResults = unacknowledgedNotifications.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(unacknowledgedNotifications);
            UnacknowledgedNotification =
                paginatedItems.Select(notification => new UnacknowledgedNotificationViewModel(notification))
                    .FirstOrDefault();
        }

        public UnacknowledgedNotificationViewModel? UnacknowledgedNotification { get; set; }
    }
}
