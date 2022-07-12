namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;

    public interface INotificationPreferencesService
    {
        IEnumerable<NotificationPreference> GetNotificationPreferencesForUser(UserType userType, int? userId);
        void SetNotificationPreferencesForUser(UserType userType, int? userId, IEnumerable<int> notificationIds);
    }

    public class NotificationPreferencesService : INotificationPreferencesService
    {
        private readonly INotificationPreferencesDataService notificationPreferencesDataService;

        public NotificationPreferencesService(INotificationPreferencesDataService notificationPreferencesDataService)
        {
            this.notificationPreferencesDataService = notificationPreferencesDataService;
        }

        public IEnumerable<NotificationPreference> GetNotificationPreferencesForUser(UserType userType, int? userId)
        {
            if (userType.Equals(UserType.AdminUser))
            {
                return notificationPreferencesDataService.GetNotificationPreferencesForAdmin(userId);
            }

            if (userType.Equals(UserType.DelegateUser))
            {
                return notificationPreferencesDataService.GetNotificationPreferencesForDelegate(userId);
            }

            throw new Exception($"No code path for getting notification preferences for user type {userType}");
        }

        public void SetNotificationPreferencesForUser(UserType userType, int? userId, IEnumerable<int> notificationIds)
        {
            if (userType.Equals(UserType.AdminUser))
            {
                notificationPreferencesDataService.SetNotificationPreferencesForAdmin(userId, notificationIds);
            }
            else if (userType.Equals(UserType.DelegateUser))
            {
                notificationPreferencesDataService.SetNotificationPreferencesForDelegate(userId, notificationIds);
            }
            else
            {
                throw new Exception($"No code path for setting notification preferences for user type {userType}");
            }
        }
    }
}
