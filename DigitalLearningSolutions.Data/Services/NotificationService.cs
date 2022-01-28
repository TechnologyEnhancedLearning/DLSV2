namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Email;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement;
    using MimeKit;

    public interface INotificationService
    {
        Task SendUnlockRequest(int progressId);
    }

    public class NotificationService : INotificationService
    {
        private readonly IConfiguration configuration;
        private readonly IEmailService emailService;
        private readonly IFeatureManager featureManager;
        private readonly INotificationDataService notificationDataService;

        public NotificationService(
            IConfiguration configuration,
            INotificationDataService notificationDataService,
            IEmailService emailService,
            IFeatureManager featureManager
        )
        {
            this.configuration = configuration;
            this.notificationDataService = notificationDataService;
            this.emailService = emailService;
            this.featureManager = featureManager;
        }

        public async Task SendUnlockRequest(int progressId)
        {
            var unlockData = notificationDataService.GetUnlockData(progressId);
            if (unlockData == null)
            {
                throw new NotificationDataException(
                    $"No record found when trying to fetch Unlock Data. Progress ID: {progressId}"
                );
            }

            unlockData.ContactForename =
                unlockData.ContactForename == "" ? "Colleague" : unlockData.ContactForename;
            var refactoredTrackingSystemEnabled = await featureManager.IsEnabledAsync("RefactoredTrackingSystem");

            var baseUrlConfigOption = refactoredTrackingSystemEnabled
                ? configuration.GetAppRootPath()
                : configuration.GetCurrentSystemBaseUrl();
            if (string.IsNullOrEmpty(baseUrlConfigOption))
            {
                var missingConfigValue = refactoredTrackingSystemEnabled ? "AppRootPath" : "CurrentSystemBaseUrl";
                throw new Exceptions.ConfigValueMissingException(
            $"Encountered an error while trying to send an email: The value of {missingConfigValue} is null"
                );
            }

            var baseUrl = refactoredTrackingSystemEnabled
                ? $"{baseUrlConfigOption}/TrackingSystem/Delegates/CourseDelegates"
                : $"{baseUrlConfigOption}/Tracking/CourseDelegates";

            var unlockUrl = new UriBuilder(baseUrl)
            {
                Query = $"CustomisationID={unlockData.CustomisationId}",
            };

            var emailSubjectLine = "Digital Learning Solutions Progress Unlock Request";
            var builder = new BodyBuilder
            {
                TextBody = $@"Dear {unlockData.ContactForename}
                    Digital Learning Solutions Delegate, {unlockData.DelegateName}, has requested that you unlock their progress for the course {unlockData.CourseName}.
                    They have reached the maximum number of assessment attempt allowed without passing.
                    To review and unlock their progress, visit the this url: ${unlockUrl.Uri}.",
                HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'>
                    <p>Dear {unlockData.ContactForename}</p>
                    <p>Digital Learning Solutions Delegate, {unlockData.DelegateName}, has requested that you unlock their progress for the course {unlockData.CourseName}</p>
                    <p>They have reached the maximum number of assessment attempt allowed without passing.</p><p>To review and unlock their progress, <a href='{unlockUrl.Uri}'>click here</a>.</p>
                    </body>",
            };

            emailService.SendEmail(
                new Email(emailSubjectLine, builder, unlockData.ContactEmail, unlockData.DelegateEmail)
            );
        }
    }
}
