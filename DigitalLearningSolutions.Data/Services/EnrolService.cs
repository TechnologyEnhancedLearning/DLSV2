using DigitalLearningSolutions.Data.DataServices;
using System;
using System.Linq;

namespace DigitalLearningSolutions.Data.Services
{
    public interface IEnrolService
    {
        void EnrolDelegateOnCourse(
           int delegateId,
            int customisationId,
            int customisationVersion,
            int enrollmentMethodId,
            int? enrolledByAdminId,
            DateTime? completeByDate,
            int? supervisorAdminId
       );
    }
    public class EnrolService : IEnrolService
    {
        private readonly IClockService clockService;
        private readonly IProgressDataService progressDataService;
        private readonly ITutorialContentDataService tutorialContentDataService;

        public EnrolService(
            IClockService clockService,
            ITutorialContentDataService tutorialContentDataService,
            IProgressDataService progressDataService
        )
        {
            this.clockService = clockService;
            this.tutorialContentDataService = tutorialContentDataService;
            this.progressDataService = progressDataService;
        }
        public void EnrolDelegateOnCourse(int delegateId, int customisationId, int customisationVersion, int enrollmentMethodId, int? enrolledByAdminId, DateTime? completeByDate, int? supervisorAdminId)
        {
            var candidateProgressOnCourse =
               progressDataService.GetDelegateProgressForCourse(
                   delegateId,
                   customisationId
               );
            var existingRecordsToUpdate =
                candidateProgressOnCourse.Where(
                    p => p.Completed == null && p.RemovedDate == null
                ).ToList();

            if (existingRecordsToUpdate.Any())
            {
                foreach (var progressRecord in existingRecordsToUpdate)
                {
                    progressDataService.UpdateProgressSupervisorAndCompleteByDate(
                        progressRecord.ProgressId,
                        supervisorAdminId ?? 0,
                        completeByDate
                    );
                }
            }
            else
            {
                var newProgressId = progressDataService.CreateNewDelegateProgress(
                    delegateId,
                    customisationId,
                    customisationVersion,
                    clockService.UtcNow,
                    3,
                    enrolledByAdminId,
                completeByDate,
                supervisorAdminId ?? 0
                );
                var tutorialsForCourse =
                    tutorialContentDataService.GetTutorialIdsForCourse(customisationId);

                foreach (var tutorial in tutorialsForCourse)
                {
                    progressDataService.CreateNewAspProgress(tutorial, newProgressId);
                }
            }
        }
    }
}
