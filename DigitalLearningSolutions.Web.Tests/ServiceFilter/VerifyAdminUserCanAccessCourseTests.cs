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

        [Test]
        public void Returns_NotFound_if_service_returns_false()
        {
            // Given
            var context = SetupContext();
            context.RouteData.Values["customisationId"] = 2;
            A.CallTo(() => courseService.VerifyAdminUserCanAccessCourse(A<int>._, A<int>._, A<int>._))
                .Returns(false);

            // When
            new VerifyAdminUserCanAccessCourse(courseService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNotFoundResult();
        }

        [Test]
        public void Does_not_return_NotFound_if_service_returns_true()
        {
            // Given
            var context = SetupContext();
            context.RouteData.Values["customisationId"] = 24286;
            A.CallTo(() => courseService.VerifyAdminUserCanAccessCourse(A<int>._, A<int>._, A<int>._))
                .Returns(true);

            // When
            new VerifyAdminUserCanAccessCourse(courseService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNull();
        }

        private ActionExecutingContext SetupContext()
        {
            var homeController = new HomeController(A.Fake<IConfiguration>()).WithDefaultContext().WithMockTempData()
                .WithMockUser(true, 101);
            var context = new ActionExecutingContext(
                new ActionContext(
                    new DefaultHttpContext(),
                    new RouteData(new RouteValueDictionary()),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                homeController
            );
            return context;
        }
    }
}
