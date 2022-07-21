namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class DelegateApprovalsControllerTests
    {
        private DelegateApprovalsController delegateApprovalsController = null!;
        private IDelegateApprovalsService delegateApprovalsService = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void Setup()
        {
            userDataService = A.Fake<IUserDataService>();
            delegateApprovalsService = A.Fake<IDelegateApprovalsService>();
            delegateApprovalsController = new DelegateApprovalsController(delegateApprovalsService, userDataService)
                .WithDefaultContext()
                .WithMockUser(true);
        }

        [Test]
        public void PostApproveDelegate_calls_correct_method()
        {
            // Given
            A.CallTo(() => delegateApprovalsService.ApproveDelegate(2, 2)).DoesNothing();

            // When
            var result = delegateApprovalsController.ApproveDelegate(2);

            // Then
            A.CallTo(() => delegateApprovalsService.ApproveDelegate(2, 2)).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void PostApproveDelegatesForCentre_calls_correct_method()
        {
            // Given
            A.CallTo(() => delegateApprovalsService.ApproveAllUnapprovedDelegatesForCentre(2)).DoesNothing();

            // When
            var result = delegateApprovalsController.ApproveDelegatesForCentre();

            // Then
            A.CallTo(() => delegateApprovalsService.ApproveAllUnapprovedDelegatesForCentre(2)).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void PostRejectDelegate_calls_correct_method()
        {
            // Given
            A.CallTo(() => delegateApprovalsService.RejectDelegate(2, 2)).DoesNothing();

            // When
            var result = delegateApprovalsController.RejectDelegate(2);

            // Then
            A.CallTo(() => delegateApprovalsService.RejectDelegate(2, 2)).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }
    }
}
