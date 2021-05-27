namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningMenu
{
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    public partial class LearningMenuControllerTests
    {
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
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(CandidateId, CustomisationId, httpContextSession)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._))
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
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
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
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
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
    }
}
