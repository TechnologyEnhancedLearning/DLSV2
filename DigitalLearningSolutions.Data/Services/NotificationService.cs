namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Email;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement;
    using MimeKit;

    public interface INotificationService
    {
        Task SendUnlockRequest(int progressId);

        void SendProgressCompletionNotificationEmail(
            DetailedCourseProgress progress,
            int completionStatus,
            int numLearningLogItemsAffected
        );
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
                throw new ConfigValueMissingException(
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

            const string emailSubjectLine = "Digital Learning Solutions Progress Unlock Request";
            var builder = new BodyBuilder
            {
                TextBody = $@"Dear {unlockData.ContactForename}
                    Digital Learning Solutions Delegate, {unlockData.DelegateName}, has requested that you unlock their progress for the course {unlockData.CourseName}.
                    They have reached the maximum number of assessment attempt allowed without passing.
                    To review and unlock their progress, visit this url: ${unlockUrl.Uri}.",
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

        public void SendProgressCompletionNotificationEmail(
            DetailedCourseProgress progress,
            int completionStatus,
            int numLearningLogItemsAffected
        )
        {
            var progressCompletionData = notificationDataService.GetProgressCompletionData(
                progress.ProgressId,
                progress.DelegateId,
                progress.CustomisationId
            );

            if (progressCompletionData == null || progress.DelegateEmail == null ||
                progress.DelegateEmail.Trim() == string.Empty)
            {
                return;
            }

            var finaliseUrl = configuration.GetAppRootPath() + "/tracking/finalise" +
                              $@"?SessionID={progressCompletionData.SessionId}&ProgressID={progress.ProgressId}&UserCentreID={progressCompletionData.CentreId}";

            var htmlActivityCompletionInfo = completionStatus == 2
                ? $@"<p>To evaluate the activity and access your certificate of completion, click <a href='{finaliseUrl}'>here</a>.</p>"
                : $@"<p>If you haven't already done so, please evaluate the activity to help us to improve provision for future delegates by clicking
                    <a href='{finaliseUrl}'>here</a>. Only one evaluation can be submitted per completion.</p>";
            var textActivityCompletionInfo = completionStatus == 2
                ? $@"To evaluate the activity and access your certificate of completion, visit this url: {finaliseUrl}."
                : "If you haven't already done so, please evaluate the activity to help us to improve provision " +
                  $@"for future delegates by visiting this url: {finaliseUrl}. Only one evaluation can be submitted per completion.";

            if (numLearningLogItemsAffected == 1)
            {
                htmlActivityCompletionInfo +=
                    "<p>This activity is related to <b>1</b> planned development log action in another activity " +
                    "in your Learning Portal. This has automatically been marked as complete.</p>";
                textActivityCompletionInfo +=
                    " This activity is related to 1 planned development log action in another activity " +
                    "in your Learning Portal. This has automatically been marked as complete.";
            }
            else if (numLearningLogItemsAffected > 1)
            {
                htmlActivityCompletionInfo +=
                    $@"<p>This activity is related to <b>{numLearningLogItemsAffected}</b> planned development log actions " +
                    "in other activities in your Learning Portal. These have automatically been marked as complete.</p>";
                textActivityCompletionInfo +=
                    $@" This activity is related to {numLearningLogItemsAffected} planned development log actions " +
                    "in other activities in your Learning Portal. These have automatically been marked as complete.";
            }

            if (progressCompletionData.AdminEmail != null || progressCompletionData.CourseNotificationEmail != null)
            {
                htmlActivityCompletionInfo +=
                    "<p><b>Note:</b> This message has been copied to the administrator(s) managing this activity, for their information.</p>";
                textActivityCompletionInfo +=
                    " Note: This message has been copied to the administrator(s) managing this activity, for their information.";
            }

            const string emailSubjectLine = "Digital Learning Solutions Activity Complete";
            var delegateNameOrGenericTitle = progress.DelegateFirstName ?? "Digital Learning Solutions Delegate";
            var emailsToCc = GetEmailsToCc(
                progressCompletionData.AdminEmail,
                progressCompletionData.CourseNotificationEmail
            );

            var builder = new BodyBuilder
            {
                TextBody = $@"Dear {delegateNameOrGenericTitle},
                    You have completed the Digital Learning Solutions learning activity - {progressCompletionData.CourseName}.
                    {textActivityCompletionInfo}",
                HtmlBody = $@"<body style=""font-family: Calibri; font-size: small;"">
                                <p>Dear {delegateNameOrGenericTitle},</p>
                                <p>You have completed the Digital Learning Solutions learning activity - {progressCompletionData.CourseName}.</p>
                                {htmlActivityCompletionInfo}
                            </body>",
            };

            var email = new Email(
                emailSubjectLine,
                builder,
                new[] { progress.DelegateEmail },
                emailsToCc
            );
            emailService.SendEmail(email);
        }

        private static string[]? GetEmailsToCc(string? adminEmail, string? courseNotificationEmail)
        {
            var emailsToCc = new List<string>();

            if (adminEmail != null)
            {
                emailsToCc.Add(adminEmail);
            }

            if (courseNotificationEmail != null)
            {
                emailsToCc.Add(courseNotificationEmail);
            }

            return emailsToCc.Any() ? emailsToCc.ToArray() : null;
        }
    }
}
