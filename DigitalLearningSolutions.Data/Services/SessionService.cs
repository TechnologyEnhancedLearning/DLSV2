namespace DigitalLearningSolutions.Data.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using Microsoft.AspNetCore.Http;

    public interface ISessionService
    {
        void StartOrUpdateDelegateSession(int candidateId, int customisationId, ISession httpContextSession);

        void StopDelegateSession(int candidateId, ISession httpContextSession);

        void StartAdminSession(int? adminId);
    }

    public class SessionService : ISessionService
    {
        private readonly ISessionDataService sessionDataService;

        public SessionService(ISessionDataService sessionDataService)
        {
            this.sessionDataService = sessionDataService;
        }

        public void StartOrUpdateDelegateSession(int candidateId, int customisationId, ISession httpContextSession)
        {
            var currentSessionId = httpContextSession.GetInt32($"SessionID-{customisationId}");
            if (currentSessionId != null)
            {
                sessionDataService.UpdateDelegateSessionDuration(currentSessionId.Value);
            }
            else
            {
                // Clear all session variables
                httpContextSession.Clear();

                // Make and keep track of a new session starting at this request
                var newSessionId = sessionDataService.StartOrRestartDelegateSession(candidateId, customisationId);
                httpContextSession.SetInt32($"SessionID-{customisationId}", newSessionId);
            }
        }

        public void StopDelegateSession(int candidateId, ISession httpContextSession)
        {
            sessionDataService.StopDelegateSession(candidateId);
            httpContextSession.Clear();
        }

        public void StartAdminSession(int? adminId)
        {
            if (adminId != null)
            {
                sessionDataService.StartAdminSession((int)adminId);
            }
        }
    }
}
