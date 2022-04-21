namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using System;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;
    using Microsoft.Data.SqlClient;

    public class ProgressTestHelper
    {
        private readonly SqlConnection connection;

        public ProgressTestHelper(SqlConnection connection)
        {
            this.connection = connection;
        }

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
                CompleteByDate = completeByDate,
            };
        }

        public DateTime? GetSupervisorVerificationRequestedByAspProgressId(int aspProgressId)
        {
            return connection.Query<DateTime?>(
                @"SELECT SupervisorVerificationRequested
                    FROM aspProgress
                    WHERE aspProgressId = @aspProgressId",
                new { aspProgressId }
            ).Single();
        }

        public DiagnosticScore GetDiagnosticInfoByAspProgressId(int aspProgressId)
        {
            return connection.QueryFirstOrDefault<DiagnosticScore>(
                @"SELECT DiagHigh,
                            DiagLow,
                            DiagLast,
                            DiagAttempts
                    FROM aspProgress
                    WHERE aspProgressId = @aspProgressId",
                new { aspProgressId }
            );
        }

        public bool GetCourseProgressLockedStatusByProgressId(int progressId)
        {
            return connection.Query<bool>(
                @"SELECT PLLocked
                    FROM Progress
                    WHERE ProgressId = @ProgressId",
                new { progressId }
            ).Single();
        }

        public string GetAdminFieldAnswer1ByProgressId(int progressId)
        {
            return connection.Query<string>(
                @"SELECT Answer1
                    FROM Progress
                    WHERE ProgressId = @ProgressId",
                new { progressId }
            ).Single();
        }
    }
}
