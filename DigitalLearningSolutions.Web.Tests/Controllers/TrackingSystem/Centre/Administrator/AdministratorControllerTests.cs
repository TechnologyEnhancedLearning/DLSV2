namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Administrator;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public class AdministratorControllerTests
    {
        private readonly List<AdminUser> adminUsers = new List<AdminUser>
        {
            UserTestHelper.GetDefaultAdminUser(firstName: "a", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "b", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "c", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "d", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "e", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "f", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "g", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "h", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "i", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "j", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "k", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "l", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "m", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "n", lastName: "Surname"),
            UserTestHelper.GetDefaultAdminUser(firstName: "o", lastName: "Surname")
        };

        private readonly List<Category> categories = new List<Category>
        {
            new Category { CategoryName = "All" },
            new Category { CategoryName = "Office" }
        };

        private AdministratorController administratorController = null!;
        private ICourseCategoriesDataService courseCategoriesDataService = null!;

        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void Setup()
        {
            courseCategoriesDataService = A.Fake<ICourseCategoriesDataService>();
            userDataService = A.Fake<IUserDataService>();

            A.CallTo(() => userDataService.GetAdminUsersByCentreId(A<int>._)).Returns(adminUsers);
            A.CallTo(() => courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(A<int>._))
                .Returns(categories);

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            const string cookieName = "AdminFilter";
            const string cookieValue = "Role|IsCentreAdmin|true";

            administratorController = new AdministratorController(userDataService, courseCategoriesDataService)
                .WithMockHttpContext(httpRequest, cookieName, cookieValue, httpResponse)
                .WithMockUser(true)
                .WithMockServices()
                .WithMockTempData();
        }

        [Test]
        public void Index_with_no_query_parameters_uses_cookie_value_for_filterBy()
        {
            // When
            var result = administratorController.Index();

            // Then
            result.As<ViewResult>().Model.As<CentreAdministratorsViewModel>().FilterBy.Should()
                .Be("Role|IsCentreAdmin|true");
        }

        [Test]
        public void Index_with_query_parameters_uses_query_parameter_value_for_filterBy()
        {
            // Given
            const string filterBy = "Role|IsCmsManager|true";
            A.CallTo(() => httpRequest.Query.ContainsKey("filterBy")).Returns(true);

            // When
            var result = administratorController.Index(filterBy: filterBy);

            // Then
            result.As<ViewResult>().Model.As<CentreAdministratorsViewModel>().FilterBy.Should()
                .Be(filterBy);
        }

        [Test]
        public void Index_with_CLEAR_filterBy_query_parameter_removes_cookie()
        {
            // Given
            const string? filterBy = "CLEAR";

            // When
            var result = administratorController.Index(filterBy: filterBy);

            // Then
            A.CallTo(() => httpResponse.Cookies.Delete("AdminFilter")).MustHaveHappened();
            result.As<ViewResult>().Model.As<CentreAdministratorsViewModel>().FilterBy.Should()
                .BeNull();
        }

        [Test]
        public void Index_with_null_filterBy_and_new_filter_query_parameter_add_new_cookie_value()
        {
            // Given
            const string? filterBy = null;
            const string? newFilterValue = "Role|IsCmsManager|true";

            // When
            var result = administratorController.Index(filterBy: filterBy, filterValue: newFilterValue);

            // Then
            A.CallTo(() => httpResponse.Cookies.Append("AdminFilter", newFilterValue, A<CookieOptions>._))
                .MustHaveHappened();
            result.As<ViewResult>().Model.As<CentreAdministratorsViewModel>().FilterBy.Should()
                .Be(newFilterValue);
        }

        [Test]
        public void Index_with_CLEAR_filterBy_and_new_filter_query_parameter_sets_new_cookie_value()
        {
            // Given
            const string? filterBy = "CLEAR";
            const string? newFilterValue = "Role|IsCmsManager|true";

            // When
            var result = administratorController.Index(filterBy: filterBy, filterValue: newFilterValue);

            // Then
            A.CallTo(() => httpResponse.Cookies.Append("AdminFilter", newFilterValue, A<CookieOptions>._))
                .MustHaveHappened();
            result.As<ViewResult>().Model.As<CentreAdministratorsViewModel>().FilterBy.Should()
                .Be(newFilterValue);
        }

        [Test]
        public void UnlockAccount_returns_not_found_if_admin_to_unlock_does_not_exist()
        {
            // Given
            A.CallTo(() => userDataService.GetAdminUserById(A<int>._)).Returns(null);

            // When
            var result = administratorController.UnlockAccount(1);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void UnlockAccount_returns_not_found_if_admin_to_unlock_is_at_different_centre()
        {
            // Given
            A.CallTo(() => userDataService.GetAdminUserById(A<int>._))
                .Returns(UserTestHelper.GetDefaultAdminUser(centreId: 3));

            // When
            var result = administratorController.UnlockAccount(1);

            // Then
            result.Should().BeNotFoundResult();
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
            A.CallTo(() => userDataService.UpdateAdminUserFailedLoginCount(1, 0)).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void DeactivateAdminUser_does_not_deactivate_admin_user_without_confirmation()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(active: true);
            A.CallTo(() => userDataService.GetAdminUserById(A<int>._)).Returns(UserTestHelper.GetDefaultAdminUser());
            var deactivateViewModel = new DeactivateAdminViewModel(adminUser) { Confirm = false };
            const string expectedErrorMessage = "You must confirm before deactivating this account.";

            // When
            var result = administratorController.DeactivateAdmin(1, deactivateViewModel);

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<DeactivateAdminViewModel>();
            administratorController.ModelState[nameof(DeactivateAdminViewModel.Confirm)].Errors[0].ErrorMessage.Should()
                .BeEquivalentTo(expectedErrorMessage);
            A.CallTo(() => userDataService.DeactivateAdmin(A<int>._)).MustNotHaveHappened();
        }

        [Test]
        public void DeactivateAdminUser_deactivate_admin_user_with_confirmation()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(active: true);
            A.CallTo(() => userDataService.GetAdminUserById(1)).Returns(UserTestHelper.GetDefaultAdminUser());
            var deactivateViewModel = new DeactivateAdminViewModel(adminUser) { Confirm = true };

            // When
            var result = administratorController.DeactivateAdmin(1, deactivateViewModel);

            // Then
            A.CallTo(() => userDataService.GetAdminUserById(1)).MustHaveHappened();
            result.Should().BeViewResult().WithViewName("DeactivateAdminConfirmation");
        }
    }
}
