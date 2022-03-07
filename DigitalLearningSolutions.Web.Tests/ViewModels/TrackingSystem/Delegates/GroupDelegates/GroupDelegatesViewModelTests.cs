namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.GroupDelegates
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupDelegates;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class GroupDelegatesViewModelTests
    {
        private readonly DelegateGroupsSideNavViewModel expectedNavViewModel =
            new DelegateGroupsSideNavViewModel(1, "Group name", DelegateGroupPage.Delegates);

        private readonly GroupDelegate[] groupDelegates =
        {
            new GroupDelegate { FirstName = "A", LastName = "Surname" },
            new GroupDelegate { FirstName = "B", LastName = "Surname" },
            new GroupDelegate { FirstName = "C", LastName = "Surname" },
            new GroupDelegate { FirstName = "D", LastName = "Surname" },
            new GroupDelegate { FirstName = "E", LastName = "Surname" },
            new GroupDelegate { FirstName = "F", LastName = "Surname" },
            new GroupDelegate { FirstName = "G", LastName = "Surname" },
            new GroupDelegate { FirstName = "H", LastName = "Surname" },
            new GroupDelegate { FirstName = "I", LastName = "Surname" },
            new GroupDelegate { FirstName = "J", LastName = "Surname" },
            new GroupDelegate { FirstName = "K", LastName = "Surname" },
            new GroupDelegate { FirstName = "L", LastName = "Surname" },
            new GroupDelegate { FirstName = "M", LastName = "Surname" },
            new GroupDelegate { FirstName = "N", LastName = "Surname" },
            new GroupDelegate { FirstName = "O", LastName = "Surname" },
        };

        [Test]
        public void GroupDelegatesViewModel_should_return_the_first_page_worth_of_delegates()
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
                model.GroupDelegates.Any(groupDelegate => groupDelegate.Name == "K Surname").Should()
                    .BeFalse();
            }
        }

        [Test]
        public void GroupDelegatesViewModel_should_correctly_return_the_second_page_of_delegates()
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
                model.GroupDelegates.First().Name.Should().BeEquivalentTo("K Surname");
            }
        }
    }
}
