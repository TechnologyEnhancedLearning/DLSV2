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

        [TestCase("https://example.com/testVideo.mp4")]
        [TestCase("example.com/testVideo.mp4")]
        [TestCase("/testVideo.mp4")]
        public void GetNullableContentPath_should_parse_paths_when_not_null(string videoPath)
        {
            // Given
            var expectedParsedPath = ContentUrlHelper.GetContentPath(config, videoPath);

            // When
            var actualParsedPath = ContentUrlHelper.GetNullableContentPath(config, videoPath);

            // Then
            actualParsedPath.Should().Be(expectedParsedPath);
        }

        [Test]
        public void GetNullableContentPath_should_return_null_when_path_is_null()
        {
            // When
            var parsedPath = ContentUrlHelper.GetNullableContentPath(config, null);

            // Then
            parsedPath.Should().BeNull();
        }
    }
}
