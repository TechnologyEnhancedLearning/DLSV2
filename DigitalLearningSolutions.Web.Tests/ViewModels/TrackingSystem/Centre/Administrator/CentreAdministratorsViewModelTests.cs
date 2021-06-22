namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Centre.Administrator
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
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
                null,
                "SearchableName",
                "Ascending",
                1
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
                null,
                "SearchableName",
                "Ascending",
                2
            );

            model.Admins.Count().Should().Be(5);
            model.Admins.First().Name.Should().BeEquivalentTo("k Surname");
        }
    }
}
