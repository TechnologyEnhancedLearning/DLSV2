namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class CentreAdministratorsViewModelTests
    {
        private readonly AdminUser[] adminUsers =
        {
            UserTestHelper.GetDefaultAdminUser(firstName: "a", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "b", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "c", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "d", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "e", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "f", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "g", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "h", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "i", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "j", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "k", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "l", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "m", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "n", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "o", lastName: "Surname")
        };

        [Test]
        public void Centre_administrators_should_default_to_returning_the_first_ten_admins()
        {
            // When
            var model = new CentreAdministratorsViewModel(
                1,
                adminUsers,
                new List<string>(),
                null,
                null,
                1,
                UserTestHelper.GetDefaultAdminUser(),
                null
            );

            // Then
            using (new AssertionScope())
            {
                model.Admins.Count().Should().Be(10);
                model.Admins.FirstOrDefault(adminUser => adminUser.Name == "Surname, k").Should().BeNull();
            }
        }

        [Test]
        public void Centre_administrators_should_correctly_return_the_second_page_of_admins()
        {
            // When
            var model = new CentreAdministratorsViewModel(
                1,
                adminUsers,
                new List<string>(),
                null,
                null,
                2,
                UserTestHelper.GetDefaultAdminUser(),
                null
            );

            // Then
            using (new AssertionScope())
            {
                model.Admins.Count().Should().Be(5);
                model.Admins.First().Name.Should().BeEquivalentTo("Surname, k");
            }
        }

        [Test]
        public void Centre_Administrators_filters_should_be_set()
        {
            // Given
            var roleOptions = new[]
            {
                AdminRoleFilterOptions.CentreAdministrator,
                AdminRoleFilterOptions.Supervisor,
                AdminRoleFilterOptions.NominatedSupervisor,
                AdminRoleFilterOptions.Trainer,
                AdminRoleFilterOptions.ContentCreatorLicense,
                AdminRoleFilterOptions.CmsAdministrator,
                AdminRoleFilterOptions.CmsManager
            };
            var accountStatusOptions = new[]
            {
                AdminAccountStatusFilterOptions.IsLocked,
                AdminAccountStatusFilterOptions.IsNotLocked
            };
            var expectedFilters = new[]
            {
                new FilterModel("Role", "Role", roleOptions),
                new FilterModel("CategoryName", "Category", new List<FilterOptionModel>()),
                new FilterModel("AccountStatus", "Account Status", accountStatusOptions)
            }.AsEnumerable();

            // When
            var model = new CentreAdministratorsViewModel(
                1,
                adminUsers,
                new List<string>(),
                null,
                null,
                2,
                UserTestHelper.GetDefaultAdminUser(),
                null
            );

            // Then
            model.Filters.Should().BeEquivalentTo(expectedFilters);
        }

        [Test]
        public void Centre_administrators_with_custom_items_per_page_should_return_the_specified_number_of_admins()
        {
            // Given
            const int itemsPerPage = 12;

            // When
            var model = new CentreAdministratorsViewModel(
                1,
                adminUsers,
                new List<string>(),
                null,
                null,
                1,
                UserTestHelper.GetDefaultAdminUser(),
                itemsPerPage
            );

            // Then
            using (new AssertionScope())
            {
                model.Admins.Count().Should().Be(itemsPerPage);
                model.Admins.FirstOrDefault(adminUser => adminUser.Name == "Surname, m").Should().BeNull();
            }
        }
    }
}
