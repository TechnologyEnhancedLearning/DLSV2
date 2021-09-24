namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using System;
    using DigitalLearningSolutions.Data.Models;

    public static class ProgressTestHelper
    {
        public static Progress GetDefaultProgress(
            int progressId = 1,
            int candidateId = 1,
            int customisationId = 1,
            DateTime? completed = null,
            DateTime? removedDate = null,
            int supervisorAdminId = 1,
            DateTime? completeByDate = null
        )
        {
            return new Progress
            {
                ProgressId = progressId,
                CandidateId = candidateId,
                CustomisationId = customisationId,
                Completed = completed,
                RemovedDate = removedDate,
                SupervisorAdminId = supervisorAdminId,
                CompleteByDate = completeByDate
            };
        }
    }
}
