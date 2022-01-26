namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using NUnit.Framework;

    public class NotificationPreferenceServiceTests
    {
        private NotificationPreferencesService notificationPreferencesService;
        private INotificationPreferencesDataService notificationPreferencesDataService;

        [SetUp]
        public void OneTimeSetUp()
        {
            notificationPreferencesDataService = A.Fake<INotificationPreferencesDataService>();
            notificationPreferencesService =
                new NotificationPreferencesService(notificationPreferencesDataService);
        }

        [Test]
        public void Gets_preferences_for_admin_when_user_type_is_admin()
        {
            // Given
            var userType = UserType.AdminUser;

            // When
            notificationPreferencesService.GetNotificationPreferencesForUser(userType, 1);

            // Then
            A.CallTo(() => notificationPreferencesDataService.GetNotificationPreferencesForAdmin(1))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void Gets_preferences_for_delegate_when_user_type_is_delegate()
        {
            // Given
            var userType = UserType.DelegateUser;

            // When
            notificationPreferencesService.GetNotificationPreferencesForUser(userType, 1);

            // Then
            A.CallTo(() => notificationPreferencesDataService.GetNotificationPreferencesForDelegate(1))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void Sets_preferences_for_admin_when_user_type_is_admin()
        {
            // Given
            var userType = UserType.AdminUser;

            // When
            notificationPreferencesService.SetNotificationPreferencesForUser(userType, 1, new int[] { });

            // Then
            A.CallTo(
                    () => notificationPreferencesDataService.SetNotificationPreferencesForAdmin(
                        1,
                        A<IEnumerable<int>>.That.IsEmpty()))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Test]
        public void Sets_preferences_for_delegate_when_user_type_is_delegate()
        {
            // Given
            var userType = UserType.DelegateUser;

            // When
            notificationPreferencesService.SetNotificationPreferencesForUser(userType, 1, new int[] { });

            // Then
            A.CallTo(
                    () => notificationPreferencesDataService.SetNotificationPreferencesForDelegate(
                        1,
                        A<IEnumerable<int>>.That.IsEmpty()))
                .MustHaveHappened(1, Times.Exactly);
        }
    }
}
