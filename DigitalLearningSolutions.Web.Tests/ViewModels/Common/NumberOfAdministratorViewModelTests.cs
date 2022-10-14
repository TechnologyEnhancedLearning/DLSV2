namespace DigitalLearningSolutions.Web.Tests.ViewModels.Common
{
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using FluentAssertions;
    using NUnit.Framework;

    public class NumberOfAdministratorViewModelTests
    {
        [Test]
        public void ViewModel_populates_expected_values()
        {
            // Given
            var numberOfAdmins = CentreContractAdminUsageTestHelper.GetDefaultNumberOfAdministrators(
                admins: 7,
                supervisors: 6,
                trainers: 1,
                trainerSpots: 15,
                cmsAdministrators: 3,
                cmsAdministratorSpots: 12,
                cmsManagers: 2,
                cmsManagerSpots: 13,
                ccLicences: 2,
                ccLicenceSpots: 14
            );

            // When
            var viewModel = new NumberOfAdministratorsViewModel(numberOfAdmins);

            // Then
            viewModel.Admins.Should().Be("7");
            viewModel.Supervisors.Should().Be("6");
            viewModel.CmsAdministrators.Should().Be("3 / 12");
            viewModel.CmsManagers.Should().Be("2 / 13");
            viewModel.CcLicences.Should().Be("2 / 14");
            viewModel.Trainers.Should().Be("1 / 15");
        }

        [Test]
        public void No_limit_should_be_displayed_when_centre_has_no_limit_on_spots_available()
        {
            // Given
            var numberOfAdmins = CentreContractAdminUsageTestHelper.GetDefaultNumberOfAdministrators(
                trainers: 1,
                trainerSpots: -1,
                cmsAdministrators: 3,
                cmsAdministratorSpots: -1,
                cmsManagers: 2,
                cmsManagerSpots: -1,
                ccLicences: 2,
                ccLicenceSpots: -1
            );

            // When
            var viewModel = new NumberOfAdministratorsViewModel(numberOfAdmins);

            // Then
            viewModel.CmsAdministrators.Should().Be("3");
            viewModel.CmsManagers.Should().Be("2");
            viewModel.CcLicences.Should().Be("2");
            viewModel.Trainers.Should().Be("1");
        }
    }
}
