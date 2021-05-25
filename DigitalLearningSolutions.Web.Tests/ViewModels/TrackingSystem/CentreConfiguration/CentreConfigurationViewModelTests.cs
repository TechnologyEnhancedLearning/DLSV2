namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.CentreConfiguration
{
    using System;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class CentreConfigurationViewModelTests
    {
        [Test]
        public void CentreConfigurationViewModel_populates_expected_values_from_Centre()
        {
            // When
            var centre =
                CentreTestHelper.GetDefaultCentre(
                    centreLogo: Convert.FromBase64String(CentreLogoTestHelper.DefaultCentreLogoAsBase64String));
            var viewModel = new CentreConfigurationViewModel(centre);
            var expectedTrainingCentres = new[]
                { "Hollins Park House", "Hollins Lane", "Winwick", "Warrington WA2 8WA" };

            // Then
            using (new AssertionScope())
            {
                viewModel.CentreId.Should().Be(centre.CentreId);
                viewModel.CentreName.Should().BeEquivalentTo(centre.CentreName);
                viewModel.RegionName.Should().BeEquivalentTo(centre.RegionName);
                viewModel.NotifyEmail.Should().BeEquivalentTo(centre.NotifyEmail);
                viewModel.BannerText.Should().BeEquivalentTo(centre.BannerText);
                viewModel.SignatureImage.Should().BeEquivalentTo(centre.SignatureImage);
                viewModel.CentreLogo.Should().BeEquivalentTo(centre.CentreLogo);
                viewModel.ContactForename.Should().BeEquivalentTo(centre.ContactForename);
                viewModel.ContactSurname.Should().BeEquivalentTo(centre.ContactSurname);
                viewModel.ContactEmail.Should().BeEquivalentTo(centre.ContactEmail);
                viewModel.ContactTelephone.Should().BeEquivalentTo(centre.ContactTelephone);
                viewModel.CentreTelephone.Should().BeEquivalentTo(centre.CentreTelephone);
                viewModel.CentreEmail.Should().BeEquivalentTo(centre.CentreEmail);
                viewModel.CentrePostcode.Should().BeEquivalentTo(centre.CentrePostcode);
                viewModel.OpeningHours.Should().BeEquivalentTo(centre.OpeningHours);
                viewModel.CentreWebAddress.Should().BeEquivalentTo(centre.CentreWebAddress);
                viewModel.OrganisationsCovered.Should().BeEquivalentTo(centre.OrganisationsCovered);
                viewModel.TrainingVenues.Should().BeEquivalentTo(expectedTrainingCentres);
                viewModel.OtherInformation.Should().BeEmpty();
            }
        }

        [Test]
        public void CentreConfigurationViewModel_should_split_training_venues_on_new_lines()
        {
            // When
            var centre = CentreTestHelper.GetDefaultCentre(
                trainingVenues: "Address Line 1\r\nAddress Line 2\r\nAddress Line 3\r\nAddress Line 4");
            var viewModel = new CentreConfigurationViewModel(centre);
            var expectedTrainingCentres = new[]
                { "Address Line 1", "Address Line 2", "Address Line 3", "Address Line 4" };

            // Then
            viewModel.TrainingVenues.Should().BeEquivalentTo(expectedTrainingCentres);
        }

        [Test]
        public void CentreConfigurationViewModel_should_not_split_training_venues_if_no_new_lines_found()
        {
            // When
            var centre = CentreTestHelper.GetDefaultCentre(
                trainingVenues: "Address Line 1, Address Line 2, Address Line 3, Address Line 4");
            var viewModel = new CentreConfigurationViewModel(centre);
            var expectedTrainingCentres = new[] { "Address Line 1, Address Line 2, Address Line 3, Address Line 4" };

            // Then
            viewModel.TrainingVenues.Should().BeEquivalentTo(expectedTrainingCentres);
        }
    }
}
