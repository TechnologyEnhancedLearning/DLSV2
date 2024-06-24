using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Models;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.Services
{
    public interface ISystemNotificationsService
    {
        public IEnumerable<SystemNotification> GetUnacknowledgedSystemNotifications(int adminId);

        public void AcknowledgeNotification(int notificationId, int adminId);
    }
    public class SystemNotificationsService : ISystemNotificationsService
    {
        private readonly ISystemNotificationsDataService systemNotificationsDataService;
        public SystemNotificationsService(ISystemNotificationsDataService systemNotificationsDataService)
        {
            this.systemNotificationsDataService = systemNotificationsDataService;
        }
        public void AcknowledgeNotification(int notificationId, int adminId)
        {
            systemNotificationsDataService.AcknowledgeNotification(notificationId, adminId);
        }

        public IEnumerable<SystemNotification> GetUnacknowledgedSystemNotifications(int adminId)
        {
            return systemNotificationsDataService.GetUnacknowledgedSystemNotifications(adminId);
        }
    }
}
