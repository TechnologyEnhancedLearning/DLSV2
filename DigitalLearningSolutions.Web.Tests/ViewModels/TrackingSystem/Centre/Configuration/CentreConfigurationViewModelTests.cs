namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Centre.Configuration
{
    using System;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Configuration;
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
                CentreTestHelper.GetDefaultCentre
                (
                    centreLogo: Convert.FromBase64String(CentreLogoTestHelper.DefaultCentreLogoAsBase64String)
                );
            var viewModel = new CentreConfigurationViewModel(centre);

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
                viewModel.ShowCentreOnMap.Should().BeTrue();
                viewModel.OpeningHours.Should().BeEquivalentTo(centre.OpeningHours);
                viewModel.CentreWebAddress.Should().BeEquivalentTo(centre.CentreWebAddress);
                viewModel.OrganisationsCovered.Should().BeEquivalentTo(centre.OrganisationsCovered);
                viewModel.TrainingVenues.Should().BeEquivalentTo(centre.TrainingVenues);
                viewModel.OtherInformation.Should().BeEquivalentTo(centre.OtherInformation);
            }
        }
    }
}
