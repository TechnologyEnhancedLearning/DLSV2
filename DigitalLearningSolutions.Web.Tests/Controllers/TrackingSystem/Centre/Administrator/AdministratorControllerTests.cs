﻿namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Administrator;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
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
        private ICourseCategoriesService courseCategoriesService = null!;
        const string CookieName = "AdminFilter";
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;
        private ISearchSortFilterPaginateService searchSortFilterPaginateService = null!;
        private IUserService userService = null!;
        private IEmailService emailService = null!;
        private IEmailGenerationService emailGenerationService = null!;

        [SetUp]
        public void Setup()
        {
            courseCategoriesService = A.Fake<ICourseCategoriesService>();
            centreContractAdminUsageService = A.Fake<ICentreContractAdminUsageService>();
            userService = A.Fake<IUserService>();
            searchSortFilterPaginateService = A.Fake<ISearchSortFilterPaginateService>();
            emailService = A.Fake<IEmailService>();
            emailGenerationService = A.Fake<IEmailGenerationService>();

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            const string cookieValue = "Role|IsCentreAdmin|true";

            administratorController = new AdministratorController(
                    courseCategoriesService,
                    centreContractAdminUsageService,
                    userService,
                    searchSortFilterPaginateService,
                    emailService,
                    emailGenerationService
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
                A.CallTo(() => userService.GetAdminsByCentreId(A<int>._)).MustHaveHappened();
                A.CallTo(() => courseCategoriesService.GetCategoriesForCentreAndCentrallyManagedCourses(A<int>._))
                    .MustHaveHappened();
                A.CallTo(
                    () => searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                        A<IEnumerable<AdminEntity>>._,
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
            var adminAccount = UserTestHelper.GetDefaultAdminAccount();
            A.CallTo(() => userService.ResetFailedLoginCountByUserId(A<int>._)).DoesNothing();
            A.CallTo(() => userService.GetUserIdByAdminId(adminAccount.Id)).Returns(adminAccount.UserId);

            // When
            var result = administratorController.UnlockAccount(adminAccount.Id);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => userService.ResetFailedLoginCountByUserId(adminAccount.UserId)).MustHaveHappened();
                result.Should().BeRedirectToActionResult().WithActionName("Index");
            }
        }

        [Test]
        public void DeactivateOrDeleteAdmin_returns_not_found_when_trying_to_access_page_for_own_admin_account()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            A.CallTo(() => userService.GetAdminUserById(adminUser.Id)).Returns(adminUser);

            // When
            var result = administratorController.DeactivateOrDeleteAdmin(
                adminUser.Id,
                ReturnPageQueryHelper.GetDefaultReturnPageQuery()
            );

            // Then
            result.Should().BeStatusCodeResult().WithStatusCode(410);
        }

        [Test]
        public void DeactivateOrDeleteAdmin_does_not_deactivate_admin_user_without_confirmation()
        {
            // Given
            const string expectedErrorMessage = "You must confirm before deactivating this account";
            var admin = UserTestHelper.GetDefaultAdminEntity(8);
            var loggedInAdmin = UserTestHelper.GetDefaultAdminEntity();

            A.CallTo(() => userService.GetAdminById(admin.AdminAccount.Id)).Returns(admin);
            A.CallTo(() => userService.GetAdminById(loggedInAdmin.AdminAccount.Id)).Returns(loggedInAdmin);

            var deactivateViewModel =
                Builder<DeactivateAdminViewModel>.CreateNew().With(vm => vm.Confirm = false).Build();
            administratorController.ModelState.AddModelError(
                nameof(DeactivateAdminViewModel.Confirm),
                expectedErrorMessage
            );

            // When
            var result = administratorController.DeactivateOrDeleteAdmin(admin.AdminAccount.Id, deactivateViewModel);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeViewResult().WithDefaultViewName().ModelAs<DeactivateAdminViewModel>();
                administratorController.ModelState[nameof(DeactivateAdminViewModel.Confirm)]?.Errors[0].ErrorMessage
                    .Should()
                    .BeEquivalentTo(expectedErrorMessage);
                A.CallTo(() => userService.DeactivateAdmin(admin.AdminAccount.Id)).MustNotHaveHappened();
            }
        }

        [Test]
        public void DeactivateOrDeleteAdmin_deactivates_admin_user_with_confirmation()
        {
            // Given
            var admin = UserTestHelper.GetDefaultAdminEntity(8);
            var loggedInAdmin = UserTestHelper.GetDefaultAdminEntity();

            A.CallTo(() => userService.GetAdminById(admin.AdminAccount.Id)).Returns(admin);
            A.CallTo(() => userService.GetAdminById(loggedInAdmin.AdminAccount.Id)).Returns(loggedInAdmin);

            A.CallTo(() => userService.DeactivateOrDeleteAdmin(admin.AdminAccount.Id)).DoesNothing();
            var deactivateViewModel =
                Builder<DeactivateAdminViewModel>.CreateNew().With(vm => vm.Confirm = true).Build();

            // When
            var result = administratorController.DeactivateOrDeleteAdmin(admin.AdminAccount.Id, deactivateViewModel);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => userService.DeactivateOrDeleteAdmin(admin.AdminAccount.Id)).MustHaveHappened();
                result.Should().BeViewResult().WithViewName("DeactivateOrDeleteAdminConfirmation");
            }
        }

        [Test]
        public void DeactivateOrDeleteAdmin_submit_returns_not_found_when_trying_to_deactivate_own_admin_account()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            A.CallTo(() => userService.GetAdminUserById(adminUser.Id)).Returns(adminUser);
            A.CallTo(() => userService.DeactivateAdmin(adminUser.Id)).DoesNothing();
            var deactivateViewModel =
                Builder<DeactivateAdminViewModel>.CreateNew().With(vm => vm.Confirm = true).Build();

            // When
            var result = administratorController.DeactivateOrDeleteAdmin(adminUser.Id, deactivateViewModel);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => userService.DeactivateAdmin(adminUser.Id)).MustNotHaveHappened();
                result.Should().BeStatusCodeResult().WithStatusCode(410);
            }
        }
    }
}
