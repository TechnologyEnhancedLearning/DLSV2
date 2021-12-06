namespace DigitalLearningSolutions.Web.Tests.Controllers.Signposting
{
    using System;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Signposting.LinkLearningHubSso;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.Signposting;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class LinkLearningHubSsoControllerTests
    {
        private ILearningHubSsoService learningHubSsoService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void Setup()
        {
            learningHubSsoService = A.Fake<ILearningHubSsoService>();
            userService = A.Fake<IUserService>();
        }

        [Test]
        public void LinkLearningHubSso_Index_invalid_state_throws_exception()
        {
            // Given
            var controller = new LinkLearningHubSsoController(learningHubSsoService, userService)
                .WithDefaultContext()
                .WithMockUser(true);
            controller.ModelState.AddModelError("Hash", "Required.");

            // When
            Action a = () => controller.Index(new LinkLearningHubRequest());

            // Then
            using (new AssertionScope())
            {
                a.Should().Throw<LearningHubLinkingRequestException>();
                A.CallTo(() => learningHubSsoService.ParseSsoAccountLinkingRequest(A<LinkLearningHubRequest>._))
                    .MustNotHaveHappened();
                A.CallTo(() => userService.SetDelegateUserLearningHubAuthId(A<int>._, A<int>._)).MustNotHaveHappened();
            }
        }

        [Test]
        public void LinkLearningHubSso_Index_invalid_request_throws_exception_in_service()
        {
            // Given
            var controller = new LinkLearningHubSsoController(learningHubSsoService, userService)
                .WithDefaultContext()
                .WithMockTempData()
                .WithMockUser(true);
            A.CallTo(() => learningHubSsoService.ParseSsoAccountLinkingRequest(A<LinkLearningHubRequest>._))
                .Throws(() => new LearningHubLinkingRequestException("Error"));

            // When
            Action a = () => controller.Index(new LinkLearningHubRequest());

            // Then
            using (new AssertionScope())
            {
                a.Should().Throw<LearningHubLinkingRequestException>();
                A.CallTo(() => learningHubSsoService.ParseSsoAccountLinkingRequest(A<LinkLearningHubRequest>._))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(() => userService.SetDelegateUserLearningHubAuthId(A<int>._, A<int>._)).MustNotHaveHappened();
            }
        }

        [Test]
        public void LinkLearningHubSso_Index_valid_request_returns_view()
        {
            // Given
            var controller = new LinkLearningHubSsoController(learningHubSsoService, userService)
                .WithDefaultContext()
                .WithMockTempData()
                .WithMockUser(true);
            A.CallTo(() => learningHubSsoService.ParseSsoAccountLinkingRequest(A<LinkLearningHubRequest>._)).Returns(1);

            // When
            var result = controller.Index(new LinkLearningHubRequest());

            // Then
            using (new AssertionScope())
            {
                result.Should().BeViewResult().WithViewName("Index");
                A.CallTo(() => learningHubSsoService.ParseSsoAccountLinkingRequest(A<LinkLearningHubRequest>._))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(() => userService.SetDelegateUserLearningHubAuthId(A<int>._, A<int>._))
                    .MustHaveHappenedOnceExactly();
            }
        }
    }
}
