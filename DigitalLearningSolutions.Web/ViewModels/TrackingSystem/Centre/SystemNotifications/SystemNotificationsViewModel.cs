namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.SystemNotifications
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SystemNotificationsViewModel : BasePaginatedViewModel<SystemNotification>
    {
        public SystemNotificationsViewModel(
            PaginateResult<SystemNotification> result
        ) : base(result)
        {
            UnacknowledgedNotification =
                result.ItemsToDisplay.Select(notification => new UnacknowledgedNotificationViewModel(notification))
                    .FirstOrDefault();
        }

        public UnacknowledgedNotificationViewModel? UnacknowledgedNotification { get; set; }
    }
}
