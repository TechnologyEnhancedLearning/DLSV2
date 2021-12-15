namespace DigitalLearningSolutions.Web.Tests.Controllers.Signposting
{
    using System;
    using System.Collections.Generic;
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
        private LinkLearningHubSsoController controller = null!;
        private ILearningHubSsoSecurityService learningHubSsoSecurityService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void Setup()
        {
            learningHubSsoSecurityService = A.Fake<ILearningHubSsoSecurityService>();
            userService = A.Fake<IUserService>();

            controller = new LinkLearningHubSsoController(learningHubSsoSecurityService, userService)
                .WithDefaultContext()
                .WithMockUser(true);

            A.CallTo(() => userService.DelegateUserLearningHubAccountIsLinked(A<int>._)).Returns(false);
        }

        [Test]
        public void LinkLearningHubSso_Index_invalid_state_throws_exception()
        {
            // Given
            controller.ModelState.AddModelError("Hash", "Required.");

            // When
            Action a = () => controller.Index(new LinkLearningHubRequest());

            // Then
            using (new AssertionScope())
            {
                a.Should().Throw<LearningHubLinkingRequestException>();
                A.CallTo(() => learningHubSsoSecurityService.ParseSsoAccountLinkingRequest(A<LinkLearningHubRequest>._))
                    .MustNotHaveHappened();
                A.CallTo(() => userService.SetDelegateUserLearningHubAuthId(A<int>._, A<int>._)).MustNotHaveHappened();
            }
        }

        [Test]
        public void LinkLearningHubSso_Index_invalid_request_throws_exception_in_service()
        {
            // Given
            var sessionData = new Dictionary<string, string>
            {
                { LinkLearningHubRequest.SessionIdentifierKey, Guid.NewGuid().ToString() },
            };
            var testController = controller.WithMockSessionData(sessionData);

            A.CallTo(() => learningHubSsoSecurityService.ParseSsoAccountLinkingRequest(A<LinkLearningHubRequest>._))
                .Throws(() => new LearningHubLinkingRequestException("Error"));

            // When
            Action a = () => testController.Index(new LinkLearningHubRequest());

            // Then
            using (new AssertionScope())
            {
                a.Should().Throw<LearningHubLinkingRequestException>();
                A.CallTo(() => learningHubSsoSecurityService.ParseSsoAccountLinkingRequest(A<LinkLearningHubRequest>._))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(() => userService.SetDelegateUserLearningHubAuthId(A<int>._, A<int>._)).MustNotHaveHappened();
            }
        }

        [Test]
        public void LinkLearningHubSso_Index_valid_request_returns_view()
        {
            // Given
            var sessionData = new Dictionary<string, string>
            {
                { LinkLearningHubRequest.SessionIdentifierKey, Guid.NewGuid().ToString() },
            };
            var testController = controller.WithMockSessionData(sessionData);

            A.CallTo(() => learningHubSsoSecurityService.ParseSsoAccountLinkingRequest(A<LinkLearningHubRequest>._))
                .Returns(1);

            // When
            var result = testController.Index(new LinkLearningHubRequest());

            // Then
            using (new AssertionScope())
            {
                result.Should().BeViewResult().WithViewName("Index");
                A.CallTo(() => learningHubSsoSecurityService.ParseSsoAccountLinkingRequest(A<LinkLearningHubRequest>._))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(() => userService.SetDelegateUserLearningHubAuthId(A<int>._, A<int>._))
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void LinkLearningHubSso_Index_account_already_linked_returns_view()
        {
            // Given
            var sessionData = new Dictionary<string, string>
            {
                { LinkLearningHubRequest.SessionIdentifierKey, Guid.NewGuid().ToString() },
            };
            var testController = controller.WithMockSessionData(sessionData);

            A.CallTo(() => learningHubSsoSecurityService.ParseSsoAccountLinkingRequest(A<LinkLearningHubRequest>._))
                .Returns(1);
            A.CallTo(() => userService.DelegateUserLearningHubAccountIsLinked(A<int>._)).Returns(true);

            // When
            var result = testController.Index(new LinkLearningHubRequest());

            // Then
            using (new AssertionScope())
            {
                result.Should().BeViewResult().WithViewName("Index");
                A.CallTo(() => learningHubSsoSecurityService.ParseSsoAccountLinkingRequest(A<LinkLearningHubRequest>._))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(() => userService.SetDelegateUserLearningHubAuthId(A<int>._, A<int>._))
                    .MustNotHaveHappened();
            }
        }
    }
}
