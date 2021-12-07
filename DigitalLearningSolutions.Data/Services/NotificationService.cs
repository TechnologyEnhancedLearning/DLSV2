namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Email;
    using Microsoft.FeatureManagement;
    using MimeKit;

    public interface INotificationService
    {
        void SendUnlockRequest(int progressId);
    }

    public class NotificationService : INotificationService
    {
        private readonly IConfigService configService;
        private readonly IEmailService emailService;
        private readonly IFeatureManager featureManager;
        private readonly INotificationDataService notificationDataService;

        public NotificationService(
            INotificationDataService notificationDataService,
            IConfigService configService,
            IEmailService emailService,
            IFeatureManager featureManager
        )
        {
            this.notificationDataService = notificationDataService;
            this.configService = configService;
            this.emailService = emailService;
            this.featureManager = featureManager;
        }

        public void SendUnlockRequest(int progressId)
        {
            var unlockData = notificationDataService.GetUnlockData(progressId);
            if (unlockData == null)
            {
                throw new NotificationDataException(
                    $"No record found when trying to fetch Unlock Data. Progress ID: {progressId}"
                );
            }

            unlockData.ContactForename = unlockData.ContactForename == "" ? "Colleague" : unlockData.ContactForename;
            var trackingSystemBaseUrl = configService.GetConfigValue(ConfigService.TrackingSystemBaseUrl) ??
                                        throw new ConfigValueMissingException(
                                            configService.GetConfigValueMissingExceptionMessage("TrackingSystemBaseUrl")
                                        );

            var v2AppBaseUrl = configService.GetConfigValue(ConfigService.AppBaseUrl) ??
                               throw new ConfigValueMissingException(
                                   configService.GetConfigValueMissingExceptionMessage("AppBaseUrl")
                               );

            var trackingSystemSupportEnabled = featureManager.IsEnabledAsync("RefactoredTrackingSystem").Result;

            var unlockUrl = trackingSystemSupportEnabled
                ? new UriBuilder(trackingSystemBaseUrl): new UriBuilder(v2AppBaseUrl);

            unlockUrl.Path += "coursedelegates";
            unlockUrl.Query = $"CustomisationID={unlockData.CustomisationId}";
            string emailSubjectLine = "Digital Learning Solutions Progress Unlock Request";
            var builder = new BodyBuilder
            {
                TextBody = $@"Dear {unlockData?.ContactForename}
Digital Learning Solutions Delegate, {unlockData?.DelegateName}, has requested that you unlock their progress for the course {unlockData?.CourseName}.
They have reached the maximum number of assessment attempt allowed without passing.
To review and unlock their progress, visit the this url: ${unlockUrl.Uri}.",
                HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'>
                                    <p>Dear {unlockData?.ContactForename}</p>
                                    <p>Digital Learning Solutions Delegate, {unlockData?.DelegateName}, has requested that you unlock their progress for the course {unlockData?.CourseName}</p>
                                    <p>They have reached the maximum number of assessment attempt allowed without passing.</p><p>To review and unlock their progress, <a href='{unlockUrl.Uri}'>click here</a>.</p>
                                </body>",
            };

            emailService.SendEmail(
                new Email(emailSubjectLine, builder, unlockData.ContactEmail, unlockData.DelegateEmail)
            );
        }
    }
}
