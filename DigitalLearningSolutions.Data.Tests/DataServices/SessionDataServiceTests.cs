namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    internal class SessionDataServiceTests
    {
        private const int TwoMinutesInMilliseconds = 120 * 1000;
        private readonly DateTime currentTime = new DateTime(2022, 06, 14, 11, 12, 13, 14);
        private SessionDataService sessionDataService = null!;
        private SessionTestHelper sessionTestHelper = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            sessionDataService = new SessionDataService(connection);
            sessionTestHelper = new SessionTestHelper(connection);
        }

        [Test]
        public void StartOrRestartDelegateSession_Should_Start_Users_First_Session()
        {
            using (new TransactionScope())
            {
                // When
                const int candidateId = 29;
                const int customisationId = 100;
                var sessionId = sessionDataService.StartOrRestartDelegateSession(candidateId, customisationId);

                // Then
                var newSession = sessionTestHelper.GetSession(sessionId);

                newSession.Should().NotBeNull();
                SessionTestHelper.SessionsShouldBeApproximatelyEquivalent(
                    newSession!,
                    SessionTestHelper.CreateDefaultSession(sessionId, candidateId, customisationId)
                );
            }
        }

        [Test]
        public void StartOrRestartDelegateSession_Should_Start_Subsequent_Session()
        {
            using (new TransactionScope())
            {
                // When
                const int candidateId = 101;
                const int customisationId = 1240;
                var sessionId = sessionDataService.StartOrRestartDelegateSession(candidateId, customisationId);

                // Then
                var newSession = sessionTestHelper.GetSession(sessionId);

                newSession.Should().NotBeNull();
                SessionTestHelper.SessionsShouldBeApproximatelyEquivalent(
                    newSession!,
                    SessionTestHelper.CreateDefaultSession(sessionId)
                );
            }
        }

        [Test]
        public void StartOrRestartDelegateSession_Should_Stop_Other_Sessions()
        {
            using (new TransactionScope())
            {
                // When
                const int candidateId = 101;
                const int customisationId = 1240;
                var sessionId = sessionDataService.StartOrRestartDelegateSession(candidateId, customisationId);

                // Then
                var sessions = sessionTestHelper.GetCandidateSessions(candidateId);
                var activeSessions = sessions.Where(session => session.Active).ToList();

                activeSessions.Should().HaveCount(1);
                activeSessions.First().SessionId.Should().Be(sessionId);
            }
        }

        [Test]
        public void StopDelegateSession_Should_Stop_Candidates_Sessions()
        {
            using (new TransactionScope())
            {
                // When
                const int candidateId = 101;
                sessionDataService.StopDelegateSession(candidateId);

                // Then
                var sessions = sessionTestHelper.GetCandidateSessions(candidateId);
                var activeSessions = sessions.Where(session => session.Active);

                activeSessions.Should().BeEmpty();
            }
        }

        [Test]
        public void UpdateDelegateSessionDuration_Should_Only_Update_Given_Active_Sessions()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 9;
                var startingSessions = sessionTestHelper.GetCandidateSessions(candidateId);

                // When
                const int sessionId = 473;
                sessionDataService.UpdateDelegateSessionDuration(sessionId, currentTime);

                // Then
                var updatedSessions = sessionTestHelper.GetCandidateSessions(candidateId).ToList();

                updatedSessions
                    .Where(session => session.SessionId != sessionId)
                    .Should()
                    .BeEquivalentTo(startingSessions.Where(session => session.SessionId != sessionId));

                var activeSession = updatedSessions.First(session => session.SessionId == sessionId);
                activeSession.LoginTime.AddMinutes(activeSession.Duration)
                    .Should().BeCloseTo(currentTime, TwoMinutesInMilliseconds);
            }
        }

        [Test]
        public void UpdateDelegateSessionDuration_Should_Not_Update_Inactive_Session()
        {
            using (new TransactionScope())
            {
                // Given
                const int candidateId = 9;
                var startingSessions = sessionTestHelper.GetCandidateSessions(candidateId);

                // When
                const int sessionId = 468;
                sessionDataService.UpdateDelegateSessionDuration(sessionId, currentTime);

                // Then
                var updatedSessions = sessionTestHelper.GetCandidateSessions(candidateId);

                updatedSessions.Should().BeEquivalentTo(startingSessions);
            }
        }

        [Test]
        public void StartAdminSession_Should_Start_Users_New_Admin_Session()
        {
            using (new TransactionScope())
            {
                // Given
                var sessionId = sessionDataService.StartAdminSession(7);

                // When
                var newAdminSession = sessionTestHelper.GetAdminSession(sessionId);

                // Then
                newAdminSession.Should().NotBe(null);
                newAdminSession!.AdminId.Should().Be(7);
                newAdminSession.LoginTime.Should().BeCloseTo(DateTime.UtcNow, TwoMinutesInMilliseconds);
            }
        }

        [Test]
        public void HasAdminGotSessions_returns_true_when_admin_has_sessions()
        {
            // When
            var result = sessionDataService.HasAdminGotSessions(1);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void HasAdminGotSessions_returns_false_when_admin_does_not_have_sessions()
        {
            // Given
            const int fakeVeryLargeAdminId = 123123123;

            // When
            var result = sessionDataService.HasAdminGotSessions(fakeVeryLargeAdminId);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void GetSessionBySessionId_gets_session_correctly()
        {
            // Given
            const int sessionId = 1;

            // When
            var result = sessionDataService.GetSessionById(sessionId);

            // Then
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var expectedLoginTime = new DateTime(2010, 9, 22, 6, 52, 9, 540);
                result!.SessionId.Should().Be(1);
                result.CandidateId.Should().Be(1);
                result.CustomisationId.Should().Be(100);
                result.LoginTime.Should().Be(expectedLoginTime);
                result.Duration.Should().Be(51);
                result.Active.Should().BeFalse();
            }
        }

        [Test]
        public void AddTutorialTimeToSessionDuration_should_add_input_time_correctly_to_correct_record()
        {
            using var transaction = new TransactionScope();

            // Given
            const int sessionId = 1;

            // When
            sessionDataService.AddTutorialTimeToSessionDuration(1, 12);

            // Then
            var result = sessionDataService.GetSessionById(sessionId);
            result!.Duration.Should().Be(63);
        }
    }
}
