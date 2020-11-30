namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System;
    using System.Linq;
    using Dapper;
    using Microsoft.Data.SqlClient;

    public class CourseContentTestHelper
    {
        private SqlConnection connection;

        public CourseContentTestHelper(SqlConnection connection)
        {
            this.connection = connection;
        }

        public int GetLoginCount(int progressId)
        {
            return connection.QueryFirstOrDefault<int>(
                @"SELECT LoginCount
                        FROM Progress
                        WHERE ProgressID = @progressId",
                new { progressId }
            );
        }

        public int GetDuration(int progressId)
        {
            return connection.QueryFirstOrDefault<int>(
                @"SELECT Duration
                        FROM Progress
                        WHERE ProgressID = @progressId",
                new { progressId }
            );
        }

        public DateTime GetSubmittedTime(int progressId)
        {
            return connection.QueryFirstOrDefault<DateTime>(
                @"SELECT SubmittedTime
                        FROM Progress
                        WHERE ProgressID = @progressId",
                new { progressId }
            );
        }

        public void InsertSession(
            int candidateId,
            int customisationId,
            DateTime loginTime,
            int duration)
        {
            connection.Execute(
                @"INSERT INTO SESSIONS
                        ([CandidateID]
                        ,[CustomisationID]
                        ,[LoginTime]
                        ,[Duration]
                        ,[Active])
                    VALUES (@candidateId, @customisationId, @loginTime, @duration, 0)",
                new { candidateId, customisationId, loginTime, duration }
            );
        }

        public bool IsApproximatelyNow(DateTime timeToCheck)
        {
            var twoMinutesAgo = DateTime.Now.AddMinutes(-2);
            return timeToCheck >= twoMinutesAgo && timeToCheck <= DateTime.Now;
        }

        public bool DoesProgressExist(int candidateId, int customisationId)
        {
            return connection.Query<int>(
                @"SELECT ProgressId
                        FROM Progress
                        WHERE CandidateID = @candidateId
                          AND CustomisationID = @customisationId
                          AND SystemRefreshed = 0
                          AND RemovedDate IS NULL",
                new { candidateId, customisationId }
            ).Any();
        }
    }
}
