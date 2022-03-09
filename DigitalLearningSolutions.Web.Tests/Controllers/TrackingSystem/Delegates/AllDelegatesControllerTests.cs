namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AllDelegates;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public class AllDelegatesControllerTests
    {
        private AllDelegatesController allDelegatesController = null!;

        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private PromptsService promptsHelper = null!;
        private ISearchSortFilterPaginateService searchSortFilterPaginateService = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void Setup()
        {
            var centreRegistrationPromptsService = A.Fake<ICentreRegistrationPromptsService>();
            promptsHelper = new PromptsService(centreRegistrationPromptsService);
            userDataService = A.Fake<IUserDataService>();
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            searchSortFilterPaginateService = A.Fake<ISearchSortFilterPaginateService>();

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            const string cookieName = "DelegateFilter";
            const string cookieValue = "ActiveStatus|Active|false";

            allDelegatesController = new AllDelegatesController(
                    userDataService,
                    promptsHelper,
                    jobGroupsDataService,
                    searchSortFilterPaginateService
                )
                .WithMockHttpContext(httpRequest, cookieName, cookieValue, httpResponse)
                .WithMockUser(true)
                .WithMockServices()
                .WithMockTempData();
        }

        [Test]
        public void Index_with_no_query_parameters_uses_cookie_value_for_existingFilterString()
        {
            // Given
            SearchSortFilterAndPaginateTestHelper
                .GivenACallToSearchSortFilterPaginateServiceReturnsResult<DelegateUserCard>(
                    searchSortFilterPaginateService
                );

            // When
            var result = allDelegatesController.Index();

            // Then
            result.As<ViewResult>().Model.As<AllDelegatesViewModel>().ExistingFilterString.Should()
                .Be("ActiveStatus|Active|false");
        }

        [Test]
        public void Index_with_query_parameters_uses_query_parameter_value_for_existingFilterString()
        {
            // Given
            const string existingFilterString = "PasswordStatus|IsPasswordSet|true";
            A.CallTo(() => httpRequest.Query.ContainsKey("existingFilterString")).Returns(true);
            SearchSortFilterAndPaginateTestHelper
                .GivenACallToSearchSortFilterPaginateServiceReturnsResult<DelegateUserCard>(
                    searchSortFilterPaginateService
                );

            // When
            var result = allDelegatesController.Index(existingFilterString: existingFilterString);

            // Then
            result.As<ViewResult>().Model.As<AllDelegatesViewModel>().ExistingFilterString.Should()
                .Be(existingFilterString);
        }

        [Test]
        public void Index_with_clearFilters_query_parameter_true_sets_cookie_to_CLEAR()
        {
            // Given
            SearchSortFilterAndPaginateTestHelper
                .GivenACallToSearchSortFilterPaginateServiceReturnsResult<DelegateUserCard>(
                    searchSortFilterPaginateService
                );

            // When
            var result = allDelegatesController.Index(clearFilters: true);

            // Then
            A.CallTo(
                    () => httpResponse.Cookies.Append(
                        "DelegateFilter",
                        FilteringHelper.EmptyFiltersCookieValue,
                        A<CookieOptions>._
                    )
                )
                .MustHaveHappened();
            result.As<ViewResult>().Model.As<AllDelegatesViewModel>().ExistingFilterString.Should()
                .BeNull();
        }

        [Test]
        public void Index_with_null_existingFilterString_and_new_filter_query_parameter_adds_new_cookie_value()
        {
            // Given
            const string? existingFilterString = null;
            const string newFilterToAdd = "PasswordStatus|IsPasswordSet|true";
            SearchSortFilterAndPaginateTestHelper
                .GivenACallToSearchSortFilterPaginateServiceReturnsResult<DelegateUserCard>(
                    searchSortFilterPaginateService
                );

            // When
            var result = allDelegatesController.Index(
                existingFilterString: existingFilterString,
                newFilterToAdd: newFilterToAdd
            );

            // Then
            A.CallTo(() => httpResponse.Cookies.Append("DelegateFilter", newFilterToAdd, A<CookieOptions>._))
                .MustHaveHappened();
            result.As<ViewResult>().Model.As<AllDelegatesViewModel>().ExistingFilterString.Should()
                .Be(newFilterToAdd);
        }

        [Test]
        public void Index_with_no_filtering_should_default_to_Active_delegates()
        {
            // Given
            A.CallTo(() => httpRequest.Cookies).Returns(A.Fake<IRequestCookieCollection>());
            SearchSortFilterAndPaginateTestHelper
                .GivenACallToSearchSortFilterPaginateServiceReturnsResult<DelegateUserCard>(
                    searchSortFilterPaginateService
                );

            // When
            var result = allDelegatesController.Index();

            // Then
            result.As<ViewResult>().Model.As<AllDelegatesViewModel>().ExistingFilterString.Should()
                .Be("ActiveStatus|Active|true");
        }
    }
}
