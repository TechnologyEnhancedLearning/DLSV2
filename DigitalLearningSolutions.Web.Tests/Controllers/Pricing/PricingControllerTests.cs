namespace DigitalLearningSolutions.Web.Tests.Controllers.Pricing
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Signposting;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class PricingControllerTests
    {
        private IConfiguration configuration = null!;
        private PricingController controller = null!;

        [SetUp]
        public void SetUp()
        {
            configuration = A.Fake<IConfiguration>();

            controller = new PricingController(
                configuration
            ).WithDefaultContext();
        }

        [Test]
        public void Index_returns_view_when_using_pricing_page()
        {
            // Given
            A.CallTo(() => configuration[ConfigHelper.PricingPage]).Returns("true");

            // When
            var result = controller.Index();

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void Index_returns_not_found_when_not_using_pricing_page()
        {
            // Given
            A.CallTo(() => configuration[ConfigHelper.PricingPage]).Returns("false");

            // When
            var result = controller.Index();

            // Then
            result.Should().BeNotFoundResult();
        }
    }
}
