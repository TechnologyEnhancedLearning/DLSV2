namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Signposting;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class LearningHubLinkServiceTests
    {
        private ILearningHubLinkService learningHubLinkService = null!;
        private ILearningHubSsoSecurityService learningHubSsoSecurityService = null!;

        [SetUp]
        public void Setup()
        {
            learningHubSsoSecurityService = A.Fake<ILearningHubSsoSecurityService>();
            A.CallTo(() => learningHubSsoSecurityService.VerifyHash("56789", A<string>._)).Returns(false);
            A.CallTo(() => learningHubSsoSecurityService.VerifyHash("12345", A<string>._)).Returns(true);
            learningHubLinkService = new LearningHubLinkService(learningHubSsoSecurityService);
        }


        [Test]
        public void ValidateLinkingRequestAndExtractDestinationResourceId_throws_exception_when_stored_sessionIdentifier_is_invalid()
        {
            // Given
            var linkLearningHubRequest = new LinkLearningHubRequest
            {
                Hash = "some-valid-hash",
                State = $"{Guid.NewGuid()}_refId:1234",
                UserId = 12345,
            };

            // When
            Action act = () => learningHubLinkService.ValidateLinkingRequestAndExtractDestinationResourceId(linkLearningHubRequest, "invalid-guid");

            // Then
            act.Should().Throw<LearningHubLinkingRequestException>().WithMessage("Invalid Learning Hub linking request session identifier.");
        }

        [Test]
        public void ValidateLinkingRequestAndExtractDestinationResourceId_throws_exception_when_verifyHash_returns_false()
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
            Action act = () => learningHubLinkService.ValidateLinkingRequestAndExtractDestinationResourceId(linkLearningHubRequest, storedSessionIdentifier.ToString());

            // Then
            act.Should().Throw<LearningHubLinkingRequestException>().WithMessage("Invalid Learning Hub UserId hash.");
        }

        [Test]
        public void ValidateLinkingRequestAndExtractDestinationResourceId_throws_exception_when_sessionIdentifier_does_not_match()
        {
            // Given
            var storedSessionIdentifier = Guid.NewGuid();
            var linkLearningHubRequest = new LinkLearningHubRequest
            {
                Hash = "some-valid-hash",
                State = $"{Guid.NewGuid()}_refId:1234",
                UserId = 12345,
            };

            // When
            Action act = () => learningHubLinkService.ValidateLinkingRequestAndExtractDestinationResourceId(linkLearningHubRequest, storedSessionIdentifier.ToString());

            // Then
            act.Should().Throw<LearningHubLinkingRequestException>().WithMessage("Invalid Learning Hub linking session.");
        }

        [Test]
        public void ValidateLinkingRequestAndExtractDestinationResourceId_throws_exception_when_state_is_incomplete()
        {
            // Given
            var storedSessionIdentifier = Guid.NewGuid();
            var linkLearningHubRequest = new LinkLearningHubRequest
            {
                Hash = "some-valid-hash",
                State = $"{storedSessionIdentifier}",
                UserId = 12345,
            };

            // When
            Action act = () => learningHubLinkService.ValidateLinkingRequestAndExtractDestinationResourceId(linkLearningHubRequest, storedSessionIdentifier.ToString());

            // Then
            act.Should().Throw<LearningHubLinkingRequestException>().WithMessage("Invalid Learning Hub linking state.");
        }

        [Test]
        public void ValidateLinkingRequestAndExtractDestinationResourceId_throws_exception_when_state_sessionIdentifier_is_not_guid()
        {
            // Given
            var storedSessionIdentifier = Guid.NewGuid();
            var linkLearningHubRequest = new LinkLearningHubRequest
            {
                Hash = "some-valid-hash",
                State = "not-a-guid_refId:1234",
                UserId = 12345,
            };

            // When
            Action act = () => learningHubLinkService.ValidateLinkingRequestAndExtractDestinationResourceId(linkLearningHubRequest, storedSessionIdentifier.ToString());

            // Then
            act.Should().Throw<LearningHubLinkingRequestException>().WithMessage("Invalid Learning Hub linking session.");
        }

        [Test]
        public void ValidateLinkingRequestAndExtractDestinationResourceId_returns_null_when_resource_id_could_not_be_parsed()
        {
            // Given
            var storedSessionIdentifier = Guid.NewGuid();
            var linkLearningHubRequest = new LinkLearningHubRequest
            {
                Hash = "some-valid-hash",
                State = $"{storedSessionIdentifier}_refId:badInteger",
                UserId = 12345,
            };

            // When
            var result = learningHubLinkService.ValidateLinkingRequestAndExtractDestinationResourceId(linkLearningHubRequest, storedSessionIdentifier.ToString());

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void ValidateLinkingRequestAndExtractDestinationResourceId_returns_resourceId_when_request_parsed_successfully()
        {
            // Given
            var storedSessionIdentifier = Guid.NewGuid();
            var linkLearningHubRequest = new LinkLearningHubRequest
            {
                Hash = "some-valid-hash",
                State = $"{storedSessionIdentifier}_refId:1234",
                UserId = 12345,
            };

            // When
            var result = learningHubLinkService.ValidateLinkingRequestAndExtractDestinationResourceId(linkLearningHubRequest, storedSessionIdentifier.ToString());

            // Then
            result.Should().Be(1234);
        }
    }
}
