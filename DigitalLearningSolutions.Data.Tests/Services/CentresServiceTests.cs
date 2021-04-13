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
    }
}
