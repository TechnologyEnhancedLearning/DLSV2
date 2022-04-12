namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Administrator;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    public class AdministratorControllerTests
    {
        private AdministratorController administratorController = null!;
        private ICentreContractAdminUsageService centreContractAdminUsageService = null!;
        private ICourseCategoriesDataService courseCategoriesDataService = null!;
        const string CookieName = "AdminFilter";
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;
        private ISearchSortFilterPaginateService searchSortFilterPaginateService = null!;
        private IUserDataService userDataService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void Setup()
        {
            courseCategoriesDataService = A.Fake<ICourseCategoriesDataService>();
            userDataService = A.Fake<IUserDataService>();
            centreContractAdminUsageService = A.Fake<ICentreContractAdminUsageService>();
            userService = A.Fake<IUserService>();
            searchSortFilterPaginateService = A.Fake<ISearchSortFilterPaginateService>();

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            const string cookieValue = "Role|IsCentreAdmin|true";

            administratorController = new AdministratorController(
                    userDataService,
                    courseCategoriesDataService,
                    centreContractAdminUsageService,
                    userService,
                    searchSortFilterPaginateService
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
            var result = administratorController.Index();

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => userDataService.GetAdminUsersByCentreId(A<int>._)).MustHaveHappened();
                A.CallTo(() => courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(A<int>._))
                    .MustHaveHappened();
                A.CallTo(() => userDataService.GetAdminUserById(A<int>._)).MustHaveHappened();
                A.CallTo(
                    () => searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                        A<IEnumerable<AdminUser>>._,
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

        [Test]
        public void UnlockAccount_unlocks_account_and_returns_to_page()
        {
            // Given
            A.CallTo(() => userDataService.GetAdminUserById(A<int>._)).Returns(UserTestHelper.GetDefaultAdminUser());
            A.CallTo(() => userDataService.UpdateAdminUserFailedLoginCount(A<int>._, A<int>._)).DoesNothing();

            // When
            var result = administratorController.UnlockAccount(1);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => userDataService.UpdateAdminUserFailedLoginCount(1, 0)).MustHaveHappened();
                result.Should().BeRedirectToActionResult().WithActionName("Index");
            }
        }

        [Test]
        public void DeactivateOrDeleteAdmin_returns_not_found_when_trying_to_access_page_for_own_admin_account()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            A.CallTo(() => userDataService.GetAdminUserById(adminUser.Id)).Returns(adminUser);

            // When
            var result = administratorController.DeactivateOrDeleteAdmin(adminUser.Id, ReturnPageQueryHelper.GetDefaultReturnPageQuery());

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void DeactivateOrDeleteAdmin_does_not_deactivate_admin_user_without_confirmation()
        {
            // Given
            const string expectedErrorMessage = "You must confirm before deactivating this account";
            var adminUser = UserTestHelper.GetDefaultAdminUser(8);
            var loggedInAdminUser = UserTestHelper.GetDefaultAdminUser();

            A.CallTo(() => userDataService.GetAdminUserById(adminUser.Id)).Returns(adminUser);
            A.CallTo(() => userDataService.GetAdminUserById(loggedInAdminUser.Id)).Returns(loggedInAdminUser);

            var deactivateViewModel =
                Builder<DeactivateAdminViewModel>.CreateNew().With(vm => vm.Confirm = false).Build();
            administratorController.ModelState.AddModelError(
                nameof(DeactivateAdminViewModel.Confirm),
                expectedErrorMessage
            );

            // When
            var result = administratorController.DeactivateOrDeleteAdmin(adminUser.Id, deactivateViewModel);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeViewResult().WithDefaultViewName().ModelAs<DeactivateAdminViewModel>();
                administratorController.ModelState[nameof(DeactivateAdminViewModel.Confirm)].Errors[0].ErrorMessage
                    .Should()
                    .BeEquivalentTo(expectedErrorMessage);
                A.CallTo(() => userDataService.DeactivateAdmin(adminUser.Id)).MustNotHaveHappened();
            }
        }

        [Test]
        public void DeactivateOrDeleteAdmin_deactivates_admin_user_with_confirmation()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(8);
            var loggedInAdminUser = UserTestHelper.GetDefaultAdminUser();

            A.CallTo(() => userDataService.GetAdminUserById(adminUser.Id)).Returns(adminUser);
            A.CallTo(() => userDataService.GetAdminUserById(loggedInAdminUser.Id)).Returns(loggedInAdminUser);

            A.CallTo(() => userService.DeactivateOrDeleteAdmin(adminUser.Id)).DoesNothing();
            var deactivateViewModel =
                Builder<DeactivateAdminViewModel>.CreateNew().With(vm => vm.Confirm = true).Build();

            // When
            var result = administratorController.DeactivateOrDeleteAdmin(adminUser.Id, deactivateViewModel);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => userService.DeactivateOrDeleteAdmin(adminUser.Id)).MustHaveHappened();
                result.Should().BeViewResult().WithViewName("DeactivateOrDeleteAdminConfirmation");
            }
        }

        [Test]
        public void DeactivateOrDeleteAdmin_submit_returns_not_found_when_trying_to_deactivate_own_admin_account()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            A.CallTo(() => userDataService.GetAdminUserById(adminUser.Id)).Returns(adminUser);
            A.CallTo(() => userDataService.DeactivateAdmin(adminUser.Id)).DoesNothing();
            var deactivateViewModel =
                Builder<DeactivateAdminViewModel>.CreateNew().With(vm => vm.Confirm = true).Build();

            // When
            var result = administratorController.DeactivateOrDeleteAdmin(adminUser.Id, deactivateViewModel);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => userDataService.DeactivateAdmin(adminUser.Id)).MustNotHaveHappened();
                result.Should().BeNotFoundResult();
            }
        }
    }
}
