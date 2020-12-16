namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningMenu
{
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Web.Controllers.LearningMenuController;
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
        private ITutorialContentService tutorialContentService;
        private ISessionService sessionService;
        private ISectionContentService sectionContentService;
        private ISession httpContextSession;
        private IConfiguration config;
        private const int CandidateId = 11;
        private const int CentreId = 2;
        private const int CustomisationId = 12;
        private const int SectionId = 199;
        private const int TutorialId = 842;

        [SetUp]
        public void SetUp()
        {
            var logger = A.Fake<ILogger<LearningMenuController>>();
            config = A.Fake<IConfiguration>();
            courseContentService = A.Fake<ICourseContentService>();
            tutorialContentService = A.Fake<ITutorialContentService>();
            sessionService = A.Fake<ISessionService>();
            sectionContentService = A.Fake<ISectionContentService>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("learnCandidateID", CandidateId.ToString()),
                new Claim("UserCentreID", CentreId.ToString())
            }, "mock"));
            httpContextSession = new MockHttpContextSession();

            controller = new LearningMenuController(
                logger,
                config,
                courseContentService,
                sectionContentService,
                tutorialContentService,
                sessionService)
            {
                ControllerContext = new ControllerContext
                {
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
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(3);

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
        public void Index_should_return_404_if_unable_to_enrol()
        {
            // Given
            var defaultCourseContent = CourseContentHelper.CreateDefaultCourseContent(CustomisationId);
            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, CustomisationId))
                .Returns(defaultCourseContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

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
        public void Index_valid_customisationId_should_StartOrUpdate_course_sessions()
        {
            // Given
            var defaultCourseContent = CourseContentHelper.CreateDefaultCourseContent(CustomisationId);
            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, CustomisationId))
                .Returns(defaultCourseContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(1);

            // When
            controller.Index(CustomisationId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(CandidateId, CustomisationId, httpContextSession)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._))
                .WhenArgumentsMatch((int candidateId, int customisationId, ISession session) =>
                    candidateId != CandidateId || customisationId != CustomisationId)
                .MustNotHaveHappened();
        }

        [Test]
        public void Index_invalid_customisationId_should_not_StartOrUpdate_course_sessions()
        {
            // Given
            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, CustomisationId)).Returns(null);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(1);

            // When
            controller.Index(CustomisationId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
        }

        [Test]
        public void Index_unable_to_enrol_should_not_StartOrUpdate_course_sessions()
        {
            // Given
            var defaultCourseContent = CourseContentHelper.CreateDefaultCourseContent(CustomisationId);
            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, CustomisationId)).Returns(defaultCourseContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(null);

            // When
            controller.Index(CustomisationId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
        }

        [Test]
        public void Sections_should_StartOrUpdate_course_sessions_if_valid_section()
        {
            // Given
            const int progressId = 299;
            var defaultSectionContent = SectionContentHelper.CreateDefaultSectionContent();
            A.CallTo(() => sectionContentService.GetSectionContent(CustomisationId, CandidateId, SectionId))
                .Returns(defaultSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.Section(CustomisationId, SectionId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(CandidateId, CustomisationId, httpContextSession)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._))
                .WhenArgumentsMatch((int candidateId, int customisationId, ISession session) =>
                    candidateId != CandidateId || customisationId != CustomisationId)
                .MustNotHaveHappened();
        }

        [Test]
        public void Sections_should_not_StartOrUpdate_course_sessions_if_session_not_found()
        {
            // Given
            A.CallTo(() => sectionContentService.GetSectionContent(CustomisationId, CandidateId, SectionId))
                .Returns(null);

            // When
            controller.Section(CustomisationId, SectionId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
        }

        [Test]
        public void Sections_should_not_StartOrUpdate_course_sessions_if_unable_to_enrol()
        {
            // Given
            var defaultSectionContent = SectionContentHelper.CreateDefaultSectionContent();
            A.CallTo(() => sectionContentService.GetSectionContent(CustomisationId, CandidateId, SectionId))
                .Returns(defaultSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

            // When
            controller.Section(CustomisationId, SectionId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
        }

        [Test]
        public void Sections_should_UpdateProgress_if_valid_section()
        {
            // Given
            const int progressId = 299;
            var defaultSectionContent = SectionContentHelper.CreateDefaultSectionContent();
            A.CallTo(() => sectionContentService.GetSectionContent(CustomisationId, CandidateId, SectionId))
                .Returns(defaultSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.Section(CustomisationId, SectionId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(progressId)).MustHaveHappened();
        }

        [Test]
        public void Sections_should_not_UpdateProgress_if_invalid_section()
        {
            // Given
            A.CallTo(() => sectionContentService.GetSectionContent(CustomisationId, CandidateId, SectionId))
                .Returns(null);

            // When
            controller.Section(CustomisationId, SectionId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void Sections_should_UpdateProgress_if_unable_to_enrol()
        {
            // Given
            var defaultSectionContent = SectionContentHelper.CreateDefaultSectionContent();
            A.CallTo(() => sectionContentService.GetSectionContent(CustomisationId, CandidateId, SectionId))
                .Returns(defaultSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

            // When
            controller.Section(CustomisationId, SectionId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void Sections_should_render_view()
        {
            // Given
            const int progressId = 299;
            var defaultSectionContent = SectionContentHelper.CreateDefaultSectionContent();
            A.CallTo(() => sectionContentService.GetSectionContent(CustomisationId, CandidateId, SectionId))
                .Returns(defaultSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).
                Returns(progressId);

            // When
            var result = controller.Section(CustomisationId, SectionId);

            // Then
            var expectedModel = new SectionContentViewModel(defaultSectionContent, CustomisationId, SectionId);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Sections_should_404_if_section_not_found()
        {
            // Given
            A.CallTo(() => sectionContentService.GetSectionContent(CustomisationId, CandidateId, SectionId))
                .Returns(null);

            // When
            var result = controller.Section(CustomisationId, SectionId);

            // Then
            A.CallTo(() => courseContentService.GetOrCreateProgressId(A<int>._, A<int>._, A<int>._)).MustNotHaveHappened();
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }

        [Test]
        public void Sections_should_404_if_failed_to_enrol()
        {
            // Given
            var defaultSectionContent = SectionContentHelper.CreateDefaultSectionContent();
            A.CallTo(() => sectionContentService.GetSectionContent(CustomisationId, CandidateId, SectionId))
                .Returns(defaultSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(null);

            // When
            var result = controller.Section(CustomisationId, SectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }

        [Test]
        public void Tutorials_should_StartOrUpdate_course_sessions_if_valid_tutorial()
        {
            // Given
            var defaultTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(TutorialId);
            A.CallTo(() => tutorialContentService.GetTutorialInformation(CandidateId, CustomisationId, SectionId, TutorialId))
                .Returns(defaultTutorialInformation);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(1);

            // When
            controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(CandidateId, CustomisationId, httpContextSession)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._))
                .WhenArgumentsMatch((int candidateId, int customisationId, ISession session) =>
                    candidateId != CandidateId || customisationId != CustomisationId)
                .MustNotHaveHappened();
        }

        [Test]
        public void Tutorials_should_not_StartOrUpdate_course_sessions_if_invalid_tutorial()
        {
            // Given
            A.CallTo(() => tutorialContentService.GetTutorialInformation(CandidateId, CustomisationId, SectionId, TutorialId))
                .Returns(null);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(1);

            // When
            controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
        }

        [Test]
        public void Tutorials_should_not_StartOrUpdate_course_sessions_if_unable_to_enrol()
        {
            // Given
            var defaultTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(TutorialId);
            A.CallTo(() => tutorialContentService.GetTutorialInformation(CandidateId, CustomisationId, SectionId, TutorialId))
                .Returns(defaultTutorialInformation);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(null);

            // When
            controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
        }

        [Test]
        public void Tutorials_should_UpdateProgress_if_valid_tutorial()
        {
            // Given
            const int progressId = 3;
            var defaultTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(TutorialId);
            A.CallTo(() => tutorialContentService.GetTutorialInformation(CandidateId, CustomisationId, SectionId, TutorialId))
                .Returns(defaultTutorialInformation);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(progressId);

            // When
            controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(progressId)).MustHaveHappened();
        }

        [Test]
        public void Tutorials_should_not_UpdateProgress_if_invalid_tutorial()
        {
            // Given
            const int progressId = 3;
            A.CallTo(() => tutorialContentService.GetTutorialInformation(CandidateId, CustomisationId, SectionId, TutorialId))
                .Returns(null);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(progressId);

            // When
            controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void Tutorials_should_not_UpdateProgress_if_unable_to_enrol()
        {
            // Given
            var defaultTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(TutorialId);
            A.CallTo(() => tutorialContentService.GetTutorialInformation(CandidateId, CustomisationId, SectionId, TutorialId))
                .Returns(defaultTutorialInformation);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(null);

            // When
            controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void Tutorials_should_render_view()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(TutorialId);
            A.CallTo(() => tutorialContentService.GetTutorialInformation(CandidateId, CustomisationId, SectionId, TutorialId))
                .Returns(expectedTutorialInformation);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(3);

            // When
            var result = controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            var expectedModel = new TutorialViewModel(config, expectedTutorialInformation, CustomisationId, SectionId);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Tutorials_should_return_404_if_invalid_tutorial()
        {
            // Given
            A.CallTo(() => tutorialContentService.GetTutorialInformation(
                CandidateId,
                CustomisationId,
                SectionId,
                TutorialId
            )).Returns(null);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(3);

            // When
            var result = controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }

        [Test]
        public void Tutorials_should_return_404_if_unable_to_enrol()
        {
            // Given
            var defaultTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(TutorialId);
            A.CallTo(() => tutorialContentService.GetTutorialInformation(
                CandidateId,
                CustomisationId,
                SectionId,
                TutorialId
            )).Returns(defaultTutorialInformation);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

            // When
            var result = controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }

        [Test]
        public void ContentViewer_should_StartOrUpdate_session_if_valid_tutorial()
        {
            // Given
            var expectedTutorialContent = TutorialContentHelper.CreateDefaultTutorialContent();
            const int progressId = 3;

            A.CallTo(() => tutorialContentService.GetTutorialContent(CustomisationId, SectionId, TutorialId))
                .Returns(expectedTutorialContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.ContentViewer(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(CandidateId, CustomisationId, httpContextSession))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._))
                .WhenArgumentsMatch((int candidateId, int customisationId, ISession session) =>
                    candidateId != CandidateId || customisationId != CustomisationId)
                .MustNotHaveHappened();
        }

        [Test]
        public void ContentViewer_should_not_StartOrUpdate_session_if_invalid_tutorial()
        {
            // Given
            const int progressId = 3;

            A.CallTo(() => tutorialContentService.GetTutorialContent(CustomisationId, SectionId, TutorialId))
                .Returns(null);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.ContentViewer(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void ContentViewer_should_not_StartOrUpdate_session_if_unable_to_enrol()
        {
            // Given
            var expectedTutorialContent = TutorialContentHelper.CreateDefaultTutorialContent();

            A.CallTo(() => tutorialContentService.GetTutorialContent(CustomisationId, SectionId, TutorialId))
                .Returns(expectedTutorialContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

            // When
            controller.ContentViewer(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void ContentViewer_should_UpdateProgress_if_valid_tutorial()
        {
            // Given
            var expectedTutorialContent = TutorialContentHelper.CreateDefaultTutorialContent();
            const int progressId = 3;

            A.CallTo(() => tutorialContentService.GetTutorialContent(CustomisationId, SectionId, TutorialId))
                .Returns(expectedTutorialContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.ContentViewer(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(progressId)).MustHaveHappened();
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._))
                .WhenArgumentsMatch((int id) => id != progressId)
                .MustNotHaveHappened();
        }

        [Test]
        public void ContentViewer_should_not_UpdateProgress_if_invalid_tutorial()
        {
            // Given
            const int progressId = 3;

            A.CallTo(() => tutorialContentService.GetTutorialContent(CustomisationId, SectionId, TutorialId))
                .Returns(null);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.ContentViewer(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void ContentViewer_should_not_UpdateProgress_if_unable_to_enrol()
        {
            // Given
            var expectedTutorialContent = TutorialContentHelper.CreateDefaultTutorialContent();

            A.CallTo(() => tutorialContentService.GetTutorialContent(CustomisationId, SectionId, TutorialId))
                .Returns(expectedTutorialContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

            // When
            controller.ContentViewer(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void ContentViewer_should_render_view()
        {
            // Given
            var expectedTutorialContent = TutorialContentHelper.CreateDefaultTutorialContent();
            const int progressId = 101;

            A.CallTo(() => tutorialContentService.GetTutorialContent(CustomisationId, SectionId, TutorialId))
                .Returns(expectedTutorialContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            var result = controller.ContentViewer(CustomisationId, SectionId, TutorialId);

            // Then
            var expectedModel = new ContentViewerViewModel(
                config,
                expectedTutorialContent,
                CustomisationId,
                CentreId,
                SectionId,
                TutorialId,
                CandidateId,
                progressId
            );

            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void ContentViewer_should_return_404_if_invalid_tutorial()
        {
            // Given
            const int progressId = 101;

            A.CallTo(() => tutorialContentService.GetTutorialContent(CustomisationId, SectionId, TutorialId))
                .Returns(null);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            var result = controller.ContentViewer(CustomisationId, SectionId, TutorialId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }

        [Test]
        public void ContentViewer_should_return_404_if_unable_to_enrol()
        {
            // Given
            var expectedTutorialContent = TutorialContentHelper.CreateDefaultTutorialContent();

            A.CallTo(() => tutorialContentService.GetTutorialContent(CustomisationId, SectionId, TutorialId))
                .Returns(expectedTutorialContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

            // When
            var result = controller.ContentViewer(CustomisationId, SectionId, TutorialId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }

        [Test]
        public void TutorialVideo_should_StartOrUpdate_session_if_valid_tutorial()
        {
            // Given
            var expectedTutorialVideo = TutorialContentHelper.CreateDefaultTutorialVideo();
            const int progressId = 3;

            A.CallTo(() => tutorialContentService.GetTutorialVideo(CustomisationId, SectionId, TutorialId))
                .Returns(expectedTutorialVideo);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.TutorialVideo(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(CandidateId, CustomisationId, httpContextSession))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._))
                .WhenArgumentsMatch((int candidateId, int customisationId, ISession session) =>
                    candidateId != CandidateId || customisationId != CustomisationId)
                .MustNotHaveHappened();
        }

        [Test]
        public void TutorialVideo_should_not_StartOrUpdate_session_if_invalid_tutorial()
        {
            // Given
            const int progressId = 3;

            A.CallTo(() => tutorialContentService.GetTutorialVideo(CustomisationId, SectionId, TutorialId))
                .Returns(null);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.TutorialVideo(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void TutorialVideo_should_not_StartOrUpdate_session_if_unable_to_enrol()
        {
            // Given
            var expectedTutorialVideo = TutorialContentHelper.CreateDefaultTutorialVideo();

            A.CallTo(() => tutorialContentService.GetTutorialVideo(CustomisationId, SectionId, TutorialId))
                .Returns(expectedTutorialVideo);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

            // When
            controller.TutorialVideo(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void TutorialVideo_should_UpdateProgress_if_valid_tutorial()
        {
            // Given
            var expectedTutorialVideo = TutorialContentHelper.CreateDefaultTutorialVideo();
            const int progressId = 3;

            A.CallTo(() => tutorialContentService.GetTutorialVideo(CustomisationId, SectionId, TutorialId))
                .Returns(expectedTutorialVideo);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.TutorialVideo(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(progressId)).MustHaveHappened();
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._))
                .WhenArgumentsMatch((int id) => id != progressId)
                .MustNotHaveHappened();
        }

        [Test]
        public void TutorialVideo_should_not_UpdateProgress_if_invalid_tutorial()
        {
            // Given
            const int progressId = 3;

            A.CallTo(() => tutorialContentService.GetTutorialVideo(CustomisationId, SectionId, TutorialId))
                .Returns(null);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.TutorialVideo(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void TutorialVideo_should_not_UpdateProgress_if_unable_to_enrol()
        {
            // Given
            var expectedTutorialVideo = TutorialContentHelper.CreateDefaultTutorialVideo();

            A.CallTo(() => tutorialContentService.GetTutorialVideo(CustomisationId, SectionId, TutorialId))
                .Returns(expectedTutorialVideo);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

            // When
            controller.TutorialVideo(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void TutorialVideo_should_render_view()
        {
            // Given
            var expectedTutorialVideo = TutorialContentHelper.CreateDefaultTutorialVideo();
            const int progressId = 101;

            A.CallTo(() => tutorialContentService.GetTutorialVideo(CustomisationId, SectionId, TutorialId))
                .Returns(expectedTutorialVideo);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            var result = controller.TutorialVideo(CustomisationId, SectionId, TutorialId);

            // Then
            var expectedModel = new TutorialVideoViewModel(
                config,
                expectedTutorialVideo,
                CustomisationId,
                SectionId,
                TutorialId
            );

            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void TutorialVideo_should_return_404_if_invalid_tutorial()
        {
            // Given
            const int progressId = 101;

            A.CallTo(() => tutorialContentService.GetTutorialVideo(CustomisationId, SectionId, TutorialId))
                .Returns(null);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            var result = controller.TutorialVideo(CustomisationId, SectionId, TutorialId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }

        [Test]
        public void TutorialVideo_should_return_404_if_unable_to_enrol()
        {
            // Given
            var expectedTutorialVideo = TutorialContentHelper.CreateDefaultTutorialVideo();

            A.CallTo(() => tutorialContentService.GetTutorialVideo(CustomisationId, SectionId, TutorialId))
                .Returns(expectedTutorialVideo);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

            // When
            var result = controller.TutorialVideo(CustomisationId, SectionId, TutorialId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }

        [Test]
        public void Close_should_close_sessions()
        {
            // When
            controller.Close();

            // Then
            A.CallTo(() => sessionService.StopSession(CandidateId, httpContextSession)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionService.StopSession(A<int>._, A<ISession>._))
                .WhenArgumentsMatch((int candidateId, ISession _) => candidateId != CandidateId)
                .MustNotHaveHappened();
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
