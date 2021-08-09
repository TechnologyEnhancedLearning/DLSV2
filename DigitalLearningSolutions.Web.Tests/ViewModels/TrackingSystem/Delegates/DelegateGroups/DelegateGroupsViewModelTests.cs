namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Collections.Generic;
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

    public class DelegateGroupsViewModelTests
    {
        private Group[] groups = null!;

        [SetUp]
        public void SetUp()
        {
            BuilderSetup.DisablePropertyNamingFor<Group, string>(g => g.SearchableName);
            groups = Builder<Group>.CreateListOfSize(15)
                .All()
                .With((g, i) => g.GroupLabel = NBuilderAlphabeticalPropertyNamingHelper.IndexToAlphabeticalString(i))
                .TheFirst(5)
                .With(g => g.AddedByAdminId = 1)
                .With(g => g.AddedByFirstName = "Test")
                .With(g => g.AddedByLastName = "Admin")
                .TheRest()
                .With(g => g.AddedByAdminId = 2)
                .With(g => g.AddedByFirstName = "Test")
                .With(g => g.AddedByLastName = "Person")
                .Build().ToArray();
        }

        [Test]
        public void GroupDelegatesViewModel_should_default_to_returning_the_first_page_worth_of_delegates()
        {
            var model = new DelegateGroupsViewModel(
                groups,
                new List<(int, string)>(),
                nameof(Group.SearchableName),
                BaseSearchablePageViewModel.Ascending,
                null,
                1
            );

            using (new AssertionScope())
            {
                model.DelegateGroups.Count().Should().Be(BaseSearchablePageViewModel.DefaultItemsPerPage);
                model.DelegateGroups.Any(groupDelegate => groupDelegate.Name == "K").Should()
                    .BeFalse();
            }
        }

        [Test]
        public void GroupDelegatesViewModel_should_correctly_return_the_second_page_of_delegates()
        {
            var model = new DelegateGroupsViewModel(
                groups,
                new List<(int, string)>(),
                nameof(Group.SearchableName),
                BaseSearchablePageViewModel.Ascending,
                null,
                2
            );

            var expectedFirstGroupDelegate =
                groups.Skip(BaseSearchablePageViewModel.DefaultItemsPerPage).First();

            using (new AssertionScope())
            {
                model.DelegateGroups.Count().Should().Be(5);
                model.DelegateGroups.First().Name.Should().BeEquivalentTo(expectedFirstGroupDelegate.GroupLabel);
            }
        }

        [Test]
        public void Centre_Administrators_filters_should_be_set()
        {
            // Given
            var admins = new[]
            {
                (1, "Test Admin"),
                (2, "Test Person")
            };

            var expectedFilters = new[]
            {
                new FilterViewModel(
                    nameof(Group.AddedByAdminId),
                    "Added by",
                    DelegateGroupsViewModelFilterOptions.GetAddedByOptions(admins)
                ),
                new FilterViewModel(
                    nameof(Group.LinkedToField),
                    "Linked field",
                    DelegateGroupsViewModelFilterOptions.GetLinkedFieldOptions(new List<(int, string)>())
                )
            }.AsEnumerable();

            // When
            var model = new DelegateGroupsViewModel(
                groups,
                new List<(int, string)>(),
                nameof(Group.SearchableName),
                BaseSearchablePageViewModel.Ascending,
                null,
                2
            );

            // Then
            model.Filters.Should().BeEquivalentTo(expectedFilters);
        }
    }
}
