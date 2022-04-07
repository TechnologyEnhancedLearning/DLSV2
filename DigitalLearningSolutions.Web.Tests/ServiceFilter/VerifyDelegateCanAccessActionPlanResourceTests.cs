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

    public class VerifyDelegateCanAccessActionPlanResourceTests
    {
        private const int LearningLogItemId = 1;
        private const int DelegateId = 2;
        private IActionPlanService actionPlanService = null!;
        private ActionExecutingContext context = null!;

        [SetUp]
        public void Setup()
        {
            actionPlanService = A.Fake<IActionPlanService>();
            var homeController = new HomeController(A.Fake<IConfiguration>(), A.Fake<IBrandsService>()).WithDefaultContext().WithMockTempData()
                .WithMockUser(true, 101, delegateId: DelegateId);
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
            context.RouteData.Values["learningLogItemId"] = LearningLogItemId;
            A.CallTo(() => actionPlanService.VerifyDelegateCanAccessActionPlanResource(LearningLogItemId, DelegateId))
                .Returns(null);

            // When
            new VerifyDelegateCanAccessActionPlanResource(actionPlanService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNotFoundResult();
        }

        [Test]
        public void Returns_AccessDenied_if_service_returns_false()
        {
            // Given
            context.RouteData.Values["learningLogItemId"] = LearningLogItemId;
            A.CallTo(() => actionPlanService.VerifyDelegateCanAccessActionPlanResource(LearningLogItemId, DelegateId))
                .Returns(false);

            // When
            new VerifyDelegateCanAccessActionPlanResource(actionPlanService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeRedirectToActionResult().WithControllerName("LearningSolutions")
                .WithActionName("AccessDenied");
        }

        [Test]
        public void Does_not_return_NotFound_Or_AccessDenied_if_service_returns_true()
        {
            // Given
            context.RouteData.Values["learningLogItemId"] = LearningLogItemId;
            A.CallTo(() => actionPlanService.VerifyDelegateCanAccessActionPlanResource(LearningLogItemId, DelegateId))
                .Returns(true);

            // When
            new VerifyDelegateCanAccessActionPlanResource(actionPlanService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNull();
        }
    }
}
