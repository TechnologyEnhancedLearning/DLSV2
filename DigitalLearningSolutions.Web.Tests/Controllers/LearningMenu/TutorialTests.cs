namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningMenu
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningMenu;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    public partial class LearningMenuControllerTests
    {
        [Test]
        public async Task Tutorial_should_StartOrUpdate_course_sessions_if_valid_tutorial()
        {
            // Given
            var defaultTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(TutorialId);
            A.CallTo(() => tutorialContentDataService.GetTutorialInformation(CandidateId, CustomisationId, SectionId, TutorialId))
                .Returns(defaultTutorialInformation);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(1);

            // When
            await controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(CandidateId, CustomisationId, httpContextSession)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._))
                .WhenArgumentsMatch((int candidateId, int customisationId, ISession session) =>
                    candidateId != CandidateId || customisationId != CustomisationId)
                .MustNotHaveHappened();
        }

        [Test]
        [TestCase(45)]
        [TestCase(90)]
        public async Task Tutorial_should_sign_in_with_longer_expiry_if_valid_tutorial_with_average_duration_of_45_minutes_or_over(int averageTutorialDuration)
        {
            // Given
            var utcNow = new DateTime(2022, 1, 1, 10, 0, 0);
            var defaultTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(TutorialId, averageTutorialDuration: averageTutorialDuration);
            A.CallTo(() => tutorialContentDataService.GetTutorialInformation(CandidateId, CustomisationId, SectionId, TutorialId))
                .Returns(defaultTutorialInformation);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(1);
            A.CallTo(() => clockService.UtcNow).Returns(utcNow);

            // When
            await controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            var expectedExpiryTime = new DateTime(2022, 1, 1, 18, 0, 0);
            A.CallTo(
                () => authenticationService.SignInAsync(
                    A<HttpContext>._,
                    A<string>._,
                    A<ClaimsPrincipal>._,
                    A<AuthenticationProperties>.That.Matches(ap => ap.ExpiresUtc!.Value == expectedExpiryTime)
                )
            ).MustHaveHappenedOnceExactly();
        }

        [Test]
        [TestCase(44)]
        [TestCase(10)]
        public async Task Tutorial_should_not_sign_in_with_longer_expiry_if_valid_tutorial_with_less_than_45_minutes(int averageTutorialDuration)
        {
            // Given
            var defaultTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(TutorialId, averageTutorialDuration: averageTutorialDuration);
            A.CallTo(() => tutorialContentDataService.GetTutorialInformation(CandidateId, CustomisationId, SectionId, TutorialId))
                .Returns(defaultTutorialInformation);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(1);

            // When
            await controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(
                () => authenticationService.SignInAsync(
                    A<HttpContext>._,
                    A<string>._,
                    A<ClaimsPrincipal>._,
                    A<AuthenticationProperties>._
                )
            ).MustNotHaveHappened();
        }

        [Test]
        public async Task Tutorial_should_not_StartOrUpdate_course_sessions_if_invalid_tutorial()
        {
            // Given
            A.CallTo(() => tutorialContentDataService.GetTutorialInformation(CandidateId, CustomisationId, SectionId, TutorialId))
                .Returns(null);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(1);

            // When
            await controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task Tutorial_should_not_StartOrUpdate_course_sessions_if_unable_to_enrol()
        {
            // Given
            var defaultTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(TutorialId);
            A.CallTo(() => tutorialContentDataService.GetTutorialInformation(CandidateId, CustomisationId, SectionId, TutorialId))
                .Returns(defaultTutorialInformation);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(null);

            // When
            await controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => sessionService.StartOrUpdateDelegateSession(A<int>._, A<int>._, A<ISession>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task Tutorial_should_UpdateProgress_if_valid_tutorial()
        {
            // Given
            const int progressId = 3;
            var defaultTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(TutorialId);
            A.CallTo(() => tutorialContentDataService.GetTutorialInformation(CandidateId, CustomisationId, SectionId, TutorialId))
                .Returns(defaultTutorialInformation);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(progressId);

            // When
            await controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(progressId)).MustHaveHappened();
        }

        [Test]
        public async Task Tutorial_should_not_UpdateProgress_if_invalid_tutorial()
        {
            // Given
            const int progressId = 3;
            A.CallTo(() => tutorialContentDataService.GetTutorialInformation(CandidateId, CustomisationId, SectionId, TutorialId))
                .Returns(null);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(progressId);

            // When
            await controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task Tutorial_should_not_UpdateProgress_if_unable_to_enrol()
        {
            // Given
            var defaultTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(TutorialId);
            A.CallTo(() => tutorialContentDataService.GetTutorialInformation(CandidateId, CustomisationId, SectionId, TutorialId))
                .Returns(defaultTutorialInformation);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(null);

            // When
            await controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            A.CallTo(() => courseContentService.UpdateProgress(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task Tutorial_should_render_view()
        {
            // Given
            var expectedTutorialInformation = TutorialContentHelper.CreateDefaultTutorialInformation(TutorialId);
            A.CallTo(() => tutorialContentDataService.GetTutorialInformation(CandidateId, CustomisationId, SectionId, TutorialId))
                .Returns(expectedTutorialInformation);
            A.CallTo(() => courseContentService.GetOrCreateProgressId(CandidateId, CustomisationId, CentreId)).Returns(3);

            // When
            var result = await controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            var expectedModel = new TutorialViewModel(config, expectedTutorialInformation, CustomisationId, SectionId);
            result.Should().BeViewResult()
                .Model.Should().BeEquivalentTo(expectedModel);
        }

        [Test]
        public async Task Tutorial_should_return_404_if_invalid_tutorial()
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
            var result = await controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }

        [Test]
        public async Task Tutorial_should_return_404_if_unable_to_enrol()
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
            var result = await controller.Tutorial(CustomisationId, SectionId, TutorialId);

            // Then
            result.Should()
                .BeRedirectToActionResult()
                .WithControllerName("LearningSolutions")
                .WithActionName("StatusCode")
                .WithRouteValue("code", 404);
        }
    }
}
