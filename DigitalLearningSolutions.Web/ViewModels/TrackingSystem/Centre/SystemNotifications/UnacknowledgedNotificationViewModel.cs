namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.SystemNotifications
{
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Helpers;

    public class UnacknowledgedNotificationViewModel
    {
        public UnacknowledgedNotificationViewModel(SystemNotification notification)
        {
            SystemNotificationId = notification.SystemNotificationId;
            Subject = notification.SubjectLine;
            Body = notification.BodyHtml;
            ExpiryDate = notification.ExpiryDate?.ToString(DateHelper.StandardDateFormat) ?? "This notification never expires.";
        }

        public int SystemNotificationId { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string ExpiryDate { get; set; }
    }
}
