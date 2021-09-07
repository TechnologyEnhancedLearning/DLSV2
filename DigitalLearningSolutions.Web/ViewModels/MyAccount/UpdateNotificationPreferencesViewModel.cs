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
            ApplicationType? application
        )
        {
            Notifications = notifications;
            UserType = userType;
            Application = application;
        }

        public string UserType { get; set; }
        public IEnumerable<NotificationPreference> Notifications { get; set; }
        public ApplicationType? Application { get; set; }
    }
}
