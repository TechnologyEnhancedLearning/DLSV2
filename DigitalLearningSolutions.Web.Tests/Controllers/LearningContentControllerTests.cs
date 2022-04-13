namespace DigitalLearningSolutions.Web.Tests.Controllers
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class LearningContentControllerTests
    {
        private LearningContentController controller = null!;
        private IBrandsService brandsService = null!;

        [SetUp]
        public void SetUp()
        {
            brandsService = A.Fake<IBrandsService>();
            controller = new LearningContentController(brandsService);
        }

        [Test]
        public void Index_returns_not_found_with_null_brand()
        {
            // Given
            A.CallTo(() => brandsService.GetPublicBrandById(1)).Returns(null);

            // When
            var result = controller.Index(1);

            // Then
            result.Should().BeNotFoundResult();
        }
    }
}
