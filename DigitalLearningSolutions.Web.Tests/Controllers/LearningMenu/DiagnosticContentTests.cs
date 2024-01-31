namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningMenu
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    public partial class LearningMenuControllerTests
    {
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
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(CandidateId, CustomisationId, httpContextSession)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._))
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
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
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
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
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
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._)).MustHaveHappened();
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
    }
}
