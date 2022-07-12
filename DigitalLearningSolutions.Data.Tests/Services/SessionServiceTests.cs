namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    public class SessionServiceTests
    {
        private const int CandidateId = 11;
        private const int CustomisationId = 12;
        private const int DefaultSessionId = 13;
        private IClockService clockService = null!;
        private ISession httpContextSession = null!;
        private ISessionDataService sessionDataService = null!;
        private ISessionService sessionService = null!;

        [SetUp]
        public void SetUp()
        {
            clockService = A.Fake<IClockService>();
            sessionDataService = A.Fake<ISessionDataService>();
            A.CallTo(() => sessionDataService.StartOrRestartDelegateSession(A<int>._, A<int>._))
                .Returns(DefaultSessionId);

            httpContextSession = new MockHttpContextSession();

            sessionService = new SessionService(clockService, sessionDataService);
        }

        [Test]
        public void StartOrUpdateDelegateSession_should_StartOrRestartDelegateSession_for_course_not_in_session()
        {
            // Given
            httpContextSession.Clear();
            const int newCourseId = CustomisationId; // Not in session
            const int oldCourseInSession = CustomisationId + 1;
            httpContextSession.SetInt32($"SessionID-{oldCourseInSession}", DefaultSessionId + 1);

            // When
            sessionService.StartOrUpdateDelegateSession(CandidateId, newCourseId, httpContextSession);

            // Then
            A.CallTo(() => sessionDataService.StartOrRestartDelegateSession(CandidateId, newCourseId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionDataService.StartOrRestartDelegateSession(A<int>._, A<int>._))
                .WhenArgumentsMatch(
                    (int candidateId, int customisationId) =>
                        candidateId != CandidateId || customisationId != newCourseId
                )
                .MustNotHaveHappened();

            A.CallTo(() => sessionDataService.UpdateDelegateSessionDuration(A<int>._, A<DateTime>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void StartOrUpdateDelegateSession_should_add_session_to_context_for_course_not_in_session()
        {
            // Given
            httpContextSession.Clear();
            const int newCourseId = CustomisationId; // Not in session
            const int oldCourseInSession = CustomisationId + 1;
            httpContextSession.SetInt32($"SessionID-{oldCourseInSession}", DefaultSessionId + 1);

            // When
            sessionService.StartOrUpdateDelegateSession(CandidateId, newCourseId, httpContextSession);

            // Then
            httpContextSession.Keys.Should().BeEquivalentTo($"SessionID-{newCourseId}");
            httpContextSession.GetInt32($"SessionID-{newCourseId}").Should().Be(DefaultSessionId);
        }

        [Test]
        public void StartOrUpdateDelegateSession_should_UpdateDelegateSession_for_course_in_session()
        {
            // Given
            httpContextSession.Clear();
            const int courseInSession = CustomisationId;
            httpContextSession.SetInt32($"SessionID-{courseInSession}", DefaultSessionId);
            var currentUtcTime = new DateTime(2022, 06, 14, 12, 01, 01);
            A.CallTo(() => clockService.UtcNow).Returns(currentUtcTime);

            // When
            sessionService.StartOrUpdateDelegateSession(CandidateId, courseInSession, httpContextSession);

            // Then
            A.CallTo(() => sessionDataService.UpdateDelegateSessionDuration(DefaultSessionId, currentUtcTime))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionDataService.UpdateDelegateSessionDuration(A<int>._, A<DateTime>._))
                .WhenArgumentsMatch((int sessionId, DateTime currentTime) => sessionId != DefaultSessionId)
                .MustNotHaveHappened();

            A.CallTo(() => sessionDataService.StartOrRestartDelegateSession(A<int>._, A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void StartOrUpdateDelegateSession_should_not_modify_context_for_course_in_session()
        {
            // Given
            httpContextSession.Clear();
            const int courseInSession = CustomisationId;
            httpContextSession.SetInt32($"SessionID-{courseInSession}", DefaultSessionId);

            // When
            sessionService.StartOrUpdateDelegateSession(CandidateId, courseInSession, httpContextSession);

            // Then
            httpContextSession.Keys.Should().BeEquivalentTo($"SessionID-{courseInSession}");
            httpContextSession.GetInt32($"SessionID-{courseInSession}").Should().Be(DefaultSessionId);
        }

        [Test]
        public void StopDelegateSession_should_close_sessions()
        {
            // Given
            httpContextSession.Clear();
            const int courseInSession = CustomisationId;
            httpContextSession.SetInt32($"SessionID-{courseInSession}", DefaultSessionId);

            // When
            sessionService.StopDelegateSession(CandidateId, httpContextSession);

            // Then
            A.CallTo(() => sessionDataService.StopDelegateSession(CandidateId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionDataService.StopDelegateSession(A<int>._))
                .WhenArgumentsMatch((int candidateId) => candidateId != CandidateId)
                .MustNotHaveHappened();
        }

        [Test]
        public void StopDelegateSession_should_clear_context()
        {
            // Given
            httpContextSession.Clear();
            const int courseInSession = CustomisationId;
            httpContextSession.SetInt32($"SessionID-{courseInSession}", DefaultSessionId);

            // When
            sessionService.StopDelegateSession(CandidateId, httpContextSession);

            // Then
            httpContextSession.Keys.Should().BeEmpty();
        }

        [Test]
        public void StartAdminSession_should_call_data_service()
        {
            // Given
            A.CallTo(() => sessionDataService.StartAdminSession(A<int>._)).Returns(1);

            // When
            sessionService.StartAdminSession(1);

            // Then
            A.CallTo(() => sessionDataService.StartAdminSession(1)).MustHaveHappenedOnceExactly();
        }
    }
}
