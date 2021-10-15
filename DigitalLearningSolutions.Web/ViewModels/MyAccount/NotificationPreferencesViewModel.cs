namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Models.Enums;

    public class NotificationPreferencesViewModel
    {
        public NotificationPreferencesViewModel(
            IEnumerable<NotificationPreference> adminNotifications,
            IEnumerable<NotificationPreference> delegateNotifications,
            DlsSubApplication dlsSubApplication
        )
        {
            AdminNotifications = new NotificationPreferenceListViewModel(
                adminNotifications,
                UserType.AdminUser,
                delegateNotifications.Any(),
                dlsSubApplication
            );
            DelegateNotifications = new NotificationPreferenceListViewModel(
                delegateNotifications,
                UserType.DelegateUser,
                adminNotifications.Any(),
                dlsSubApplication
            );

            DlsSubApplication = dlsSubApplication;
        }

        public NotificationPreferenceListViewModel AdminNotifications { get; set; }

        public NotificationPreferenceListViewModel DelegateNotifications { get; set; }

        public DlsSubApplication DlsSubApplication { get; set; }
    }

    public class NotificationPreferenceListViewModel
    {
        public NotificationPreferenceListViewModel(
            IEnumerable<NotificationPreference> notifications,
            UserType userType,
            bool showAsExpandable,
            DlsSubApplication dlsSubApplication
        )
        {
            Notifications = notifications;
            UserType = userType;
            ShowAsExpandable = showAsExpandable;
            DlsSubApplication = dlsSubApplication;
        }

        public UserType UserType { get; set; }

        public IEnumerable<NotificationPreference> Notifications { get; set; }

        public bool ShowAsExpandable { get; set; }

        public DlsSubApplication DlsSubApplication { get; set; }
    }
}
