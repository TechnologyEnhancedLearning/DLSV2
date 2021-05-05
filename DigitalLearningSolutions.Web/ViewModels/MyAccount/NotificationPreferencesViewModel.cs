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
            AdminNotifications = new NotificationPreferenceExpanderViewModel(
                adminNotifications,
                UserTypes.Admin,
                delegateNotifications.Any());
            DelegateNotifications = new NotificationPreferenceExpanderViewModel(
                delegateNotifications,
                UserTypes.Delegate,
                adminNotifications.Any());
        }

        public NotificationPreferenceExpanderViewModel AdminNotifications { get; set; }

        public NotificationPreferenceExpanderViewModel DelegateNotifications { get; set; }
    }

    public class NotificationPreferenceExpanderViewModel
    {
        public NotificationPreferenceExpanderViewModel(IEnumerable<NotificationPreference> notifications, string userType, bool showAsExpandable)
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
