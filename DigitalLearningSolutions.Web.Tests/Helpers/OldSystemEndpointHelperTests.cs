namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using DigitalLearningSolutions.Web.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class OldSystemEndpointHelperTests
    {
        private IConfiguration config = null!;

        [SetUp]
        public void Setup()
        {
            config = A.Fake<IConfiguration>();

            A.CallTo(() => config["CurrentSystemBaseUrl"]).Returns("https://www.dls.nhs.uk");
        }

        [Test]
        public void GetEvaluateUrl_returns_expected()
        {
            // Given
            const int progressId = 1;
            const string expected = "https://www.dls.nhs.uk/tracking/finalise?ProgressID=1";

            // Then
            OldSystemEndpointHelper.GetEvaluateUrl(config, progressId).Should().Be(expected);
        }

        [Test]
        public void GetTrackingUrl_returns_expected()
        {
            // Given
            const string expected = "https://www.dls.nhs.uk/tracking/tracker";

            // Then
            OldSystemEndpointHelper.GetTrackingUrl(config).Should().Be(expected);
        }

        [Test]
        public void GetScormPlayerUrl_returns_expected()
        {
            // Given
            const string expected = "https://www.dls.nhs.uk/scoplayer/sco";

            // Then
            OldSystemEndpointHelper.GetScormPlayerUrl(config).Should().Be(expected);
        }

        [Test]
        public void GetDownloadSummaryUrl_returns_expected()
        {
            // Given
            const int progressId = 1;
            const string expected = "https://www.dls.nhs.uk/tracking/summary?ProgressID=1";

            // Then
            OldSystemEndpointHelper.GetDownloadSummaryUrl(config, progressId).Should().Be(expected);
        }

        [Test]
        public void GetConsolidationPathUrl_returns_expected()
        {
            // Given
            const string consolidationPath = "path";
            const string expected = "https://www.dls.nhs.uk/tracking/dlconsolidation?client=path";

            // Then
            OldSystemEndpointHelper.GetConsolidationPathUrl(config, consolidationPath).Should().Be(expected);
        }
    }
}
