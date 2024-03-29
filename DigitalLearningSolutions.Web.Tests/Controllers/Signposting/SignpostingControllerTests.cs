﻿namespace DigitalLearningSolutions.Web.Tests.Controllers.Signposting
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;    
    using DigitalLearningSolutions.Web.Controllers.Signposting;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Signposting;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class SignpostingControllerTests
    {
        private const int DelegateId = 5;
        private const int DelegateUserId = 2;
        private const int ResourceReferenceId = 10;
        private IActionPlanService actionPlanService = null!;
        private SignpostingController controller = null!;
        private ILearningHubResourceService learningHubResourceService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void SetUp()
        {
            userService = A.Fake<IUserService>();
            learningHubResourceService = A.Fake<ILearningHubResourceService>();
            actionPlanService = A.Fake<IActionPlanService>();

            A.CallTo(() => actionPlanService.UpdateActionPlanResourcesLastAccessedDateIfPresent(A<int>._, A<int>._))
                .DoesNothing();
            A.CallTo(() => userService.GetDelegateUserById(DelegateId))
                .Returns(UserTestHelper.GetDefaultDelegateUser(DelegateId));
            A.CallTo(() => userService.DelegateUserLearningHubAccountIsLinked(DelegateId)).Returns(false);
            A.CallTo(() => learningHubResourceService.GetResourceByReferenceId(ResourceReferenceId))
                .Returns((new ResourceReferenceWithResourceDetails(), false));

            controller = new SignpostingController(
                userService,
                learningHubResourceService,
                actionPlanService
            ).WithDefaultContext().WithMockUser(true, delegateId: DelegateId);
        }

        [Test]
        public async Task LaunchLearningResource_updates_action_plan_if_present_for_delegate()
        {
            // When
            await controller.LaunchLearningResource(ResourceReferenceId);

            // Then
            A.CallTo(
                    () => actionPlanService.UpdateActionPlanResourcesLastAccessedDateIfPresent(
                        ResourceReferenceId,
                        DelegateUserId
                    )
                )
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task LaunchLearningResource_skips_warning_if_already_dismissed_and_redirects_to_resource()
        {
            // Given
            A.CallTo(() => userService.GetDelegateUserById(DelegateId))
                .Returns(UserTestHelper.GetDefaultDelegateUser(DelegateId, hasDismissedLhLoginWarning: true));

            // When
            var result = await controller.LaunchLearningResource(ResourceReferenceId);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("ViewResource").WithControllerName("SignpostingSso")
                .WithRouteValue("resourceReferenceId", ResourceReferenceId);
            A.CallTo(() => userService.DelegateUserLearningHubAccountIsLinked(A<int>._)).MustNotHaveHappened();
            A.CallTo(() => learningHubResourceService.GetResourceByReferenceId(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task LaunchLearningResource_shows_warning_if_not_dismissed()
        {
            // When
            var result = await controller.LaunchLearningResource(ResourceReferenceId);

            // Then
            result.Should().BeViewResult().WithViewName("LearningHubLoginWarning");
            A.CallTo(() => userService.DelegateUserLearningHubAccountIsLinked(A<int>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => learningHubResourceService.GetResourceByReferenceIdAndPopulateDeletedDetailsFromDatabase(A<int>._)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void LaunchLearningResource_Post_updates_warning_dismissal_flag_if_checkbox_is_checked()
        {
            // Given
            var model = new LearningHubLoginWarningViewModel
            {
                LearningHubLoginWarningDismissed = true,
            };

            // When
            var result = controller.LaunchLearningResource(ResourceReferenceId, model);

            // Then
            A.CallTo(() => userService.UpdateDelegateLhLoginWarningDismissalStatus(DelegateId, true))
                .MustHaveHappenedOnceExactly();
            result.Should().BeRedirectToActionResult().WithActionName("ViewResource").WithControllerName("SignpostingSso")
                .WithRouteValue("resourceReferenceId", ResourceReferenceId);
        }

        [Test]
        public void LaunchLearningResource_Post_does_not_update_warning_dismissal_flag_if_checkbox_is_unchecked()
        {
            // Given
            var model = new LearningHubLoginWarningViewModel
            {
                LearningHubLoginWarningDismissed = false,
            };

            // When
            var result = controller.LaunchLearningResource(ResourceReferenceId, model);

            // Then
            A.CallTo(() => userService.UpdateDelegateLhLoginWarningDismissalStatus(A<int>._, A<bool>._))
                .MustNotHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("ViewResource").WithControllerName("SignpostingSso")
                .WithRouteValue("resourceReferenceId", ResourceReferenceId);
        }
    }
}
