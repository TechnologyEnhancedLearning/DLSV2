namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningMenu
{
    using System.Collections.Generic;
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
        private IDiagnosticAssessmentDataService diagnosticAssessmentDataService;
        private IDiagnosticAssessmentService diagnosticAssessmentService;
        private IPostLearningAssessmentService postLearningAssessmentService;
        private ICourseCompletionService courseCompletionService;
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
            diagnosticAssessmentDataService = A.Fake<IDiagnosticAssessmentDataService>();
            diagnosticAssessmentService = A.Fake<IDiagnosticAssessmentService>();
            postLearningAssessmentService = A.Fake<IPostLearningAssessmentService>();
            courseCompletionService = A.Fake<ICourseCompletionService>();

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
                diagnosticAssessmentDataService,
                diagnosticAssessmentService,
                postLearningAssessmentService,
                sessionService,
                courseCompletionService
            )
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
        public void Index_should_redirect_to_section_page_if_one_section_in_course()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            var section = CourseContentHelper.CreateDefaultCourseSection(id: sectionId);
            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(customisationId);
            expectedCourseContent.Sections.Add(section);

            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, customisationId))
                .Returns(expectedCourseContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Index(customisationId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningMenu")
                .WithActionName("Section")
                .WithRouteValue("customisationId", customisationId)
                .WithRouteValue("sectionId", sectionId);
        }

        [Test]
        public void Index_should_not_redirect_to_section_page_if_more_than_one_section_in_course()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            var section1 = CourseContentHelper.CreateDefaultCourseSection(id: sectionId + 1);
            var section2 = CourseContentHelper.CreateDefaultCourseSection(id: sectionId + 2);
            var section3 = CourseContentHelper.CreateDefaultCourseSection(id: sectionId + 3);

            var expectedCourseContent = CourseContentHelper.CreateDefaultCourseContent(customisationId);
            expectedCourseContent.Sections.AddRange(new[] { section1, section2, section3 });

            A.CallTo(() => courseContentService.GetCourseContent(CandidateId, customisationId))
                .Returns(expectedCourseContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Index(customisationId);

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
            var expectedModel = new SectionContentViewModel(config, defaultSectionContent, CustomisationId, SectionId);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Sections_should_redirect_to_tutorial_page_if_one_tutorial_in_section()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            const int tutorialId = 789;
            var tutorial = SectionContentHelper.CreateDefaultSectionTutorial(id: tutorialId);
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: null,
                plAssessPath: null,
                consolidationPath: null,
                otherSectionsExist: true
            );
            expectedSectionContent.Tutorials.Add(tutorial);

            A.CallTo(() => sectionContentService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningMenu")
                .WithActionName("Tutorial")
                .WithRouteValue("customisationId", customisationId)
                .WithRouteValue("sectionId", sectionId)
                .WithRouteValue("tutorialId", tutorialId);
        }

        [Test]
        public void Sections_should_redirect_to_tutorial_page_if_one_tutorial_and_has_no_diagnostic_tutorials()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            const int tutorialId = 789;
            var tutorial = SectionContentHelper.CreateDefaultSectionTutorial(id: tutorialId);
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: "some/diagnostic/path.html",
                plAssessPath: null,
                consolidationPath: null,
                otherSectionsExist: true
            );
            expectedSectionContent.DiagnosticStatus = false;
            expectedSectionContent.Tutorials.Add(tutorial);

            A.CallTo(() => sectionContentService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningMenu")
                .WithActionName("Tutorial")
                .WithRouteValue("customisationId", customisationId)
                .WithRouteValue("sectionId", sectionId)
                .WithRouteValue("tutorialId", tutorialId);
        }

        [Test]
        public void Sections_should_redirect_to_tutorial_page_if_one_tutorial_and_has_diagnostic_but_no_path()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            const int tutorialId = 789;
            var tutorial = SectionContentHelper.CreateDefaultSectionTutorial(id: tutorialId);
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: null,
                plAssessPath: null,
                consolidationPath: null,
                otherSectionsExist: true
            );
            expectedSectionContent.DiagnosticStatus = true;
            expectedSectionContent.Tutorials.Add(tutorial);

            A.CallTo(() => sectionContentService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningMenu")
                .WithActionName("Tutorial")
                .WithRouteValue("customisationId", customisationId)
                .WithRouteValue("sectionId", sectionId)
                .WithRouteValue("tutorialId", tutorialId);
        }

        [Test]
        public void Sections_should_redirect_to_post_learning_assessment_if_only_post_learning_in_section()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: null,
                plAssessPath: "some/post-learning/path.html",
                isAssessed: true,
                consolidationPath: null,
                otherSectionsExist: true
            );
            // expectedSectionContent.Tutorials is empty

            A.CallTo(() => sectionContentService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningMenu")
                .WithActionName("PostLearning")
                .WithRouteValue("customisationId", customisationId)
                .WithRouteValue("sectionId", sectionId);
        }

        [Test]
        public void Sections_should_redirect_to_post_learning_assessment_if_there_is_diagnostic_path_but_no_diagnostic_tutorials()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: "some/diagnostic/path.html",
                plAssessPath: "some/post-learning/path.html",
                isAssessed: true,
                consolidationPath: null,
                otherSectionsExist: true
            );
            expectedSectionContent.DiagnosticStatus = false;
            // expectedSectionContent.Tutorials is empty

            A.CallTo(() => sectionContentService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningMenu")
                .WithActionName("PostLearning")
                .WithRouteValue("customisationId", customisationId)
                .WithRouteValue("sectionId", sectionId);
        }

        [Test]
        public void Sections_should_redirect_to_post_learning_assessment_if_there_is_diagnostic_tutorial_but_no_path()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: null,
                plAssessPath: "some/post-learning/path.html",
                isAssessed: true,
                consolidationPath: null,
                otherSectionsExist: true
            );
            expectedSectionContent.DiagnosticStatus = true;
            // expectedSectionContent.Tutorials; viewable tutorials, is empty

            A.CallTo(() => sectionContentService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningMenu")
                .WithActionName("PostLearning")
                .WithRouteValue("customisationId", customisationId)
                .WithRouteValue("sectionId", sectionId);
        }

        [Test]
        public void Sections_should_redirect_to_diagnostic_assessment_if_only_diagnostic_in_section()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: "some/diagnostic/path.html",
                plAssessPath: null,
                consolidationPath: null,
                otherSectionsExist: true
            );
            expectedSectionContent.DiagnosticStatus = true;
            // expectedSectionContent.Tutorials is empty

            A.CallTo(() => sectionContentService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningMenu")
                .WithActionName("Diagnostic")
                .WithRouteValue("customisationId", customisationId)
                .WithRouteValue("sectionId", sectionId);
        }

        [Test]
        public void Sections_should_redirect_to_diagnostic_assessment_if_there_is_post_learning_path_but_is_not_assessed()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: "some/diagnostic/path.html",
                plAssessPath: "some/post-learning/path.html",
                isAssessed: false,
                consolidationPath: null,
                otherSectionsExist: true
            );
            expectedSectionContent.DiagnosticStatus = true;
            // expectedSectionContent.Tutorials is empty

            A.CallTo(() => sectionContentService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningMenu")
                .WithActionName("Diagnostic")
                .WithRouteValue("customisationId", customisationId)
                .WithRouteValue("sectionId", sectionId);
        }

        [Test]
        public void Sections_should_redirect_to_diagnostic_assessment_if_is_assessed_but_there_is_no_post_learning_path()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: "some/diagnostic/path.html",
                plAssessPath: null,
                isAssessed: true,
                consolidationPath: null,
                otherSectionsExist: true
            );
            expectedSectionContent.DiagnosticStatus = true;
            // expectedSectionContent.Tutorials; viewable tutorials, is empty

            A.CallTo(() => sectionContentService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningMenu")
                .WithActionName("Diagnostic")
                .WithRouteValue("customisationId", customisationId)
                .WithRouteValue("sectionId", sectionId);
        }

        [Test]
        public void Sections_should_return_section_page_if_there_is_diagnostic_and_tutorial()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            const int tutorialId = 789;
            var tutorial = SectionContentHelper.CreateDefaultSectionTutorial(id: tutorialId);
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: "some/diagnostic/path.html",
                plAssessPath: null,
                consolidationPath: null,
                otherSectionsExist: true
            );
            expectedSectionContent.DiagnosticStatus = true;
            expectedSectionContent.Tutorials.Add(tutorial);

            A.CallTo(() => sectionContentService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            var expectedModel = new SectionContentViewModel(config, expectedSectionContent, customisationId, sectionId);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Sections_should_return_section_page_if_there_is_diagnostic_and_post_learning_assessments()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: "some/diagnostic/path.html",
                plAssessPath: "some/post-learning/path.html",
                isAssessed: true,
                consolidationPath: null,
                otherSectionsExist: true
            );
            expectedSectionContent.DiagnosticStatus = true;
            // expectedSectionContent.Tutorials; viewable tutorials, is empty

            A.CallTo(() => sectionContentService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            var expectedModel = new SectionContentViewModel(config, expectedSectionContent, customisationId, sectionId);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Sections_should_return_section_page_if_there_is_post_learning_assessment_and_tutorial()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            const int tutorialId = 789;
            var tutorial = SectionContentHelper.CreateDefaultSectionTutorial(id: tutorialId);
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: null,
                plAssessPath: "some/post-learning/path.html",
                isAssessed: true,
                consolidationPath: null,
                otherSectionsExist: true
            );
            expectedSectionContent.Tutorials.Add(tutorial);

            A.CallTo(() => sectionContentService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            var expectedModel = new SectionContentViewModel(config, expectedSectionContent, customisationId, sectionId);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Sections_should_redirect_to_tutorial_page_if_one_tutorial_and_is_not_assessed()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            const int tutorialId = 789;
            var tutorial = SectionContentHelper.CreateDefaultSectionTutorial(id: tutorialId);
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: null,
                plAssessPath: "some/post-learning/path.html",
                isAssessed: false,
                consolidationPath: null,
                otherSectionsExist: true
            );
            expectedSectionContent.Tutorials.Add(tutorial);

            A.CallTo(() => sectionContentService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningMenu")
                .WithActionName("Tutorial")
                .WithRouteValue("customisationId", customisationId)
                .WithRouteValue("sectionId", sectionId)
                .WithRouteValue("tutorialId", tutorialId);
        }

        [Test]
        public void Sections_should_redirect_to_tutorial_page_if_one_tutorial_and_is_assessed_but_has_no_path()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            const int tutorialId = 789;
            var tutorial = SectionContentHelper.CreateDefaultSectionTutorial(id: tutorialId);
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: null,
                plAssessPath: null,
                isAssessed: true,
                consolidationPath: null,
                otherSectionsExist: true
            );
            expectedSectionContent.Tutorials.Add(tutorial);

            A.CallTo(() => sectionContentService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningMenu")
                .WithActionName("Tutorial")
                .WithRouteValue("customisationId", customisationId)
                .WithRouteValue("sectionId", sectionId)
                .WithRouteValue("tutorialId", tutorialId);
        }

        [Test]
        public void Sections_should_return_section_page_if_there_is_one_tutorial_and_consolidation()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            const int tutorialId = 789;
            var tutorial = SectionContentHelper.CreateDefaultSectionTutorial(id: tutorialId);
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: null,
                plAssessPath: null,
                consolidationPath: "some/consolidation/path.pdf",
                otherSectionsExist: true
            );
            expectedSectionContent.Tutorials.Add(tutorial);

            A.CallTo(() => sectionContentService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            var expectedModel = new SectionContentViewModel(config, expectedSectionContent, customisationId, sectionId);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Sections_should_return_section_page_if_there_is_post_learning_assessment_and_consolidation()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: null,
                plAssessPath: "some/post-learning/path.html",
                isAssessed: true,
                consolidationPath: "some/consolidation/path.pdf",
                otherSectionsExist: true
            );
            // expectedSectionContent.Tutorials; viewable tutorials, is empty

            A.CallTo(() => sectionContentService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            var expectedModel = new SectionContentViewModel(config, expectedSectionContent, customisationId, sectionId);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Sections_should_return_section_page_if_there_is_diagnostic_assessment_and_consolidation()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: "some/diagnostic/path.html",
                plAssessPath: null,
                consolidationPath: "some/consolidation/path.pdf",
                otherSectionsExist: true
            );
            expectedSectionContent.DiagnosticStatus = true;
            // expectedSectionContent.Tutorials; viewable tutorials, is empty

            A.CallTo(() => sectionContentService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            var expectedModel = new SectionContentViewModel(config, expectedSectionContent, customisationId, sectionId);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Sections_should_return_section_page_if_more_than_one_tutorial_in_section()
        {
            // Given
            const int customisationId = 123;
            const int sectionId = 456;
            const int tutorialId = 789;
            var tutorial1 = SectionContentHelper.CreateDefaultSectionTutorial(id: tutorialId + 1);
            var tutorial2 = SectionContentHelper.CreateDefaultSectionTutorial(id: tutorialId + 2);
            var tutorial3 = SectionContentHelper.CreateDefaultSectionTutorial(id: tutorialId + 3);

            var expectedSectionContent = SectionContentHelper.CreateDefaultSectionContent(
                diagAssessPath: null,
                plAssessPath: null,
                consolidationPath: null,
                otherSectionsExist: true
            );
            expectedSectionContent.Tutorials.AddRange(new[] { tutorial1, tutorial2, tutorial3 });

            A.CallTo(() => sectionContentService.GetSectionContent(customisationId, CandidateId, sectionId))
                .Returns(expectedSectionContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, customisationId, CentreId)).Returns(10);

            var expectedModel = new SectionContentViewModel(config, expectedSectionContent, customisationId, sectionId);

            // When
            var result = controller.Section(customisationId, sectionId);

            // Then
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
        public void Diagnostic_assessment_should_StartOrUpdate_course_sessions_if_valid_diagnostic_assessment()
        {
            // Given
            const int progressId = 299;
            var defaultDiagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment();
            A.CallTo(() => diagnosticAssessmentService.GetDiagnosticAssessment(CustomisationId, CandidateId, SectionId))
                .Returns(defaultDiagnosticAssessment);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.Diagnostic(CustomisationId, SectionId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(CandidateId, CustomisationId, httpContextSession)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._))
                .WhenArgumentsMatch((int candidateId, int customisationId, ISession session) =>
                    candidateId != CandidateId || customisationId != CustomisationId)
                .MustNotHaveHappened();
        }

        [Test]
        public void Diagnostic_assessment_should_not_StartOrUpdate_course_sessions_if_diagnostic_assessment_not_found()
        {
            // Given
            A.CallTo(() => diagnosticAssessmentService.GetDiagnosticAssessment(CustomisationId, CandidateId, SectionId))
                .Returns(null);

            // When
            controller.Diagnostic(CustomisationId, SectionId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
        }

        [Test]
        public void Diagnostic_assessment_should_not_StartOrUpdate_course_sessions_if_unable_to_enrol()
        {
            // Given
            var defaultDiagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment();
            A.CallTo(() => diagnosticAssessmentService.GetDiagnosticAssessment(CustomisationId, CandidateId, SectionId))
                .Returns(defaultDiagnosticAssessment);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

            // When
            controller.Diagnostic(CustomisationId, SectionId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
        }

        [Test]
        public void Diagnostic_assessment_should_UpdateProgress_if_valid_diagnostic_assessment()
        {
            // Given
            const int progressId = 299;
            var defaultDiagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment();
            A.CallTo(() => diagnosticAssessmentService.GetDiagnosticAssessment(CustomisationId, CandidateId, SectionId))
                .Returns(defaultDiagnosticAssessment);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.Diagnostic(CustomisationId, SectionId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(progressId)).MustHaveHappened();
        }

        [Test]
        public void Diagnostic_assessment_should_not_UpdateProgress_if_invalid_diagnostic_assessment()
        {
            // Given
            A.CallTo(() => diagnosticAssessmentService.GetDiagnosticAssessment(CustomisationId, CandidateId, SectionId))
                .Returns(null);

            // When
            controller.Diagnostic(CustomisationId, SectionId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void Diagnostic_assessment_should_not_UpdateProgress_if_unable_to_enrol()
        {
            // Given
            var defaultDiagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment();
            A.CallTo(() => diagnosticAssessmentService.GetDiagnosticAssessment(CustomisationId, CandidateId, SectionId))
                .Returns(defaultDiagnosticAssessment);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

            // When
            controller.Diagnostic(CustomisationId, SectionId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void Diagnostic_assessment_should_render_view()
        {
            // Given
            const int progressId = 299;
            var defaultDiagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment();
            A.CallTo(() => diagnosticAssessmentService.GetDiagnosticAssessment(CustomisationId, CandidateId, SectionId))
                .Returns(defaultDiagnosticAssessment);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).
                Returns(progressId);

            // When
            var result = controller.Diagnostic(CustomisationId, SectionId);

            // Then
            var expectedModel = new DiagnosticAssessmentViewModel(defaultDiagnosticAssessment, CustomisationId, SectionId);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Diagnostic_assessment_should_404_if_diagnostic_assessment_not_found()
        {
            // Given
            A.CallTo(() => diagnosticAssessmentService.GetDiagnosticAssessment(CustomisationId, CandidateId, SectionId))
                .Returns(null);

            // When
            var result = controller.Diagnostic(CustomisationId, SectionId);

            // Then
            A.CallTo(() => courseContentService.GetOrCreateProgressId(A<int>._, A<int>._, A<int>._)).MustNotHaveHappened();
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }

        [Test]
        public void Diagnostic_assessment_should_404_if_failed_to_enrol()
        {
            // Given
            var defaultDiagnosticAssessment = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticAssessment();
            A.CallTo(() => diagnosticAssessmentService.GetDiagnosticAssessment(CustomisationId, CandidateId, SectionId))
                .Returns(defaultDiagnosticAssessment);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(null);

            // When
            var result = controller.Diagnostic(CustomisationId, SectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }

        [Test]
        public void Diagnostic_content_should_StartOrUpdate_course_sessions_if_valid_diagnostic_content()
        {
            // Given
            const int progressId = 299;
            var emptySelectedTutorials = new List<int>();
            var defaultDiagnosticContent = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticContent();
            A.CallTo(() => diagnosticAssessmentService.GetDiagnosticContent(CustomisationId, SectionId, emptySelectedTutorials))
                .Returns(defaultDiagnosticContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.DiagnosticContent(CustomisationId, SectionId, emptySelectedTutorials);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(CandidateId, CustomisationId, httpContextSession)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._))
                .WhenArgumentsMatch((int candidateId, int customisationId, ISession session) =>
                    candidateId != CandidateId || customisationId != CustomisationId)
                .MustNotHaveHappened();
        }

        [Test]
        public void Diagnostic_content_should_not_StartOrUpdate_course_sessions_if_diagnostic_content_not_found()
        {
            // Given
            var emptySelectedTutorials = new List<int>();
            A.CallTo(() => diagnosticAssessmentService.GetDiagnosticContent(CustomisationId, SectionId, emptySelectedTutorials))
                .Returns(null);

            // When
            controller.DiagnosticContent(CustomisationId, SectionId, emptySelectedTutorials);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
        }

        [Test]
        public void Diagnostic_content_should_not_StartOrUpdate_course_sessions_if_unable_to_enrol()
        {
            // Given
            var emptySelectedTutorials = new List<int>();
            var defaultDiagnosticContent = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticContent();
            A.CallTo(() => diagnosticAssessmentService.GetDiagnosticContent(CustomisationId, SectionId, emptySelectedTutorials))
                .Returns(defaultDiagnosticContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

            // When
            controller.DiagnosticContent(CustomisationId, SectionId, emptySelectedTutorials);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
        }

        [Test]
        public void Diagnostic_content_should_UpdateProgress_if_valid_diagnostic_content()
        {
            // Given
            var emptySelectedTutorials = new List<int>();
            const int progressId = 299;
            var defaultDiagnosticContent = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticContent();
            A.CallTo(() => diagnosticAssessmentService.GetDiagnosticContent(CustomisationId, SectionId, emptySelectedTutorials))
                .Returns(defaultDiagnosticContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.DiagnosticContent(CustomisationId, SectionId, emptySelectedTutorials);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(progressId)).MustHaveHappened();
        }

        [Test]
        public void Diagnostic_content_should_not_UpdateProgress_if_invalid_diagnostic_content()
        {
            // Given
            var emptySelectedTutorials = new List<int>();
            A.CallTo(() => diagnosticAssessmentService.GetDiagnosticContent(CustomisationId, SectionId, emptySelectedTutorials))
                .Returns(null);

            // When
            controller.DiagnosticContent(CustomisationId, SectionId, emptySelectedTutorials);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void Diagnostic_content_should_not_UpdateProgress_if_unable_to_enrol()
        {
            // Given
            var emptySelectedTutorials = new List<int>();
            var defaultDiagnosticContent = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticContent();
            A.CallTo(() => diagnosticAssessmentService.GetDiagnosticContent(CustomisationId, SectionId, emptySelectedTutorials))
                .Returns(defaultDiagnosticContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

            // When
            controller.DiagnosticContent(CustomisationId, SectionId, emptySelectedTutorials);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void Diagnostic_content_should_render_view()
        {
            // Given
            const int progressId = 299;
            var emptySelectedTutorials = new List<int>();
            var selectedTutorials = new List<int>();
            var defaultDiagnosticContent = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticContent();
            A.CallTo(() => diagnosticAssessmentService.GetDiagnosticContent(CustomisationId, SectionId, emptySelectedTutorials))
                .Returns(defaultDiagnosticContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).
                Returns(progressId);

            // When
            var result = controller.DiagnosticContent(CustomisationId, SectionId, emptySelectedTutorials);

            // Then
            var expectedModel = new DiagnosticContentViewModel(
                config,
                defaultDiagnosticContent,
                selectedTutorials,
                CustomisationId,
                CentreId,
                SectionId,
                progressId,
                CandidateId);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Diagnostic_content_should_404_if_diagnostic_content_not_found()
        {
            // Given
            var emptySelectedTutorials = new List<int>();
            A.CallTo(() => diagnosticAssessmentService.GetDiagnosticContent(CustomisationId, SectionId, emptySelectedTutorials))
                .Returns(null);

            // When
            var result = controller.DiagnosticContent(CustomisationId, SectionId, emptySelectedTutorials);

            // Then
            A.CallTo(() => courseContentService.GetOrCreateProgressId(A<int>._, A<int>._, A<int>._)).MustNotHaveHappened();
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }

        [Test]
        public void Diagnostic_content_should_404_if_failed_to_enrol()
        {
            // Given
            var emptySelectedTutorials = new List<int>();
            var defaultDiagnosticContent = DiagnosticAssessmentTestHelper.CreateDefaultDiagnosticContent();
            A.CallTo(() => diagnosticAssessmentService.GetDiagnosticContent(CustomisationId, SectionId, emptySelectedTutorials))
                .Returns(defaultDiagnosticContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(null);

            // When
            var result = controller.DiagnosticContent(CustomisationId, SectionId, emptySelectedTutorials);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }

        [Test]
        public void Post_learning_content_should_StartOrUpdate_course_sessions_if_valid_post_learning_content()
        {
            // Given
            const int progressId = 299;
            var defaultPostLearningContent = PostLearningAssessmentTestHelper.CreateDefaultPostLearningContent();
            A.CallTo(() => postLearningAssessmentService.GetPostLearningContent(CustomisationId, SectionId))
                .Returns(defaultPostLearningContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.PostLearningContent(CustomisationId, SectionId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(CandidateId, CustomisationId, httpContextSession)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._))
                .WhenArgumentsMatch((int candidateId, int customisationId, ISession session) =>
                    candidateId != CandidateId || customisationId != CustomisationId)
                .MustNotHaveHappened();
        }

        [Test]
        public void Post_learning_content_should_not_StartOrUpdate_course_sessions_if_post_learning_content_not_found()
        {
            // Given
            A.CallTo(() => postLearningAssessmentService.GetPostLearningContent(CustomisationId, SectionId))
                .Returns(null);

            // When
            controller.PostLearningContent(CustomisationId, SectionId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
        }

        [Test]
        public void Post_learning_content_should_not_StartOrUpdate_course_sessions_if_unable_to_enrol()
        {
            // Given
            var defaultPostLearningContent = PostLearningAssessmentTestHelper.CreateDefaultPostLearningContent();
            A.CallTo(() => postLearningAssessmentService.GetPostLearningContent(CustomisationId, SectionId))
                .Returns(defaultPostLearningContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

            // When
            controller.PostLearningContent(CustomisationId, SectionId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
        }

        [Test]
        public void Post_learning_content_should_UpdateProgress_if_valid_post_learning_content()
        {
            // Given
            const int progressId = 299;
            var defaultPostLearningContent = PostLearningAssessmentTestHelper.CreateDefaultPostLearningContent();
            A.CallTo(() => postLearningAssessmentService.GetPostLearningContent(CustomisationId, SectionId))
                .Returns(defaultPostLearningContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.PostLearningContent(CustomisationId, SectionId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(progressId)).MustHaveHappened();
        }

        [Test]
        public void Post_learning_content_should_not_UpdateProgress_if_invalid_post_learning_content()
        {
            // Given
            A.CallTo(() => postLearningAssessmentService.GetPostLearningContent(CustomisationId, SectionId))
                .Returns(null);

            // When
            controller.PostLearningContent(CustomisationId, SectionId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void Post_learning_content_should_not_UpdateProgress_if_unable_to_enrol()
        {
            // Given
            var defaultPostLearningContent = PostLearningAssessmentTestHelper.CreateDefaultPostLearningContent();
            A.CallTo(() => postLearningAssessmentService.GetPostLearningContent(CustomisationId, SectionId))
                .Returns(defaultPostLearningContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

            // When
            controller.PostLearningContent(CustomisationId, SectionId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void Post_learning_content_should_render_view()
        {
            // Given
            const int progressId = 299;
            var defaultPostLearningContent = PostLearningAssessmentTestHelper.CreateDefaultPostLearningContent();
            A.CallTo(() => postLearningAssessmentService.GetPostLearningContent(CustomisationId, SectionId))
                .Returns(defaultPostLearningContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).
                Returns(progressId);

            // When
            var result = controller.PostLearningContent(CustomisationId, SectionId);

            // Then
            var expectedModel = new PostLearningContentViewModel(
                config,
                defaultPostLearningContent,
                CustomisationId,
                CentreId,
                SectionId,
                progressId,
                CandidateId);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Post_learning_content_should_404_if_post_learning_content_not_found()
        {
            // Given
            A.CallTo(() => postLearningAssessmentService.GetPostLearningContent(CustomisationId, SectionId))
                .Returns(null);

            // When
            var result = controller.PostLearningContent(CustomisationId, SectionId);

            // Then
            A.CallTo(() => courseContentService.GetOrCreateProgressId(A<int>._, A<int>._, A<int>._)).MustNotHaveHappened();
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }

        [Test]
        public void Post_learning_content_should_404_if_failed_to_enrol()
        {
            // Given
            var defaultPostLearningContent = PostLearningAssessmentTestHelper.CreateDefaultPostLearningContent();
            A.CallTo(() => postLearningAssessmentService.GetPostLearningContent(CustomisationId, SectionId))
                .Returns(defaultPostLearningContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(null);

            // When
            var result = controller.PostLearningContent(CustomisationId, SectionId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }

        [Test]
        public void Post_learning_assessment_should_StartOrUpdate_course_sessions_if_valid_post_learning_assessment()
        {
            // Given
            const int progressId = 299;
            var defaultPostLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment();
            A.CallTo(() => postLearningAssessmentService.GetPostLearningAssessment(CustomisationId, CandidateId, SectionId))
                .Returns(defaultPostLearningAssessment);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.PostLearning(CustomisationId, SectionId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(CandidateId, CustomisationId, httpContextSession)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._))
                .WhenArgumentsMatch((int candidateId, int customisationId, ISession session) =>
                    candidateId != CandidateId || customisationId != CustomisationId)
                .MustNotHaveHappened();
        }

        [Test]
        public void Post_learning_assessment_should_not_StartOrUpdate_course_sessions_if_post_learning_assessment_not_found()
        {
            // Given
            A.CallTo(() => postLearningAssessmentService.GetPostLearningAssessment(CustomisationId, CandidateId, SectionId))
                .Returns(null);

            // When
            controller.PostLearning(CustomisationId, SectionId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
        }

        [Test]
        public void Post_learning_assessment_should_not_StartOrUpdate_course_sessions_if_unable_to_enrol()
        {
            // Given
            var defaultPostLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment();
            A.CallTo(() => postLearningAssessmentService.GetPostLearningAssessment(CustomisationId, CandidateId, SectionId))
                .Returns(defaultPostLearningAssessment);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

            // When
            controller.PostLearning(CustomisationId, SectionId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
        }

        [Test]
        public void Post_learning_assessment_should_UpdateProgress_if_valid_post_learning_assessment()
        {
            // Given
            const int progressId = 299;
            var defaultPostLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment();
            A.CallTo(() => postLearningAssessmentService.GetPostLearningAssessment(CustomisationId, CandidateId, SectionId))
                .Returns(defaultPostLearningAssessment);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.PostLearning(CustomisationId, SectionId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(progressId)).MustHaveHappened();
        }

        [Test]
        public void Post_learning_assessment_should_not_UpdateProgress_if_invalid_post_learning_assessment()
        {
            // Given
            A.CallTo(() => postLearningAssessmentService.GetPostLearningAssessment(CustomisationId, CandidateId, SectionId))
                .Returns(null);

            // When
            controller.PostLearning(CustomisationId, SectionId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void Post_learning_assessment_should_not_UpdateProgress_if_unable_to_enrol()
        {
            // Given
            var defaultPostLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment();
            A.CallTo(() => postLearningAssessmentService.GetPostLearningAssessment(CustomisationId, CandidateId, SectionId))
                .Returns(defaultPostLearningAssessment);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

            // When
            controller.PostLearning(CustomisationId, SectionId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void Post_learning_assessment_should_render_view()
        {
            // Given
            const int progressId = 299;
            var defaultPostLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment();
            A.CallTo(() => postLearningAssessmentService.GetPostLearningAssessment(CustomisationId, CandidateId, SectionId))
                .Returns(defaultPostLearningAssessment);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).
                Returns(progressId);

            // When
            var result = controller.PostLearning(CustomisationId, SectionId);

            // Then
            var expectedModel = new PostLearningAssessmentViewModel(defaultPostLearningAssessment, CustomisationId, SectionId);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void Post_learning_assessment_should_404_if_post_learning_assessment_not_found()
        {
            // Given
            A.CallTo(() => postLearningAssessmentService.GetPostLearningAssessment(CustomisationId, CandidateId, SectionId))
                .Returns(null);

            // When
            var result = controller.PostLearning(CustomisationId, SectionId);

            // Then
            A.CallTo(() => courseContentService.GetOrCreateProgressId(A<int>._, A<int>._, A<int>._)).MustNotHaveHappened();
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }

        [Test]
        public void Post_learning_assessment_should_404_if_failed_to_enrol()
        {
            // Given
            var defaultPostLearningAssessment = PostLearningAssessmentHelper.CreateDefaultPostLearningAssessment();
            A.CallTo(() => postLearningAssessmentService.GetPostLearningAssessment(CustomisationId, CandidateId, SectionId))
                .Returns(defaultPostLearningAssessment);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(null);

            // When
            var result = controller.PostLearning(CustomisationId, SectionId);

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
        public void CompletionSummary_should_StartOrUpdate_session_if_valid_course()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(CustomisationId);
            const int progressId = 3;

            A.CallTo(() => courseCompletionService.GetCourseCompletion(CandidateId, CustomisationId))
                .Returns(expectedCourseCompletion);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.CompletionSummary(CustomisationId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(CandidateId, CustomisationId, httpContextSession))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._))
                .WhenArgumentsMatch((int candidateId, int customisationId, ISession session) =>
                    candidateId != CandidateId || customisationId != CustomisationId)
                .MustNotHaveHappened();
        }

        [Test]
        public void CompletionSummary_should_not_StartOrUpdate_session_if_invalid_course()
        {
            // Given
            const int progressId = 3;

            A.CallTo(() => courseCompletionService.GetCourseCompletion(CandidateId, CustomisationId))
                .Returns(null);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.CompletionSummary(CustomisationId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void CompletionSummary_should_not_StartOrUpdate_session_if_unable_to_enrol()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(CustomisationId);

            A.CallTo(() => courseCompletionService.GetCourseCompletion(CandidateId, CustomisationId))
                .Returns(expectedCourseCompletion);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

            // When
            controller.CompletionSummary(CustomisationId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateSession(A<int>._, A<int>._, A<ISession>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void CourseCompletion_should_UpdateProgress_if_valid_course()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(CustomisationId);
            const int progressId = 3;

            A.CallTo(() => courseCompletionService.GetCourseCompletion(CandidateId, CustomisationId))
                .Returns(expectedCourseCompletion);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.CompletionSummary(CustomisationId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(progressId)).MustHaveHappened();
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._))
                .WhenArgumentsMatch((int id) => id != progressId)
                .MustNotHaveHappened();
        }

        [Test]
        public void CourseCompletion_should_not_UpdateProgress_if_invalid_course()
        {
            // Given
            const int progressId = 3;

            A.CallTo(() => courseCompletionService.GetCourseCompletion(CandidateId, CustomisationId))
                .Returns(null);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.CompletionSummary(CustomisationId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void CourseCompletion_should_not_UpdateProgress_if_unable_to_enrol()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(CustomisationId);

            A.CallTo(() => courseCompletionService.GetCourseCompletion(CandidateId, CustomisationId))
                .Returns(expectedCourseCompletion);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

            // When
            controller.CompletionSummary(CustomisationId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void CourseCompletion_should_render_view()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(CustomisationId);
            const int progressId = 101;

            A.CallTo(() => courseCompletionService.GetCourseCompletion(CandidateId, CustomisationId))
                .Returns(expectedCourseCompletion);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            var result = controller.CompletionSummary(CustomisationId);

            // Then
            var expectedModel = new CourseCompletionViewModel(config, expectedCourseCompletion, progressId);

            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public void CompletionSummary_should_return_404_if_invalid_course()
        {
            // Given
            const int progressId = 101;

            A.CallTo(() => courseCompletionService.GetCourseCompletion(CandidateId, CustomisationId))
                .Returns(null);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            var result = controller.CompletionSummary(CustomisationId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }

        [Test]
        public void CompletionSummary_should_return_404_if_unable_to_enrol()
        {
            // Given
            var expectedCourseCompletion = CourseCompletionHelper.CreateDefaultCourseCompletion(CustomisationId);

            A.CallTo(() => courseCompletionService.GetCourseCompletion(CandidateId, CustomisationId))
                .Returns(expectedCourseCompletion);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

            // When
            var result = controller.CompletionSummary(CustomisationId);

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
