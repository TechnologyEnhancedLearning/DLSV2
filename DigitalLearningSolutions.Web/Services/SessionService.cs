namespace DigitalLearningSolutions.Web.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Utilities;
    using Microsoft.AspNetCore.Http;

    public interface ISessionService
    {
        void StartOrUpdateDelegateSession(int candidateId, int customisationId, ISession httpContextSession);

        void StopDelegateSession(int candidateId, ISession httpContextSession);

        int StartAdminSession(int? adminId);

        void StopAdminSession(int? adminId, int adminSessionId);

        void StopAllAdminSessions(int? adminId);
    }

    public class SessionService : ISessionService
    {
        private readonly IClockUtility clockUtility;
        private readonly ISessionDataService sessionDataService;

        public SessionService(IClockUtility clockUtility, ISessionDataService sessionDataService)
        {
            this.clockUtility = clockUtility;
            this.sessionDataService = sessionDataService;
        }

        public void StartOrUpdateDelegateSession(int candidateId, int customisationId, ISession httpContextSession)
        {
            var currentSessionId = httpContextSession.GetInt32($"SessionID-{customisationId}");
            if (currentSessionId != null)
            {
                sessionDataService.UpdateDelegateSessionDuration(currentSessionId.Value, clockUtility.UtcNow);
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

        public int StartAdminSession(int? adminId)
        {
            if (adminId != null)
            {
                return sessionDataService.StartAdminSession((int)adminId);
            }
            return -1;
        }

        public void StopAdminSession(int? adminId, int adminSessionId)
        {
            if (adminId != null)
            {
                sessionDataService.StopAdminSession((int)adminId, adminSessionId);
            }
        }

        public void StopAllAdminSessions(int? adminId)
        {
            if (adminId != null)
            {
                sessionDataService.StopAllAdminSessions((int)adminId);
            }
        }
    }
}
