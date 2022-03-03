namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EmailDelegates;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public class EmailDelegatesControllerTests
    {
        private PromptsService promptsHelper = null!;
        private EmailDelegatesController emailDelegatesController = null!;

        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private IPasswordResetService passwordResetService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void Setup()
        {
            var centreCustomPromptsService = A.Fake<ICentreRegistrationPromptsService>();
            promptsHelper = new PromptsService(centreCustomPromptsService);
            userService = A.Fake<IUserService>();
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            passwordResetService = A.Fake<IPasswordResetService>();

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            const string cookieName = "EmailDelegateFilter";
            const string cookieValue = "JobGroupId|JobGroupId|1";

            emailDelegatesController = new EmailDelegatesController(
                    promptsHelper,
                    jobGroupsDataService,
                    passwordResetService,
                    userService
                )
                .WithMockHttpContext(httpRequest, cookieName, cookieValue, httpResponse)
                .WithMockUser(true)
                .WithMockServices()
                .WithMockTempData();
        }

        [Test]
        public void Index_with_no_query_parameters_uses_cookie_value_for_filterBy()
        {
            // When
            var result = emailDelegatesController.Index();

            // Then
            result.As<ViewResult>().Model.As<EmailDelegatesViewModel>().FilterBy.Should()
                .Be("JobGroupId|JobGroupId|1");
        }

        [Test]
        public void Index_with_query_parameters_uses_query_parameter_value_for_filterBy()
        {
            // Given
            const string filterBy = "JobGroupId|JobGroupId|2";
            A.CallTo(() => httpRequest.Query.ContainsKey("filterBy")).Returns(true);

            // When
            var result = emailDelegatesController.Index(filterBy);

            // Then
            result.As<ViewResult>().Model.As<EmailDelegatesViewModel>().FilterBy.Should()
                .Be(filterBy);
        }

        [Test]
        public void Index_with_CLEAR_filterBy_query_parameter_removes_cookie()
        {
            // Given
            const string filterBy = "CLEAR";

            // When
            var result = emailDelegatesController.Index(filterBy);

            // Then
            A.CallTo(() => httpResponse.Cookies.Delete("EmailDelegateFilter")).MustHaveHappened();
            result.As<ViewResult>().Model.As<EmailDelegatesViewModel>().FilterBy.Should()
                .BeNull();
        }

        [Test]
        public void Index_with_null_filterBy_and_new_filter_query_parameter_adds_new_cookie_value()
        {
            // Given
            const string? filterBy = null;
            const string newFilterValue = "JobGroupId|JobGroupId|2";

            // When
            var result = emailDelegatesController.Index(filterBy, newFilterValue);

            // Then
            A.CallTo(() => httpResponse.Cookies.Append("EmailDelegateFilter", newFilterValue, A<CookieOptions>._))
                .MustHaveHappened();
            result.As<ViewResult>().Model.As<EmailDelegatesViewModel>().FilterBy.Should()
                .Be(newFilterValue);
        }

        [Test]
        public void Index_with_CLEAR_filterBy_and_new_filter_query_parameter_sets_new_cookie_value()
        {
            // Given
            const string filterBy = "CLEAR";
            const string newFilterValue = "JobGroupId|JobGroupId|2";

            // When
            var result = emailDelegatesController.Index(filterBy, newFilterValue);

            // Then
            A.CallTo(() => httpResponse.Cookies.Append("EmailDelegateFilter", newFilterValue, A<CookieOptions>._))
                .MustHaveHappened();
            result.As<ViewResult>().Model.As<EmailDelegatesViewModel>().FilterBy.Should()
                .Be(newFilterValue);
        }
    }
}
