namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class CentresServiceTests
    {
        private CentresService centresService;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            centresService = new CentresService(connection);
        }

        [Test]
        public void Get_banner_text_should_return_the_correct_value()
        {
            // When
            var result = centresService.GetBannerText(2);

            // Then
            result.Should().Be("xxxxxxxxxxxxxxxxxxxx");
        }

        [Test]
        public void Get_banner_text_should_return_null_when_the_centre_does_not_exist()
        {
            // When
            var result = centresService.GetBannerText(1);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_banner_text_should_return_null_when_the_banner_text_is_null()
        {
            // When
            var result = centresService.GetBannerText(3);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetCentreLogo_Returns_Correct_Information()
        {
            // When
            var result = centresService.GetCentreLogo(25);

            // Then
            result.Width.Should().Be(121);
            result.Height.Should().Be(55);
            result.LogoUrl.Should().NotBeNullOrEmpty();
            result.CentreName.Should().Be("Royal Cornwall Hospitals NHS Trust");
        }

        [Test]
        public void GetCentreLogo_Scales_Logo_Dimensions()
        {
            // When
            var result = centresService.GetCentreLogo(5);

            // Then
            result.Width.Should().Be(174);
            result.Height.Should().Be(75);
        }

        [Test]
        public void GetCentreLogo_Should_Return_Null_LogoUrl_When_No_Logo()
        {
            // When
            var result = centresService.GetCentreLogo(3);

            // Then
            result.LogoUrl.Should().BeNull();
        }
    }
}
