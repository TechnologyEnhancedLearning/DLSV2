﻿namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Tests.NBuilderHelpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class GroupDelegatesViewModelTests
    {
        private readonly DelegateGroupsSideNavViewModel expectedNavViewModel =
            new DelegateGroupsSideNavViewModel("Group name", DelegateGroupPage.Delegates);

        private GroupDelegate[] groupDelegates = null!;

        [SetUp]
        public void SetUp()
        {
            BuilderSetup.DisablePropertyNamingFor<GroupDelegate, string>(g => g.SearchableName);
            groupDelegates = Builder<GroupDelegate>.CreateListOfSize(15)
                .All()
                .With(g => g.LastName = "Surname")
                .With((g, i) => g.FirstName = NBuilderAlphabeticalPropertyNamingHelper.IndexToAlphabeticalString(i))
                .Build().ToArray();
        }

        [Test]
        public void GroupDelegatesViewModel_should_default_to_returning_the_first_page_worth_of_delegates()
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
                model.GroupDelegates.Count().Should().Be(BaseSearchablePageViewModel.DefaultItemsPerPage);
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
            var expectedFirstGroupDelegate =
                groupDelegates.Skip(BaseSearchablePageViewModel.DefaultItemsPerPage).First();

            using (new AssertionScope())
            {
                model.GroupId.Should().Be(1);
                model.NavViewModel.Should().BeEquivalentTo(expectedNavViewModel);
                model.GroupDelegates.Count().Should().Be(5);
                model.GroupDelegates.First().Name.Should().BeEquivalentTo($"{expectedFirstGroupDelegate.FirstName} {expectedFirstGroupDelegate.LastName}");
            }
        }
    }
}
