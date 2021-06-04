namespace DigitalLearningSolutions.Web.Tests.ViewModels.Common
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using FluentAssertions;
    using NUnit.Framework;

    public class NumberOfAdministratorViewModelTests
    {
        [Test]
        public void AdminUsers_and_Centre_populate_expected_values()
        {
            // Given
            var centre = CentreTestHelper.GetDefaultCentre(
                cmsAdministratorSpots: 12,
                cmsManagerSpots: 13,
                ccLicenceSpots: 14,
                trainerSpots: 15
            );
            var adminUsersAtCentre = GetAdminUsersForTest();

            // When
            var viewModel = new NumberOfAdministratorsViewModel(centre, adminUsersAtCentre);

            // Then
            viewModel.CmsAdministratorSpotsAvailable.Should().Be(12);
            viewModel.CmsManagerSpotsAvailable.Should().Be(13);
            viewModel.CcLicencesAvailable.Should().Be(14);
            viewModel.TrainerSpotsAvailable.Should().Be(15);
            viewModel.Admins.Should().Be(7);
            viewModel.Supervisors.Should().Be(6);
            viewModel.CmsAdministrators.Should().Be(4);
            viewModel.CmsManagers.Should().Be(1);
            viewModel.CcLicences.Should().Be(2);
            viewModel.Trainers.Should().Be(1);
        }

        private List<AdminUser> GetAdminUsersForTest()
        {
            return new List<AdminUser>
            {
                UserTestHelper.GetDefaultAdminUser(
                    isCentreAdmin: true,
                    isSupervisor: true,
                    isContentManager: true,
                    importOnly: true,
                    isContentCreator: false,
                    isTrainer: true
                ),
                UserTestHelper.GetDefaultAdminUser(
                    isCentreAdmin: true,
                    isSupervisor: true,
                    isContentManager: true,
                    importOnly: false,
                    isContentCreator: true,
                    isTrainer: false
                ),
                UserTestHelper.GetDefaultAdminUser(
                    isCentreAdmin: true,
                    isSupervisor: true,
                    isContentManager: false,
                    importOnly: true,
                    isContentCreator: true,
                    isTrainer: false
                ),
                UserTestHelper.GetDefaultAdminUser(
                    isCentreAdmin: true,
                    isSupervisor: true,
                    isContentManager: true,
                    importOnly: true,
                    isContentCreator: false,
                    isTrainer: false
                ),
                UserTestHelper.GetDefaultAdminUser(
                    isCentreAdmin: true,
                    isSupervisor: true,
                    isContentManager: true,
                    importOnly: true,
                    isContentCreator: false,
                    isTrainer: false
                ),
                UserTestHelper.GetDefaultAdminUser(
                    isCentreAdmin: true,
                    isSupervisor: true,
                    isContentManager: true,
                    importOnly: false,
                    isContentCreator: false,
                    isTrainer: false
                ),
                UserTestHelper.GetDefaultAdminUser(
                    isCentreAdmin: true,
                    isSupervisor: false,
                    isContentManager: false,
                    importOnly: false,
                    isContentCreator: false,
                    isTrainer: false
                ),
                UserTestHelper.GetDefaultAdminUser(
                    isCentreAdmin: false,
                    isSupervisor: false,
                    isContentManager: false,
                    importOnly: false,
                    isContentCreator: false,
                    isTrainer: false
                )
            };
        }
    }
}
