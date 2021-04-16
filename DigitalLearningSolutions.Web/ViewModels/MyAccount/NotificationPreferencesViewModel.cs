namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models;

    public class NotificationPreferencesViewModel
    {
        public NotificationPreferencesViewModel(
            int? adminId,
            int? delegateId,
            IEnumerable<NotificationPreference> adminNotifications,
            IEnumerable<NotificationPreference> delegateNotifications)
        {
            AdminId = adminId;
            DelegateId = delegateId;
            AdminNotifications = adminNotifications;
            DelegateNotifications = delegateNotifications;
        }

        public int? AdminId { get; set; }

        public int? DelegateId { get; set; }

        public IEnumerable<NotificationPreference> AdminNotifications { get; set; }

        public IEnumerable<NotificationPreference> DelegateNotifications { get; set; }
    }
}
