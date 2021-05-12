namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;

    public class NotificationPreferencesViewModel
    {
        public NotificationPreferencesViewModel(
            IEnumerable<NotificationPreference> adminNotifications,
            IEnumerable<NotificationPreference> delegateNotifications)
        {
            AdminNotifications = new NotificationPreferenceListViewModel(
                adminNotifications,
                UserType.AdminUser,
                delegateNotifications.Any());
            DelegateNotifications = new NotificationPreferenceListViewModel(
                delegateNotifications,
                UserType.DelegateUser,
                adminNotifications.Any());
        }

        public NotificationPreferenceListViewModel AdminNotifications { get; set; }

        public NotificationPreferenceListViewModel DelegateNotifications { get; set; }
    }

    public class NotificationPreferenceListViewModel
    {
        public NotificationPreferenceListViewModel(
            IEnumerable<NotificationPreference> notifications,
            UserType userType,
            bool showAsExpandable)
        {
            Notifications = notifications;
            UserType = userType;
            ShowAsExpandable = showAsExpandable;
        }

        public UserType UserType { get; set; }

        public IEnumerable<NotificationPreference> Notifications { get; set; }

        public bool ShowAsExpandable { get; set; }
    }
}
