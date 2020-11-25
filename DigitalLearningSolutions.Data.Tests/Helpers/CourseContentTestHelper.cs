namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System;
    using Dapper;

    public static class CourseContentTestHelper
    {
        public static int GetLoginCount(int progressId)
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            return connection.QueryFirstOrDefault<int>(
                @"SELECT LoginCount
                        FROM Progress
                        WHERE ProgressID = @progressId",
                new { progressId }
            );
        }

        public static int GetDuration(int progressId)
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            return connection.QueryFirstOrDefault<int>(
                @"SELECT Duration
                        FROM Progress
                        WHERE ProgressID = @progressId",
                new { progressId }
            );
        }

        public static void InsertSession(
            int candidateId, int customisationId, DateTime loginTime, int duration, int active)
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            connection.Execute(
                @"INSERT INTO SESSIONS
                        ([CandidateID]
                        ,[CustomisationID]
                        ,[LoginTime]
                        ,[Duration]
                        ,[Active])
                    VALUES (@candidateId, @customisationId, @loginTime, @duration, @active)",
                new { candidateId, customisationId, loginTime, duration, active }
            );
        }
    }
}
