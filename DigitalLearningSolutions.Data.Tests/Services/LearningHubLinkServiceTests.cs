namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Web;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Signposting;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class LearningHubLinkServiceTests
    {
        private ILearningHubLinkService learningHubLinkService = null!;
        private ILearningHubSsoSecurityService learningHubSsoSecurityService = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void Setup()
        {
            var config = A.Fake<IConfiguration>();
            userDataService = A.Fake<IUserDataService>();
            learningHubSsoSecurityService = A.Fake<ILearningHubSsoSecurityService>();

            A.CallTo(() => config[$"{ConfigHelper.LearningHubSsoSectionKey}:{ConfigHelper.LearningHubAuthApiBaseUrl}"])
                .Returns("www.example.com");
            A.CallTo(
                () => config[$"{ConfigHelper.LearningHubSsoSectionKey}:{ConfigHelper.LearningHubAuthApiClientCode}"]
            ).Returns("test");
            A.CallTo(
                () => config[$"{ConfigHelper.LearningHubSsoSectionKey}:{ConfigHelper.LearningHubAuthApiLoginEndpoint}"]
            ).Returns("/insert-log");
            A.CallTo(
                () => config[
                    $"{ConfigHelper.LearningHubSsoSectionKey}:{ConfigHelper.LearningHubAuthApiLinkingEndpoint}"]
            ).Returns("/to-the-past");

            A.CallTo(() => learningHubSsoSecurityService.VerifyHash("56789", "invalid-hash")).Returns(false);
            A.CallTo(() => learningHubSsoSecurityService.VerifyHash("12345", "valid-hash")).Returns(true);
            A.CallTo(() => learningHubSsoSecurityService.GenerateHash(A<string>._)).Returns("hash_brown");

            learningHubLinkService = new LearningHubLinkService(learningHubSsoSecurityService, userDataService, config);
        }

        [Test]
        public void
            ValidateLinkingRequestAndExtractDestinationResourceId_throws_exception_when_stored_sessionIdentifier_is_invalid()
        {
            // Given
            var linkLearningHubRequest = new LinkLearningHubRequest
            {
                Hash = "valid-hash",
                State = $"{Guid.NewGuid()}_refId:1234",
                UserId = 12345,
            };

            // When
            Action act = () =>
                learningHubLinkService.ValidateLinkingRequestAndExtractDestinationResourceId(
                    linkLearningHubRequest,
                    "invalid-guid"
                );

            // Then
            act.Should().Throw<LearningHubLinkingRequestException>()
                .WithMessage("Invalid Learning Hub linking request session identifier.");
        }

        [Test]
        public void
            ValidateLinkingRequestAndExtractDestinationResourceId_throws_exception_when_verifyHash_returns_false()
        {
            // Given
            var storedSessionIdentifier = Guid.NewGuid();
            var linkLearningHubRequest = new LinkLearningHubRequest
            {
                Hash = "invalid-hash",
                State = $"{storedSessionIdentifier}_refId:56789",
                UserId = 56789,
            };

            // When
            Action act = () => learningHubLinkService.ValidateLinkingRequestAndExtractDestinationResourceId(
                linkLearningHubRequest,
                storedSessionIdentifier.ToString()
            );

            // Then
            act.Should().Throw<LearningHubLinkingRequestException>().WithMessage("Invalid Learning Hub UserId hash.");
        }

        [Test]
        public void
            ValidateLinkingRequestAndExtractDestinationResourceId_throws_exception_when_sessionIdentifier_does_not_match()
        {
            // Given
            var storedSessionIdentifier = Guid.NewGuid();
            var linkLearningHubRequest = new LinkLearningHubRequest
            {
                Hash = "valid-hash",
                State = $"{Guid.NewGuid()}_refId:1234",
                UserId = 12345,
            };

            // When
            Action act = () => learningHubLinkService.ValidateLinkingRequestAndExtractDestinationResourceId(
                linkLearningHubRequest,
                storedSessionIdentifier.ToString()
            );

            // Then
            act.Should().Throw<LearningHubLinkingRequestException>()
                .WithMessage("Invalid Learning Hub linking session.");
        }

        [Test]
        public void ValidateLinkingRequestAndExtractDestinationResourceId_throws_exception_when_state_is_incomplete()
        {
            // Given
            var storedSessionIdentifier = Guid.NewGuid();
            var linkLearningHubRequest = new LinkLearningHubRequest
            {
                Hash = "valid-hash",
                State = $"{storedSessionIdentifier}",
                UserId = 12345,
            };

            // When
            Action act = () => learningHubLinkService.ValidateLinkingRequestAndExtractDestinationResourceId(
                linkLearningHubRequest,
                storedSessionIdentifier.ToString()
            );

            // Then
            act.Should().Throw<LearningHubLinkingRequestException>().WithMessage("Invalid Learning Hub linking state.");
        }

        [Test]
        public void
            ValidateLinkingRequestAndExtractDestinationResourceId_throws_exception_when_state_sessionIdentifier_is_not_guid()
        {
            // Given
            var storedSessionIdentifier = Guid.NewGuid();
            var linkLearningHubRequest = new LinkLearningHubRequest
            {
                Hash = "valid-hash",
                State = "not-a-guid_refId:1234",
                UserId = 12345,
            };

            // When
            Action act = () => learningHubLinkService.ValidateLinkingRequestAndExtractDestinationResourceId(
                linkLearningHubRequest,
                storedSessionIdentifier.ToString()
            );

            // Then
            act.Should().Throw<LearningHubLinkingRequestException>()
                .WithMessage("Invalid Learning Hub linking session.");
        }

        [Test]
        public void
            ValidateLinkingRequestAndExtractDestinationResourceId_returns_null_when_resource_id_could_not_be_parsed()
        {
            // Given
            var storedSessionIdentifier = Guid.NewGuid();
            var linkLearningHubRequest = new LinkLearningHubRequest
            {
                Hash = "valid-hash",
                State = $"{storedSessionIdentifier}_refId:badInteger",
                UserId = 12345,
            };

            // When
            var result = learningHubLinkService.ValidateLinkingRequestAndExtractDestinationResourceId(
                linkLearningHubRequest,
                storedSessionIdentifier.ToString()
            );

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void
            ValidateLinkingRequestAndExtractDestinationResourceId_returns_resourceId_when_request_parsed_successfully()
        {
            // Given
            var storedSessionIdentifier = Guid.NewGuid();
            var linkLearningHubRequest = new LinkLearningHubRequest
            {
                Hash = "valid-hash",
                State = $"{storedSessionIdentifier}_refId:1234",
                UserId = 12345,
            };

            // When
            var result = learningHubLinkService.ValidateLinkingRequestAndExtractDestinationResourceId(
                linkLearningHubRequest,
                storedSessionIdentifier.ToString()
            );

            // Then
            result.Should().Be(1234);
        }

        [Test]
        public void IsLearningHubAccountLinked_returns_true_when_user_has_authid()
        {
            // Given
            const int delegateId = 3;
            const int learningHubAuthId = 1;

            A.CallTo(() => userDataService.GetDelegateUserLearningHubAuthId(delegateId))
                .Returns(learningHubAuthId);

            // When
            var result = learningHubLinkService.IsLearningHubAccountLinked(delegateId);

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void DelegateUserLearningHubAccountIsLinked_returns_false_when_user_does_not_have_authid()
        {
            // Given
            const int delegateId = 3;

            A.CallTo(() => userDataService.GetDelegateUserLearningHubAuthId(delegateId))
                .Returns(null);

            // When
            var result = learningHubLinkService.IsLearningHubAccountLinked(delegateId);

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void LinkLearningHubAccountIfNotLinked_calls_service_to_update_delegate_authId_when_not_linked()
        {
            // Given
            const int delegateId = 3;
            const int learningHubAuthId = 1;

            A.CallTo(() => userDataService.GetDelegateUserLearningHubAuthId(delegateId))
                .Returns(null);

            // When
            learningHubLinkService.LinkLearningHubAccountIfNotLinked(delegateId, learningHubAuthId);

            // Then
            A.CallTo(() => userDataService.SetDelegateUserLearningHubAuthId(A<int>._, A<int>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void
            LinkLearningHubAccountIfNotLinked_does_not_call_service_to_update_delegate_authId_when_already_linked()
        {
            // Given
            const int delegateId = 3;
            const int learningHubAuthId = 1;

            A.CallTo(() => userDataService.GetDelegateUserLearningHubAuthId(delegateId))
                .Returns(learningHubAuthId);

            // When
            learningHubLinkService.LinkLearningHubAccountIfNotLinked(delegateId, learningHubAuthId);

            // Then
            A.CallTo(() => userDataService.SetDelegateUserLearningHubAuthId(A<int>._, A<int>._))
                .MustNotHaveHappened();
        }

        [Test]
        public void GetLoginUrlForDelegateAuthIdAndResourceUrl_returns_expected_value()
        {
            // Given
            const string resourceUrl = "De/Humani/Corporis/Fabrica";
            const int authId = 2;

            // When
            var url = learningHubLinkService.GetLoginUrlForDelegateAuthIdAndResourceUrl(resourceUrl, authId);

            // Then
            url.Should().Be(
                $"www.example.com/insert-log?clientCode=test&userId={authId}&hash=hash_brown&endClientUrl={HttpUtility.UrlEncode(resourceUrl)}"
            );
        }

        [Test]
        public void GetLinkingUrlForResource_returns_expected_value()
        {
            // Given
            const int referenceId = 5;
            const string sessionReferenceId = "abcdefghijklmnopqrstuvwxyz";

            // When
            var url = learningHubLinkService.GetLinkingUrlForResource(referenceId, sessionReferenceId);

            // Then
            url.Should().Be(
                $"www.example.com/to-the-past?clientCode=test&state={sessionReferenceId}_refId%3a{referenceId}&hash=hash_brown"
            );
        }
    }
}
