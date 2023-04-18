namespace DigitalLearningSolutions.Web.Tests.Controllers.Signposting
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;
    using DigitalLearningSolutions.Data.Models.Signposting;
    using DigitalLearningSolutions.Web.Controllers.Signposting;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class SignpostingSsoControllerTests
    {
        private SignpostingSsoController controller = null!;
        private ILearningHubLinkService learningHubLinkService = null!;
        private ILearningHubResourceService learningHubResourceService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void Setup()
        {
            learningHubLinkService = A.Fake<ILearningHubLinkService>();
            learningHubResourceService = A.Fake<ILearningHubResourceService>();
            userService = A.Fake<IUserService>();

            controller = new SignpostingSsoController(
                    learningHubLinkService,
                    learningHubResourceService,
                    userService
                )
                .WithDefaultContext()
                .WithMockSessionData(new Dictionary<string, string>())
                .WithMockUser(true);

            A.CallTo(() => learningHubLinkService.IsLearningHubAccountLinked(A<int>._)).Returns(true);
        }

        [Test]
        public void LinkLearningHubSso_invalid_state_throws_exception()
        {
            // Given
            controller.ModelState.AddModelError("Hash", "Required.");

            // When
            Action a = () => controller.LinkLearningHubSso(new LinkLearningHubRequest());

            // Then
            using (new AssertionScope())
            {
                a.Should().Throw<LearningHubLinkingRequestException>();
                A.CallTo(
                        () => learningHubLinkService.ValidateLinkingRequestAndExtractDestinationResourceId(
                            A<LinkLearningHubRequest>._,
                            A<string>._
                        )
                    )
                    .MustNotHaveHappened();
                A.CallTo(() => learningHubLinkService.IsLearningHubAccountLinked(A<int>._)).MustNotHaveHappened();
                A.CallTo(() => learningHubLinkService.LinkLearningHubAccountIfNotLinked(A<int>._, A<int>._))
                    .MustNotHaveHappened();
            }
        }

        [Test]
        public void LinkLearningHubSso_invalid_request_throws_exception_in_service()
        {
            // Given
            var sessionData = new Dictionary<string, string>
            {
                { LinkLearningHubRequest.SessionIdentifierKey, Guid.NewGuid().ToString() },
            };
            var testController = controller.WithMockSessionData(sessionData);

            A.CallTo(
                    () => learningHubLinkService.ValidateLinkingRequestAndExtractDestinationResourceId(
                        A<LinkLearningHubRequest>._,
                        A<string>._
                    )
                )
                .Throws(() => new LearningHubLinkingRequestException("Error"));

            // When
            Action a = () => testController.LinkLearningHubSso(new LinkLearningHubRequest());

            // Then
            using (new AssertionScope())
            {
                a.Should().Throw<LearningHubLinkingRequestException>();
                A.CallTo(
                        () => learningHubLinkService.ValidateLinkingRequestAndExtractDestinationResourceId(
                            A<LinkLearningHubRequest>._,
                            A<string>._
                        )
                    )
                    .MustHaveHappenedOnceExactly();
                A.CallTo(() => learningHubLinkService.IsLearningHubAccountLinked(A<int>._)).MustNotHaveHappened();
                A.CallTo(() => learningHubLinkService.LinkLearningHubAccountIfNotLinked(A<int>._, A<int>._))
                    .MustNotHaveHappened();
            }
        }

        [Test]
        public void LinkLearningHubSso_valid_request_returns_view()
        {
            // Given
            var sessionData = new Dictionary<string, string>
            {
                { LinkLearningHubRequest.SessionIdentifierKey, Guid.NewGuid().ToString() },
            };
            var testController = controller.WithMockSessionData(sessionData);

            A.CallTo(
                    () => learningHubLinkService.ValidateLinkingRequestAndExtractDestinationResourceId(
                        A<LinkLearningHubRequest>._,
                        A<string>._
                    )
                )
                .Returns(1);

            // When
            var result = testController.LinkLearningHubSso(new LinkLearningHubRequest());

            // Then
            using (new AssertionScope())
            {
                result.Should().BeViewResult().WithViewName("../LinkLearningHubSso");
                A.CallTo(
                        () => learningHubLinkService.ValidateLinkingRequestAndExtractDestinationResourceId(
                            A<LinkLearningHubRequest>._,
                            A<string>._
                        )
                    )
                    .MustHaveHappenedOnceExactly();
                A.CallTo(() => learningHubLinkService.IsLearningHubAccountLinked(A<int>._))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(() => learningHubLinkService.LinkLearningHubAccountIfNotLinked(A<int>._, A<int>._))
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public void LinkLearningHubSso_account_already_linked_returns_view()
        {
            // Given
            var sessionData = new Dictionary<string, string>
            {
                { LinkLearningHubRequest.SessionIdentifierKey, Guid.NewGuid().ToString() },
            };
            var testController = controller.WithMockSessionData(sessionData);

            A.CallTo(
                    () => learningHubLinkService.ValidateLinkingRequestAndExtractDestinationResourceId(
                        A<LinkLearningHubRequest>._,
                        A<string>._
                    )
                )
                .Returns(1);

            // When
            var result = testController.LinkLearningHubSso(new LinkLearningHubRequest());

            // Then
            using (new AssertionScope())
            {
                result.Should().BeViewResult().WithViewName("../LinkLearningHubSso");
                A.CallTo(
                        () => learningHubLinkService.ValidateLinkingRequestAndExtractDestinationResourceId(
                            A<LinkLearningHubRequest>._,
                            A<string>._
                        )
                    )
                    .MustHaveHappenedOnceExactly();
                A.CallTo(() => learningHubLinkService.IsLearningHubAccountLinked(A<int>._))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(() => learningHubLinkService.LinkLearningHubAccountIfNotLinked(A<int>._, A<int>._))
                    .MustHaveHappenedOnceExactly();
            }
        }

        [Test]
        public async Task ViewResource_returns_redirect_to_login_result_when_user_linked()
        {
            // Given
            var authId = 1;
            var resourceUrl = "De/Humani/Corporis/Fabrica";
            var resourceDetails = new ResourceReferenceWithResourceDetails { Link = resourceUrl };

            A.CallTo(() => userService.GetDelegateUserLearningHubAuthId(A<int>._)).Returns(authId);
            A.CallTo(() => learningHubResourceService.GetResourceByReferenceId(5))
                .Returns((resourceDetails, false));
            A.CallTo(() => learningHubLinkService.GetLoginUrlForDelegateAuthIdAndResourceUrl(resourceUrl, authId))
                .Returns("www.example.com/login");

            // When
            var result = await controller.ViewResource(5);

            // Then
            result.Should().BeRedirectResult().WithUrl(
                "www.example.com/login"
            );
        }

        [Test]
        public async Task
            ViewResource_returns_not_found_result_when_user_linked_but_no_relevant_resource_entry()
        {
            // Given
            A.CallTo(() => userService.GetDelegateUserLearningHubAuthId(A<int>._)).Returns(1);
            A.CallTo(() => learningHubResourceService.GetResourceByReferenceId(5))
                .Returns((null, true));

            // When
            var result = await controller.ViewResource(5);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public async Task ViewResource_returns_redirect_result_to_create_user_when_user_not_linked()
        {
            // Given
            A.CallTo(() => userService.GetDelegateUserLearningHubAuthId(A<int>._)).Returns(null);
            A.CallTo(() => learningHubLinkService.GetLinkingUrlForResource(5, A<string>._))
                .Returns("www.example.com/link");

            // When
            var result = await controller.ViewResource(5);

            // Then
            result.Should().BeRedirectResult().WithUrl(
                "www.example.com/link"
            );
            A.CallTo(() => controller.HttpContext.Session.Set(LinkLearningHubRequest.SessionIdentifierKey, A<byte[]>._))
                .MustHaveHappenedOnceExactly();
        }
    }
}
