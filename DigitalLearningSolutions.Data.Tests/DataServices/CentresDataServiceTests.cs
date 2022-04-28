namespace DigitalLearningSolutions.Data.Tests.DataServices
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
                var summaries = centresDataService.GetAllCentreSummariesForSuperAdmin().ToList();

                // Then
                var activeCentre = summaries.Single(c => c.CentreId == 2);
                var inActiveCentre = summaries.Single(c => c.CentreId == 6);

                activeCentre.Active.Should().BeTrue();
                activeCentre.CentreType.Should().Be("NHS Organisation");
                activeCentre.RegionName.Should().Be("North West");

                inActiveCentre.Active.Should().BeFalse();
                inActiveCentre.CentreType.Should().Be("NHS Organisation");
                inActiveCentre.RegionName.Should().Be("East Of England");
        }

        [Test]
        public void GetAllCentreSummariesForFindCentre_returns_expected_summary()
        {
            //When
            var summaries = centresDataService.GetAllCentreSummariesForFindCentre().ToList();

            //Then
            summaries.Should().HaveCount(315);
            summaries.Single(s => s.CentreId == 8)!.CentreName.Should().Be("Buckinghamshire Healthcare NHS Trust");
            summaries.Single(s => s.CentreId == 2)!.RegionName.Should().Be("North West");
            summaries.Single(s => s.CentreId == 190)!.Email.Should().BeNull();
            summaries.Single(s => s.CentreId == 205)!.Telephone.Should().Be("01895 238282");
        }
    }
}
