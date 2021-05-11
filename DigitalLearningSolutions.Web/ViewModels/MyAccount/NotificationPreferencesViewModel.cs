namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Helpers;

    public class NotificationPreferencesViewModel
    {
        public NotificationPreferencesViewModel(
            IEnumerable<NotificationPreference> adminNotifications,
            IEnumerable<NotificationPreference> delegateNotifications)
        {
            AdminNotifications = new NotificationPreferenceExpanderViewModel(adminNotifications, UserType.AdminUser);
            DelegateNotifications = new NotificationPreferenceExpanderViewModel(delegateNotifications, UserType.DelegateUser);
        }

        public NotificationPreferenceExpanderViewModel AdminNotifications { get; set; }

        public NotificationPreferenceExpanderViewModel DelegateNotifications { get; set; }
    }

    public class NotificationPreferenceExpanderViewModel
    {
        public NotificationPreferenceExpanderViewModel(IEnumerable<NotificationPreference> notifications, UserType userType)
        {
            Notifications = notifications;
            UserType = userType;
        }

        public string UserType { get; set; }

        public IEnumerable<NotificationPreference> Notifications { get; set; }
    }
}
