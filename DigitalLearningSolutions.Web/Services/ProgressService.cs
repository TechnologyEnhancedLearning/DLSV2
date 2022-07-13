namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.Progress;
    using DigitalLearningSolutions.Data.Models.Tracker;
    using DigitalLearningSolutions.Data.Utilities;

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
            string? progressText,
            int tutorialId,
            int tutorialTime,
            int tutorialStatus
        );

        void CheckProgressForCompletionAndSendEmailIfCompleted(DelegateCourseInfo progress);

        public SectionAndApplicationDetailsForAssessAttempts? GetSectionAndApplicationDetailsForAssessAttempts(
            int sectionId,
            int customisationId
        );
    }

    public class ProgressService : IProgressService
    {
        private readonly IClockUtility clockUtility;
        private readonly ICourseAdminFieldsService courseAdminFieldsService;
        private readonly ICourseDataService courseDataService;
        private readonly ILearningLogItemsDataService learningLogItemsDataService;
        private readonly INotificationService notificationService;
        private readonly IProgressDataService progressDataService;

        public ProgressService(
            ICourseDataService courseDataService,
            IProgressDataService progressDataService,
            INotificationService notificationService,
            ILearningLogItemsDataService learningLogItemsDataService,
            IClockUtility clockUtility,
            ICourseAdminFieldsService courseAdminFieldsService
        )
        {
            this.courseDataService = courseDataService;
            this.progressDataService = progressDataService;
            this.notificationService = notificationService;
            this.learningLogItemsDataService = learningLogItemsDataService;
            this.clockUtility = clockUtility;
            this.courseAdminFieldsService = courseAdminFieldsService;
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

            var coursePrompts = courseAdminFieldsService.GetCourseAdminFieldsWithAnswersForCourse(
                courseInfo
            );
            courseInfo.CourseAdminFields = coursePrompts;

            var sections = progressDataService.GetSectionProgressDataForProgressEntry(progressId).ToList();
            foreach (var section in sections)
            {
                section.Tutorials =
                    progressDataService.GetTutorialProgressDataForSection(progressId, section.SectionId);
            }

            return new DetailedCourseProgress(
                progress,
                sections,
                courseInfo
            );
        }

        public void UpdateCourseAdminFieldForDelegate(
            int progressId,
            int promptNumber,
            string? answer
        )
        {
            progressDataService.UpdateCourseAdminFieldForDelegate(progressId, promptNumber, answer);
        }

        public void StoreAspProgressV2(
            int progressId,
            int version,
            string? progressText,
            int tutorialId,
            int tutorialTime,
            int tutorialStatus
        )
        {
            var timeNow = clockUtility.UtcNow;
            progressDataService.UpdateProgressDetailsForStoreAspProgressV2(
                progressId,
                version,
                timeNow,
                progressText ?? string.Empty
            );
            progressDataService.UpdateAspProgressTutTime(tutorialId, progressId, tutorialTime);
            progressDataService.UpdateAspProgressTutStat(tutorialId, progressId, tutorialStatus);
        }

        public void CheckProgressForCompletionAndSendEmailIfCompleted(DelegateCourseInfo progress)
        {
            if (progress.Completed != null)
            {
                return;
            }

            var completionStatus = progressDataService.GetCompletionStatusForProgress(progress.ProgressId);
            if (completionStatus > 0)
            {
                progressDataService.SetCompletionDate(progress.ProgressId, DateTime.UtcNow);
                var numLearningLogItemsAffected =
                    learningLogItemsDataService.MarkLearningLogItemsCompleteByProgressId(progress.ProgressId);
                notificationService.SendProgressCompletionNotificationEmail(
                    progress,
                    completionStatus,
                    numLearningLogItemsAffected
                );
            }
        }

        public SectionAndApplicationDetailsForAssessAttempts? GetSectionAndApplicationDetailsForAssessAttempts(
            int sectionId,
            int customisationId
        )
        {
            return progressDataService.GetSectionAndApplicationDetailsForAssessAttempts(sectionId, customisationId);
        }
    }
}
