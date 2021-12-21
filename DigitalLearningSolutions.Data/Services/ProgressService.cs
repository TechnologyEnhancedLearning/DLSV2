namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Exceptions;

    public interface IProgressService
    {
        void UpdateSupervisor(int progressId, int? newSupervisorId);

        void UpdateCompleteByDate(int progressId, DateTime? completeByDate);

        void UpdateCompletionDate(int progressId, DateTime? completionDate);

        public void UpdateDiagnosticScore(int progressId, int tutorialId, int myScore);

        void UnlockProgress(int progressId);
    }

    public class ProgressService : IProgressService
    {
        private readonly ICourseDataService courseDataService;
        private readonly IProgressDataService progressDataService;

        public ProgressService(ICourseDataService courseDataService, IProgressDataService progressDataService)
        {
            this.courseDataService = courseDataService;
            this.progressDataService = progressDataService;
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
    }
}
