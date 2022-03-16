namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.EmailDelegates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EmailDelegates;
    using FluentAssertions;
    using NUnit.Framework;

    public class EmailDelegatesViewModelTests
    {
        private readonly IEnumerable<FilterModel> availableFilters =
            EmailDelegatesViewModelFilterOptions.GetEmailDelegatesFilterModels(
                new List<(int, string)>(),
                new List<CentreRegistrationPrompt>()
            );

        private readonly DelegateUserCard[] delegateUsers =
        {
            new DelegateUserCard { Id = 1, FirstName = "a", LastName = "Surname", Answer4 = string.Empty },
            new DelegateUserCard { Id = 2, FirstName = "b purple", LastName = "Surname", Answer4 = string.Empty },
            new DelegateUserCard { Id = 3, FirstName = "c", LastName = "Surname" },
            new DelegateUserCard { Id = 4, FirstName = "d purple", LastName = "Surname" },
            new DelegateUserCard { Id = 5, FirstName = "e", LastName = "Surname", Answer4 = "C 1" },
            new DelegateUserCard { Id = 6, FirstName = "f", LastName = "Surname", Answer4 = "C 1" },
            new DelegateUserCard { Id = 7, FirstName = "g", LastName = "Surname", Answer4 = "C 1" },
            new DelegateUserCard { Id = 8, FirstName = "h", LastName = "Surname", Answer4 = "C 1" },
            new DelegateUserCard { Id = 9, FirstName = "i", LastName = "Surname", Answer4 = "C 1" },
            new DelegateUserCard { Id = 10, FirstName = "j", LastName = "Surname", Answer4 = "C 1" },
            new DelegateUserCard { Id = 11, FirstName = "k", LastName = "Surname", DateRegistered = DateTime.Today },
            new DelegateUserCard { Id = 12, FirstName = "l", LastName = "Surname", Answer4 = "C 1" },
            new DelegateUserCard { Id = 13, FirstName = "m", LastName = "Surname", Answer4 = "C 2" },
            new DelegateUserCard { Id = 14, FirstName = "n", LastName = "Surname", Answer4 = "C 2" },
            new DelegateUserCard
                { Id = 15, FirstName = "o", LastName = "Surname", DateRegistered = DateTime.Today.AddDays(1) },
        };

        [Test]
        public void EmailDelegatesViewModel_sets_delivery_date_today_by_default()
        {
            // When
            var model = new EmailDelegatesViewModel(
                new SearchSortFilterPaginationResult<DelegateUserCard>(
                    new PaginationResult<DelegateUserCard>(delegateUsers, 1, 1, int.MaxValue, 15, 15),
                    null,
                    null,
                    null,
                    null
                ),
                availableFilters
            );

            // Then
            model.Day.Should().Be(DateTime.Today.Day);
            model.Month.Should().Be(DateTime.Today.Month);
            model.Year.Should().Be(DateTime.Today.Year);
        }

        [Test]
        public void EmailDelegatesViewModel_should_set_IsDelegateSelected_values_based_on_selectedDelegateIds()
        {
            // Given
            var selectedDelegateIds = new List<int> { 1, 4, 7 };

            // When
            var model = new EmailDelegatesViewModel(
                    new SearchSortFilterPaginationResult<DelegateUserCard>(
                        new PaginationResult<DelegateUserCard>(delegateUsers, 1, 1, int.MaxValue, 15, 15),
                        null,
                        null,
                        null,
                        null
                    ),
                    availableFilters
                )
                { SelectedDelegateIds = selectedDelegateIds };

            // Then
            model.Delegates!.Count().Should().Be(delegateUsers.Length);
            model.MatchingSearchResults.Should().Be(delegateUsers.Length);
            model.Delegates!.Where(x => x.IsDelegateSelected).Select(x => x.Id).Should()
                .BeEquivalentTo(selectedDelegateIds);
        }

        [Test]
        public void EmailDelegatesViewModel_should_set_all_items_IsDelegateSelected_true_if_selectAll_true()
        {
            // When
            var model = new EmailDelegatesViewModel(
                new SearchSortFilterPaginationResult<DelegateUserCard>(
                    new PaginationResult<DelegateUserCard>(delegateUsers, 1, 1, int.MaxValue, 15, 15),
                    null,
                    null,
                    null,
                    null
                ),
                availableFilters,
                true
            );

            // Then
            model.Delegates!.Count().Should().Be(delegateUsers.Length);
            model.MatchingSearchResults.Should().Be(delegateUsers.Length);
            foreach (var emailDelegatesItemViewModel in model.Delegates!)
            {
                emailDelegatesItemViewModel.IsDelegateSelected.Should().BeTrue();
            }
        }
    }
}
