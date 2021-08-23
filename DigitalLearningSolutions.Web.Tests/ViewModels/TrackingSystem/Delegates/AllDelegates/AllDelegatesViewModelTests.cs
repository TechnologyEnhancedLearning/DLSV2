namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.AllDelegates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class AllDelegatesViewModelTests
    {
        private readonly CentreCustomPromptHelper customPromptHelper;
        private readonly ICentreCustomPromptsService customPromptsService;

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

        public AllDelegatesViewModelTests()
        {
            customPromptsService = A.Fake<ICentreCustomPromptsService>();
            customPromptHelper = new CentreCustomPromptHelper(customPromptsService);
        }

        [Test]
        public void All_delegates_should_default_to_returning_the_first_ten_delegates()
        {
            var model = new AllDelegatesViewModel(
                1,
                delegateUsers,
                new List<(int, string)>(),
                customPromptHelper,
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
                1,
                delegateUsers,
                new List<(int, string)>(),
                customPromptHelper,
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
        public void All_delegates_filters_should_be_set()
        {
            // Given
            var passwordStatusOptions = new[]
            {
                DelegatePasswordStatusFilterOptions.PasswordSet,
                DelegatePasswordStatusFilterOptions.PasswordNotSet
            };

            var adminStatusOptions = new[]
            {
                DelegateAdminStatusFilterOptions.IsAdmin,
                DelegateAdminStatusFilterOptions.IsNotAdmin
            };

            var activeStatusOptions = new[]
            {
                DelegateActiveStatusFilterOptions.IsActive,
                DelegateActiveStatusFilterOptions.IsNotActive
            };

            var jobGroups = new List<(int id, string name)>
                { (1, "J 1"), (2, "J 2") };
            var jobGroupOptions = new[]
            {
                new FilterOptionViewModel(
                    "J 1",
                    "JobGroupId" + FilteringHelper.Separator +
                    "JobGroupId" + FilteringHelper.Separator + 1,
                    FilterStatus.Default
                ),
                new FilterOptionViewModel(
                    "J 2",
                    "JobGroupId" + FilteringHelper.Separator +
                    "JobGroupId" + FilteringHelper.Separator + 2,
                    FilterStatus.Default
                )
            };

            var registrationTypeOptions = new[]
            {
                DelegateRegistrationTypeFilterOptions.SelfRegistered,
                DelegateRegistrationTypeFilterOptions.SelfRegisteredExternal,
                DelegateRegistrationTypeFilterOptions.RegisteredByCentre,
            };

            var customPrompt1 = CustomPromptsTestHelper.GetDefaultCustomPrompt(
                1,
                "First prompt",
                "Clinical\r\nNon-Clinical"
            );
            var customPrompt3 = CustomPromptsTestHelper.GetDefaultCustomPrompt(3);
            var customPrompt4 = CustomPromptsTestHelper.GetDefaultCustomPrompt(4, "Fourth prompt", "C 1\r\nC 2\r\nC 3");
            var centreCustomPrompts = new CentreCustomPrompts(
                1,
                new List<CustomPrompt> { customPrompt1, customPrompt3, customPrompt4 }
            );
            A.CallTo(() => customPromptsService.GetCustomPromptsForCentreByCentreId(1)).Returns(centreCustomPrompts);
            var customPrompt1Options = new[]
            {
                new FilterOptionViewModel(
                    "Clinical",
                    "Answer1" + FilteringHelper.Separator +
                    "Answer1" + FilteringHelper.Separator + "Clinical",
                    FilterStatus.Default
                ),
                new FilterOptionViewModel(
                    "Non-Clinical",
                    "Answer1" + FilteringHelper.Separator +
                    "Answer1" + FilteringHelper.Separator + "Non-Clinical",
                    FilterStatus.Default
                ),
                new FilterOptionViewModel(
                    "No option selected",
                    "Answer1" + FilteringHelper.Separator +
                    "Answer1" + FilteringHelper.Separator + FilteringHelper.EmptyValue,
                    FilterStatus.Default
                )
            };
            var customPrompt4Options = new[]
            {
                new FilterOptionViewModel(
                    "C 1",
                    "Answer4" + FilteringHelper.Separator +
                    "Answer4" + FilteringHelper.Separator + "C 1",
                    FilterStatus.Default
                ),
                new FilterOptionViewModel(
                    "C 2",
                    "Answer4" + FilteringHelper.Separator +
                    "Answer4" + FilteringHelper.Separator + "C 2",
                    FilterStatus.Default
                ),
                new FilterOptionViewModel(
                    "C 3",
                    "Answer4" + FilteringHelper.Separator +
                    "Answer4" + FilteringHelper.Separator + "C 3",
                    FilterStatus.Default
                ),
                new FilterOptionViewModel(
                    "No option selected",
                    "Answer4" + FilteringHelper.Separator +
                    "Answer4" + FilteringHelper.Separator + FilteringHelper.EmptyValue,
                    FilterStatus.Default
                )
            };

            var expectedFilters = new[]
            {
                new FilterViewModel("PasswordStatus", "Password Status", passwordStatusOptions),
                new FilterViewModel("AdminStatus", "Admin Status", adminStatusOptions),
                new FilterViewModel("ActiveStatus", "Active Status", activeStatusOptions),
                new FilterViewModel("JobGroupId", "Job Group", jobGroupOptions),
                new FilterViewModel("RegistrationType", "Registration Type", registrationTypeOptions),
                new FilterViewModel("CustomPrompt1", "First prompt", customPrompt1Options),
                new FilterViewModel("CustomPrompt4", "Fourth prompt", customPrompt4Options)
            }.AsEnumerable();

            // When
            var model = new AllDelegatesViewModel(
                1,
                delegateUsers,
                jobGroups,
                customPromptHelper,
                2,
                null,
                DelegateSortByOptions.Name.PropertyName,
                BaseSearchablePageViewModel.Ascending,
                null
            );

            // Then
            model.Filters.Should().BeEquivalentTo(expectedFilters);
        }

        [Test]
        public void All_delegates_should_search_delegates_correctly()
        {
            var model = new AllDelegatesViewModel(
                1,
                delegateUsers,
                new List<(int, string)>(),
                customPromptHelper,
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
                1,
                delegateUsers,
                new List<(int, string)>(),
                customPromptHelper,
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
                1,
                delegateUsers,
                new List<(int, string)>(),
                customPromptHelper,
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
                1,
                delegateUsers,
                new List<(int, string)>(),
                customPromptHelper,
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
