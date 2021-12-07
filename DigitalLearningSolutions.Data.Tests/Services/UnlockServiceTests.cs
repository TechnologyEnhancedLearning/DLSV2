namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using Microsoft.FeatureManagement;
    using NUnit.Framework;

    public class UnlockServiceTests
    {
        private IConfigService configService;
        private IEmailService emailService;
        private INotificationDataService notificationDataService;
        private NotificationService notificationService;
        private IFeatureManager featureManager;

        [SetUp]
        public void Setup()
        {
            notificationDataService = A.Fake<INotificationDataService>();
            configService = A.Fake<IConfigService>();
            emailService = A.Fake<IEmailService>();
            featureManager = A.Fake<IFeatureManager>();

            A.CallTo(() => notificationDataService.GetUnlockData(A<int>._)).Returns(new UnlockData
            {
                ContactEmail = "recipient@example.com",
                ContactForename = "Forename",
                CourseName = "Activity Name",
                CustomisationId = 22,
                DelegateEmail = "cc@example.com",
                DelegateName = "Delegate Name"
            });

            A.CallTo(() => configService.GetConfigValue(ConfigService.TrackingSystemBaseUrl)).Returns("https://example.com");

            notificationService = new NotificationService(notificationDataService, configService, emailService, featureManager);
        }

        [Test]
        public void Trying_to_send_unlock_request_with_null_unlock_data_should_throw_an_exception()
        {
            // Given
            A.CallTo(() => notificationDataService.GetUnlockData(A<int>._)).Returns(null);

            // Then
            Assert.Throws<NotificationDataException>(() => notificationService.SendUnlockRequest(1));
        }

        [Test]
        public void Throws_an_exception_when_tracking_system_base_url_is_null()
        {
            // Given
            A.CallTo(() => configService.GetConfigValue(ConfigService.TrackingSystemBaseUrl)).Returns(null);

            // Then
            Assert.Throws<ConfigValueMissingException>(() => notificationService.SendUnlockRequest(1));
        }

        [Test]
        public void Trying_to_send_unlock_request_sends_email()
        {
            // When
            notificationService.SendUnlockRequest(1);

            // Then
            A.CallTo(() =>
                    emailService.SendEmail(A<Email>._)
                )
                .MustHaveHappened();
        }
    }
}
