﻿namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public class AllDelegatesControllerTests
    {
        private AllDelegatesController allDelegatesController = null!;
        private CentreCustomPromptHelper centreCustomPromptsHelper = null!;

        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void Setup()
        {
            var centreCustomPromptsService = A.Fake<ICentreCustomPromptsService>();
            centreCustomPromptsHelper = new CentreCustomPromptHelper(centreCustomPromptsService);
            userDataService = A.Fake<IUserDataService>();
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            const string cookieName = "DelegateFilter";
            const string cookieValue = "ActiveStatus|Active|true";

            allDelegatesController = new AllDelegatesController(
                    userDataService,
                    centreCustomPromptsHelper,
                    jobGroupsDataService
                )
                .WithMockHttpContextWithCookie(httpRequest, cookieName, cookieValue, httpResponse)
                .WithMockUser(true)
                .WithMockServices()
                .WithMockTempData();
        }

        [Test]
        public void Index_with_no_query_parameters_uses_cookie_value_for_filterBy()
        {
            // When
            var result = allDelegatesController.Index();

            // Then
            result.As<ViewResult>().Model.As<AllDelegatesViewModel>().FilterBy.Should()
                .Be("ActiveStatus|Active|true");
        }

        [Test]
        public void Index_with_query_parameters_uses_query_parameter_value_for_filterBy()
        {
            // Given
            const string filterBy = "PasswordStatus|IsPasswordSet|true";
            A.CallTo(() => httpRequest.Query.ContainsKey("filterBy")).Returns(true);

            // When
            var result = allDelegatesController.Index(filterBy: filterBy);

            // Then
            result.As<ViewResult>().Model.As<AllDelegatesViewModel>().FilterBy.Should()
                .Be(filterBy);
        }

        [Test]
        public void Index_with_CLEAR_filterBy_query_parameter_removes_cookie()
        {
            // Given
            const string? filterBy = "CLEAR";

            // When
            var result = allDelegatesController.Index(filterBy: filterBy);

            // Then
            A.CallTo(() => httpResponse.Cookies.Delete("DelegateFilter")).MustHaveHappened();
            result.As<ViewResult>().Model.As<AllDelegatesViewModel>().FilterBy.Should()
                .BeNull();
        }

        [Test]
        public void Index_with_null_filterBy_and_new_filter_query_parameter_add_new_cookie_value()
        {
            // Given
            const string? filterBy = null;
            const string? newFilterValue = "PasswordStatus|IsPasswordSet|true";

            // When
            var result = allDelegatesController.Index(filterBy: filterBy, filterValue: newFilterValue);

            // Then
            A.CallTo(() => httpResponse.Cookies.Append("DelegateFilter", newFilterValue, A<CookieOptions>._))
                .MustHaveHappened();
            result.As<ViewResult>().Model.As<AllDelegatesViewModel>().FilterBy.Should()
                .Be(newFilterValue);
        }

        [Test]
        public void Index_with_CLEAR_filterBy_and_new_filter_query_parameter_sets_new_cookie_value()
        {
            // Given
            const string? filterBy = null;
            const string? newFilterValue = "PasswordStatus|IsPasswordSet|true";

            // When
            var result = allDelegatesController.Index(filterBy: filterBy, filterValue: newFilterValue);

            // Then
            A.CallTo(() => httpResponse.Cookies.Append("DelegateFilter", newFilterValue, A<CookieOptions>._))
                .MustHaveHappened();
            result.As<ViewResult>().Model.As<AllDelegatesViewModel>().FilterBy.Should()
                .Be(newFilterValue);
        }
    }
}
