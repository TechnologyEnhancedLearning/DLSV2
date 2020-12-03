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
            A.CallTo(() => sessionDataService.StartOrRestartSession(A<int>._, A<int>._)).Returns(DefaultSessionId);

            httpContextSession = new MockHttpContextSession();

            sessionService = new SessionService(sessionDataService);
        }

        [Test]
        public void StartOrUpdateSession_should_StartOrRestartSession_for_course_not_in_session()
        {
            // Given
            httpContextSession.Clear();
            const int newCourseId = CustomisationId;  // Not in session
            const int oldCourseInSession = CustomisationId + 1;
            httpContextSession.SetInt32($"SessionID-{oldCourseInSession}", DefaultSessionId + 1);

            // When
            sessionService.StartOrUpdateSession(CandidateId, newCourseId, httpContextSession);

            // Then
            A.CallTo(() => sessionDataService.StartOrRestartSession(CandidateId, newCourseId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionDataService.StartOrRestartSession(A<int>._, A<int>._))
                .WhenArgumentsMatch((int candidateId, int customisationId) =>
                    candidateId != CandidateId || customisationId != newCourseId)
                .MustNotHaveHappened();

            A.CallTo(() => sessionDataService.UpdateSessionDuration(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void StartOrUpdateSession_should_add_session_to_context_for_course_not_in_session()
        {
            // Given
            httpContextSession.Clear();
            const int newCourseId = CustomisationId;  // Not in session
            const int oldCourseInSession = CustomisationId + 1;
            httpContextSession.SetInt32($"SessionID-{oldCourseInSession}", DefaultSessionId + 1);

            // When
            sessionService.StartOrUpdateSession(CandidateId, newCourseId, httpContextSession);

            // Then
            httpContextSession.Keys.Should().BeEquivalentTo($"SessionID-{newCourseId}");
            httpContextSession.GetInt32($"SessionID-{newCourseId}").Should().Be(DefaultSessionId);
        }

        [Test]
        public void StartOrUpdateSession_should_UpdateSession_for_course_in_session()
        {
            // Given
            httpContextSession.Clear();
            const int courseInSession = CustomisationId;
            httpContextSession.SetInt32($"SessionID-{courseInSession}", DefaultSessionId);

            // When
            sessionService.StartOrUpdateSession(CandidateId, courseInSession, httpContextSession);

            // Then
            A.CallTo(() => sessionDataService.UpdateSessionDuration(DefaultSessionId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionDataService.UpdateSessionDuration(A<int>._))
                .WhenArgumentsMatch((int sessionId) => sessionId != DefaultSessionId)
                .MustNotHaveHappened();

            A.CallTo(() => sessionDataService.StartOrRestartSession(A<int>._, A<int>._)).MustNotHaveHappened();

            httpContextSession.Keys.Should().BeEquivalentTo($"SessionID-{courseInSession}");
            httpContextSession.GetInt32($"SessionID-{courseInSession}").Should().Be(DefaultSessionId);
        }

        [Test]
        public void StartOrUpdateSession_should_not_modify_context_for_course_in_session()
        {
            // Given
            httpContextSession.Clear();
            const int courseInSession = CustomisationId;
            httpContextSession.SetInt32($"SessionID-{courseInSession}", DefaultSessionId);

            // When
            sessionService.StartOrUpdateSession(CandidateId, courseInSession, httpContextSession);

            // Then
            httpContextSession.Keys.Should().BeEquivalentTo($"SessionID-{courseInSession}");
            httpContextSession.GetInt32($"SessionID-{courseInSession}").Should().Be(DefaultSessionId);
        }

        [Test]
        public void StopSession_should_close_sessions()
        {
            // Given
            httpContextSession.Clear();
            const int courseInSession = CustomisationId;
            httpContextSession.SetInt32($"SessionID-{courseInSession}", DefaultSessionId);

            // When
            sessionService.StopSession(CandidateId, httpContextSession);

            // Then
            A.CallTo(() => sessionDataService.StopSession(CandidateId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionDataService.StopSession(A<int>._))
                .WhenArgumentsMatch((int candidateId) => candidateId != CandidateId)
                .MustNotHaveHappened();
        }

        [Test]
        public void StopSession_should_clear_context()
        {
            // Given
            httpContextSession.Clear();
            const int courseInSession = CustomisationId;
            httpContextSession.SetInt32($"SessionID-{courseInSession}", DefaultSessionId);

            // When
            sessionService.StopSession(CandidateId, httpContextSession);

            // Then
            httpContextSession.Keys.Should().BeEmpty();
        }
    }
}
