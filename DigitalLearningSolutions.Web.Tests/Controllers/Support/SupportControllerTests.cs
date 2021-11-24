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
    }
}
