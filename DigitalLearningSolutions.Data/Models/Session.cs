using System;
namespace DigitalLearningSolutions.Data.Models
{
    public class Session
    {
        public int SessionId { get; }
        public int CandidateId { get; }
        public int CustomisationId { get; }
        public DateTime LoginTime { get; }
        public int Duration { get; }
        public bool Active { get; }

        public Session(
            int sessionId,
            int candidateId,
            int customisationId,
            DateTime loginTime,
            int duration,
            bool active
        )
        {
            SessionId = sessionId;
            CandidateId = candidateId;
            CustomisationId = customisationId;
            LoginTime = loginTime;
            Duration = duration;
            Active = active;
        }
    }
}
