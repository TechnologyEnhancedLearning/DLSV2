﻿namespace DigitalLearningSolutions.Web.Tests.Controllers.SuperAdmin
{
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Controllers.SuperAdmin.Administrators;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;
    using System.Collections.Generic;

    public class AdminAccountsControllerTests
    {
        private AdminAccountsController administratorsController = null!;
        private IAdminDownloadFileService adminDownloadFileService = null!;
        private ISearchSortFilterPaginateService searchSortFilterPaginateService = null!;
        private ICentresService centresService = null!;
        private ICourseCategoriesService courseCategoriesService = null!;
        private IUserService userService = null!;
        private ICentreContractAdminUsageService centreContractAdminUsageService = null!;
        private INotificationPreferencesService notificationPreferencesService = null!;
        private INotificationService notificationService = null!;
        const string CookieName = "AdminFilter";
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;

        [SetUp]
        public void Setup()
        {
            centresService = A.Fake<ICentresService>();
            searchSortFilterPaginateService = A.Fake<ISearchSortFilterPaginateService>();
            adminDownloadFileService = A.Fake<IAdminDownloadFileService>();
            courseCategoriesService = A.Fake<ICourseCategoriesService>();
            userService = A.Fake<IUserService>();
            centreContractAdminUsageService = A.Fake<ICentreContractAdminUsageService>();
            notificationPreferencesService = A.Fake<INotificationPreferencesService>();
            notificationService = A.Fake<INotificationService>();

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            const string cookieValue = "Role|IsCentreAdmin|true";

            administratorsController = new AdminAccountsController(
                    centresService,
                    searchSortFilterPaginateService,
                    adminDownloadFileService,
                    courseCategoriesService,
                    userService,
                    centreContractAdminUsageService,
                    notificationPreferencesService,
                    notificationService
                )
                .WithMockHttpContext(httpRequest, CookieName, cookieValue, httpResponse)
                .WithMockUser(true)
                .WithMockServices()
                .WithMockTempData();
        }

        [Test]
        public void Index_calls_expected_methods_and_returns_view()
        {
            // Given
            var loggedInAdmin = UserTestHelper.GetDefaultAdminEntity();
            A.CallTo(() => userService.GetAdminById(loggedInAdmin.AdminAccount.Id)).Returns(loggedInAdmin);

            // When
            var result = administratorsController.Index();

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => userService.GetAllAdmins(A<string>._, A<int>._, A<int>._, A<int>._, A<string>._, A<string>._, A<int>._, A<int>._)).MustHaveHappened();
                A.CallTo(() => centresService.GetAllCentres(false)).MustHaveHappened();
                A.CallTo(
                    () => searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                        A<IEnumerable<AdminEntity>>._,
                        A<SearchSortFilterAndPaginateOptions>._
                    )
                ).MustHaveHappened();

                result.Should().BeViewResult().WithDefaultViewName();
            }
        }
        [Test]
        public void EditCentre_calls_expected_methods_and_returns_view()
        {
            // Given
            int adminId = 1;
            var loggedInAdmin = UserTestHelper.GetDefaultAdminEntity();

            A.CallTo(() => userService.GetAdminById(loggedInAdmin.AdminAccount.Id)).Returns(loggedInAdmin);

            // When
            var result = administratorsController.EditCentre(adminId);

            //Then
            using (new AssertionScope())
            {
                A.CallTo(() => userService.GetAdminUserById(adminId)).MustHaveHappened();
                A.CallTo(() => centresService.GetAllCentres(true)).MustHaveHappened();
                result.Should().BeViewResult().WithDefaultViewName();
            }
        }
        [Test]
        public void Export_passes_in_used_parameters_to_file()
        {
            // Given
            const string searchString = "SearchQuery|-AdminID|0";
            const string filters = "UserStatus|Any-Role|Any-CentreID|0";

            // When
            administratorsController.Export(searchString, filters);

            // Then
            A.CallTo(
                () => adminDownloadFileService.GetAllAdminsFile(searchString, filters)
            ).MustHaveHappenedOnceExactly();
        }
    }
}
