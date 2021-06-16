namespace DigitalLearningSolutions.Web.Tests.ServiceFilter
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.CentreConfiguration;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration;
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
                new HomeController(A.Fake<IConfiguration>()).WithDefaultContext().WithMockTempData()
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
            var homeController = new HomeController(A.Fake<IConfiguration>()).WithDefaultContext().WithMockTempData();
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

        [Test]
        public void RedirectEmptySessionDataXor_redirects_to_index_if_no_temp_data_matching_either_model()
        {
            // Given
            var controller =
                new RegistrationPromptsController(A.Fake<ICustomPromptsService>(), A.Fake<IUserDataService>())
                    .WithDefaultContext().WithMockTempData();
            var context = new ActionExecutingContext(
                new ActionContext(
                    new DefaultHttpContext(),
                    new RouteData(new RouteValueDictionary()),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                controller
            );

            // When
            new RedirectEmptySessionDataXor<AddRegistrationPromptData, EditRegistrationPromptData>().OnActionExecuting(
                context
            );

            // Then
            context.Result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void
            RedirectEmptySessionDataXor_does_not_set_action_result_if_there_is_temp_data_matching_the_first_model()
        {
            // Given
            var controller =
                new RegistrationPromptsController(A.Fake<ICustomPromptsService>(), A.Fake<IUserDataService>())
                    .WithDefaultContext().WithMockTempData();
            var context = new ActionExecutingContext(
                new ActionContext(
                    new DefaultHttpContext(),
                    new RouteData(new RouteValueDictionary()),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                controller
            );
            controller.TempData["AddRegistrationPromptData"] =
                JsonConvert.SerializeObject(new AddRegistrationPromptData());

            // When
            new RedirectEmptySessionDataXor<AddRegistrationPromptData, EditRegistrationPromptData>().OnActionExecuting(
                context
            );

            // Then
            context.Result.Should().BeNull();
        }

        [Test]
        public void
            RedirectEmptySessionDataXor_does_not_set_action_result_if_there_is_temp_data_matching_the_second_model()
        {
            // Given
            var controller =
                new RegistrationPromptsController(A.Fake<ICustomPromptsService>(), A.Fake<IUserDataService>())
                    .WithDefaultContext().WithMockTempData();
            var context = new ActionExecutingContext(
                new ActionContext(
                    new DefaultHttpContext(),
                    new RouteData(new RouteValueDictionary()),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                controller
            );
            controller.TempData["EditRegistrationPromptData"] =
                JsonConvert.SerializeObject(new EditRegistrationPromptData(new EditRegistrationPromptViewModel()));

            // When
            new RedirectEmptySessionDataXor<AddRegistrationPromptData, EditRegistrationPromptData>().OnActionExecuting(
                context
            );

            // Then
            context.Result.Should().BeNull();
        }

        [Test]
        public void RedirectEmptySessionDataXor_redirects_to_index_if_there_is_temp_data_matching_both_models()
        {
            // Given
            var controller =
                new RegistrationPromptsController(A.Fake<ICustomPromptsService>(), A.Fake<IUserDataService>())
                    .WithDefaultContext().WithMockTempData();
            var context = new ActionExecutingContext(
                new ActionContext(
                    new DefaultHttpContext(),
                    new RouteData(new RouteValueDictionary()),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                controller
            );
            controller.TempData["AddRegistrationPromptData"] =
                JsonConvert.SerializeObject(new AddRegistrationPromptData());
            controller.TempData["EditRegistrationPromptData"] =
                JsonConvert.SerializeObject(new EditRegistrationPromptData(new EditRegistrationPromptViewModel()));

            // When
            new RedirectEmptySessionDataXor<AddRegistrationPromptData, EditRegistrationPromptData>().OnActionExecuting(
                context
            );

            // Then
            context.Result.Should().BeRedirectToActionResult().WithActionName("Index");
        }
    }
}
