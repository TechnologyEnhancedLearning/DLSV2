namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.SystemNotifications
{
    using DigitalLearningSolutions.Data.Models;

    public class UnacknowledgedNotificationViewModel
    {
        public UnacknowledgedNotificationViewModel(SystemNotification notification)
        {
            SystemNotificationId = notification.SystemNotificationId;
            Subject = notification.SubjectLine;
            Body = notification.BodyHtml;
            ExpiryDate = notification.ExpiryDate?.ToString("dd/MM/yyyy") ?? "This notification never expires";
        }

        public int SystemNotificationId { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string ExpiryDate { get; set; }
    }
}
