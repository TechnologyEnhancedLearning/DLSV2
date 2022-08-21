namespace DigitalLearningSolutions.Web.Tests.ServiceFilter
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Models.Enums;
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

    public class VerifyDelegateProgressAccessedViaValidRouteTests
    {
        private ActionExecutingContext context = null!;

        [SetUp]
        public void Setup()
        {
            var homeController = new HomeController(A.Fake<IConfiguration>(), A.Fake<IBrandsService>()).WithDefaultContext().WithMockTempData()
                .WithMockUser(true, 101, adminCategoryId: 1);
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
        public void Returns_NotFound_if_route_is_not_from_valid_enumeration()
        {
            // Given
            context.RouteData.Values["accessedVia"] = "WrongRoute";

            // When
            new VerifyDelegateAccessedViaValidRoute().OnActionExecuting(context);

            // Then
            context.Result.Should().BeNotFoundResult();
        }

        [Test]
        public void Does_not_return_NotFound_if_route_is_from_ViewDelegate()
        {
            // Given
            context.RouteData.Values["accessedVia"] = DelegateAccessRoute.ViewDelegate.Name;

            // When
            new VerifyDelegateAccessedViaValidRoute().OnActionExecuting(context);

            // Then
            context.Result.Should().BeNull();
        }

        [Test]
        public void Does_not_return_NotFound_if_route_is_from_CourseDelegates()
        {
            // Given
            context.RouteData.Values["accessedVia"] = DelegateAccessRoute.CourseDelegates.Name;

            // When
            new VerifyDelegateAccessedViaValidRoute().OnActionExecuting(context);

            // Then
            context.Result.Should().BeNull();
        }
    }
}
