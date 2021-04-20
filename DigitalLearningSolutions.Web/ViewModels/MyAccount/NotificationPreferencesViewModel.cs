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
            AdminNotifications = adminNotifications;
            DelegateNotifications = delegateNotifications;
        }

        public IEnumerable<NotificationPreference> AdminNotifications { get; set; }

        public IEnumerable<NotificationPreference> DelegateNotifications { get; set; }
    }
}
