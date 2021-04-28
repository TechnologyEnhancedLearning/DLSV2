namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models;

    public class UpdateNotificationPreferencesViewModel // TODO 
    {
        public UpdateNotificationPreferencesViewModel(
            IEnumerable<NotificationPreference> notifications, string userType)
        {
            Notifications = notifications;
            UserType = userType;
        }

        public string UserType { get; set; }
        public IEnumerable<NotificationPreference> Notifications { get; set; }
    }
}
