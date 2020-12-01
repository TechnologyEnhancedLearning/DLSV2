namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System.Collections.Generic;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;
    using FluentAssertions;
    using Microsoft.Data.SqlClient;

    public class SessionTestHelper
    {
        private SqlConnection connection;

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

        public IEnumerable<Session> GetCandidateSessions(int candidateId)
        {
            return connection.Query<Session>(
                @"SELECT SessionID, CandidateID, CustomisationID, LoginTime, Duration, Active
                    FROM Sessions
                   WHERE CandidateID = @candidateId",
                new { candidateId });
        }

        public static void SessionsShouldBeApproximatelyEquivalent(Session session1, Session session2)
        {
            session1.CandidateId.Should().Be(session2.CandidateId);
            session1.CustomisationId.Should().Be(session2.CustomisationId);
            session1.Duration.Should().Be(session2.Duration);
            session1.Active.Should().Be(session2.Active);
            session1.LoginTime.Should().BeCloseTo(session2.LoginTime, 10000);
        }
    }
}
