namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    public class SessionServiceTests
    {
        private ISessionService sessionService;
        private ISessionDataService sessionDataService;
        private ISession httpContextSession;

        private const int CandidateId = 11;
        private const int CustomisationId = 12;
        private const int DefaultSessionId = 13;

        [SetUp]
        public void SetUp()
        {
            sessionDataService = A.Fake<ISessionDataService>();
            A.CallTo(() => sessionDataService.StartOrRestartDelegateSession(A<int>._, A<int>._)).Returns(DefaultSessionId);

            httpContextSession = new MockHttpContextSession();

            sessionService = new SessionService(sessionDataService);
        }

        [Test]
        public void StartOrUpdateDelegateSession_should_StartOrRestartDelegateSession_for_course_not_in_session()
        {
            // Given
            httpContextSession.Clear();
            const int newCourseId = CustomisationId;  // Not in session
            const int oldCourseInSession = CustomisationId + 1;
            httpContextSession.SetInt32($"SessionID-{oldCourseInSession}", DefaultSessionId + 1);

            // When
            sessionService.StartOrUpdateDelegateSession(CandidateId, newCourseId, httpContextSession);

            // Then
            A.CallTo(() => sessionDataService.StartOrRestartDelegateSession(CandidateId, newCourseId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionDataService.StartOrRestartDelegateSession(A<int>._, A<int>._))
                .WhenArgumentsMatch((int candidateId, int customisationId) =>
                    candidateId != CandidateId || customisationId != newCourseId)
                .MustNotHaveHappened();

            A.CallTo(() => sessionDataService.UpdateDelegateSessionDuration(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void StartOrUpdateDelegateSession_should_add_session_to_context_for_course_not_in_session()
        {
            // Given
            httpContextSession.Clear();
            const int newCourseId = CustomisationId;  // Not in session
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

            // When
            sessionService.StartOrUpdateDelegateSession(CandidateId, courseInSession, httpContextSession);

            // Then
            A.CallTo(() => sessionDataService.UpdateDelegateSessionDuration(DefaultSessionId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionDataService.UpdateDelegateSessionDuration(A<int>._))
                .WhenArgumentsMatch((int sessionId) => sessionId != DefaultSessionId)
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
    }
}
