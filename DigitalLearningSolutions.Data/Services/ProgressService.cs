namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Email;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement;
    using MimeKit;

    public interface IProgressService
    {
        void UpdateSupervisor(int progressId, int? newSupervisorId);

        void UpdateCompleteByDate(int progressId, DateTime? completeByDate);

        void UpdateCompletionDate(int progressId, DateTime? completionDate);

        void UpdateDiagnosticScore(int progressId, int tutorialId, int myScore);

        void UnlockProgress(int progressId);

        DetailedCourseProgress? GetDetailedCourseProgress(int progressId);

        void UpdateCourseAdminFieldForDelegate(
            int progressId,
            int promptNumber,
            string? answer
        );

        void StoreAspProgressV2(
            int progressId,
            int version,
            string? lmGvSectionRow,
            int tutorialId,
            int tutorialTime,
            int tutorialStatus
        );

        void CheckProgressForCompletion(DetailedCourseProgress progress);
    }

    public class ProgressService : IProgressService
    {
        private readonly IConfiguration configuration;
        private readonly ICourseDataService courseDataService;
        private readonly ICourseService courseService;
        private readonly IEmailService emailService;
        private readonly IFeatureManager featureManager;
        private readonly IProgressDataService progressDataService;
        private readonly ISessionService sessionService;
        private readonly IUserService userService;

        public ProgressService(
            IConfiguration configuration,
            ICourseDataService courseDataService,
            ICourseService courseService,
            IEmailService emailService,
            IFeatureManager featureManager,
            IProgressDataService progressDataService,
            ISessionService sessionService,
            IUserService userService
        )
        {
            this.configuration = configuration;
            this.courseDataService = courseDataService;
            this.courseService = courseService;
            this.emailService = emailService;
            this.featureManager = featureManager;
            this.progressDataService = progressDataService;
            this.sessionService = sessionService;
            this.userService = userService;
        }

        public void UpdateSupervisor(int progressId, int? newSupervisorId)
        {
            var courseInfo = courseDataService.GetDelegateCourseInfoByProgressId(progressId);

            if (courseInfo == null)
            {
                throw new ProgressNotFoundException($"No progress record found for ProgressID {progressId}");
            }

            if (courseInfo.SupervisorAdminId == newSupervisorId)
            {
                return;
            }

            var supervisorId = newSupervisorId ?? 0;

            using var transaction = new TransactionScope();

            progressDataService.UpdateProgressSupervisorAndCompleteByDate(
                progressId,
                supervisorId,
                courseInfo.CompleteBy
            );

            progressDataService.ClearAspProgressVerificationRequest(progressId);

            transaction.Complete();
        }

        public void UpdateCompleteByDate(int progressId, DateTime? completeByDate)
        {
            var courseInfo = courseDataService.GetDelegateCourseInfoByProgressId(progressId);

            if (courseInfo == null)
            {
                throw new ProgressNotFoundException($"No progress record found for ProgressID {progressId}");
            }

            courseDataService.SetCompleteByDate(progressId, courseInfo.DelegateId, completeByDate);
        }

        public void UpdateCompletionDate(int progressId, DateTime? date)
        {
            var courseInfo = courseDataService.GetDelegateCourseInfoByProgressId(progressId);

            if (courseInfo == null)
            {
                throw new ProgressNotFoundException($"No progress record found for ProgressID {progressId}");
            }

            progressDataService.SetCompletionDate(progressId, date);
        }

        public void UpdateDiagnosticScore(int progressId, int tutorialId, int myScore)
        {
            progressDataService.UpdateDiagnosticScore(progressId, tutorialId, myScore);
        }

        public void UnlockProgress(int progressId)
        {
            progressDataService.UnlockProgress(progressId);
        }

        public DetailedCourseProgress? GetDetailedCourseProgress(int progressId)
        {
            var progress = progressDataService.GetProgressByProgressId(progressId);

            var courseInfo = courseDataService.GetDelegateCourseInfoByProgressId(progressId);

            if (progress == null || courseInfo == null)
            {
                return null;
            }

            var sections = progressDataService.GetSectionProgressDataForProgressEntry(progressId).ToList();
            foreach (var section in sections)
            {
                section.Tutorials =
                    progressDataService.GetTutorialProgressDataForSection(progressId, section.SectionId);
            }

            return new DetailedCourseProgress(progress, sections, courseInfo);
        }

        public void UpdateCourseAdminFieldForDelegate(
            int progressId,
            int promptNumber,
            string? answer
        )
        {
            progressDataService.UpdateCourseAdminFieldForDelegate(progressId, promptNumber, answer);
        }

        // TODO: 410 - Write progress service tests
        public void StoreAspProgressV2(
            int progressId,
            int version,
            string? lmGvSectionRow,
            int tutorialId,
            int tutorialTime,
            int tutorialStatus
        )
        {
            progressDataService.UpdateProgressDetails(
                progressId,
                version,
                DateTime.UtcNow,
                lmGvSectionRow ?? string.Empty
            );
            progressDataService.UpdateAspProgressTutTime(tutorialId, progressId, tutorialTime);
            progressDataService.UpdateAspProgressTutStat(tutorialId, progressId, tutorialStatus);
        }

        public void CheckProgressForCompletion(DetailedCourseProgress progress)
        {
            if (!(progress is { Completed: null }))
            {
                return;
            }

            var completionStatus = progressDataService.GetCompletionStatusForProgress(progress.ProgressId);
            if (completionStatus > 0)
            {
                progressDataService.UpdateProgressCompletedDate(progress.ProgressId, DateTime.UtcNow);
                var numLearningLogItemsAffected =
                    progressDataService.MarkLearningLogItemsWithProgressIdComplete(progress.ProgressId);
                EmailDelegatesAboutProgressCompletion(progress, completionStatus, numLearningLogItemsAffected);
            }
        }

        private async void EmailDelegatesAboutProgressCompletion(
            DetailedCourseProgress progress,
            int completionStatus,
            int numLearningLogItemsAffected
        )
        {
            var adminsToCc = progressDataService.GetAdminsToEmailAboutProgressCompletion(progress.ProgressId).ToArray();
            var delegateUser = userService.GetDelegateUserById(progress.DelegateId);
            if (delegateUser?.EmailAddress == null)
            {
                return;
            }

            var customisationName = courseService.GetCourseNameAndApplication(progress.CustomisationId);

            // TODO: 410 Put this in a helper?
            // TODO: 410 Do I need the url to be dynamic?
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

            var sessionId = sessionService.GetHighestSessionIdForCandidateAndCustomisation(
                delegateUser.Id,
                progress.CustomisationId
            );
            var finaliseUrl = new UriBuilder(baseUrl)
            {
                Query = $"SessionID={sessionId}&ProgressID={progress.ProgressId}&UserCentreID={delegateUser.CentreId}",
            };

            var htmlActivityCompletionInfo = completionStatus == 2
                ? $@"<p>To evaluate the activity and access your certificate of completion, click <a href='{finaliseUrl}'>here</a>.</p>"
                : $@"<p>If you haven't already done so, please evaluate the activity to help us to improve provision for future delegates by clicking
                    <a href='{finaliseUrl}'>here</a>. Only one evaluation can be submitted per completion.</p>";

            if (numLearningLogItemsAffected > 0)
            {
                htmlActivityCompletionInfo +=
                    $@"<p>This activity is related to <b>{numLearningLogItemsAffected}</b> planned development log actions in other activities in your Learning Portal. These have automatically been marked as complete.</p>";
            }

            if (adminsToCc.Any())
            {
                htmlActivityCompletionInfo +=
                    "<p><b>Note:</b> This message has been copied to the administrator(s) managing this activity, for their information.</p>";
            }

            const string emailSubjectLine = "Digital Learning Solutions Progress Unlock Request";
            var builder = new BodyBuilder
            {
                HtmlBody = $@"<body style=""font-family: Calibri; font-size: small;"">
                                <p>Dear {delegateUser.FirstName ?? "Digital learning Solutions delegate"},</p>
                                <p>You have completed the Digital Learning Solutions learning activity - {customisationName}</p>
                                {htmlActivityCompletionInfo}
                            </body>",
            };

            emailService.SendEmail(
                new Email(emailSubjectLine, builder, new[] { delegateUser.EmailAddress }, adminsToCc)
            );
        }
    }
}
