namespace DigitalLearningSolutions.Web.Tests.ServiceFilter
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Models;
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
    using Newtonsoft.Json;
    using NUnit.Framework;

    public class RedirectEmptySessionDataTests
    {
        [Test]
        public void Redirects_to_index_if_no_temp_data_matching_model()
        {
            // Given
            var context = new ActionExecutingContext(
                new ActionContext(
                    new DefaultHttpContext(),
                    new RouteData(new RouteValueDictionary()),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new HomeController(A.Fake<IConfiguration>(), A.Fake<IBrandsService>()).WithDefaultContext().WithMockTempData()
            );

            // When
            new RedirectEmptySessionData<ResetPasswordData>().OnActionExecuting(context);

            // Then
            context.Result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void Does_not_set_action_result_if_there_is_temp_data_matching_model()
        {
            // Given
            var homeController = new HomeController(A.Fake<IConfiguration>(), A.Fake<IBrandsService>()).WithDefaultContext().WithMockTempData();
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
            homeController.TempData["ResetPasswordData"] =
                JsonConvert.SerializeObject(new ResetPasswordData("email", "hash"));

            // When
            new RedirectEmptySessionData<ResetPasswordData>().OnActionExecuting(context);

            // Then
            context.Result.Should().BeNull();
        }
    }
}
