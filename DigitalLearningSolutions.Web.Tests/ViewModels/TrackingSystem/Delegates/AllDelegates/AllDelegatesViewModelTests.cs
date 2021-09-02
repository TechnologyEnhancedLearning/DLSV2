namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.AllDelegates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
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
                { FirstName = "o", LastName = "Surname", DateRegistered = DateTime.Today.AddDays(1) }
        };

        [Test]
        public void All_delegates_should_default_to_returning_the_first_ten_delegates()
        {
            var model = new AllDelegatesViewModel(
                delegateUsers,
                new List<(int, string)>(),
                new List<CustomPrompt>(),
                1,
                null,
                DelegateSortByOptions.Name.PropertyName,
                BaseSearchablePageViewModel.Ascending,
                null
            );

            model.Delegates.Count().Should().Be(10);
            model.Delegates.FirstOrDefault(delegateUser => delegateUser.DelegateInfo.Name == "k Surname").Should()
                .BeNull();
        }

        [Test]
        public void All_delegates_should_correctly_return_the_second_page_of_delegates()
        {
            var model = new AllDelegatesViewModel(
                delegateUsers,
                new List<(int, string)>(),
                new List<CustomPrompt>(),
                2,
                null,
                DelegateSortByOptions.Name.PropertyName,
                BaseSearchablePageViewModel.Ascending,
                null
            );

            model.Delegates.Count().Should().Be(5);
            model.Delegates.First().DelegateInfo.Name.Should().BeEquivalentTo("k Surname");
        }

        [Test]
        public void All_delegates_should_search_delegates_correctly()
        {
            var model = new AllDelegatesViewModel(
                delegateUsers,
                new List<(int, string)>(),
                new List<CustomPrompt>(),
                1,
                "purple",
                DelegateSortByOptions.Name.PropertyName,
                BaseSearchablePageViewModel.Ascending,
                null
            );

            model.Delegates.Count().Should().Be(2);
            model.Delegates.ToList()[0].DelegateInfo.Name.Should().Be("b purple Surname");
            model.Delegates.ToList()[1].DelegateInfo.Name.Should().Be("d purple Surname");
        }

        [Test]
        public void All_delegates_should_sort_delegates_correctly()
        {
            var model = new AllDelegatesViewModel(
                delegateUsers,
                new List<(int, string)>(),
                new List<CustomPrompt>(),
                1,
                null,
                DelegateSortByOptions.RegistrationDate.PropertyName,
                BaseSearchablePageViewModel.Descending,
                null
            );

            model.Delegates.Count().Should().Be(10);
            model.Delegates.ToList()[0].DelegateInfo.Name.Should().Be("o Surname");
            model.Delegates.ToList()[1].DelegateInfo.Name.Should().Be("k Surname");
        }

        [Test]
        public void All_delegates_should_filter_delegates_correctly()
        {
            var model = new AllDelegatesViewModel(
                delegateUsers,
                new List<(int, string)>(),
                new List<CustomPrompt>(),
                1,
                null,
                DelegateSortByOptions.RegistrationDate.PropertyName,
                BaseSearchablePageViewModel.Ascending,
                "Answer4" + FilteringHelper.Separator + "Answer4" + FilteringHelper.Separator + "C 2"
            );

            model.Delegates.Count().Should().Be(2);
            model.Delegates.ToList()[0].DelegateInfo.Name.Should().Be("m Surname");
            model.Delegates.ToList()[1].DelegateInfo.Name.Should().Be("n Surname");
        }

        [Test]
        public void All_delegates_should_filter_delegates_correctly_by_empty_values()
        {
            var model = new AllDelegatesViewModel(
                delegateUsers,
                new List<(int, string)>(),
                new List<CustomPrompt>(),
                1,
                null,
                DelegateSortByOptions.RegistrationDate.PropertyName,
                BaseSearchablePageViewModel.Ascending,
                "Answer4" + FilteringHelper.Separator + "Answer4" + FilteringHelper.Separator +
                FilteringHelper.EmptyValue
            );

            model.Delegates.Count().Should().Be(6);
            model.Delegates.ToList()[0].DelegateInfo.Name.Should().Be("a Surname");
            model.Delegates.ToList()[1].DelegateInfo.Name.Should().Be("b purple Surname");
            model.Delegates.ToList()[2].DelegateInfo.Name.Should().Be("c Surname");
            model.Delegates.ToList()[3].DelegateInfo.Name.Should().Be("d purple Surname");
            model.Delegates.ToList()[4].DelegateInfo.Name.Should().Be("k Surname");
            model.Delegates.ToList()[5].DelegateInfo.Name.Should().Be("o Surname");
        }
    }
}
