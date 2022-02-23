namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.AllDelegates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AllDelegates;
    using FluentAssertions;
    using NUnit.Framework;

    public class AllDelegatesViewModelTests
    {
        private readonly DelegateUserCard[] delegateUsers =
        {
            new DelegateUserCard
                { FirstName = "a", LastName = "Surname", Answer4 = string.Empty },
            new DelegateUserCard
                { FirstName = "b purple", LastName = "Surname", Answer4 = string.Empty },
            new DelegateUserCard
                { FirstName = "c", LastName = "Surname" },
            new DelegateUserCard
                { FirstName = "d purple", LastName = "Surname" },
            new DelegateUserCard
                { FirstName = "e", LastName = "Surname", Answer4 = "C 1" },
            new DelegateUserCard
                { FirstName = "f", LastName = "Surname", Answer4 = "C 1" },
            new DelegateUserCard
                { FirstName = "g", LastName = "Surname", Answer4 = "C 1" },
            new DelegateUserCard
                { FirstName = "h", LastName = "Surname", Answer4 = "C 1" },
            new DelegateUserCard
                { FirstName = "i", LastName = "Surname", Answer4 = "C 1" },
            new DelegateUserCard
                { FirstName = "j", LastName = "Surname", Answer4 = "C 1" },
            new DelegateUserCard
                { FirstName = "k", LastName = "Surname", DateRegistered = DateTime.Today },
            new DelegateUserCard
                { FirstName = "l", LastName = "Surname", Answer4 = "C 1" },
            new DelegateUserCard
                { FirstName = "m", LastName = "Surname", Answer4 = "C 2" },
            new DelegateUserCard
                { FirstName = "n", LastName = "Surname", Answer4 = "C 2" },
            new DelegateUserCard
                { FirstName = "o", LastName = "Surname", DateRegistered = DateTime.Today.AddDays(1) },
            new DelegateUserCard
                { FirstName = "p", LastName = "Surname", Answer4 = "C 1" },
        };

        [Test]
        public void All_delegates_should_default_to_returning_the_first_ten_delegates()
        {
            // When
            var model = new AllDelegatesViewModel(
                delegateUsers,
                new List<(int, string)>(),
                new List<CustomPrompt>(),
                1,
                null,
                DelegateSortByOptions.Name.PropertyName,
                GenericSortingHelper.Ascending,
                null,
                null
            );

            // Then
            model.Delegates.Count().Should().Be(10);
            model.Delegates.FirstOrDefault(delegateUser => delegateUser.DelegateInfo.Name == "k Surname").Should()
                .BeNull();
        }

        [Test]
        public void All_delegates_should_correctly_return_the_second_page_of_delegates()
        {
            // When
            var model = new AllDelegatesViewModel(
                delegateUsers,
                new List<(int, string)>(),
                new List<CustomPrompt>(),
                2,
                null,
                DelegateSortByOptions.Name.PropertyName,
                GenericSortingHelper.Ascending,
                null,
                null
            );

            // Then
            model.Delegates.Count().Should().Be(6);
            model.Delegates.First().DelegateInfo.Name.Should().BeEquivalentTo("k Surname");
        }

        [Test]
        public void All_delegates_should_default_to_returning_the_first_fifteen_delegates()
        {
            // When
            var model = new AllDelegatesViewModel(
                delegateUsers,
                new List<(int, string)>(),
                new List<CustomPrompt>(),
                1,
                null,
                DelegateSortByOptions.Name.PropertyName,
                GenericSortingHelper.Ascending,
                null,
                15
            );

            // Then
            model.Delegates.Count().Should().Be(15);
            model.Delegates.FirstOrDefault(delegateUser => delegateUser.DelegateInfo.Name == "p Surname").Should()
                .BeNull();
        }

        [Test]
        public void All_delegates_should_search_delegates_correctly()
        {
            // When
            var model = new AllDelegatesViewModel(
                delegateUsers,
                new List<(int, string)>(),
                new List<CustomPrompt>(),
                1,
                "purple",
                DelegateSortByOptions.Name.PropertyName,
                GenericSortingHelper.Ascending,
                null,
                null
            );

            // Then
            model.Delegates.Count().Should().Be(2);
            model.Delegates.ToList()[0].DelegateInfo.Name.Should().Be("b purple Surname");
            model.Delegates.ToList()[1].DelegateInfo.Name.Should().Be("d purple Surname");
        }

        [Test]
        public void All_delegates_should_sort_delegates_correctly()
        {
            // When
            var model = new AllDelegatesViewModel(
                delegateUsers,
                new List<(int, string)>(),
                new List<CustomPrompt>(),
                1,
                null,
                DelegateSortByOptions.RegistrationDate.PropertyName,
                GenericSortingHelper.Descending,
                null,
                null
            );

            // Then
            model.Delegates.Count().Should().Be(10);
            model.Delegates.ToList()[0].DelegateInfo.Name.Should().Be("o Surname");
            model.Delegates.ToList()[1].DelegateInfo.Name.Should().Be("k Surname");
        }

        [Test]
        public void All_delegates_should_filter_delegates_correctly()
        {
            // When
            var model = new AllDelegatesViewModel(
                delegateUsers,
                new List<(int, string)>(),
                new List<CustomPrompt>(),
                1,
                null,
                DelegateSortByOptions.RegistrationDate.PropertyName,
                GenericSortingHelper.Ascending,
                "Answer4" + FilteringHelper.Separator + "Answer4" + FilteringHelper.Separator + "C 2",
                null
            );

            // Then
            model.Delegates.Count().Should().Be(2);
            model.Delegates.ToList()[0].DelegateInfo.Name.Should().Be("m Surname");
            model.Delegates.ToList()[1].DelegateInfo.Name.Should().Be("n Surname");
        }

        [Test]
        public void All_delegates_should_filter_delegates_correctly_by_empty_values()
        {
            // When
            var model = new AllDelegatesViewModel(
                delegateUsers,
                new List<(int, string)>(),
                new List<CustomPrompt>(),
                1,
                null,
                DelegateSortByOptions.RegistrationDate.PropertyName,
                GenericSortingHelper.Ascending,
                "Answer4" + FilteringHelper.Separator + "Answer4" + FilteringHelper.Separator +
                FilteringHelper.EmptyValue,
                null
            );

            // Then
            model.Delegates.Count().Should().Be(6);
            model.Delegates.ToList()[0].DelegateInfo.Name.Should().Be("a Surname");
            model.Delegates.ToList()[1].DelegateInfo.Name.Should().Be("b purple Surname");
            model.Delegates.ToList()[2].DelegateInfo.Name.Should().Be("c Surname");
            model.Delegates.ToList()[3].DelegateInfo.Name.Should().Be("d purple Surname");
            model.Delegates.ToList()[4].DelegateInfo.Name.Should().Be("k Surname");
            model.Delegates.ToList()[5].DelegateInfo.Name.Should().Be("o Surname");
        }

        [Test]
        public void All_delegates_filters_should_only_include_custom_prompts_with_options()
        {
            // Given
            var customPrompts = new List<CustomPrompt>
            {
                new CustomPrompt(RegistrationField.CentreCustomPrompt1, "free text", null, true),
                new CustomPrompt(RegistrationField.CentreCustomPrompt2, "with options", "A\r\nB", true),
            };

            // When
            var model = new AllDelegatesViewModel(
                delegateUsers,
                new List<(int, string)>(),
                customPrompts,
                1,
                null,
                DelegateSortByOptions.RegistrationDate.PropertyName,
                GenericSortingHelper.Ascending,
                null,
                null
            );

            // Then
            model.Filters.Should().NotContain(filter => filter.FilterProperty == "CustomPrompt1");
            model.Filters.Should().Contain(filter => filter.FilterProperty == "CustomPrompt2");
        }
    }
}
