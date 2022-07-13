namespace DigitalLearningSolutions.Web.Tests.ServiceFilter
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    internal class VerifyAdminUserCanAccessAdminUserTests
    {
        private readonly IUserDataService userDataService = A.Fake<IUserDataService>();
        private ActionExecutingContext context = null!;
        private const int AdminId = 2;

        [SetUp]
        public void Setup()
        {
            var homeController = new HomeController(A.Fake<IConfiguration>(), A.Fake<IBrandsService>()).WithDefaultContext().WithMockTempData()
                .WithMockUser(true, 101);
            context = new ActionExecutingContext(
                new ActionContext(
                    new DefaultHttpContext(),
                    new RouteData(new RouteValueDictionary()),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                homeController
            );
        }

        [Test]
        public void Returns_NotFound_if_service_returns_null()
        {
            // Given
            context.RouteData.Values["adminId"] = AdminId;
            A.CallTo(() => userDataService.GetAdminUserById(AdminId))
                .Returns(null);

            // When
            new VerifyAdminUserCanAccessAdminUser(userDataService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNotFoundResult();
        }

        [Test]
        public void Returns_AccessDenied_if_service_returns_admin_user_at_different_centre()
        {
            // Given
            context.RouteData.Values["adminId"] = AdminId;
            A.CallTo(() => userDataService.GetAdminUserById(AdminId))
                .Returns(UserTestHelper.GetDefaultAdminUser(centreId: 100));

            // When
            new VerifyAdminUserCanAccessAdminUser(userDataService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void Does_not_return_any_redirect_page_if_service_returns_admin_user_at_same_centre()
        {
            // Given
            context.RouteData.Values["adminId"] = AdminId;
            A.CallTo(() => userDataService.GetAdminUserById(AdminId))
                .Returns(UserTestHelper.GetDefaultAdminUser(centreId: 101));

            // When
            new VerifyAdminUserCanAccessAdminUser(userDataService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNull();
        }
    }
}
