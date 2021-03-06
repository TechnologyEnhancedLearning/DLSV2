﻿namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    internal class SessionDataServiceTests
    {
        private const int twoMinutesInMilliseconds = 120 * 1000;
        private SessionDataService sessionDataService;
        private SessionTestHelper sessionTestHelper;

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
                var activeSessions = sessions.Where(session => session.Active);

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
                sessionDataService.UpdateDelegateSessionDuration(sessionId);

                // Then
                var updatedSessions = sessionTestHelper.GetCandidateSessions(candidateId).ToList();

                updatedSessions
                    .Where(session => session.SessionId != sessionId)
                    .Should()
                    .BeEquivalentTo(startingSessions.Where(session => session.SessionId != sessionId));

                var activeSession = updatedSessions.First(session => session.SessionId == sessionId);
                activeSession.LoginTime.AddMinutes(activeSession.Duration)
                    .Should().BeCloseTo(DateTime.UtcNow, twoMinutesInMilliseconds);
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
                sessionDataService.UpdateDelegateSessionDuration(sessionId);

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
                newAdminSession.AdminId.Should().Be(7);
                newAdminSession.LoginTime.Should().BeCloseTo(DateTime.UtcNow, twoMinutesInMilliseconds);
            }
        }
    }
}
