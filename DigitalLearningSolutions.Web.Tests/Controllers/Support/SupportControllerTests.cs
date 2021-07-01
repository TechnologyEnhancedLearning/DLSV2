namespace DigitalLearningSolutions.Web.Tests.Controllers.Support
{
    using DigitalLearningSolutions.Web.Controllers.Support;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class SupportControllerTests
    {
        [Test]
        public void Frameworks_Support_page_should_be_shown_for_valid_claims()
        {
            // Given
            var controller = new SupportController()
                .WithDefaultContext()
                .WithMockUser(true, isCentreAdmin: false, isFrameworkDeveloper: true);

            // When
            var result = controller.Index("Frameworks");

            // Then
            result.Should().BeViewResult().WithViewName("Support");
        }

        [Test]
        public void Invalid_application_name_should_redirect_to_404_page()
        {
            // Given
            var controller = new SupportController()
                .WithDefaultContext()
                .WithMockUser(true, isCentreAdmin: true, isFrameworkDeveloper: true);

            // When
            var result = controller.Index("Main");

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Home_page_should_be_shown_when_accessing_tracking_system_support_without_appropriate_claims()
        {
            // Given
            var controller = new SupportController()
                .WithDefaultContext()
                .WithMockUser(true, isCentreAdmin: false, isFrameworkDeveloper: true);

            // When
            var result = controller.Index("TrackingSystem");

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public void Home_page_should_be_shown_when_accessing_frameworks_support_without_appropriate_claims()
        {
            // Given
            var controller = new SupportController()
                .WithDefaultContext()
                .WithMockUser(true, isCentreAdmin: true, isFrameworkDeveloper: false);

            // When
            var result = controller.Index("Frameworks");

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("Home").WithActionName("Index");
        }
    }
}
