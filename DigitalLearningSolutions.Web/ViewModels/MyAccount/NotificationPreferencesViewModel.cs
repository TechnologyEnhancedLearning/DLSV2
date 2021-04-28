namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Helpers;

    public class NotificationPreferencesViewModel
    {
        public NotificationPreferencesViewModel(
            IEnumerable<NotificationPreference> adminNotifications,
            IEnumerable<NotificationPreference> delegateNotifications)
        {
            AdminNotifications = new NotificationPreferenceListViewModel(adminNotifications, UserTypes.Admin);
            DelegateNotifications = new NotificationPreferenceListViewModel(delegateNotifications, UserTypes.Delegate);
        }

        public NotificationPreferenceListViewModel AdminNotifications { get; set; }

        public NotificationPreferenceListViewModel DelegateNotifications { get; set; }
    }

    public class NotificationPreferenceListViewModel
    {
        public NotificationPreferenceListViewModel(IEnumerable<NotificationPreference> notifications, string userType)
        {
            Notifications = notifications;
            UserType = userType;
        }

        public string UserType { get; set; }

        public IEnumerable<NotificationPreference> Notifications { get; set; }
    }
}
