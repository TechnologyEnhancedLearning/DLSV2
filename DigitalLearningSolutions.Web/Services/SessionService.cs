﻿namespace DigitalLearningSolutions.Web.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Utilities;
    using Microsoft.AspNetCore.Http;

    public interface ISessionService
    {
        int StartOrUpdateDelegateSession(int candidateId, int customisationId, ISession httpContextSession);

        void StopDelegateSession(int candidateId, ISession httpContextSession);

        void StartAdminSession(int? adminId);
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

        public int StartOrUpdateDelegateSession(int candidateId, int customisationId, ISession httpContextSession)
        {
            var currentSessionId = httpContextSession.GetInt32($"SessionID-{customisationId}");
            int returnValue = 0;
            if (currentSessionId != null)
            {
                returnValue = sessionDataService.UpdateDelegateSessionDuration(currentSessionId.Value, clockUtility.UtcNow);
            }
            else
            {
                // Clear all session variables
                httpContextSession.Clear();

                // Make and keep track of a new session starting at this request
                var newSessionId = sessionDataService.StartOrRestartDelegateSession(candidateId, customisationId);
                httpContextSession.SetInt32($"SessionID-{customisationId}", newSessionId);
                returnValue = newSessionId;
            }
            return returnValue;
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
