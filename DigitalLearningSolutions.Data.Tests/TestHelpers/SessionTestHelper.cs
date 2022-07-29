namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using System;
    using System.Collections.Generic;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Utilities;
    using FluentAssertions;
    using Microsoft.Data.SqlClient;

    public class SessionTestHelper
    {
        private readonly SqlConnection connection;
        private static readonly IClockUtility ClockUtility = new ClockUtility();

        public SessionTestHelper(SqlConnection connection)
        {
            this.connection = connection;
        }

        public Session? GetSession(int sessionId)
        {
            return connection.QueryFirstOrDefault<Session>(
                @"SELECT SessionID, CandidateID, CustomisationID, LoginTime, Duration, Active

                    FROM Sessions

                   WHERE SessionID = @sessionId",
                new { sessionId });
        }

        public AdminSession? GetAdminSession(int adminSessionId)
        {
            return connection.QueryFirstOrDefault<AdminSession>(
                @"SELECT AdminSessionID, AdminID, LoginTime, Duration, Active

                    FROM AdminSessions

                   WHERE AdminSessionID = @adminSessionId",
                new { adminSessionId });
        }

        public IEnumerable<Session> GetCandidateSessions(int candidateId)
        {
            return connection.Query<Session>(
                @"SELECT SessionID, CandidateID, CustomisationID, LoginTime, Duration, Active

                    FROM Sessions

                   WHERE CandidateID = @candidateId",
                new { candidateId });
        }

        public static Session CreateDefaultSession(
            int sessionId,
            int candidateId = 101,
            int customisationId = 1240,
            DateTime? loginTime = null,
            int duration = 0,
            bool active = true
        )
        {
            loginTime ??= ClockUtility.UtcNow;
            return new Session(sessionId, candidateId, customisationId, loginTime.Value, duration, active);
        }

        public static void SessionsShouldBeApproximatelyEquivalent(Session session1, Session session2)
        {
            const int tenSecondsInMilliseconds = 10000;

            session1.CandidateId.Should().Be(session2.CandidateId);
            session1.CustomisationId.Should().Be(session2.CustomisationId);
            session1.Duration.Should().Be(session2.Duration);
            session1.Active.Should().Be(session2.Active);
            session1.LoginTime.Should().BeCloseTo(session2.LoginTime, tenSecondsInMilliseconds);
        }
    }
}
