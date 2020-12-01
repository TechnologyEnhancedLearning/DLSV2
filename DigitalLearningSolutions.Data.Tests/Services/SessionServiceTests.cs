namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    internal class SessionServiceTests
    {
        private SessionService sessionService;
        private SessionTestHelper sessionTestHelper;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            sessionService = new SessionService(connection);
            sessionTestHelper = new SessionTestHelper(connection);
        }

        [Test]
        public void StartOrRestartSession_Should_Start_Users_First_Session()
        {
            using (new TransactionScope())
            {
                // When
                const int candidateId = 29;
                const int customisationId = 100;
                var sessionId = sessionService.StartOrRestartSession(candidateId, customisationId);

                // Then
                var newSession = sessionTestHelper.GetSession(sessionId);

                newSession.Should().NotBeNull();
                SessionTestHelper.SessionsShouldBeApproximatelyEquivalent(
                    newSession!,
                    new Session(
                        sessionId,
                        candidateId,
                        customisationId,
                        DateTime.Now,
                        0,
                        true
                    )
                );
            }
        }

        [Test]
        public void StartOrRestartSession_Should_Start_Subsequent_Session()
        {
            using (new TransactionScope())
            {
                // When
                const int candidateId = 101;
                const int customisationId = 1240;
                var sessionId = sessionService.StartOrRestartSession(candidateId, customisationId);

                // Then
                var newSession = sessionTestHelper.GetSession(sessionId);

                newSession.Should().NotBeNull();
                SessionTestHelper.SessionsShouldBeApproximatelyEquivalent(
                    newSession!,
                    new Session(
                        sessionId,
                        candidateId,
                        customisationId,
                        DateTime.Now,
                        0,
                        true
                    )
                );
            }
        }

        [Test]
        public void StartOrRestartSession_Should_Stop_Other_Sessions()
        {
            using (new TransactionScope())
            {
                // When
                const int candidateId = 101;
                const int customisationId = 1240;
                var sessionId = sessionService.StartOrRestartSession(candidateId, customisationId);

                // Then
                var sessions = sessionTestHelper.GetCandidateSessions(candidateId);
                var activeSessions = sessions.Where(session => session.Active);

                activeSessions.Should().HaveCount(1);
                activeSessions.First().SessionId.Should().Be(sessionId);
            }
        }

        [Test]
        public void StopSession_Should_Stop_Candidates_Sessions()
        {
            using (new TransactionScope())
            {
                // When
                const int candidateId = 101;
                sessionService.StopSession(candidateId);

                // Then
                var sessions = sessionTestHelper.GetCandidateSessions(candidateId);
                var activeSessions = sessions.Where(session => session.Active);

                activeSessions.Should().BeEmpty();
            }
        }

        [Test]
        public void UpdateSessionDuration_Should_Only_Update_Given_Active_Sessions()
        {
            const int twoMinutesInMilliseconds = 120 * 1000;

            using (new TransactionScope())
            {
                // When
                const int candidateId = 9;
                const int sessionId = 473;
                var startingSessions = sessionTestHelper.GetCandidateSessions(candidateId);

                sessionService.UpdateSessionDuration(sessionId);

                // Then
                var newSessions = sessionTestHelper.GetCandidateSessions(candidateId).ToList();

                newSessions
                    .Where(session => session.SessionId != sessionId)
                    .Should()
                    .BeEquivalentTo(startingSessions.Where(session => session.SessionId != sessionId));

                var newSession = newSessions.First(session => session.SessionId == sessionId);
                DateTime.Now.Should().BeCloseTo(
                    newSession.LoginTime.AddMinutes(newSession.Duration),
                    twoMinutesInMilliseconds
                );
            }
        }

        [Test]
        public void UpdateSessionDuration_Should_Not_Update_Inactive_Session()
        {
            using (new TransactionScope())
            {
                // When
                const int candidateId = 9;
                const int sessionId = 468;
                var startingSessions = sessionTestHelper.GetCandidateSessions(candidateId);

                sessionService.UpdateSessionDuration(sessionId);

                // Then
                var newSessions = sessionTestHelper.GetCandidateSessions(candidateId);

                newSessions.Should().BeEquivalentTo(startingSessions);
            }
        }
    }
}
