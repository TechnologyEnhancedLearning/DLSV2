namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Centre.Dashboard
{
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Dashboard;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class DashboardCentreDetailsViewModelTests
    {
        [Test]
        public void DashboardCentreDetailsViewModel_populates_expected_values_from_centre()
        {
            const string userIp = "1.1.1.1";
            var centre =
                CentreTestHelper.GetDefaultCentre();
            var viewModel = new DashboardCentreDetailsViewModel(centre, userIp, 12);

            // Then
            using (new AssertionScope())
            {
                viewModel.CentreId.Should().Be(centre.CentreId);
                viewModel.CentreName.Should().BeEquivalentTo(centre.CentreName);
                viewModel.Region.Should().BeEquivalentTo(centre.RegionName);
                viewModel.ContractType.Should().BeEquivalentTo(centre.ContractType);
                viewModel.BannerText.Should().BeEquivalentTo(centre.BannerText);
                viewModel.CentreManager.Should().BeEquivalentTo("xxxxx xxxx");
                viewModel.IpAddress.Should().BeEquivalentTo(userIp);
                viewModel.ApprovedIps.Should().BeEquivalentTo(centre.IpPrefix);
                viewModel.Telephone.Should().BeEquivalentTo(centre.ContactTelephone);
                viewModel.Email.Should().BeEquivalentTo(centre.ContactEmail);
                viewModel.CentreRank.Should().Be("12");
            }
        }

        [Test]
        public void DashboardCentreDetailsViewModel_populates_expected_values_from_centre_with_null_centre_rank()
        {
            const string userIp = "1.1.1.1";
            var centre =
                CentreTestHelper.GetDefaultCentre();
            var viewModel = new DashboardCentreDetailsViewModel(centre, userIp, null);

            // Then
            using (new AssertionScope())
            {
                viewModel.CentreId.Should().Be(centre.CentreId);
                viewModel.CentreName.Should().BeEquivalentTo(centre.CentreName);
                viewModel.Region.Should().BeEquivalentTo(centre.RegionName);
                viewModel.ContractType.Should().BeEquivalentTo(centre.ContractType);
                viewModel.BannerText.Should().BeEquivalentTo(centre.BannerText);
                viewModel.CentreManager.Should().BeEquivalentTo("xxxxx xxxx");
                viewModel.IpAddress.Should().BeEquivalentTo(userIp);
                viewModel.ApprovedIps.Should().BeEquivalentTo(centre.IpPrefix);
                viewModel.Telephone.Should().BeEquivalentTo(centre.ContactTelephone);
                viewModel.Email.Should().BeEquivalentTo(centre.ContactEmail);
                viewModel.CentreRank.Should().Be("No activity");
            }
        }
    }
}
