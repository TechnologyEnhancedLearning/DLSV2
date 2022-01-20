namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Web;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class LearningHubAuthApiUrlServiceTests
    {
        private LearningHubAuthApiUrlService service = null!;
        private IGuidService guidService = null!;

        [SetUp]
        public void SetUp()
        {
            guidService = A.Fake<IGuidService>();
            var securityService = A.Fake<ILearningHubSsoSecurityService>();
            var config = A.Fake<IConfiguration>();

            A.CallTo(() => config[$"{ConfigHelper.LearningHubSsoSectionKey}:{ConfigHelper.LearningHubAuthApiBaseUrl}"]).Returns("www.example.com");
            A.CallTo(() => config[$"{ConfigHelper.LearningHubSsoSectionKey}:{ConfigHelper.LearningHubAuthApiClientCode}"]).Returns("test");
            A.CallTo(() => config[$"{ConfigHelper.LearningHubSsoSectionKey}:{ConfigHelper.LearningHubAuthApiLoginEndpoint}"]).Returns("/insert-log");
            A.CallTo(() => config[$"{ConfigHelper.LearningHubSsoSectionKey}:{ConfigHelper.LearningHubAuthApiLinkingEndpoint}"]).Returns("/to-the-past");

            A.CallTo(() => securityService.GenerateHash(A<string>._)).Returns("hash_brown");
            A.CallTo(() => guidService.NewGuid()).Returns(Guid.Empty);

            service = new LearningHubAuthApiUrlService(config, guidService, securityService);
        }

        [Test]
        public void GetLoginUrlForDelegateAuthIdAndResourceUrl_returns_expected_value()
        {
            // Given
            var resourceUrl = "De/Humani/Corporis/Fabrica";
            var authId = 2;

            // When
            var url = service.GetLoginUrlForDelegateAuthIdAndResourceUrl(resourceUrl, authId);

            // Then
            url.Should().Be(
                $"www.example.com/insert-log?clientcode=test&userid={authId}&hash=hash_brown&endclientUrl={HttpUtility.UrlEncode(resourceUrl)}"
            );
        }

        [Test]
        public void GetLinkingUrlForResource_returns_expected_value()
        {
            // Given
            var referenceId = 5;

            // When
            var url = service.GetLinkingUrlForResource(referenceId);

            // Then
            url.Should().Be(
                $"www.example.com/to-the-past?clientcode=test&state={Guid.Empty}_refId:{referenceId}&hash=hash_brown"
            );
        }
    }
}
