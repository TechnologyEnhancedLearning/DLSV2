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
        public void Tutorial_should_StartOrUpdate_course_sessions_if_valid_tutorial()
        {
            // Given
            var defaultTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(TutorialId);
            A.CallTo(() => tutorialContentDataService.GetTutorialInformation(CandidateId, CustomisationId, SectionId, TutorialId))
                .Returns(defaultTutorialInformation);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(1);

            // When
            controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(CandidateId, CustomisationId, httpContextSession)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._))
                .WhenArgumentsMatch((int candidateId, int customisationId, ISession session) =>
                    candidateId != CandidateId || customisationId != CustomisationId)
                .MustNotHaveHappened();
        }

        [Test]
        public void Tutorial_should_not_StartOrUpdate_course_sessions_if_invalid_tutorial()
        {
            // Given
            A.CallTo(() => tutorialContentDataService.GetTutorialInformation(CandidateId, CustomisationId, SectionId, TutorialId))
                .Returns(null);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(1);

            // When
            controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
        }

        [Test]
        public void Tutorial_should_not_StartOrUpdate_course_sessions_if_unable_to_enrol()
        {
            // Given
            var defaultTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(TutorialId);
            A.CallTo(() => tutorialContentDataService.GetTutorialInformation(CandidateId, CustomisationId, SectionId, TutorialId))
                .Returns(defaultTutorialInformation);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(null);

            // When
            controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
        }

        [Test]
        public void Tutorial_should_UpdateProgress_if_valid_tutorial()
        {
            // Given
            const int progressId = 3;
            var defaultTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(TutorialId);
            A.CallTo(() => tutorialContentDataService.GetTutorialInformation(CandidateId, CustomisationId, SectionId, TutorialId))
                .Returns(defaultTutorialInformation);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(progressId);

            // When
            controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(progressId)).MustHaveHappened();
        }

        [Test]
        public void Tutorial_should_not_UpdateProgress_if_invalid_tutorial()
        {
            // Given
            const int progressId = 3;
            A.CallTo(() => tutorialContentDataService.GetTutorialInformation(CandidateId, CustomisationId, SectionId, TutorialId))
                .Returns(null);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(progressId);

            // When
            controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void Tutorial_should_not_UpdateProgress_if_unable_to_enrol()
        {
            // Given
            var defaultTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(TutorialId);
            A.CallTo(() => tutorialContentDataService.GetTutorialInformation(CandidateId, CustomisationId, SectionId, TutorialId))
                .Returns(defaultTutorialInformation);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(null);

            // When
            controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void Tutorial_should_render_view()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(TutorialId);
            A.CallTo(() => tutorialContentDataService.GetTutorialInformation(CandidateId, CustomisationId, SectionId, TutorialId))
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
        public void Tutorial_should_return_404_if_invalid_tutorial()
        {
            // Given
            A.CallTo(() => tutorialContentDataService.GetTutorialInformation(
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
        public void Tutorial_should_return_404_if_unable_to_enrol()
        {
            // Given
            var defaultTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(TutorialId);
            A.CallTo(() => tutorialContentDataService.GetTutorialInformation(
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
    }
}
