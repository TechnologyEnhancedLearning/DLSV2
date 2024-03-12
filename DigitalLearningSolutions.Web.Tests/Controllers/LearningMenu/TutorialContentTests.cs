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
        public void ContentViewer_should_StartOrUpdateDelegateSession_if_valid_tutorial()
        {
            // Given
            var expectedTutorialContent = TutorialContentHelper.CreateDefaultTutorialContent();
            const int progressId = 3;

            A.CallTo(() => tutorialContentDataService.GetTutorialContent(CustomisationId, SectionId, TutorialId))
                .Returns(expectedTutorialContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.ContentViewer(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(CandidateId, CustomisationId, httpContextSession))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._))
                .WhenArgumentsMatch((int candidateId, int customisationId, ISession session) =>
                    candidateId != CandidateId || customisationId != CustomisationId)
                .MustNotHaveHappened();
        }

        [Test]
        public void ContentViewer_should_not_StartOrUpdateDelegateSession_if_invalid_tutorial()
        {
            // Given
            const int progressId = 3;

            A.CallTo(() => tutorialContentDataService.GetTutorialContent(CustomisationId, SectionId, TutorialId))
                .Returns(null);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.ContentViewer(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void ContentViewer_should_not_StartOrUpdateDelegateSession_if_unable_to_enrol()
        {
            // Given
            var expectedTutorialContent = TutorialContentHelper.CreateDefaultTutorialContent();

            A.CallTo(() => tutorialContentDataService.GetTutorialContent(CustomisationId, SectionId, TutorialId))
                .Returns(expectedTutorialContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(null);

            // When
            controller.ContentViewer(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void ContentViewer_should_UpdateProgress_if_valid_tutorial()
        {
            // Given
            var expectedTutorialContent = TutorialContentHelper.CreateDefaultTutorialContent();
            const int progressId = 3;

            A.CallTo(() => tutorialContentDataService.GetTutorialContent(CustomisationId, SectionId, TutorialId))
                .Returns(expectedTutorialContent);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId))
                .Returns(progressId);

            // When
            controller.ContentViewer(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._)).MustHaveHappened();
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._))
                .WhenArgumentsMatch((int id) => id != progressId)
                .MustNotHaveHappened();
        }

        [Test]
        public void ContentViewer_should_not_UpdateProgress_if_invalid_tutorial()
        {
            // Given
            const int progressId = 3;

            A.CallTo(() => tutorialContentDataService.GetTutorialContent(CustomisationId, SectionId, TutorialId))
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

            A.CallTo(() => tutorialContentDataService.GetTutorialContent(CustomisationId, SectionId, TutorialId))
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

            A.CallTo(() => tutorialContentDataService.GetTutorialContent(CustomisationId, SectionId, TutorialId))
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

            A.CallTo(() => tutorialContentDataService.GetTutorialContent(CustomisationId, SectionId, TutorialId))
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

            A.CallTo(() => tutorialContentDataService.GetTutorialContent(CustomisationId, SectionId, TutorialId))
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
    }
}
