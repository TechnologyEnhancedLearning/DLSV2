namespace DigitalLearningSolutions.Web.Tests.ServiceFilter
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Controllers.LearningPortalController;
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
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    internal class VerifyLearningLogItemExistsTests
    {
        private readonly ILearningLogItemsService learningLogItemsService = A.Fake<ILearningLogItemsService>();
        private readonly ILogger<LearningPortalController> logger = A.Fake<ILogger<LearningPortalController>>();
        private ActionExecutingContext context = null!;

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
            const int learningLogItemId = 4;
            context.RouteData.Values["learningLogItemId"] = learningLogItemId;
            A.CallTo(() => learningLogItemsService.SelectLearningLogItemById(learningLogItemId))
                .Returns(null);

            // When
            new VerifyLearningLogItemExists(learningLogItemsService, logger).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNotFoundResult();
        }

        [Test]
        public void Does_not_return_NotFound_if_service_returns_existing_item()
        {
            // Given
            const int learningLogItemId = 1;
            context.RouteData.Values["learningLogItemId"] = learningLogItemId;
            A.CallTo(() => learningLogItemsService.SelectLearningLogItemById(learningLogItemId))
                .Returns(new LearningLogItem());

            // When
            new VerifyLearningLogItemExists(learningLogItemsService, logger).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNull();
        }
    }
}
