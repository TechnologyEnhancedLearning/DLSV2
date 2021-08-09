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

            // When
            var result = new EditRolesViewModel(adminUser, 1, categories);

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

            // When
            var result = new EditRolesViewModel(adminUser, 1, categories);

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

            // When
            var result = new EditRolesViewModel(adminUser, 1, categories);

            // Then
            result.ContentManagementRole.Should().BeEquivalentTo(ContentManagementRole.NoContentManagementRole);
        }
    }
}
