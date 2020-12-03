namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    internal class SessionServiceTests
    {
        private SessionService sessionService;
        private SessionTestHelper sessionTestHelper;
        private ISession httpContextSession;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            sessionService = new SessionService(connection);
            sessionTestHelper = new SessionTestHelper(connection);
            httpContextSession = new MockHttpContextSession();
        }

        [Test]
        public void StartOrUpdateSession_Should_Start_Users_First_Session()
        {
            using (new TransactionScope())
            {
                // Given
                httpContextSession.Clear();

                // When
                const int candidateId = 29;
                const int customisationId = 100;
                sessionService.StartOrUpdateSession(candidateId, customisationId, httpContextSession);

                // Then
                httpContextSession.Keys.Should().BeEquivalentTo($"SessionID-{customisationId}");

                var sessionId = httpContextSession.GetInt32($"SessionID-{customisationId}");
                sessionId.Should().NotBeNull();

                var newSession = sessionTestHelper.GetSession(sessionId.Value);
                newSession.Should().NotBeNull();
                SessionTestHelper.SessionsShouldBeApproximatelyEquivalent(
                    newSession!,
                    SessionTestHelper.CreateDefaultSession(sessionId.Value, candidateId, customisationId)
                );
            }
        }

        [Test]
        public void StartOrUpdateSession_Should_Start_Subsequent_Session()
        {
            using (new TransactionScope())
            {
                // Given
                httpContextSession.Clear();

                // When
                const int candidateId = 101;
                const int customisationId = 1240;
                sessionService.StartOrUpdateSession(candidateId, customisationId, httpContextSession);

                // Then
                httpContextSession.Keys.Should().BeEquivalentTo($"SessionID-{customisationId}");

                var sessionId = httpContextSession.GetInt32($"SessionID-{customisationId}");
                sessionId.Should().NotBeNull();

                var newSession = sessionTestHelper.GetSession(sessionId.Value);

                newSession.Should().NotBeNull();
                SessionTestHelper.SessionsShouldBeApproximatelyEquivalent(
                    newSession!,
                    SessionTestHelper.CreateDefaultSession(sessionId.Value)
                );
            }
        }

        [Test]
        public void StartOrUpdateSession_Should_Stop_Other_Sessions()
        {
            using (new TransactionScope())
            {
                // Given
                httpContextSession.Clear();
                const int customisationId = 1240;
                httpContextSession.SetInt32($"SessionID-{customisationId + 1}", 10);

                // When
                const int candidateId = 101;
                sessionService.StartOrUpdateSession(candidateId, customisationId, httpContextSession);

                // Then
                httpContextSession.Keys.Should().BeEquivalentTo($"SessionID-{customisationId}");

                var sessionId = httpContextSession.GetInt32($"SessionID-{customisationId}");
                sessionId.Should().NotBeNull();

                var sessions = sessionTestHelper.GetCandidateSessions(candidateId);
                var activeSessions = sessions.Where(session => session.Active).ToList();

                activeSessions.Should().HaveCount(1);
                activeSessions.First().SessionId.Should().Be(sessionId);
            }
        }

        [Test]
        public void StopSession_Should_Stop_Candidates_Sessions()
        {
            using (new TransactionScope())
            {
                // Given
                httpContextSession.Clear();
                httpContextSession.SetInt32("SessionID-1240", 10);

                // When
                const int candidateId = 101;
                sessionService.StopSession(candidateId, httpContextSession);

                // Then
                httpContextSession.Keys.Should().BeEmpty();

                var sessions = sessionTestHelper.GetCandidateSessions(candidateId);
                var activeSessions = sessions.Where(session => session.Active);

                activeSessions.Should().BeEmpty();
            }
        }

        [Test]
        public void StartOrUpdateSession_Should_Only_Update_Active_Session()
        {
            const int twoMinutesInMilliseconds = 120 * 1000;

            using (new TransactionScope())
            {
                // Given
                const int customisationId = 325;
                const int sessionId = 473;
                const int candidateId = 9;

                httpContextSession.Clear();
                httpContextSession.SetInt32($"SessionID-{customisationId}", sessionId);
                var startingSessions = sessionTestHelper.GetCandidateSessions(candidateId);

                // When
                sessionService.StartOrUpdateSession(candidateId, customisationId, httpContextSession);

                // Then
                httpContextSession.Keys.Should().BeEquivalentTo($"SessionID-{customisationId}");
                httpContextSession.GetInt32($"SessionID-{customisationId}").Should().Be(sessionId);

                var updatedSessions = sessionTestHelper.GetCandidateSessions(candidateId).ToList();

                updatedSessions
                    .Where(session => session.SessionId != sessionId)
                    .Should()
                    .BeEquivalentTo(startingSessions.Where(session => session.SessionId != sessionId));

                var activeSession = updatedSessions.First(session => session.SessionId == sessionId);
                activeSession.LoginTime.AddMinutes(activeSession.Duration)
                    .Should().BeCloseTo(DateTime.Now, twoMinutesInMilliseconds);
            }
        }

        [Test]
        public void StartOrUpdateSession_Should_Not_Update_Inactive_Session()
        {
            using (new TransactionScope())
            {
                // Given
                const int customisationId = 325;
                const int sessionId = 468;
                const int candidateId = 9;

                httpContextSession.Clear();
                httpContextSession.SetInt32($"SessionID-{customisationId}", sessionId);
                var startingSessions = sessionTestHelper.GetCandidateSessions(candidateId);

                // When
                sessionService.StartOrUpdateSession(candidateId, customisationId, httpContextSession);

                // Then
                var updatedSessions = sessionTestHelper.GetCandidateSessions(candidateId);

                updatedSessions.Should().BeEquivalentTo(startingSessions);
            }
        }
    }
}
