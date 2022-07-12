namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    internal class LogoServiceTests
    {
        private LogoService logoService;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            logoService = new LogoService(connection);
        }

        [Test]
        public void GetLogo_Should_Return_Centre_Logo()
        {
            // When
            var result = logoService.GetLogo(25, 1176);

            // Then
            result.Should().NotBeNull();
            result.LogoUrl.Should().NotBeNullOrEmpty();
            result.Name.Should().Be("Royal Cornwall Hospitals NHS Trust");
        }

        [Test]
        public void GetLogo_Should_Return_Centre_Logo_When_CustomisationId_Is_Null()
        {
            // When
            var result = logoService.GetLogo(25, null);

            // Then
            result.Should().NotBeNull();
            result.LogoUrl.Should().NotBeNullOrEmpty();
            result.Name.Should().Be("Royal Cornwall Hospitals NHS Trust");
        }

        [Test]
        public void GetLogo_Should_Return_Brand_Logo_When_No_Centre_Logo()
        {
            // When
            var result = logoService.GetLogo(3, 7793);

            // Then
            result.Should().NotBeNull();
            result.LogoUrl.Should().NotBeNullOrEmpty();
            result.Name.Should().Be("IT Skills Pathway");
        }

        [Test]
        public void GetLogo_Should_Return_Null_LogoUrl_When_No_Logo_Is_Available()
        {
            // When
            var result = logoService.GetLogo(3, 100);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetLogo_Should_Return_Null_LogoUrl_When_CentreId_Is_Invalid()
        {
            // When
            var result = logoService.GetLogo(-100, -100);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetLogo_Should_Return_Null_LogoUrl_When_Inputs_Are_Null()
        {
            // When
            var result = logoService.GetLogo(null, null);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void GetLogo_Should_Return_Centre_Logo_When_Course_And_Centre_Do_Not_Match()
        {
            // When
            var result = logoService.GetLogo(25, 7793);

            // Then
            result.Should().NotBeNull();
            result.LogoUrl.Should().NotBeNullOrEmpty();
            result.Name.Should().Be("Royal Cornwall Hospitals NHS Trust");
        }
    }
}
