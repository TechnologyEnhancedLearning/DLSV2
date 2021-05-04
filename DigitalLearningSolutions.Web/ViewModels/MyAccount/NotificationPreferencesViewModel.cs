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
            AdminNotifications = new NotificationPreferenceExpanderViewModel(adminNotifications, UserTypes.Admin);
            DelegateNotifications = new NotificationPreferenceExpanderViewModel(delegateNotifications, UserTypes.Delegate);
        }

        public NotificationPreferenceExpanderViewModel AdminNotifications { get; set; }

        public NotificationPreferenceExpanderViewModel DelegateNotifications { get; set; }
    }

    public class NotificationPreferenceExpanderViewModel
    {
        public NotificationPreferenceExpanderViewModel(IEnumerable<NotificationPreference> notifications, string userType)
        {
            Notifications = notifications;
            UserType = userType;
        }

        public string UserType { get; set; }

        public IEnumerable<NotificationPreference> Notifications { get; set; }
    }
}
