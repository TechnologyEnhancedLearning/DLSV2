namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using Dapper;

    public interface ISessionDataService
    {
        int StartOrRestartSession(int candidateId, int customisationId);
        void StopSession(int candidateId);
        void UpdateSessionDuration(int sessionId);
    }

    public class SessionDataService : ISessionDataService
    {
        private const string stopSessionsSql =
            @"UPDATE Sessions SET Active = 0
               WHERE CandidateId = @candidateId;";

        private readonly IDbConnection connection;

        public SessionDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public int StartOrRestartSession(int candidateId, int customisationId)
        {
            return connection.QueryFirst<int>(
                stopSessionsSql +
                @"INSERT INTO Sessions (CandidateID, CustomisationID, LoginTime, Duration, Active)
                  VALUES (@candidateId, @customisationId, GetUTCDate(), 0, 1);

                  SELECT SCOPE_IDENTITY();",
                new { candidateId, customisationId }
            );
        }

        public void StopSession(int candidateId)
        {
            connection.Query(stopSessionsSql, new { candidateId });
        }

        public void UpdateSessionDuration(int sessionId)
        {
            connection.Query(
                @"UPDATE Sessions SET Duration = DATEDIFF(minute, LoginTime, GetUTCDate())
                   WHERE [SessionID] = @sessionId AND Active = 1;",
                new { sessionId }
            );
        }
    }
}
