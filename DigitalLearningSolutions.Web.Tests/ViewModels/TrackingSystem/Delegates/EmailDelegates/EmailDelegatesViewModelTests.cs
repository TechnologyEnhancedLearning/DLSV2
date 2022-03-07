namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.EmailDelegates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EmailDelegates;
    using FluentAssertions;
    using NUnit.Framework;

    public class EmailDelegatesViewModelTests
    {
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
        public void EmailDelegatesViewModel_should_return_all_delegates_on_one_page()
        {
            // When
            var model = new EmailDelegatesViewModel(
                delegateUsers,
                new List<(int, string)>(),
                new List<CentreRegistrationPrompt>(),
                null
            );

            // Then
            model.Delegates!.Should().HaveCount(delegateUsers.Length);
            model.Delegates!.First().Name.Should().Be("a Surname");
            model.Delegates!.Last().Name.Should().Be("o Surname");
        }

        [Test]
        public void EmailDelegatesViewModel_sets_delivery_date_today_by_default()
        {
            // When
            var model = new EmailDelegatesViewModel(
                delegateUsers,
                new List<(int, string)>(),
                new List<CentreRegistrationPrompt>(),
                null
            );

            // Then
            model.Day.Should().Be(DateTime.Today.Day);
            model.Month.Should().Be(DateTime.Today.Month);
            model.Year.Should().Be(DateTime.Today.Year);
        }

        [Test]
        public void EmailDelegatesViewModel_should_only_include_custom_prompts_with_options()
        {
            // Given
            var centreRegistrationPrompts = new List<CentreRegistrationPrompt>
            {
                new CentreRegistrationPrompt(1, "free text", null, true),
                new CentreRegistrationPrompt(2, "with options", "A\r\nB", true),
            };

            // When
            var model = new EmailDelegatesViewModel(delegateUsers, new List<(int, string)>(), centreRegistrationPrompts, null);

            // Then
            model.Filters.Should().NotContain(filter => filter.FilterProperty == "CentreRegistrationPrompt1");
            model.Filters.Should().Contain(filter => filter.FilterProperty == "CentreRegistrationPrompt2");
        }

        [Test]
        public void EmailDelegatesViewModel_should_filter_delegates_correctly()
        {
            // Given
            var existingFilterString = "Answer4" + FilteringHelper.Separator + "Answer4" + FilteringHelper.Separator + "C 2";

            // When
            var model = new EmailDelegatesViewModel(
                delegateUsers,
                new List<(int id, string name)>(),
                new List<CentreRegistrationPrompt>(),
                existingFilterString
            );

            // Then
            model.Delegates!.Should().HaveCount(2);
            model.Delegates!.ToList()[0].Name.Should().Be("m Surname");
            model.Delegates!.ToList()[1].Name.Should().Be("n Surname");
            model.MatchingSearchResults.Should().Be(2);
        }

        [Test]
        public void EmailDelegatesViewModel_should_filter_delegates_correctly_by_empty_values()
        {
            // Given
            var existingFilterString = "Answer4" + FilteringHelper.Separator + "Answer4" + FilteringHelper.Separator +
                           FilteringHelper.EmptyValue;

            // When
            var model = new EmailDelegatesViewModel(
                delegateUsers,
                new List<(int id, string name)>(),
                new List<CentreRegistrationPrompt>(),
                existingFilterString
            );

            // Then
            model.Delegates!.Should().HaveCount(6);
            model.Delegates!.ToList()[0].Name.Should().Be("a Surname");
            model.Delegates!.ToList()[1].Name.Should().Be("b purple Surname");
            model.Delegates!.ToList()[2].Name.Should().Be("c Surname");
            model.Delegates!.ToList()[3].Name.Should().Be("d purple Surname");
            model.Delegates!.ToList()[4].Name.Should().Be("k Surname");
            model.Delegates!.ToList()[5].Name.Should().Be("o Surname");
            model.MatchingSearchResults.Should().Be(6);
        }

        [Test]
        public void EmailDelegatesViewModel_should_set_all_delegates_if_existingFilterString_is_null()
        {
            // When
            var model = new EmailDelegatesViewModel(
                delegateUsers,
                new List<(int id, string name)>(),
                new List<CentreRegistrationPrompt>(),
                null
            );

            // Then
            model.Delegates!.Count().Should().Be(delegateUsers.Length);
            model.MatchingSearchResults.Should().Be(delegateUsers.Length);
        }

        [Test]
        public void EmailDelegatesViewModel_should_set_IsDelegateSelected_values_based_on_selectedDelegateIds()
        {
            // Given
            var selectedDelegateIds = new List<int> { 1, 4, 7 };

            // When
            var model = new EmailDelegatesViewModel(
                delegateUsers,
                new List<(int id, string name)>(),
                new List<CentreRegistrationPrompt>(),
                null
            ) { SelectedDelegateIds = selectedDelegateIds };

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
                delegateUsers,
                new List<(int id, string name)>(),
                new List<CentreRegistrationPrompt>(),
                null,
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
