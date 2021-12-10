namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class DelegateGroupsViewModelTests
    {
        private readonly Group[] groups =
        {
            new Group { GroupLabel = "A", AddedByAdminId = 1, AddedByFirstName = "Test", AddedByLastName = "Admin" },
            new Group { GroupLabel = "B", AddedByAdminId = 1, AddedByFirstName = "Test", AddedByLastName = "Admin" },
            new Group { GroupLabel = "C", AddedByAdminId = 1, AddedByFirstName = "Test", AddedByLastName = "Admin" },
            new Group { GroupLabel = "D", AddedByAdminId = 1, AddedByFirstName = "Test", AddedByLastName = "Admin" },
            new Group { GroupLabel = "E", AddedByAdminId = 1, AddedByFirstName = "Test", AddedByLastName = "Admin" },
            new Group { GroupLabel = "F", AddedByAdminId = 2, AddedByFirstName = "Test", AddedByLastName = "Person" },
            new Group { GroupLabel = "G", AddedByAdminId = 2, AddedByFirstName = "Test", AddedByLastName = "Person" },
            new Group { GroupLabel = "H", AddedByAdminId = 2, AddedByFirstName = "Test", AddedByLastName = "Person" },
            new Group { GroupLabel = "I", AddedByAdminId = 2, AddedByFirstName = "Test", AddedByLastName = "Person" },
            new Group { GroupLabel = "J", AddedByAdminId = 2, AddedByFirstName = "Test", AddedByLastName = "Person" },
            new Group { GroupLabel = "K", AddedByAdminId = 2, AddedByFirstName = "Test", AddedByLastName = "Person" },
            new Group { GroupLabel = "L", AddedByAdminId = 2, AddedByFirstName = "Test", AddedByLastName = "Person" },
            new Group { GroupLabel = "M", AddedByAdminId = 2, AddedByFirstName = "Test", AddedByLastName = "Person" },
            new Group { GroupLabel = "N", AddedByAdminId = 2, AddedByFirstName = "Test", AddedByLastName = "Person" },
            new Group { GroupLabel = "O", AddedByAdminId = 2, AddedByFirstName = "Test", AddedByLastName = "Person" }
        };

        [Test]
        public void DelegateGroupsViewModel_should_default_to_returning_the_first_page_worth_of_delegates()
        {
            var model = new DelegateGroupsViewModel(
                groups.ToList(),
                new List<CustomPrompt>(),
                null!,
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
        public void DelegateGroupsViewModel_should_correctly_return_the_second_page_of_delegates()
        {
            var model = new DelegateGroupsViewModel(
                groups.ToList(),
                new List<CustomPrompt>(),
                null!,
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
        public void DelegateGroupsViewModel_filters_and_search_string_should_be_set()
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
                    DelegateGroupsViewModelFilterOptions.GetLinkedFieldOptions(new List<CustomPrompt>())
                )
            }.AsEnumerable();

            // When
            var model = new DelegateGroupsViewModel(
                groups.ToList(),
                new List<CustomPrompt>(),
                "K",
                nameof(Group.SearchableName),
                BaseSearchablePageViewModel.Ascending,
                null,
                2
            );

            // Then
            model.Filters.Should().BeEquivalentTo(expectedFilters);
            model.DelegateGroups.First().Name.Should().BeEquivalentTo("K");
            model.SearchString.Should().Be("K");
        }
    }
}
