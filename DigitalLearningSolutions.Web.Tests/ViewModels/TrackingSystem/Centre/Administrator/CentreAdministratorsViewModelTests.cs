namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator;
    using FluentAssertions;
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
            var model = new CentreAdministratorsViewModel(
                1,
                adminUsers,
                new List<string>(),
                null,
                null,
                1,
                UserTestHelper.GetDefaultAdminUser()
            );

            model.Admins.Count().Should().Be(10);
            model.Admins.FirstOrDefault(adminUser => adminUser.Name == "k Surname").Should().BeNull();
        }

        [Test]
        public void Centre_administrators_should_correctly_return_the_second_page_of_admins()
        {
            var model = new CentreAdministratorsViewModel(
                1,
                adminUsers,
                new List<string>(),
                null,
                null,
                2,
                UserTestHelper.GetDefaultAdminUser()
            );

            model.Admins.Count().Should().Be(5);
            model.Admins.First().Name.Should().BeEquivalentTo("k Surname");
        }

        [Test]
        public void Centre_Administrators_filters_should_be_set()
        {
            // Given
            var roleOptions = new[]
            {
                AdminRoleFilterOptions.CentreAdministrator,
                AdminRoleFilterOptions.Supervisor,
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
                new FilterViewModel("Role", "Role", roleOptions),
                new FilterViewModel("CategoryName", "Category", new List<FilterOptionViewModel>()),
                new FilterViewModel("AccountStatus", "Account Status", accountStatusOptions)
            }.AsEnumerable();

            // When
            var model = new CentreAdministratorsViewModel(
                1,
                adminUsers,
                new List<string>(),
                null,
                null,
                2,
                UserTestHelper.GetDefaultAdminUser()
            );

            // Then
            model.Filters.Should().BeEquivalentTo(expectedFilters);
        }
    }
}
