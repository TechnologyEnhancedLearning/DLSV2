namespace DigitalLearningSolutions.Data.DataServices
{
    using Dapper;
    using DigitalLearningSolutions.Data.Models;
    using System;
    using System.Data;

    public interface ISessionDataService
    {
        int StartOrRestartDelegateSession(int candidateId, int customisationId);

        void StopDelegateSession(int candidateId);

        void UpdateDelegateSessionDuration(int sessionId, DateTime currentUtcTime);

        int StartAdminSession(int adminId);

        void StopAdminSession(int adminId, int adminSessionId);

        void StopAllAdminSessions(int adminId);

        bool HasAdminGotSessions(int adminId);

        bool HasAdminGotActiveSessions(int adminId);

        bool HasDelegateGotSessions(int delegateId);

        Session? GetSessionById(int sessionId);

        AdminSession? GetAdminSessionById(int sessionId);

        void AddTutorialTimeToSessionDuration(int sessionId, int tutorialTime);
    }

    public class SessionDataService : ISessionDataService
    {
        private const string StopSessionsSql =
            @"UPDATE Sessions SET Active = 0
               WHERE CandidateId = @candidateId;";

        private const string StopAdminSql =
            @"UPDATE AdminSessions SET Active = 0
                WHERE AdminID = @adminId
                AND AdminSessionID = @adminSessionId;";

        private const string StopAllAdminSql =
            @"UPDATE AdminSessions SET Active = 0
                WHERE AdminID = @adminId;";

        private readonly IDbConnection connection;

        public SessionDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public int StartOrRestartDelegateSession(int candidateId, int customisationId)
        {
            return connection.QueryFirst<int>(
                StopSessionsSql +
                @"INSERT INTO Sessions (CandidateID, CustomisationID, LoginTime, Duration, Active)
                  VALUES (@candidateId, @customisationId, GetUTCDate(), 0, 1);

                  SELECT SCOPE_IDENTITY();",
                new { candidateId, customisationId }
            );
        }

        public void StopDelegateSession(int candidateId)
        {
            connection.Query(StopSessionsSql, new { candidateId });
        }

        public void UpdateDelegateSessionDuration(int sessionId, DateTime currentUtcTime)
        {
            connection.Query(
                @"UPDATE Sessions SET Duration = DATEDIFF(minute, LoginTime, @currentUtcTime)
                   WHERE [SessionID] = @sessionId AND Active = 1;",
                new { sessionId, currentUtcTime }
            );
        }

        public int StartAdminSession(int adminId)
        {
            return connection.QueryFirst<int>(
                @"INSERT INTO AdminSessions (AdminID, LoginTime, Duration, Active)
                  VALUES (@adminId, GetUTCDate(), 0, 1);

                  SELECT SCOPE_IDENTITY();",
                new { adminId }
            );
        }

        public void StopAdminSession(int adminId, int adminSessionId)
        {
            connection.Query(StopAdminSql, new { adminId, adminSessionId });
        }

        public void StopAllAdminSessions(int adminId)
        {
            connection.Query(StopAllAdminSql, new { adminId });
        }

        public bool HasAdminGotSessions(int adminId)
        {
            return connection.ExecuteScalar<bool>(
                "SELECT 1 WHERE EXISTS (SELECT AdminSessionId FROM AdminSessions WHERE AdminID = @adminId)",
                new { adminId }
            );
        }

        public bool HasAdminGotActiveSessions(int adminId)
        {
            return connection.ExecuteScalar<bool>(
                "SELECT 1 WHERE EXISTS (SELECT adminSessionId FROM AdminSessions WHERE AdminID = @adminId AND Active = 1)",
                new { adminId }
                );
        }

        public bool HasDelegateGotSessions(int delegateId)
        {
            return connection.ExecuteScalar<bool>(
                "SELECT 1 WHERE EXISTS (SELECT CandidateID FROM Sessions WHERE CandidateID = @delegateId)",
                new { delegateId }
            );
        }

        public Session? GetSessionById(int sessionId)
        {
            return connection.QueryFirstOrDefault<Session>(
                @"SELECT SessionID,
                        CandidateID,
                        CustomisationID,
                        LoginTime,
                        Duration,
                        Active
                    FROM Sessions WHERE SessionID = @sessionId",
                new { sessionId }
            );
        }

        public AdminSession? GetAdminSessionById(int sessionId)
        {
            return connection.QueryFirstOrDefault<AdminSession>(
                @"SELECT AdminSessionID,
                        AdminID,
                        LoginTime,
                        Duration,
                        Active
                    FROM AdminSessions WHERE AdminSessionID = @sessionId",
                new { sessionId }
            );
        }

        public void AddTutorialTimeToSessionDuration(int sessionId, int tutorialTime)
        {
            connection.Query(
                @"UPDATE Sessions SET Duration = Duration + @tutorialTime
                   WHERE SessionID = @sessionId",
                new { sessionId, tutorialTime }
            );
        }
    }
}
