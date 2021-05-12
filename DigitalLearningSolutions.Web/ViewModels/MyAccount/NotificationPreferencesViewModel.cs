namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Helpers;

    public class NotificationPreferencesViewModel
    {
        public NotificationPreferencesViewModel(
            IEnumerable<NotificationPreference> adminNotifications,
            IEnumerable<NotificationPreference> delegateNotifications)
        {
            AdminNotifications = new NotificationPreferenceListViewModel(
                adminNotifications,
                UserTypes.Admin,
                delegateNotifications.Any());
            DelegateNotifications = new NotificationPreferenceListViewModel(
                delegateNotifications,
                UserTypes.Delegate,
                adminNotifications.Any());
        }

        public NotificationPreferenceListViewModel AdminNotifications { get; set; }

        public NotificationPreferenceListViewModel DelegateNotifications { get; set; }
    }

    public class NotificationPreferenceListViewModel
    {
        public NotificationPreferenceListViewModel(IEnumerable<NotificationPreference> notifications, string userType, bool showAsExpandable)
        {
            Notifications = notifications;
            UserType = userType;
            ShowAsExpandable = showAsExpandable;
        }

        public string UserType { get; set; }

        public IEnumerable<NotificationPreference> Notifications { get; set; }

        public bool ShowAsExpandable { get; set; }
    }
}
