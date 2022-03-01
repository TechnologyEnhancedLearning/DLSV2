namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class EditRolesViewModelTests
    {
        private readonly List<Category> categories = new List<Category>
        {
            new Category { CategoryName = "All", CourseCategoryID = 0 },
            new Category { CategoryName = "Some", CourseCategoryID = 1 }
        };

        [Test]
        public void EditRolesViewModel_sets_expected_properties()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(
                firstName: "Test",
                lastName: "Name",
                isCentreAdmin: true,
                isSupervisor: true,
                isTrainer: true,
                isContentCreator: true,
                isContentManager: true,
                importOnly: true,
                categoryId: 0
            );
            var numberOfAdmins = CentreContractAdminUsageTestHelper.GetDefaultNumberOfAdministrators();

            // When
            var result = new EditRolesViewModel(adminUser, 1, categories, numberOfAdmins, 3);

            // Then
            using (new AssertionScope())
            {
                result.FullName.Should().Be("Test Name");
                result.IsCentreAdmin.Should().BeTrue();
                result.IsSupervisor.Should().BeTrue();
                result.IsTrainer.Should().BeTrue();
                result.IsContentCreator.Should().BeTrue();
                result.CentreId.Should().Be(1);
                result.LearningCategory.Should().Be(0);
                result.LearningCategories.Count().Should().Be(2);
                result.ContentManagementRole.Should().BeEquivalentTo(ContentManagementRole.CmsAdministrator);
                result.ReturnPage.Should().Be(3);
            }
        }

        [Test]
        public void EditRolesViewModel_sets_ContentManagementRole_cms_manager()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(
                isContentManager: true,
                importOnly: false
            );
            var numberOfAdmins = CentreContractAdminUsageTestHelper.GetDefaultNumberOfAdministrators();

            // When
            var result = new EditRolesViewModel(adminUser, 1, categories, numberOfAdmins, null);

            // Then
            result.ContentManagementRole.Should().BeEquivalentTo(ContentManagementRole.CmsManager);
        }

        [Test]
        public void EditRolesViewModel_sets_ContentManagementRole_none()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(
                isContentManager: false,
                importOnly: false
            );
            var numberOfAdmins = CentreContractAdminUsageTestHelper.GetDefaultNumberOfAdministrators();

            // When
            var result = new EditRolesViewModel(adminUser, 1, categories, numberOfAdmins, null);

            // Then
            result.ContentManagementRole.Should().BeEquivalentTo(ContentManagementRole.NoContentManagementRole);
        }

        [Test]
        public void EditRolesViewModel_sets_up_all_checkboxes_and_inputs_when_under_limits()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            var numberOfAdmins = CentreContractAdminUsageTestHelper.GetDefaultNumberOfAdministrators();

            // When
            var result = new EditRolesViewModel(adminUser, 1, categories, numberOfAdmins, null);

            // Then
            using (new AssertionScope())
            {
                result.Checkboxes.Count.Should().Be(5);
                result.Radios.Count.Should().Be(3);
                result.Checkboxes.Contains(AdminRoleInputs.CentreAdminCheckbox).Should().BeTrue();
                result.Checkboxes.Contains(AdminRoleInputs.SupervisorCheckbox).Should().BeTrue();
                result.Checkboxes.Contains(AdminRoleInputs.TrainerCheckbox).Should().BeTrue();
                result.Checkboxes.Contains(AdminRoleInputs.ContentCreatorCheckbox).Should().BeTrue();
                result.Radios.Contains(AdminRoleInputs.CmsAdministratorRadioButton).Should().BeTrue();
                result.Radios.Contains(AdminRoleInputs.CmsManagerRadioButton).Should().BeTrue();
                result.Radios.Contains(AdminRoleInputs.NoCmsPermissionsRadioButton).Should().BeTrue();
                result.NotAllRolesDisplayed.Should().BeFalse();
                result.NoContentManagerOptionsAvailable.Should().BeFalse();
            }
        }

        [Test]
        public void
            EditRolesViewModel_does_not_set_up_Trainer_checkbox_when_its_limit_is_reached_and_user_not_a_trainer()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(isTrainer: false);
            var numberOfAdmins =
                CentreContractAdminUsageTestHelper.GetDefaultNumberOfAdministrators(trainers: 5, trainerSpots: 5);

            // When
            var result = new EditRolesViewModel(adminUser, 1, categories, numberOfAdmins, null);

            // Then
            using (new AssertionScope())
            {
                result.Checkboxes.Count.Should().Be(4);
                result.Checkboxes.Contains(AdminRoleInputs.TrainerCheckbox).Should().BeFalse();
                result.NotAllRolesDisplayed.Should().BeTrue();
            }
        }

        [Test]
        public void EditRolesViewModel_does_set_up_Trainer_checkbox_when_its_limit_is_reached_and_user_is_a_trainer()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(isTrainer: true);
            var numberOfAdmins =
                CentreContractAdminUsageTestHelper.GetDefaultNumberOfAdministrators(trainers: 5, trainerSpots: 5);

            // When
            var result = new EditRolesViewModel(adminUser, 1, categories, numberOfAdmins, null);

            // Then
            using (new AssertionScope())
            {
                result.Checkboxes.Count.Should().Be(5);
                result.Checkboxes.Contains(AdminRoleInputs.TrainerCheckbox).Should().BeTrue();
                result.NotAllRolesDisplayed.Should().BeFalse();
            }
        }

        [Test]
        public void
            EditRolesViewModel_does_not_set_up_Content_creator_checkbox_when_its_limit_is_reached_and_user_is_not_a_content_creator()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(isContentCreator: false);
            var numberOfAdmins =
                CentreContractAdminUsageTestHelper.GetDefaultNumberOfAdministrators(ccLicences: 5, ccLicenceSpots: 5);

            // When
            var result = new EditRolesViewModel(adminUser, 1, categories, numberOfAdmins, null);

            // Then
            using (new AssertionScope())
            {
                result.Checkboxes.Count.Should().Be(4);
                result.Checkboxes.Contains(AdminRoleInputs.ContentCreatorCheckbox).Should().BeFalse();
                result.NotAllRolesDisplayed.Should().BeTrue();
            }
        }

        [Test]
        public void
            EditRolesViewModel_does_set_up_Content_creator_checkbox_when_its_limit_is_reached_and_user_is_a_content_creator()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(isContentCreator: true);
            var numberOfAdmins =
                CentreContractAdminUsageTestHelper.GetDefaultNumberOfAdministrators(ccLicences: 5, ccLicenceSpots: 5);

            // When
            var result = new EditRolesViewModel(adminUser, 1, categories, numberOfAdmins, null);

            // Then
            using (new AssertionScope())
            {
                result.Checkboxes.Count.Should().Be(5);
                result.Checkboxes.Contains(AdminRoleInputs.ContentCreatorCheckbox).Should().BeTrue();
                result.NotAllRolesDisplayed.Should().BeFalse();
            }
        }

        [Test]
        public void
            EditRolesViewModel_does_not_set_up_Cms_admin_radio_button_when_its_limit_is_reached_and_user_is_not_a_cms_admin()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(importOnly: false, isContentManager: false);
            var numberOfAdmins =
                CentreContractAdminUsageTestHelper.GetDefaultNumberOfAdministrators(
                    cmsAdministrators: 5,
                    cmsAdministratorSpots: 5
                );

            // When
            var result = new EditRolesViewModel(adminUser, 1, categories, numberOfAdmins, null);

            // Then
            using (new AssertionScope())
            {
                result.Radios.Count.Should().Be(2);
                result.Radios.Contains(AdminRoleInputs.CmsAdministratorRadioButton).Should().BeFalse();
                result.NotAllRolesDisplayed.Should().BeTrue();
                result.NoContentManagerOptionsAvailable.Should().BeFalse();
            }
        }

        [Test]
        public void
            EditRolesViewModel_does_set_up_Cms_admin_radio_button_when_its_limit_is_reached_and_user_is_a_cms_admin()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(importOnly: true, isContentManager: true);
            var numberOfAdmins =
                CentreContractAdminUsageTestHelper.GetDefaultNumberOfAdministrators(
                    cmsAdministrators: 5,
                    cmsAdministratorSpots: 5
                );

            // When
            var result = new EditRolesViewModel(adminUser, 1, categories, numberOfAdmins, null);

            // Then
            using (new AssertionScope())
            {
                result.Radios.Count.Should().Be(3);
                result.Radios.Contains(AdminRoleInputs.CmsAdministratorRadioButton).Should().BeTrue();
                result.NotAllRolesDisplayed.Should().BeFalse();
                result.NoContentManagerOptionsAvailable.Should().BeFalse();
            }
        }

        [Test]
        public void
            EditRolesViewModel_does_not_set_up_Cms_manager_radio_button_when_its_limit_is_reached_and_user_is_not_a_cms_manager()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(importOnly: false, isContentManager: false);
            var numberOfAdmins =
                CentreContractAdminUsageTestHelper.GetDefaultNumberOfAdministrators(cmsManagers: 5, cmsManagerSpots: 5);

            // When
            var result = new EditRolesViewModel(adminUser, 1, categories, numberOfAdmins, null);

            // Then
            using (new AssertionScope())
            {
                result.Radios.Count.Should().Be(2);
                result.Radios.Contains(AdminRoleInputs.CmsManagerRadioButton).Should().BeFalse();
                result.NotAllRolesDisplayed.Should().BeTrue();
                result.NoContentManagerOptionsAvailable.Should().BeFalse();
            }
        }

        [Test]
        public void
            EditRolesViewModel_does_set_up_Cms_manager_radio_button_when_its_limit_is_reached_and_user_is_a_cms_manager()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(importOnly: false, isContentManager: true);
            var numberOfAdmins =
                CentreContractAdminUsageTestHelper.GetDefaultNumberOfAdministrators(cmsManagers: 5, cmsManagerSpots: 5);

            // When
            var result = new EditRolesViewModel(adminUser, 1, categories, numberOfAdmins, null);

            // Then
            using (new AssertionScope())
            {
                result.Radios.Count.Should().Be(3);
                result.Radios.Contains(AdminRoleInputs.CmsManagerRadioButton).Should().BeTrue();
                result.NotAllRolesDisplayed.Should().BeFalse();
                result.NoContentManagerOptionsAvailable.Should().BeFalse();
            }
        }

        [Test]
        public void
            EditRolesViewModel_sets_only_NoCmsPermissions_radio_button_when_both_limits_reached_and_user_has_no_permissions()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(importOnly: false, isContentManager: false);
            var numberOfAdmins =
                CentreContractAdminUsageTestHelper.GetDefaultNumberOfAdministrators(
                    cmsManagers: 5,
                    cmsManagerSpots: 5,
                    cmsAdministrators: 5,
                    cmsAdministratorSpots: 5
                );

            // When
            var result = new EditRolesViewModel(adminUser, 1, categories, numberOfAdmins, null);

            // Then
            using (new AssertionScope())
            {
                result.Radios.Count.Should().Be(1);
                result.Radios.Contains(AdminRoleInputs.NoCmsPermissionsRadioButton).Should().BeTrue();
                result.NotAllRolesDisplayed.Should().BeTrue();
                result.NoContentManagerOptionsAvailable.Should().BeTrue();
            }
        }

        [Test]
        public void GetAdminRoles_maps_admin_roles_correctly()
        {
            // Given
            var viewModel = new EditRolesViewModel
            {
                IsCentreAdmin = true,
                IsSupervisor = true,
                IsContentCreator = true,
                IsTrainer = true,
                ContentManagementRole = ContentManagementRole.CmsAdministrator
            };

            // When
            var result = viewModel.GetAdminRoles();

            // Then
            using (new AssertionScope())
            {
                result.IsCentreAdmin.Should().BeTrue();
                result.IsSupervisor.Should().BeTrue();
                result.IsContentCreator.Should().BeTrue();
                result.IsTrainer.Should().BeTrue();
                result.ImportOnly.Should().BeTrue();
                result.IsContentManager.Should().BeTrue();
            }
        }
    }
}
