namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class SupportLinksHelperTests
    {
        [Test]
        public void ChangeRequestsUrl_returns_expected()
        {
            // Given
            const string expectedChangeRequestsUrl = "https://github.com/TechnologyEnhancedLearning/DLSV2/projects/1?fullscreen=true";

            // Then
            SupportLinksHelper.ChangeRequestsUrl.Should().Be(expectedChangeRequestsUrl);
        }

        [Test]
        public void GenerateHelpUrl_returns_expected()
        {
            // Given
            const string baseUrl = "base.com";
            const string expectedHelpUrl = "base.com/help/Introduction.html";

            // Then
            SupportLinksHelper.GenerateHelpUrl(baseUrl).Should().Be(expectedHelpUrl);
        }

        [Test]
        public void GenerateSupportTicketsIframeUrl_returns_expected()
        {
            // Given
            const string baseUrl = "base.com";
            const string expectedIframeUrl = "base.com/tracking/tickets?nonav=true";

            // Then
            SupportLinksHelper.GenerateSupportTicketsIframeUrl(baseUrl).Should().Be(expectedIframeUrl);
        }
    }
}
