namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningMenu
{
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
        public void CompletionSummary_should_StartOrUpdateDelegateSession_if_valid_course()
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
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(CandidateId, CustomisationId, httpContextSession))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._))
                .WhenArgumentsMatch((int candidateId, int customisationId, ISession session) =>
                    candidateId != CandidateId || customisationId != CustomisationId)
                .MustNotHaveHappened();
        }

        [Test]
        public void CompletionSummary_should_not_StartOrUpdateDelegateSession_if_invalid_course()
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
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void CompletionSummary_should_not_StartOrUpdateDelegateSession_if_unable_to_enrol()
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
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._))
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
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._)).MustHaveHappened();
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
    }
}
