namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class EmailDelegatesControllerTests
    {
        private const string CookieName = "EmailDelegateFilter";
        private IConfiguration config = null!;
        private EmailDelegatesController emailDelegatesController = null!;
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private IPasswordResetService passwordResetService = null!;
        private PromptsService promptsHelper = null!;
        private ISearchSortFilterPaginateService searchSortFilterPaginateService = null!;
        private IUserService userService = null!;
        private ICentreRegistrationPromptsService centreRegistrationPromptsService = null!;
        private IClockUtility clockUtility = null!;

        [SetUp]
        public void Setup()
        {
            centreRegistrationPromptsService = A.Fake<ICentreRegistrationPromptsService>();
            promptsHelper = new PromptsService(centreRegistrationPromptsService);
            userService = A.Fake<IUserService>();
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            passwordResetService = A.Fake<IPasswordResetService>();
            searchSortFilterPaginateService = A.Fake<ISearchSortFilterPaginateService>();
            config = A.Fake<IConfiguration>();
            clockUtility = A.Fake<IClockUtility>();

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();

            const string cookieValue = "JobGroupId|JobGroupId|1";

            emailDelegatesController = new EmailDelegatesController(
                    promptsHelper,
                    jobGroupsDataService,
                    passwordResetService,
                    userService,
                    searchSortFilterPaginateService,
                    config,
                    clockUtility
                )
                .WithMockHttpContext(httpRequest, CookieName, cookieValue, httpResponse)
                .WithMockUser(true)
                .WithMockServices()
                .WithMockTempData();
        }

        [Test]
        public void Index_calls_expected_methods_and_returns_view()
        {
            // When
            var result = emailDelegatesController.Index();

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => userService.GetDelegateUserCardsForWelcomeEmail(A<int>._)).MustHaveHappened();
                A.CallTo(() => centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(A<int>._))
                    .MustHaveHappened();
                A.CallTo(
                    () => searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                        A<IEnumerable<DelegateUserCard>>._,
                        A<SearchSortFilterAndPaginateOptions>._
                    )
                ).MustHaveHappened();
                A.CallTo(
                        () => httpResponse.Cookies.Append(
                            CookieName,
                            A<string>._,
                            A<CookieOptions>._
                        )
                    )
                    .MustHaveHappened();
                result.Should().BeViewResult().WithDefaultViewName();
            }
        }
    }
}
