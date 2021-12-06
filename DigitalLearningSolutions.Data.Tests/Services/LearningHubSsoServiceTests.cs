namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Signposting.LinkLearningHubSso;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class LearningHubSsoServiceTests
    {
        private ILearningHubSsoSecurityHelper learningHubSsoSecurityHelper = null!;
        private ILearningHubSsoService learningHubSsoService = null!;

        [SetUp]
        public void Setup()
        {
            learningHubSsoSecurityHelper = A.Fake<ILearningHubSsoSecurityHelper>();
            learningHubSsoService = new LearningHubSsoService(learningHubSsoSecurityHelper);
        }

        [Test]
        public void ParseSsoAccountLinkingRequest_throws_exception_when_verifyHash_returns_false()
        {
            // Given
            var linkLearningHubRequest = new LinkLearningHubRequest();
            A.CallTo(() => learningHubSsoSecurityHelper.VerifyHash(A<string>._, A<string>._))
                .Returns(false);

            // When
            Action act = () => learningHubSsoService.ParseSsoAccountLinkingRequest(linkLearningHubRequest);

            // Then
            act.Should().Throw<LearningHubLinkingRequestException>();
        }

        [Test]
        public void ParseSsoAccountLinkingRequest_throws_exception_when_stored_sessionIdentifier_is_null()
        {
            // Given
            var linkLearningHubRequest = new LinkLearningHubRequest
            {
                State = $"{Guid.NewGuid()}_refId:1234",
            };
            A.CallTo(() => learningHubSsoSecurityHelper.VerifyHash(A<string>._, A<string>._))
                .Returns(true);

            // When
            Action act = () => learningHubSsoService.ParseSsoAccountLinkingRequest(linkLearningHubRequest);

            // Then
            act.Should().Throw<LearningHubLinkingRequestException>();
        }

        [Test]
        public void ParseSsoAccountLinkingRequest_throws_exception_when_sessionIdentifier_does_not_match()
        {
            // Given
            var linkLearningHubRequest = new LinkLearningHubRequest
            {
                SessionIdentifier = Guid.NewGuid(),
                State = $"{Guid.NewGuid()}_refId:1234",
            };

            A.CallTo(() => learningHubSsoSecurityHelper.VerifyHash(A<string>._, A<string>._))
                .Returns(true);

            // When
            Action act = () => learningHubSsoService.ParseSsoAccountLinkingRequest(linkLearningHubRequest);

            // Then
            act.Should().Throw<LearningHubLinkingRequestException>();
        }

        [Test]
        public void ParseSsoAccountLinkingRequest_throws_exception_when_state_is_incomplete()
        {
            // Given
            var linkLearningHubRequest = new LinkLearningHubRequest
            {
                SessionIdentifier = Guid.NewGuid(),
                State = $"{Guid.NewGuid()}",
            };

            A.CallTo(() => learningHubSsoSecurityHelper.VerifyHash(A<string>._, A<string>._))
                .Returns(true);

            // When
            Action act = () => learningHubSsoService.ParseSsoAccountLinkingRequest(linkLearningHubRequest);

            // Then
            act.Should().Throw<LearningHubLinkingRequestException>();
        }

        [Test]
        public void ParseSsoAccountLinkingRequest_throws_exception_when_state_sessionIdentifier_is_not_guid()
        {
            // Given
            var linkLearningHubRequest = new LinkLearningHubRequest
            {
                SessionIdentifier = Guid.NewGuid(),
                State = "no-a-guid_refId:1234",
            };

            A.CallTo(() => learningHubSsoSecurityHelper.VerifyHash(A<string>._, A<string>._))
                .Returns(true);

            // When
            Action act = () => learningHubSsoService.ParseSsoAccountLinkingRequest(linkLearningHubRequest);

            // Then
            act.Should().Throw<LearningHubLinkingRequestException>();
        }

        [Test]
        public void ParseSsoAccountLinkingRequest_returns_null_when_resource_id_could_not_be_parsed()
        {
            // Given
            var sessionIdentifier = Guid.NewGuid();
            var linkLearningHubRequest = new LinkLearningHubRequest
            {
                SessionIdentifier = sessionIdentifier,
                State = $"{sessionIdentifier}_refId:badInteger",
            };

            A.CallTo(() => learningHubSsoSecurityHelper.VerifyHash(A<string>._, A<string>._))
                .Returns(true);

            // When
            var result = learningHubSsoService.ParseSsoAccountLinkingRequest(linkLearningHubRequest);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void ParseSsoAccountLinkingRequest_returns_resourceId_when_request_parsed_successfully()
        {
            // Given
            var sessionIdentifier = Guid.NewGuid();
            var linkLearningHubRequest = new LinkLearningHubRequest
            {
                SessionIdentifier = sessionIdentifier,
                State = $"{sessionIdentifier}_refId:1234",
            };

            A.CallTo(() => learningHubSsoSecurityHelper.VerifyHash(A<string>._, A<string>._))
                .Returns(true);

            // When
            var result = learningHubSsoService.ParseSsoAccountLinkingRequest(linkLearningHubRequest);

            // Then
            result.Should().Be(1234);
        }
    }
}
