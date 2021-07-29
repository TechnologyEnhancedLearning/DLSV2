namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class GroupDelegatesViewModelTests
    {
        private readonly DelegateGroupsSideNavViewModel expectedNavViewModel =
            new DelegateGroupsSideNavViewModel("Group name", DelegateGroupPage.Delegates);

        private readonly GroupDelegate[] groupDelegates =
        {
            GroupTestHelper.GetDefaultGroupDelegate(firstName: "a", lastName: "Surname"),
            GroupTestHelper.GetDefaultGroupDelegate(firstName: "b", lastName: "Surname"),
            GroupTestHelper.GetDefaultGroupDelegate(firstName: "c", lastName: "Surname"),
            GroupTestHelper.GetDefaultGroupDelegate(firstName: "d", lastName: "Surname"),
            GroupTestHelper.GetDefaultGroupDelegate(firstName: "e", lastName: "Surname"),
            GroupTestHelper.GetDefaultGroupDelegate(firstName: "f", lastName: "Surname"),
            GroupTestHelper.GetDefaultGroupDelegate(firstName: "g", lastName: "Surname"),
            GroupTestHelper.GetDefaultGroupDelegate(firstName: "h", lastName: "Surname"),
            GroupTestHelper.GetDefaultGroupDelegate(firstName: "i", lastName: "Surname"),
            GroupTestHelper.GetDefaultGroupDelegate(firstName: "j", lastName: "Surname"),
            GroupTestHelper.GetDefaultGroupDelegate(firstName: "k", lastName: "Surname"),
            GroupTestHelper.GetDefaultGroupDelegate(firstName: "l", lastName: "Surname"),
            GroupTestHelper.GetDefaultGroupDelegate(firstName: "m", lastName: "Surname"),
            GroupTestHelper.GetDefaultGroupDelegate(firstName: "n", lastName: "Surname"),
            GroupTestHelper.GetDefaultGroupDelegate(firstName: "o", lastName: "Surname")
        };

        [Test]
        public void Centre_administrators_should_default_to_returning_the_first_ten_admins()
        {
            var model = new GroupDelegatesViewModel(
                1,
                "Group name",
                groupDelegates,
                1
            );

            using (new AssertionScope())
            {
                model.GroupId.Should().Be(1);
                model.NavViewModel.Should().BeEquivalentTo(expectedNavViewModel);
                model.GroupDelegates.Count().Should().Be(10);
                model.GroupDelegates.FirstOrDefault(groupDelegate => groupDelegate.Name == "k Surname").Should()
                    .BeNull();
            }
        }

        [Test]
        public void Centre_administrators_should_correctly_return_the_second_page_of_admins()
        {
            var model = new GroupDelegatesViewModel(
                1,
                "Group name",
                groupDelegates,
                2
            );

            using (new AssertionScope())
            {
                model.GroupId.Should().Be(1);
                model.NavViewModel.Should().BeEquivalentTo(expectedNavViewModel);
                model.GroupDelegates.Count().Should().Be(5);
                model.GroupDelegates.First().Name.Should().BeEquivalentTo("k Surname");
            }
        }
    }
}
