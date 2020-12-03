namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using Dapper;
    using Microsoft.AspNetCore.Http;

    public interface ISessionService
    {
        void StartOrUpdateSession(int candidateId, int customisationId, ISession httpSession);
        void StopSession(int candidateId, ISession httpSession);
    }

    public class SessionService : ISessionService
    {
        private const string stopSessionsSql =
            @"UPDATE Sessions SET Active = 0
               WHERE CandidateId = @candidateId;";

        private readonly IDbConnection connection;

        public SessionService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public void StartOrUpdateSession(int candidateId, int customisationId, ISession httpSession)
        {
            var currentSessionId = httpSession.GetInt32($"SessionID-{customisationId}");
            if (currentSessionId != null)
            {
                UpdateSessionDuration(currentSessionId.Value);
            }
            else
            {
                // Clear all session variables
                httpSession.Clear();

                // Make and keep track of a new session starting at this request
                var newSessionId = StartOrRestartSession(candidateId, customisationId);
                httpSession.SetInt32($"SessionID-{customisationId}", newSessionId);
            }
        }

        public void StopSession(int candidateId, ISession httpSession)
        {
            httpSession.Clear();
            connection.Query(stopSessionsSql, new { candidateId });
        }

        private int StartOrRestartSession(int candidateId, int customisationId)
        {
            return connection.QueryFirst<int>(
                stopSessionsSql +
                @"INSERT INTO Sessions (CandidateID, CustomisationID, LoginTime, Duration, Active)
                  VALUES (@candidateId, @customisationId, GetUTCDate(), 0, 1);

                  SELECT SCOPE_IDENTITY();",
                new { candidateId, customisationId }
            );
        }

        private void UpdateSessionDuration(int sessionId)
        {
            connection.Query(
                @"UPDATE Sessions SET Duration = DATEDIFF(minute, LoginTime, GetUTCDate())
                   WHERE [SessionID] = @sessionId AND Active = 1;",
                new { sessionId }
            );
        }
    }
}
