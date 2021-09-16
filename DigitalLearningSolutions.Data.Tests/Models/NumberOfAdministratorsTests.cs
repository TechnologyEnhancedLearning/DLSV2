namespace DigitalLearningSolutions.Data.Tests.Models
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class NumberOfAdministratorsTests
    {
        [Test]
        public void Numbers_are_retrieved_correctly()
        {
            // Given
            var adminUsers = GetAdminUsersForTest();
            var centre = CentreTestHelper.GetDefaultCentre(
                cmsAdministratorSpots: 11,
                cmsManagerSpots: 12,
                ccLicenceSpots: 10,
                trainerSpots: 5
            );

            // When
            var result = new NumberOfAdministrators(centre, adminUsers);

            // Then
            using (new AssertionScope())
            {
                result.Admins.Should().Be(7);
                result.Supervisors.Should().Be(6);
                result.Trainers.Should().Be(1);
                result.CcLicences.Should().Be(2);
                result.CmsAdministrators.Should().Be(3);
                result.CmsManagers.Should().Be(2);
                result.TrainerSpots.Should().Be(5);
                result.CcLicenceSpots.Should().Be(10);
                result.CmsAdministratorSpots.Should().Be(11);
                result.CmsManagerSpots.Should().Be(12);
            }
        }

        [Test]
        public void Limits_not_reached_when_no_limits_set()
        {
            // When
            var numberOfAdmins = NumberOfAdministratorsTestHelper.GetDefaultNumberOfAdministrators();

            // Then
            using (new AssertionScope())
            {
                numberOfAdmins.CcLicencesAtOrOverLimit.Should().BeFalse();
                numberOfAdmins.TrainersAtOrOverLimit.Should().BeFalse();
                numberOfAdmins.CmsAdministratorsAtOrOverLimit.Should().BeFalse();
                numberOfAdmins.CmsManagersAtOrOverLimit.Should().BeFalse();
            }
        }

        [Test]
        public void Limits_not_reached_when_numbers_under_total_spots()
        {
            // When
            var numberOfAdmins = NumberOfAdministratorsTestHelper.GetDefaultNumberOfAdministrators(
                trainerSpots: 2,
                trainers: 1,
                cmsAdministratorSpots: 3,
                cmsAdministrators: 2,
                cmsManagerSpots: 4,
                cmsManagers: 3,
                ccLicenceSpots: 5,
                ccLicences: 4
            );

            // Then
            using (new AssertionScope())
            {
                numberOfAdmins.CcLicencesAtOrOverLimit.Should().BeFalse();
                numberOfAdmins.TrainersAtOrOverLimit.Should().BeFalse();
                numberOfAdmins.CmsAdministratorsAtOrOverLimit.Should().BeFalse();
                numberOfAdmins.CmsManagersAtOrOverLimit.Should().BeFalse();
            }
        }

        [Test]
        public void Limits_reached_when_numbers_exactly_match_total_spots()
        {
            // When
            var numberOfAdmins = NumberOfAdministratorsTestHelper.GetDefaultNumberOfAdministrators(
                trainerSpots: 2,
                trainers: 2,
                cmsAdministratorSpots: 3,
                cmsAdministrators: 3,
                cmsManagerSpots: 4,
                cmsManagers: 4,
                ccLicenceSpots: 5,
                ccLicences: 5
            );

            // Then
            using (new AssertionScope())
            {
                numberOfAdmins.CcLicencesAtOrOverLimit.Should().BeTrue();
                numberOfAdmins.TrainersAtOrOverLimit.Should().BeTrue();
                numberOfAdmins.CmsAdministratorsAtOrOverLimit.Should().BeTrue();
                numberOfAdmins.CmsManagersAtOrOverLimit.Should().BeTrue();
            }
        }

        [Test]
        public void Limits_reached_when_numbers_exceed_total_spots()
        {
            // When
            var numberOfAdmins = NumberOfAdministratorsTestHelper.GetDefaultNumberOfAdministrators(
                trainerSpots: 2,
                trainers: 3,
                cmsAdministratorSpots: 3,
                cmsAdministrators: 4,
                cmsManagerSpots: 4,
                cmsManagers: 5,
                ccLicenceSpots: 5,
                ccLicences: 6
            );

            // Then
            using (new AssertionScope())
            {
                numberOfAdmins.CcLicencesAtOrOverLimit.Should().BeTrue();
                numberOfAdmins.TrainersAtOrOverLimit.Should().BeTrue();
                numberOfAdmins.CmsAdministratorsAtOrOverLimit.Should().BeTrue();
                numberOfAdmins.CmsManagersAtOrOverLimit.Should().BeTrue();
            }
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
