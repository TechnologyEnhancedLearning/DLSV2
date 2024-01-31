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
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(CandidateId, CustomisationId, httpContextSession)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._))
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
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
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
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
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
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._)).MustHaveHappened();
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
    }
}
