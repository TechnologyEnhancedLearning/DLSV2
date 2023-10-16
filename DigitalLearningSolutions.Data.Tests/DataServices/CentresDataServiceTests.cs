﻿namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using System;
    using System.Linq;
    using System.Transactions;

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
        public void Get_centres_for_delegate_self_registration_should_contain_an_active_centre()
        {
            // When
            var result = centresDataService.GetCentresForDelegateSelfRegistrationAlphabetical().ToList();

            // Then
            result.Contains((2, "North West Boroughs Healthcare NHS Foundation Trust")).Should().BeTrue();
        }

        [Test]
        public void Get_centres_for_delegate_self_registration_should_not_contain_an_inactive_centre()
        {
            // When
            var result = centresDataService.GetCentresForDelegateSelfRegistrationAlphabetical().ToList();

            // Then
            result.Contains((6, "Closed_Basildon and Thurrock University Hospitals NHS Foundation Trust"))
                .Should().BeFalse();
        }

        [Test]
        public void
            Get_centres_for_delegate_self_registration_should_not_contain_a_centre_where_a_delegate_cannot_self_register()
        {
            // When
            var result = centresDataService.GetCentresForDelegateSelfRegistrationAlphabetical().ToList();

            // Then
            result.Contains((4, "Brighton and Sussex University Hospitals NHS Trust"))
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
                const double latitude = 52.9534611;
                const double longitude = -1.1493326;
                const string openingHours = "2:30am - 2:31am Sundays";
                const string webAddress = "really.boring.website";
                const string organisationsCovered = "Megadodo Publications, Infinidim Enterprises";
                const string trainingVenues = "Olympus Mons, Tharsis, Mars";
                const string otherInformation = "This is not the information you're looking for";

                // When
                centresDataService.UpdateCentreWebsiteDetails(
                    2,
                    postcode,
                    latitude,
                    longitude,
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
                    updatedCentre.Latitude.Should().Be(latitude);
                    updatedCentre.Longitude.Should().Be(longitude);
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

        [Test]
        public void UpdateCentreDetails_updates_centre()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const string notifyEmail = "test@centre.com";
                const string bannerText = "Test banner text";
                var signature = new byte[100];
                var logo = new byte[200];

                // When
                centresDataService.UpdateCentreDetails(2, notifyEmail, bannerText, signature, logo);
                var updatedCentre = centresDataService.GetCentreDetailsById(2)!;

                // Then
                using (new AssertionScope())
                {
                    updatedCentre.NotifyEmail.Should().BeEquivalentTo(notifyEmail);
                    updatedCentre.BannerText.Should().BeEquivalentTo(bannerText);
                    updatedCentre.SignatureImage.Should().BeEquivalentTo(signature);
                    updatedCentre.CentreLogo.Should().BeEquivalentTo(logo);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void GetCentreAutoRegisterValues_should_return_correct_values()
        {
            // When
            var result = centresDataService.GetCentreAutoRegisterValues(2);

            // Then
            result.autoRegistered.Should().BeTrue();
            result.autoRegisterManagerEmail.Should().Be(".vhrnaui@bywdskc");
        }

        [Test]
        public void SetCentreAutoRegistered_should_set_AutoRegistered_true()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const int centreId = 7;

                // When
                centresDataService.SetCentreAutoRegistered(centreId);

                // Then
                centresDataService.GetCentreAutoRegisterValues(centreId).autoRegistered.Should().BeTrue();
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void GetAllCentreSummariesForSuperAdmin_returns_active_and_inactive_summary_details_and_reference_data()
        {
            // When
            (var summaries,var count) = centresDataService.GetAllCentreSummariesForSuperAdmin("",0,1000,0,0,0,"Any");

            // Then
            var activeCentre = summaries.ToList().Single(c => c.Centre.CentreId == 2);
            var inActiveCentre = summaries.ToList().Single(c => c.Centre.CentreId == 6);

            activeCentre.Centre.Active.Should().BeTrue();
            activeCentre.CentreTypes.CentreType.Should().Be("NHS Organisation");
            activeCentre.Regions.RegionName.Should().Be("North West");

            inActiveCentre.Centre.Active.Should().BeFalse();
            inActiveCentre.CentreTypes.CentreType.Should().Be("NHS Organisation");
            inActiveCentre.Regions.RegionName.Should().Be("East Of England");
        }

        [Test]
        public void GetAllCentreSummariesForFindCentre_returns_expected_summary()
        {
            //When
            var summaries = centresDataService.GetAllCentreSummariesForFindCentre().ToList();

            //Then
            summaries.Should().HaveCount(371);
            summaries.Single(s => s.CentreId == 8)!.CentreName.Should().Be("Buckinghamshire Healthcare NHS Trust");
            summaries.Single(s => s.CentreId == 2)!.RegionName.Should().Be("North West");
            summaries.Single(s => s.CentreId == 190)!.Email.Should().BeNull();
            summaries.Single(s => s.CentreId == 205)!.Telephone.Should().Be("01895 238282");
        }

        public void GetAllCentreSummariesForMap_returns_only_active_show_on_map_centres_with_latitude_and_longitude()
        {
            // When
            var summaries = centresDataService.GetAllCentreSummariesForMap().ToList();

            // Then
            var activeCentre = summaries.SingleOrDefault(c => c.Id == 2);
            var inactiveCentre = summaries.SingleOrDefault(c => c.Id == 6);
            var noLatitudeCentre = summaries.SingleOrDefault(c => c.Id == 239);
            var noLongitudeCentre = summaries.SingleOrDefault(c => c.Id == 74);
            var noShowOnMapCentre = summaries.SingleOrDefault(c => c.Id == 101);

            using (new AssertionScope())
            {
                activeCentre.Should().NotBeNull();
                activeCentre!.CentreName.Should().Be("North West Boroughs Healthcare NHS Foundation Trust");
                activeCentre.Latitude.Should().Be(53.428349);
                activeCentre.Longitude.Should().Be(-2.608441);

                inactiveCentre.Should().BeNull();
                noLatitudeCentre.Should().BeNull();
                noLongitudeCentre.Should().BeNull();
                noShowOnMapCentre.Should().BeNull();
            }
        }

        [Test]
        public void GetFullCentreDetailsById_should_return_expected_item()
        {
            // When
            var result = centresDataService.GetFullCentreDetailsById(2);

            // Then
            result!.CentreName.Should().BeEquivalentTo("North West Boroughs Healthcare NHS Foundation Trust");
        }

        [Test]
        public void GetFullCentreDetailsById_should_return_null_if_id_does_not_exist()
        {
            // When
            var result = centresDataService.GetFullCentreDetailsById(1);

            // Then
            result.Should().BeNull();
        }


        [Test]
        public void DeactivateCentre_should_return_expected_item()
        {
            // When
            centresDataService.DeactivateCentre(2);

            // Then
            var updatedCentre = centresDataService.GetCentreDetailsById(2)!;
            updatedCentre.Active.Should().BeFalse();
        }

        [Test]
        public void ReactivateCentre_should_return_expected_item()
        {
            // When
            centresDataService.ReactivateCentre(2);

            // Then
            var updatedCentre = centresDataService.GetCentreDetailsById(2)!;
            updatedCentre.Active.Should().BeTrue();
        }

        [Test]
        public void GetCentreManagerDetailsByCentreId_should_return_expected_item()
        {
            // When
            var result = centresDataService.GetCentreManagerDetailsByCentreId(2);

            // Then
            result.ContactForename.Should().Be("xxxxx");
            result.ContactSurname.Should().Be("xxxx");
            result.ContactEmail.Should().Be("nybwhudkra@ic.vs");
            result.ContactTelephone.Should().Be("xxxxxxxxxxxx");
        }
    }
}
