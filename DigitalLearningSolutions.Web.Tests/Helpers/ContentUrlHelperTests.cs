namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using DigitalLearningSolutions.Web.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    class ContentUrlHelperTests
    {
        private IConfiguration config;
        private const string BaseUrl = "https://example.com";

        [SetUp]
        public void SetUp()
        {
            config = A.Fake<IConfiguration>();
            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns(BaseUrl);
        }

        [Test]
        public void GetContentPath_should_parse_absolute_url()
        {
            // Given
            const string videoPath = "https://example.com/testVideo.mp4";

            // When
            var parsedPath = ContentUrlHelper.GetContentPath(config, videoPath);

            // Then
            parsedPath.Should().Be(videoPath);
        }

        [Test]
        public void GetContentPath_should_parse_protocol_relative_url()
        {
            // Given
            const string videoPath = "example.com/testVideo.mp4";

            // When
            var parsedPath = ContentUrlHelper.GetContentPath(config, videoPath);

            // Then
            parsedPath.Should().Be($"https://{videoPath}");
        }

        [Test]
        public void GetContentPath_should_parse_relative_path()
        {
            // Given
            const string videoPath = "/testVideo.mp4";

            // When
            var parsedPath = ContentUrlHelper.GetContentPath(config, videoPath);

            // Then
            parsedPath.Should().Be(BaseUrl + videoPath);
        }

        [TestCase("https://example.com/testVideo.mp4", "https://example.com/testVideo.mp4")]
        [TestCase("example.com/testVideo.mp4", "https://example.com/testVideo.mp4")]
        [TestCase("/testVideo.mp4", BaseUrl + "/testVideo.mp4")]
        [TestCase(null, null)]
        public void GetNullableContentPath_should_parse_paths(
            string? givenVideoPath,
            string? expectedParsedPath
        )
        {
            // When
            var actualParsedPath = ContentUrlHelper.GetNullableContentPath(config, givenVideoPath);

            // Then
            actualParsedPath.Should().Be(expectedParsedPath);
        }
    }
}
