namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class SupportLinksHelperTests
    {
        [Test]
        public void GetChangeRequestsUrl_returns_expected()
        {
            // Given
            const string expectedChangeRequestsUrl = "https://github.com/TechnologyEnhancedLearning/DLSV2/projects/1?fullscreen=true";

            // Then
            SupportLinksHelper.GetChangeRequestsUrl.Should().Be(expectedChangeRequestsUrl);
        }

        [Test]
        public void GetHelpUrl_returns_expected()
        {
            // Given
            const string baseUrl = "base.com";
            const string expectedHelpUrl = "base.com/help/Introduction.html";

            // Then
            SupportLinksHelper.GetHelpUrl(baseUrl).Should().Be(expectedHelpUrl);
        }

        [Test]
        public void GetSupportTicketsIframeUrl_returns_expected()
        {
            // Given
            const string baseUrl = "base.com";
            const string expectedIframeUrl = "base.com/tracking/tickets?nonav=true";

            // Then
            SupportLinksHelper.GetSupportTicketsIframeUrl(baseUrl).Should().Be(expectedIframeUrl);
        }
    }
}
