namespace DigitalLearningSolutions.Web.Tests.Controllers.Support
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Web.Controllers.Support;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class SupportControllerTests
    {
        private IConfiguration configuration = null!;

        [SetUp]
        public void Setup()
        {
            configuration = A.Fake<IConfiguration>();
        }

        [Test]
        public async Task Frameworks_Support_page_should_be_shown_for_valid_claims()
        {
            // Given
            var controller = new SupportController(configuration)
                .WithDefaultContext()
                .WithMockUser(true, isCentreAdmin: false, isFrameworkDeveloper: true);

            // When
            var result = await controller.Index("Frameworks");

            // Then
            result.Should().BeViewResult().WithViewName("Support");
        }

        [Test]
        public async Task Home_page_should_be_shown_when_accessing_tracking_system_support_without_appropriate_claims()
        {
            // Given
            var controller = new SupportController(configuration)
                .WithDefaultContext()
                .WithMockUser(true, isCentreAdmin: false, isFrameworkDeveloper: true);

            // When
            var result = await controller.Index("TrackingSystem");

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("Home").WithActionName("Index");
        }

        [Test]
        public async Task Home_page_should_be_shown_when_accessing_frameworks_support_without_appropriate_claims()
        {
            // Given
            var controller = new SupportController(configuration)
                .WithDefaultContext()
                .WithMockUser(true, isCentreAdmin: true, isFrameworkDeveloper: false);

            // When
            var result = await controller.Index("Frameworks");

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("Home").WithActionName("Index");
        }
    }
}
