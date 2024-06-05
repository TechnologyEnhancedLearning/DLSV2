namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;
    using Microsoft.Extensions.Configuration;
    using Microsoft.AspNetCore.Hosting;

    public class AllDelegatesControllerTests
    {
        private const string CookieName = "DelegateFilter";
        private AllDelegatesController allDelegatesController = null!;
        private ICentreRegistrationPromptsService centreRegistrationPromptsService = null!;
        private IDelegateDownloadFileService delegateDownloadFileService = null!;
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private PromptsService promptsHelper = null!;
        private IPaginateService paginateService = null!;
        private IUserDataService userDataService = null!;
        private IGroupsService groupsService = null!;
        private IConfiguration? configuration;
        private IWebHostEnvironment? env;

        [SetUp]
        public void Setup()
        {
            centreRegistrationPromptsService = A.Fake<ICentreRegistrationPromptsService>();
            delegateDownloadFileService = A.Fake<IDelegateDownloadFileService>();
            promptsHelper = new PromptsService(centreRegistrationPromptsService);
            userDataService = A.Fake<IUserDataService>();
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            paginateService = A.Fake<IPaginateService>();
            groupsService = A.Fake<IGroupsService>();
            configuration = A.Fake<IConfiguration>();
            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            env = A.Fake<IWebHostEnvironment>();

            const string cookieValue = "ActiveStatus|Active|false";

            allDelegatesController = new AllDelegatesController(
                    delegateDownloadFileService,
                    userDataService,
                    promptsHelper,
                    jobGroupsDataService,
                    paginateService,
                    groupsService,
                    configuration,
                    env
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

            var loggedInAdmin = UserTestHelper.GetDefaultAdminEntity();
            A.CallTo(() => userDataService.GetAdminById(loggedInAdmin.AdminAccount.Id)).Returns(loggedInAdmin);

            var result = allDelegatesController.Index();

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => userDataService.GetDelegateUserCards(A<string>._, A<int>._, A<int>._, A<string>._,
                    A<string>._, A<int>._, A<string>._, A<string>._, A<string>._, A<string>._, A<string>._,
                    A<string>._, A<int>._, A<int?>._, A<string>._, A<string>._, A<string>._, A<string>._, A<string>._, A<string>._)
                ).MustHaveHappened();
                A.CallTo(() => jobGroupsDataService.GetJobGroupsAlphabetical())
                    .MustHaveHappened();
                A.CallTo(() => centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(A<int>._))
                    .MustHaveHappened();
                A.CallTo(
                    () => paginateService.Paginate(
                        A<IEnumerable<DelegateUserCard>>._,
                        A<int>._,
                        A<PaginationOptions>._, A<FilterOptions>._, A<string>._, A<string>._, A<string>._
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

        [Test]
        public void Export_passes_in_used_parameters_to_file()
        {
            // Given
            const string searchString = "Frame by Frame";
            const string sortBy = "Discipline";
            const string sortDirection = "The Sheltering Sky";
            const string filters = "Indiscipline";
            var centreId = allDelegatesController.User.GetCentreIdKnownNotNull();

            // When
            allDelegatesController.Export(searchString, sortBy, sortDirection, filters);

            // Then
            A.CallTo(
                () => delegateDownloadFileService.GetAllDelegatesFileForCentre(
                    centreId,
                    searchString,
                    sortBy,
                    sortDirection,
                    filters
                )
            ).MustHaveHappenedOnceExactly();
        }
    }
}
