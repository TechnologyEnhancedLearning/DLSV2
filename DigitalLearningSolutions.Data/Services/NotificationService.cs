namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.Email;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement;
    using MimeKit;

    public interface INotificationService
    {
        Task SendUnlockRequest(int progressId);

        void SendProgressCompletionNotificationEmail(
            DelegateCourseInfo progress,
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
        private readonly IUserService userService;

        public NotificationService(
            IConfiguration configuration,
            INotificationDataService notificationDataService,
            IEmailService emailService,
            IFeatureManager featureManager,
            IUserService userService
        )
        {
            this.configuration = configuration;
            this.notificationDataService = notificationDataService;
            this.emailService = emailService;
            this.featureManager = featureManager;
            this.userService = userService;
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

            var delegateEntity = userService.GetDelegateById(unlockData.DelegateId)!;
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
                    Digital Learning Solutions Delegate, {delegateEntity.UserAccount.FullName}, has requested that you unlock their progress for the course {unlockData.CourseName}.
                    They have reached the maximum number of assessment attempt allowed without passing.
                    To review and unlock their progress, visit this url: ${unlockUrl.Uri}.",
                HtmlBody = $@"<body style= 'font-family: Calibri; font-size: small;'>
                    <p>Dear {unlockData.ContactForename}</p>
                    <p>Digital Learning Solutions Delegate, {delegateEntity.UserAccount.FullName}, has requested that you unlock their progress for the course {unlockData.CourseName}</p>
                    <p>They have reached the maximum number of assessment attempt allowed without passing.</p><p>To review and unlock their progress, <a href='{unlockUrl.Uri}'>click here</a>.</p>
                    </body>",
            };

            emailService.SendEmail(
                new Email(emailSubjectLine, builder, unlockData.ContactEmail, delegateEntity.GetEmailForCentreNotifications())
            );
        }

        public void SendProgressCompletionNotificationEmail(
            DelegateCourseInfo progress,
            int completionStatus,
            int numLearningLogItemsAffected
        )
        {
            var progressCompletionData = notificationDataService.GetProgressCompletionData(
                progress.ProgressId,
                progress.DelegateId,
                progress.CustomisationId
            );
            var delegateEntity = userService.GetDelegateById(progress.DelegateId);

            if (progressCompletionData == null || delegateEntity == null)
            {
                return;
            }

            var finaliseUrl = configuration.GetCurrentSystemBaseUrl() + "/tracking/finalise" +
                              $@"?SessionID={progressCompletionData.SessionId}&ProgressID={progress.ProgressId}&UserCentreID={progressCompletionData.CentreId}";

            var htmlActivityCompletionInfo = completionStatus == 2
                ? $@"<a href='{finaliseUrl}'><p>Evaluate the activity and access your certificate of completion here.</p></a>"
                : $@"<p>If you haven't already done so, please <a href='{finaliseUrl}'>evaluate the activity</a> to help us
                        to improve provision for future delegates. Only one evaluation can be submitted per completion.</p>";
            var textActivityCompletionInfo = completionStatus == 2
                ? $@"To evaluate the activity and access your certificate of completion, visit this URL: {finaliseUrl}."
                : "If you haven't already done so, please evaluate the activity to help us to improve provision " +
                  $@"for future delegates by visiting this URL: {finaliseUrl}. Only one evaluation can be submitted per completion.";

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

            if (progressCompletionData.AdminId != null || progressCompletionData.CourseNotificationEmail != null)
            {
                htmlActivityCompletionInfo +=
                    "<p><b>Note:</b> This message has been copied to the administrator(s) managing this activity, for their information.</p>";
                textActivityCompletionInfo +=
                    " Note: This message has been copied to the administrator(s) managing this activity, for their information.";
            }

            const string emailSubjectLine = "Digital Learning Solutions Activity Complete";
            var delegateNameOrGenericTitle = progress.DelegateFirstName ?? "Digital Learning Solutions Delegate";
            var emailsToCc = GetEmailsToCc(
                progressCompletionData.AdminId,
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
                new[] { delegateEntity.GetEmailForCentreNotifications() },
                emailsToCc
            );
            emailService.SendEmail(email);
        }

        private string[]? GetEmailsToCc(int? adminId, string? courseNotificationEmail)
        {
            var emailsToCc = new List<string>();

            if (adminId != null)
            {
                var adminEntity = userService.GetAdminById(adminId.Value)!;
                emailsToCc.Add(adminEntity.GetEmailForCentreNotifications());
            }

            if (courseNotificationEmail != null)
            {
                emailsToCc.Add(courseNotificationEmail);
            }

            return emailsToCc.Any() ? emailsToCc.ToArray() : null;
        }
    }
}
