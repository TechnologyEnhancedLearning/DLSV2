namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models;

    public class NotificationPreferencesViewModel
    {
        public NotificationPreferencesViewModel(
            IEnumerable<NotificationPreference> adminNotifications,
            IEnumerable<NotificationPreference> delegateNotifications)
        {
            AdminNotifications = new NotificationPreferenceListViewModel(adminNotifications);
            DelegateNotifications = new NotificationPreferenceListViewModel(delegateNotifications);
        }

        public NotificationPreferenceListViewModel AdminNotifications { get; set; }

        public NotificationPreferenceListViewModel DelegateNotifications { get; set; }
    }

    public class NotificationPreferenceListViewModel
    {
        public NotificationPreferenceListViewModel(IEnumerable<NotificationPreference> notifications)
        {
            Notifications = notifications;
        }

        public IEnumerable<NotificationPreference> Notifications { get; set; }
    }
}
