namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Configuration;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using FakeItEasy;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement;
    using NUnit.Framework;
    using ConfigHelper = DigitalLearningSolutions.Data.Helpers.ConfigHelper;

    public class UnlockServiceTests
    {
        private IConfiguration configuration = null!;
        private IConfigService configService = null!;
        private IEmailService emailService = null!;
        private INotificationDataService notificationDataService = null!;
        private NotificationService notificationService = null!;
        private IFeatureManager featureManager = null!;

        [SetUp]
        public void Setup()
        {
            configuration = A.Fake<IConfiguration>();
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

            notificationService = new NotificationService(configuration, notificationDataService, configService, emailService, featureManager);

            A.CallTo(() => configuration[ConfigHelper.AppRootPathName]).Returns("https://new-tracking-system.com/");
            A.CallTo(() => configuration[ConfigHelper.CurrentSystemBaseUrlName]).Returns("https://old-tracking-system.com/");
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

        [Test]
        public void Trying_to_send_unlock_makes_request_to_feature_manager_to_get_correct_url()
        {
            //Given
            A.CallTo(() => featureManager.IsEnabledAsync("RefactoredTrackingSystem"))
                .Returns(false);

            //When
            notificationService.SendUnlockRequest(1);

            //Then
            A.CallTo(() => featureManager.IsEnabledAsync(A<string>._)).MustHaveHappened();
        }

        [Test]
        public void Trying_to_send_unlock_request_send_email_with_correct_old_url()
        {
            //Given
            A.CallTo(() => featureManager.IsEnabledAsync("RefactoredTrackingSystem"))
                .Returns(false);
            A.CallTo(() => configService.GetConfigValue(A<string>._)).Returns("https://old-tracking-system.com/");
            //When
            notificationService.SendUnlockRequest(1);

            //Then
            A.CallTo(() =>
                    emailService.SendEmail(A<Email>.That.Matches(e => e.Body.TextBody.Contains("https://old-tracking-system.com/")))
                )
                .MustHaveHappened();
        }

        [Test]
        public void trying_to_send_unlock_request_send_email_with_correct_new_url()
        {
            //Given
            A.CallTo(() => featureManager.IsEnabledAsync("RefactoredTrackingSystem"))
                .Returns(true);
            A.CallTo(() => configService.GetConfigValue(A<string>._)).Returns("https://new-tracking-system.com/");
            //When
            notificationService.SendUnlockRequest(1);

            //Then
            A.CallTo(() =>
                    emailService.SendEmail(A<Email>.That.Matches(e => e.Body.TextBody.Contains("https://new-tracking-system.com/")))
                )
                .MustHaveHappened();
        }
    }
}
