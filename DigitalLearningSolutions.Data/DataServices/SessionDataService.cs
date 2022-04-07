namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;

    public interface ISessionDataService
    {
        int StartOrRestartDelegateSession(int candidateId, int customisationId);

        void StopDelegateSession(int candidateId);

        void UpdateDelegateSessionDuration(int sessionId);

        int StartAdminSession(int adminId);

        bool HasAdminGotSessions(int adminId);

        IEnumerable<Session> GetSessionsForCandidateAndCustomisation(int candidateId, int customisationId);
    }

    public class SessionDataService : ISessionDataService
    {
        private const string StopSessionsSql =
            @"UPDATE Sessions SET Active = 0
               WHERE CandidateId = @candidateId;";

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

        public void UpdateDelegateSessionDuration(int sessionId)
        {
            connection.Query(
                @"UPDATE Sessions SET Duration = DATEDIFF(minute, LoginTime, GetUTCDate())
                   WHERE [SessionID] = @sessionId AND Active = 1;",
                new { sessionId }
            );
        }

        public int StartAdminSession(int adminId)
        {
            return connection.QueryFirst<int>(
                @"INSERT INTO AdminSessions (AdminID, LoginTime, Duration, Active)
                  VALUES (@adminId, GetUTCDate(), 0, 0);

                  SELECT SCOPE_IDENTITY();",
                new { adminId }
            );
        }

        public bool HasAdminGotSessions(int adminId)
        {
            return connection.ExecuteScalar<bool>(
                "SELECT 1 WHERE EXISTS (SELECT AdminSessionId FROM AdminSessions WHERE AdminID = @adminId)",
                new { adminId }
            );
        }

        public IEnumerable<Session> GetSessionsForCandidateAndCustomisation(int candidateId, int customisationId)
        {
            return connection.Query<Session>(
                @"SELECT SessionID, CandidateID, CustomisationID, LoginTime, Duration, Active
                       FROM Sessions
                       WHERE CandidateID = @candidateId
                       AND CustomisationID = @customisationId",
                new { candidateId, customisationId }
            );
        }
    }
}
