namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using Dapper;

    public interface ISessionService
    {
        int StartOrRestartSession(int candidateId, int customisationId);
        void StopSession(int candidateId);
        void UpdateSessionDuration(int sessionId);
    }

    public class SessionService : ISessionService
    {
        private readonly IDbConnection connection;

        public SessionService(IDbConnection connection)
        {
            this.connection = connection;
        }

        private const string stopSessionsSql =
            @"UPDATE [Sessions] SET [Active] = 0
              WHERE [CandidateId] = @candidateId;";

        public int StartOrRestartSession(int candidateId, int customisationId)
        {
            return connection.QueryFirst<int>(
                stopSessionsSql +
                @"INSERT INTO [Sessions] ([CandidateID], [CustomisationID], [LoginTime], [Duration], [Active])
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
                @"UPDATE [Sessions] SET [Duration] = DATEDIFF(minute, [LoginTime], GetUTCDate())
                   WHERE [SessionID] = @sessionId AND [Active] = 1;",
                new { sessionId }
            );
        }
    }
}
