namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.Models.Enums;

    public class UpdateNotificationPreferencesViewModel
    {
        public UpdateNotificationPreferencesViewModel(
            IEnumerable<NotificationPreference> notifications,
            string userType,
            DlsSubApplication dlsSubApplication
        )
        {
            Notifications = notifications;
            UserType = userType;
            DlsSubApplication = dlsSubApplication;
        }

        public string UserType { get; set; }
        public IEnumerable<NotificationPreference> Notifications { get; set; }
        public DlsSubApplication DlsSubApplication { get; set; }
    }
}
