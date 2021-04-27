namespace DigitalLearningSolutions.Data.Models
{
    using System;

    public class AdminSession
    {
        public AdminSession(
            int adminSessionId,
            int adminId,
            DateTime loginTime,
            int duration,
            bool active
        )
        {
            AdminSessionId = adminSessionId;
            AdminId = adminId;
            LoginTime = loginTime;
            Duration = duration;
            Active = active;
        }

        public int AdminSessionId { get; }
        public int AdminId { get; }
        public DateTime LoginTime { get; }
        public int Duration { get; }
        public bool Active { get; }
    }
}
