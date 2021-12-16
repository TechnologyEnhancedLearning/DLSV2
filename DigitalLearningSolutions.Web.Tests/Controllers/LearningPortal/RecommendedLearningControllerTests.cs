namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningPortal
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.External.Filtered;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.LearningPortalController;
    using DigitalLearningSolutions.Web.Helpers.ExternalApis;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class RecommendedLearningControllerTests
    {
        private const int DelegateId = 2;
        private const int SelfAssessmentId = 1;
        private IConfiguration configuration = null!;
        private RecommendedLearningController controller = null!;
        private IFilteredApiHelperService filteredApiHelperService = null!;
        private ISelfAssessmentService selfAssessmentService = null!;
        private IRecommendedLearningService recommendedLearningService = null!;
        private IActionPlanService actionPlanService = null!;

        [SetUp]
        public void Setup()
        {
            filteredApiHelperService = A.Fake<IFilteredApiHelperService>();
            selfAssessmentService = A.Fake<ISelfAssessmentService>();
            configuration = A.Fake<IConfiguration>();
            recommendedLearningService = A.Fake<IRecommendedLearningService>();
            actionPlanService = A.Fake<IActionPlanService>();

            controller = new RecommendedLearningController(
                    filteredApiHelperService,
                    selfAssessmentService,
                    configuration,
                    recommendedLearningService,
                    actionPlanService
                )
                .WithDefaultContext()
                .WithMockUser(true, delegateId: DelegateId);
        }

        [Test]
        public async Task
            SelfAssessmentResults_redirect_to_expected_action_does_not_call_filtered_api_when_using_signposting()
        {
            // Given
            A.CallTo(() => configuration[ConfigHelper.UseSignposting]).Returns("true");

            // When
            var result = await controller.SelfAssessmentResults(SelfAssessmentId);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => filteredApiHelperService.UpdateProfileAndGoals(A<string>._, A<Profile>._, A<List<Goal>>._)
                ).MustNotHaveHappened();
                result.Should().BeRedirectToActionResult().WithActionName("RecommendedLearning")
                    .WithRouteValue("selfAssessmentId", SelfAssessmentId);
            }
        }

        [Test]
        public async Task RecommendedLearning_returns_expected_view_when_using_signposting()
        {
            // Given
            var expectedBookmarkString = $"/LearningPortal/SelfAssessment/{SelfAssessmentId}/RecommendedLearning";
            A.CallTo(() => configuration[ConfigHelper.UseSignposting]).Returns("true");

            // When
            var result = await controller.RecommendedLearning(SelfAssessmentId);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => selfAssessmentService.SetBookmark(SelfAssessmentId, DelegateId, expectedBookmarkString))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(() => selfAssessmentService.UpdateLastAccessed(SelfAssessmentId, DelegateId))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(() => filteredApiHelperService.GetUserAccessToken<AccessToken>(A<string>._))
                    .MustNotHaveHappened();
                result.Should().BeViewResult().WithViewName("RecommendedLearning");
            }
        }
    }
}
