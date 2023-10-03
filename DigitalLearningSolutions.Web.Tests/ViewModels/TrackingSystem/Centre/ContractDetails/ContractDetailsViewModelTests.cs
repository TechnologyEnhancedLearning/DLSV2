namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Centre.ContractDetails
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.ContractDetails;
    using FluentAssertions;
    using NUnit.Framework;

    public class ContractDetailsViewModelTests
    {
        [Test]
        public void AdminUsers_and_Centre_populate_expected_values()
        {
            // Given
            var centre = CentreTestHelper.GetDefaultCentre(
                cmsAdministratorSpots: 3,
                cmsManagerSpots: 13,
                ccLicenceSpots: 14,
                trainerSpots: 15,
                customCourses: 12,
                serverSpaceUsed: 1024,
                serverSpaceBytes: 1073741824
            );
            var adminUsersAtCentre = GetAdminUsersForTest();

            // When
            var viewModel = new ContractDetailsViewModel(adminUsersAtCentre, centre, 10);

            // Then
            viewModel.Administrators.Should().Be("7");
            viewModel.Supervisors.Should().Be("6");
            viewModel.CmsAdministrators.Should().Be("3 / 3");
            viewModel.CmsAdministratorsColour.Should().Be("red");
            viewModel.CmsManagers.Should().Be("2 / 13");
            viewModel.CmsManagersColour.Should().Be("green");
            viewModel.ContentCreators.Should().Be("2 / 14");
            viewModel.ContentCreatorsColour.Should().Be("green");
            viewModel.Trainers.Should().Be("1 / 15");
            viewModel.TrainersColour.Should().Be("green");
            viewModel.CustomCourses.Should().Be("10 / 12");
            viewModel.CustomCoursesColour.Should().Be("yellow");
            viewModel.ServerSpace.Should().Be("1KB / 1GB");
            viewModel.ServerSpaceColour.Should().Be("green");
        }

        [Test]
        public void AdminUsers_and_Centre_populate_expected_values_with_no_limit()
        {
            // Given
            var centre = CentreTestHelper.GetDefaultCentre(
                cmsAdministratorSpots: -1,
                cmsManagerSpots: -1,
                ccLicenceSpots: -1,
                trainerSpots: -1,
                customCourses: 12,
                serverSpaceUsed: 0,
                serverSpaceBytes: 0
            );
            var adminUsersAtCentre = GetAdminUsersForTest();

            // When
            var viewModel = new ContractDetailsViewModel(adminUsersAtCentre, centre, 10);

            // Then
            viewModel.Administrators.Should().Be("7");
            viewModel.Supervisors.Should().Be("6");
            viewModel.CmsAdministrators.Should().Be("3");
            viewModel.CmsAdministratorsColour.Should().Be("blue");
            viewModel.CmsManagers.Should().Be("2");
            viewModel.CmsManagersColour.Should().Be("blue");
            viewModel.ContentCreators.Should().Be("2");
            viewModel.ContentCreatorsColour.Should().Be("blue");
            viewModel.Trainers.Should().Be("1");
            viewModel.TrainersColour.Should().Be("blue");
            viewModel.CustomCourses.Should().Be("10 / 12");
            viewModel.CustomCoursesColour.Should().Be("yellow");
            viewModel.ServerSpace.Should().Be("0B / 0B");
            viewModel.ServerSpaceColour.Should().Be("grey");
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
