namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class CentresDataServiceTests
    {
        private CentresDataService centresDataService;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<CentresDataService>>();
            centresDataService = new CentresDataService(connection, logger);
        }

        [Test]
        public void Get_banner_text_should_return_the_correct_value()
        {
            // When
            var result = centresDataService.GetBannerText(2);

            // Then
            result.Should().Be("xxxxxxxxxxxxxxxxxxxx");
        }

        [Test]
        public void Get_banner_text_should_return_null_when_the_centre_does_not_exist()
        {
            // When
            var result = centresDataService.GetBannerText(1);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_banner_text_should_return_null_when_the_banner_text_is_null()
        {
            // When
            var result = centresDataService.GetBannerText(3);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_centre_name_should_return_the_correct_value()
        {
            // When
            var result = centresDataService.GetCentreName(2);

            // Then
            result.Should().Be("North West Boroughs Healthcare NHS Foundation Trust");
        }

        [Test]
        public void Get_centre_name_should_return_null_when_the_centre_does_not_exist()
        {
            // When
            var result = centresDataService.GetCentreName(1);

            // Then
            result.Should().BeNull();
        }

        [Test]
        public void Get_active_centres_should_contain_an_active_centre()
        {
            // When
            var result = centresDataService.GetActiveCentresAlphabetical().ToList();

            // Then
            result.Contains((2, "North West Boroughs Healthcare NHS Foundation Trust")).Should().BeTrue();
        }

        [Test]
        public void Get_active_centres_should_not_contain_an_inactive_centre()
        {
            // When
            var result = centresDataService.GetActiveCentresAlphabetical().ToList();

            // Then
            result.Contains((6, "Closed_Basildon and Thurrock University Hospitals NHS Foundation Trust"))
                .Should().BeFalse();
        }

        [Test]
        public void GetCentreDetailsById_should_return_the_correct_values()
        {
            // Given
            var expectedCentreDetails = CentreTestHelper.GetDefaultCentre(
                centreLogo: Convert.FromBase64String(CentreLogoTestHelper.DefaultCentreLogoAsBase64String));

            // When
            var result = centresDataService.GetCentreDetailsById(2);

            // Then
            result.Should().BeEquivalentTo(expectedCentreDetails);
        }

        [Test]
        public void GetCentreDetailsById_should_return_null_centre_logo_if_logo_is_empty_image()
        {
            // When
            var result = centresDataService.GetCentreDetailsById(36);

            // Then
            result.CentreLogo.Should().BeNull();
        }

        [Test]
        public void GetCentreIPPrefix_should_return_empty_array_when_the_centre_does_not_exist()
        {
            // When
            var result = centresDataService.GetCentreIPPrefix(1);

            // Then
            result.Should().BeEmpty();
        }

        [Test]
        public void GetCentreIPPrefix_should_return_IP_Prefix()
        {
            // When
            var expectedResult = new[] { "194.176.105" };
            var result = centresDataService.GetCentreIPPrefix(2);
            
            // Then
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
