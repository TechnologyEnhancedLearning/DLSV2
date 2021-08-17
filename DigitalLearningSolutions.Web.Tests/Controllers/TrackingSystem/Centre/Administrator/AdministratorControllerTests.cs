namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Administrator;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator;
    using FakeItEasy;
    using FluentAssertions;
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
        private IRequestCookieCollection cookieCollection = null!;
        private HttpContext httpContext = null!;
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void Setup()
        {
            courseCategoriesDataService = A.Fake<ICourseCategoriesDataService>();
            userDataService = A.Fake<IUserDataService>();

            A.CallTo(() => userDataService.GetAdminUsersByCentreId(A<int>._)).Returns(adminUsers);
            A.CallTo(() => courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(A<int>._)).Returns(categories);

            httpContext = A.Fake<HttpContext>();
            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            cookieCollection = A.Fake<IRequestCookieCollection>();

            const string cookieName = "AdminFilter";
            const string cookieValue = "Role|IsCentreAdmin|true";
            var cookieList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(cookieName, cookieValue)
            };
            A.CallTo(() => cookieCollection[cookieName]).Returns(cookieValue);
            A.CallTo(() => cookieCollection.GetEnumerator()).Returns(cookieList.GetEnumerator());
            A.CallTo(() => cookieCollection.ContainsKey(cookieName)).Returns(true);
            A.CallTo(() => httpRequest.Cookies).Returns(cookieCollection);
            A.CallTo(() => httpContext.Request).Returns(httpRequest);
            A.CallTo(() => httpContext.Response).Returns(httpResponse);

            administratorController = new AdministratorController(userDataService, courseCategoriesDataService)
                .WithMockHttpContext(httpContext)
                .WithMockUser(true)
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
        public void Index_with_null_filterBy_query_parameter_removes_cookie()
        {
            // Given
            const string? filterBy = null;
            A.CallTo(() => httpRequest.Query.ContainsKey("filterBy")).Returns(true);

            // When
            var result = administratorController.Index(filterBy: filterBy);

            // Then
            A.CallTo(() => httpResponse.Cookies.Delete("AdminFilter")).MustHaveHappened();
            result.As<ViewResult>().Model.As<CentreAdministratorsViewModel>().FilterBy.Should()
                .Be(filterBy);
        }

        [Test]
        public void Index_with_null_filterBy_and_new_filter_query_parameter_add_new_cookie_value()
        {
            // Given
            const string? filterBy = null;
            const string? newFilterValue = "Role|IsCmsManager|true";
            A.CallTo(() => httpRequest.Query.ContainsKey("filterBy")).Returns(true);

            // When
            var result = administratorController.Index(filterBy: filterBy, filterValue: newFilterValue);

            // Then
            A.CallTo(() => httpResponse.Cookies.Append("AdminFilter", newFilterValue, A<CookieOptions>._))
                .MustHaveHappened();
            result.As<ViewResult>().Model.As<CentreAdministratorsViewModel>().FilterBy.Should()
                .Be(newFilterValue);
        }
    }
}
