namespace DigitalLearningSolutions.Web.Tests.ViewModels.Support
{
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Support;
    using FluentAssertions;
    using NUnit.Framework;

    public class SupportTicketsViewModelTests
    {
        [Test]
        public void SupportTicketsViewModel_constructor_should_set_properties_correctly()
        {
            // Given
            var expectedIframeUrl = "https://system.base/tracking/tickets?nonav=true";

            // When
            var model = new SupportTicketsViewModel(
                DlsSubApplication.TrackingSystem,
                SupportPage.SupportTickets,
                "https://system.base"
            );

            // Then
            model.SupportTicketsIframeUrl.Should().Be(expectedIframeUrl);
        }
    }
}
