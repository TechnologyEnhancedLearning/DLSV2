namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;
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
            var expectedCentreDetails = new Centre
            {
                CentreId = 2,
                CentreName = "North West Boroughs Healthcare NHS Foundation Trust",
                RegionId = 5,
                RegionName = "North West",
                NotifyEmail = "notify@test.com",
                BannerText = "xxxxxxxxxxxxxxxxxxxx",
                SignatureImage = null,
                CentreLogo = Convert.FromBase64String(CentreLogoTestHelper.DefaultCentreLogoAsBase64String),
                ContactForename = "xxxxx",
                ContactSurname = "xxxx",
                ContactEmail = "nybwhudkra@ic.vs",
                ContactTelephone = "xxxxxxxxxxxx",
                CentreTelephone = "01925 664457",
                CentreEmail = "5bp.informaticstraining.5bp.nhs.uk",
                CentrePostcode = "WA2 8WA",
                OpeningHours = "9.30am - 4.30pm",
                CentreWebAddress = null,
                OrganisationsCovered = "Northwest Boroughs Healthcare NHS Foundation Trust",
                TrainingVenues = "Hollins Park House\nHollins Lane\nWinwick\nWarrington WA2 8WA",
                OtherInformation = null
            };

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
        public void GetCentreName_should_return_null_when_the_centre_does_not_exist()
        {
            // When
            var result = centresDataService.GetCentreName(1);

            // Then
            result.Should().BeNull();
        }
    }
}
