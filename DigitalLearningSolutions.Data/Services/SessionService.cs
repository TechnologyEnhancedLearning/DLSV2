namespace DigitalLearningSolutions.Data.Services
{
    using Microsoft.AspNetCore.Http;

    public interface ISessionService
    {
        void StartOrUpdateSession(int candidateId, int customisationId, ISession httpContextSession);
        void StopSession(int candidateId, ISession httpContextSession);
    }

    public class SessionService : ISessionService
    {
        private readonly ISessionDataService sessionDataService;

        public SessionService(ISessionDataService sessionDataService)
        {
            this.sessionDataService = sessionDataService;
        }

        public void StartOrUpdateSession(int candidateId, int customisationId, ISession httpContextSession)
        {
            var currentSessionId = httpContextSession.GetInt32($"SessionID-{customisationId}");
            if (currentSessionId != null)
            {
                sessionDataService.UpdateSessionDuration(currentSessionId.Value);
            }
            else
            {
                // Clear all session variables
                httpContextSession.Clear();

                // Make and keep track of a new session starting at this request
                var newSessionId = sessionDataService.StartOrRestartSession(candidateId, customisationId);
                httpContextSession.SetInt32($"SessionID-{customisationId}", newSessionId);
            }
        }

        public void StopSession(int candidateId, ISession httpContextSession)
        {
            sessionDataService.StopSession(candidateId);
            httpContextSession.Clear();
        }
    }
}
