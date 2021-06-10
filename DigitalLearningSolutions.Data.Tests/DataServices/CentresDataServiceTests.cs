﻿namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class CentresDataServiceTests
    {
        private CentresDataService centresDataService = null!;

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
            var expectedCentreDetails = CentreTestHelper.GetDefaultCentre
            (
                centreLogo: Convert.FromBase64String(CentreLogoTestHelper.DefaultCentreLogoAsBase64String)
            );

            // When
            var result = centresDataService.GetCentreDetailsById(2);

            // Then
            result.Should().BeEquivalentTo(expectedCentreDetails);
        }

        [Test]
        public void GetCentreDetailsById_should_return_null_centre_logo_if_logo_is_empty_image()
        {
            // When
            var result = centresDataService.GetCentreDetailsById(36)!;

            // Then
            result.CentreLogo.Should().BeNull();
        }

        [Test]
        public void GetCentreIPPrefix_should_return_empty_array_when_the_centre_does_not_exist()
        {
            // When
            var result = centresDataService.GetCentreIpPrefixes(1);

            // Then
            result.Should().BeEmpty();
        }

        [Test]
        public void GetCentreIPPrefix_should_return_IP_Prefix()
        {
            // When
            var expectedResult = new[] { "194.176.105" };
            var result = centresDataService.GetCentreIpPrefixes(2);

            // Then
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void UpdateCentreManagerDetails_updates_centre()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const string firstName = "TestFirstName";
                const string lastName = "TestLastName";
                const string email = "test@email.com";
                const string telephone = "0123456789";

                // When
                centresDataService.UpdateCentreManagerDetails(2, firstName, lastName, email, telephone);
                var updatedCentre = centresDataService.GetCentreDetailsById(2)!;

                // Then
                using (new AssertionScope())
                {
                    updatedCentre.ContactForename.Should().BeEquivalentTo(firstName);
                    updatedCentre.ContactSurname.Should().BeEquivalentTo(lastName);
                    updatedCentre.ContactEmail.Should().BeEquivalentTo(email);
                    updatedCentre.ContactTelephone.Should().BeEquivalentTo(telephone);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void UpdateCentreWebsiteDetails_updates_centre()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const string telephone = "0118999 88199 9119725   3";
                const string email = "totallyrealemail@noreally.itis";
                const string postcode = "POST CDE";
                const bool showOnMap = false;
                const double longitude = -1.1493326;
                const double latitude = 52.9534611;
                const string openingHours = "2:30am - 2:31am Sundays";
                const string webAddress = "really.boring.website";
                const string organisationsCovered = "Megadodo Publications, Infinidim Enterprises";
                const string trainingVenues = "Olympus Mons, Tharsis, Mars";
                const string otherInformation = "This is not the information you're looking for";

                // When
                centresDataService.UpdateCentreWebsiteDetails
                (
                    2,
                    postcode,
                    showOnMap,
                    longitude,
                    latitude,
                    telephone,
                    email,
                    openingHours,
                    webAddress,
                    organisationsCovered,
                    trainingVenues,
                    otherInformation
                );
                var updatedCentre = centresDataService.GetCentreDetailsById(2)!;

                // Then
                using (new AssertionScope())
                {
                    updatedCentre.CentreTelephone.Should().BeEquivalentTo(telephone);
                    updatedCentre.CentreEmail.Should().BeEquivalentTo(email);
                    updatedCentre.CentrePostcode.Should().BeEquivalentTo(postcode);
                    updatedCentre.ShowOnMap.Should().BeFalse();
                    updatedCentre.Longitude.Should().Be(longitude);
                    updatedCentre.Latitude.Should().Be(latitude);
                    updatedCentre.OpeningHours.Should().BeEquivalentTo(openingHours);
                    updatedCentre.CentreWebAddress.Should().BeEquivalentTo(webAddress);
                    updatedCentre.OrganisationsCovered.Should().BeEquivalentTo(organisationsCovered);
                    updatedCentre.TrainingVenues.Should().BeEquivalentTo(trainingVenues);
                    updatedCentre.OtherInformation.Should().BeEquivalentTo(otherInformation);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }
    }
}
