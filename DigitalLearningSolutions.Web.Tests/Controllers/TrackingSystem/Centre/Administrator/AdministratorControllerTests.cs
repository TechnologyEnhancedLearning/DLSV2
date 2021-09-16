namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Administrator;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
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
        private INumberOfAdministratorsHelper numberOfAdministratorsHelper = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void Setup()
        {
            courseCategoriesDataService = A.Fake<ICourseCategoriesDataService>();
            userDataService = A.Fake<IUserDataService>();
            numberOfAdministratorsHelper = A.Fake<INumberOfAdministratorsHelper>();

            A.CallTo(() => userDataService.GetAdminUsersByCentreId(A<int>._)).Returns(adminUsers);
            A.CallTo(() => courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(A<int>._))
                .Returns(categories);

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            const string cookieName = "AdminFilter";
            const string cookieValue = "Role|IsCentreAdmin|true";

            administratorController = new AdministratorController(
                    userDataService,
                    courseCategoriesDataService,
                    numberOfAdministratorsHelper
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
        public void EditAdminRoles_edits_roles_when_spaces_available()
        {
            // Given
            var currentAdminUser = UserTestHelper.GetDefaultAdminUser(
                isContentCreator: false,
                isTrainer: false,
                importOnly: false,
                isContentManager: false
            );
            var newRoles = EditAdminRolesTestHelper.GetDefaultEditRolesViewModel(
                1,
                true,
                true,
                true,
                true,
                ContentManagementRole.CmsAdministrator
            );
            var numberOfAdmins = NumberOfAdministratorsTestHelper.GetDefaultNumberOfAdministrators();
            A.CallTo(() => userDataService.GetAdminUserById(A<int>._)).Returns(currentAdminUser);
            A.CallTo(() => numberOfAdministratorsHelper.GetCentreAdministratorNumbers(A<int>._))
                .Returns(numberOfAdmins);

            // When
            var result = administratorController.EditAdminRoles(newRoles, currentAdminUser.Id);

            // Then
            A.CallTo(
                () => userDataService.UpdateAdminUserPermissions(
                    currentAdminUser.Id,
                    newRoles.IsCentreAdmin,
                    newRoles.IsSupervisor,
                    newRoles.IsTrainer,
                    newRoles.IsContentCreator,
                    newRoles.ContentManagementRole.IsContentManager,
                    newRoles.ContentManagementRole.ImportOnly,
                    newRoles.LearningCategory
                )
            ).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        [TestCase(false, true, 1)]
        [TestCase(true, true, 2)]
        public void EditAdminRoles_edits_roles_when_spaces_unavailable_but_user_already_on_role(
            bool oldImportOnly,
            bool oldIsContentManager,
            int newContentManagementRoleId
        )
        {
            // Given
            var currentAdminUser = UserTestHelper.GetDefaultAdminUser(
                isContentCreator: true,
                isTrainer: true,
                importOnly: oldImportOnly,
                isContentManager: oldIsContentManager
            );
            var newRoles = EditAdminRolesTestHelper.GetDefaultEditRolesViewModel(
                1,
                true,
                true,
                true,
                true,
                Enumeration.FromId<ContentManagementRole>(newContentManagementRoleId)
            );
            var numberOfAdmins = NumberOfAdministratorsTestHelper.GetDefaultNumberOfAdministrators(
                trainerSpots: 3,
                trainers: 3,
                ccLicenceSpots: 4,
                ccLicences: 4,
                cmsAdministrators: 5,
                cmsAdministratorSpots: 5,
                cmsManagerSpots: 6,
                cmsManagers: 6
            );
            A.CallTo(() => userDataService.GetAdminUserById(A<int>._)).Returns(currentAdminUser);
            A.CallTo(() => numberOfAdministratorsHelper.GetCentreAdministratorNumbers(A<int>._))
                .Returns(numberOfAdmins);

            // When
            var result = administratorController.EditAdminRoles(newRoles, currentAdminUser.Id);

            // Then
            A.CallTo(
                () => userDataService.UpdateAdminUserPermissions(
                    currentAdminUser.Id,
                    newRoles.IsCentreAdmin,
                    newRoles.IsSupervisor,
                    newRoles.IsTrainer,
                    newRoles.IsContentCreator,
                    newRoles.ContentManagementRole.IsContentManager,
                    newRoles.ContentManagementRole.ImportOnly,
                    newRoles.LearningCategory
                )
            ).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        [TestCase(true, false, 0)]
        [TestCase(false, true, 0)]
        [TestCase(false, false, 1)]
        [TestCase(false, false, 2)]
        public void EditAdminRoles_returns_status_code_500_when_spaces_unavailable_and_user_not_on_role(
            bool newIsTrainer,
            bool newIsContentCreator,
            int newContentManagementRoleId
        )
        {
            // Given
            var currentAdminUser = UserTestHelper.GetDefaultAdminUser(
                isContentCreator: false,
                isTrainer: false,
                importOnly: false,
                isContentManager: false
            );
            var newRoles = EditAdminRolesTestHelper.GetDefaultEditRolesViewModel(
                1,
                true,
                true,
                newIsTrainer,
                newIsContentCreator,
                Enumeration.FromId<ContentManagementRole>(newContentManagementRoleId)
            );
            var numberOfAdmins = NumberOfAdministratorsTestHelper.GetDefaultNumberOfAdministrators(
                trainerSpots: 3,
                trainers: 3,
                ccLicenceSpots: 4,
                ccLicences: 4,
                cmsAdministrators: 5,
                cmsAdministratorSpots: 5,
                cmsManagerSpots: 6,
                cmsManagers: 6
            );
            A.CallTo(() => userDataService.GetAdminUserById(A<int>._)).Returns(currentAdminUser);
            A.CallTo(() => numberOfAdministratorsHelper.GetCentreAdministratorNumbers(A<int>._))
                .Returns(numberOfAdmins);

            // When
            var result = administratorController.EditAdminRoles(newRoles, currentAdminUser.Id);

            // Then
            A.CallTo(
                () => userDataService.UpdateAdminUserPermissions(
                    A<int>._,
                    A<bool>._,
                    A<bool>._,
                    A<bool>._,
                    A<bool>._,
                    A<bool>._,
                    A<bool>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
            result.Should().BeStatusCodeResult().WithStatusCode(500);
        }
    }
}
