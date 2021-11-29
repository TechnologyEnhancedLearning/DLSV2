namespace DigitalLearningSolutions.Web.Tests.ServiceFilter
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.ServiceFilter;
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

    public class VerifyAdminUserCanAccessCourseTests
    {
        private readonly ICourseService courseService = A.Fake<ICourseService>();
        private ActionExecutingContext context = null!;
        private const int CustomisationId = 2;

        [SetUp]
        public void Setup()
        {
            var homeController = new HomeController(A.Fake<IConfiguration>()).WithDefaultContext().WithMockTempData()
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
            context.RouteData.Values["customisationId"] = CustomisationId;
            A.CallTo(() => courseService.VerifyAdminUserCanAccessCourse(CustomisationId, A<int>._, A<int?>._))
                .Returns(null);

            // When
            new VerifyAdminUserCanAccessCourse(courseService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNotFoundResult();
        }

        [Test]
        public void Returns_AccessDenied_if_service_returns_false()
        {
            // Given
            context.RouteData.Values["customisationId"] = CustomisationId;
            A.CallTo(() => courseService.VerifyAdminUserCanAccessCourse(CustomisationId, A<int>._, A<int?>._))
                .Returns(false);

            // When
            new VerifyAdminUserCanAccessCourse(courseService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void Does_not_return_NotFound_Or_AccessDenied_if_service_returns_true()
        {
            // Given
            context.RouteData.Values["customisationId"] = CustomisationId;
            A.CallTo(() => courseService.VerifyAdminUserCanAccessCourse(CustomisationId, A<int>._, A<int?>._))
                .Returns(true);

            // When
            new VerifyAdminUserCanAccessCourse(courseService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNull();
        }
    }
}
