namespace DigitalLearningSolutions.Data.Services
{
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;

    public interface IProgressService
    {
        void UpdateSupervisor(int progressId, int newSupervisorId);
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

        public void UpdateSupervisor(int progressId, int newSupervisorId)
        {
            var courseInfo = courseDataService.GetDelegateCourseInfoByProgressId(progressId);

            if (courseInfo!.SupervisorAdminId == newSupervisorId)
            {
                return;
            }

            using var transaction = new TransactionScope();

            progressDataService.UpdateProgressSupervisorAndCompleteByDate(
                progressId,
                newSupervisorId,
                courseInfo.CompleteBy
            );

            progressDataService.ClearAspProgressVerificationRequest(progressId);

            transaction.Complete();
        }
    }
}
