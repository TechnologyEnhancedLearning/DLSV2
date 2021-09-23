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
            ApplicationType application
        )
        {
            AdminNotifications = new NotificationPreferenceListViewModel(
                adminNotifications,
                UserType.AdminUser,
                delegateNotifications.Any(),
                application
            );
            DelegateNotifications = new NotificationPreferenceListViewModel(
                delegateNotifications,
                UserType.DelegateUser,
                adminNotifications.Any(),
                application
            );

            Application = application;
        }

        public NotificationPreferenceListViewModel AdminNotifications { get; set; }

        public NotificationPreferenceListViewModel DelegateNotifications { get; set; }

        public ApplicationType Application { get; set; }
    }

    public class NotificationPreferenceListViewModel
    {
        public NotificationPreferenceListViewModel(
            IEnumerable<NotificationPreference> notifications,
            UserType userType,
            bool showAsExpandable,
            ApplicationType application
        )
        {
            Notifications = notifications;
            UserType = userType;
            ShowAsExpandable = showAsExpandable;
            Application = application;
        }

        public UserType UserType { get; set; }

        public IEnumerable<NotificationPreference> Notifications { get; set; }

        public bool ShowAsExpandable { get; set; }

        public ApplicationType Application { get; set; }
    }
}
