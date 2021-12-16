﻿namespace DigitalLearningSolutions.Web.Tests.Controllers.LearningPortal
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
        private IActionPlanService actionPlanService = null!;
        private IConfiguration configuration = null!;
        private RecommendedLearningController controller = null!;
        private IFilteredApiHelperService filteredApiHelperService = null!;
        private IRecommendedLearningService recommendedLearningService = null!;
        private ISelfAssessmentService selfAssessmentService = null!;

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
                A.CallTo(
                    () => recommendedLearningService.GetRecommendedLearningForSelfAssessment(
                        SelfAssessmentId,
                        DelegateId
                    )
                ).MustHaveHappenedOnceExactly();
                result.Should().BeViewResult().WithViewName("RecommendedLearning");
            }
        }

        [Test]
        public async Task AddResourceToActionPlan_returns_not_found_when_resource_already_in_action_plan()
        {
            // Given
            const int resourceReferenceId = 1;
            A.CallTo(() => actionPlanService.ResourceCanBeAddedToActionPlan(resourceReferenceId, DelegateId))
                .Returns(false);

            // When
            var result = await controller.AddResourceToActionPlan(SelfAssessmentId, resourceReferenceId);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public async Task AddResourceToActionPlan_adds_resource_and_returns_redirect_when_resource_not_in_action_plan()
        {
            // Given
            const int resourceReferenceId = 1;
            A.CallTo(() => actionPlanService.ResourceCanBeAddedToActionPlan(resourceReferenceId, DelegateId))
                .Returns(true);

            // When
            var result = await controller.AddResourceToActionPlan(SelfAssessmentId, resourceReferenceId);

            // Then
            A.CallTo(() => actionPlanService.AddResourceToActionPlan(resourceReferenceId, DelegateId, SelfAssessmentId))
                .MustHaveHappenedOnceExactly();
            result.Should().BeRedirectToActionResult().WithActionName("RecommendedLearning")
                .WithRouteValue("selfAssessmentId", SelfAssessmentId);
        }
    }
}
