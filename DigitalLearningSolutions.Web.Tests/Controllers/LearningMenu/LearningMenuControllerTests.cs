namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningMenu
{
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.LearningMenuController;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class LearningMenuControllerTests
    {
        private LearningMenuController controller;
        private ICourseContentService courseContentService;
        private ISessionService sessionService;
        private ISession httpContextSession;
        private IConfiguration config;
        private const int CandidateId = 11;
        private const int CentreId = 2;
        private const int CustomisationId = 12;
        private const int DefaultSessionId = 13;

        [SetUp]
        public void SetUp()
        {
            var logger = A.Fake<ILogger<LearningMenuController>>();
            config = A.Fake<IConfiguration>();
            courseContentService = A.Fake<ICourseContentService>();
            sessionService = A.Fake<ISessionService>();
            A.CallTo(() => sessionService.StartOrRestartSession(A<int>._, A<int>._)).Returns(DefaultSessionId);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("learnCandidateID", CandidateId.ToString()),
                new Claim("UserCentreID", CentreId.ToString())
            }, "mock"));
            httpContextSession = new MockHttpContextSession();

            controller = new LearningMenuController(logger, config, courseContentService, sessionService)
            {
                ControllerContext = new ControllerContext {
                    HttpContext = new DefaultHttpContext
                    {
                        User = user,
                        Session = httpContextSession
                    }
                }
            };
        }

        [Test]
        public void Index_should_render_view()
        {
            // Given
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(CustomisationId);
            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, CustomisationId))
             .Returns(expectedCourseContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(10);

            // When
            var result = controller.Index(CustomisationId);

            // Then
            var expectedModel = new InitialMenuViewModel(expectedCourseContent);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Index_should_return_404_if_unknown_course()
        {
            // Given
            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, CustomisationId)).Returns(null);

            // When
            var result = controller.Index(CustomisationId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }

        [Test]
        public void ContentViewer_should_render_view()
        {
            // When
            var result = controller.ContentViewer();

            // Then
            var expectedModel = new ContentViewerViewModel(config);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Index_always_calls_get_course_content()
        {
            // Given
            const int customisationId = 1;

            // When
            controller.Index(1);

            // Then
            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, customisationId)).MustHaveHappened();
        }

        [Test]
        public void Index_valid_customisation_id_should_update_login_and_duration()
        {
            // Given
            const int progressId = 13;
            var defaultCourseContent = CourseContentHelper.CreateDefaultCourseContent(CustomisationId);
            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, CustomisationId))
             .Returns(defaultCourseContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(progressId);

            // When
            controller.Index(CustomisationId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(progressId)).MustHaveHappened();
        }

        [Test]
        public void Index_invalid_customisation_id_should_not_insert_new_progress()
        {
            // Given
            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, CustomisationId)).Returns(null);

            // When
            controller.Index(CustomisationId);

            // Then
            A.CallTo(() => courseContentService.GetOrCreateProgressId(A<int>._, A<int>._, A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void Index_invalid_customisation_id_should_not_update_login_and_duration()
        {
            // Given
            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, CustomisationId)).Returns(null);

            // When
            controller.Index(CustomisationId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void Index_failing_to_insert_progress_should_not_update_progress()
        {
            // Given
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(null);

            // When
            controller.Index(CustomisationId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void Index_should_StartOrRestartSession_for_course_not_in_session()
        {
            // Given
            httpContextSession.Clear();
            const int newCourseId = CustomisationId;  // Not in session
            const int oldCourseInSession = CustomisationId + 1;
            httpContextSession.SetInt32($"SessionID-{oldCourseInSession}", DefaultSessionId + 1);

            // When
            controller.Index(newCourseId);

            // Then
            A.CallTo(() => sessionService.StartOrRestartSession(CandidateId, newCourseId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionService.StartOrRestartSession(A<int>._, A<int>._))
                .WhenArgumentsMatch((int candidateId, int customisationId) =>
                    candidateId != CandidateId || customisationId != newCourseId)
                .MustNotHaveHappened();

            A.CallTo(() => sessionService.UpdateSessionDuration(A<int>._)).MustNotHaveHappened();

            httpContextSession.Keys.Should().BeEquivalentTo($"SessionID-{newCourseId}");
            httpContextSession.GetInt32($"SessionID-{newCourseId}").Should().Be(DefaultSessionId);
        }

        [Test]
        public void Index_should_UpdateSession_for_course_in_session()
        {
            // Given
            httpContextSession.Clear();
            const int courseInSession = CustomisationId;
            httpContextSession.SetInt32($"SessionID-{courseInSession}", DefaultSessionId);

            // When
            controller.Index(courseInSession);

            // Then
            A.CallTo(() => sessionService.UpdateSessionDuration(DefaultSessionId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionService.UpdateSessionDuration(A<int>._))
                .WhenArgumentsMatch((int sessionId) => sessionId != DefaultSessionId)
                .MustNotHaveHappened();

            A.CallTo(() => sessionService.StartOrRestartSession(A<int>._, A<int>._)).MustNotHaveHappened();

            httpContextSession.Keys.Should().BeEquivalentTo($"SessionID-{courseInSession}");
            httpContextSession.GetInt32($"SessionID-{courseInSession}").Should().Be(DefaultSessionId);
        }

        [Test]
        public void Close_should_close_sessions()
        {
            // Given
            httpContextSession.Clear();
            const int courseInSession = CustomisationId;
            httpContextSession.SetInt32($"SessionID-{courseInSession}", DefaultSessionId);

            // When
            controller.Close();

            // Then
            A.CallTo(() => sessionService.StopSession(CandidateId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionService.StopSession(A<int>._))
                .WhenArgumentsMatch((int candidateId) => candidateId != CandidateId)
                .MustNotHaveHappened();

            httpContextSession.Keys.Should().BeEmpty();
        }

        [Test]
        public void Close_should_redirect_to_Current_LearningPortal()
        {
            // When
            var result = controller.Close();

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningPortal")
                .WithActionName("Current");
        }
    }
}
