namespace DigitalLearningSolutions.Web.Tests.Attributes
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class ValidateAllowedDlsSubApplicationAttributeTests
    {
        [Test]
        public void
            ValidateAllowedDlsSubApplicationAttribute_sets_not_found_result_if_accessing_wrong_application()
        {
            // Given
            var context = new ActionExecutingContext(
                new ActionContext(
                    new DefaultHttpContext().WithMockUser(true, 101),
                    new RouteData(new RouteValueDictionary()),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new HomeController(A.Fake<IConfiguration>()).WithDefaultContext()
                    .WithMockUser(true, 101)
            );
            context.ActionArguments.Add("dlsSubApplication", DlsSubApplication.TrackingSystem);

            var attribute =
                new ValidateAllowedDlsSubApplicationAttribute(new[] { nameof(DlsSubApplication.LearningPortal) });

            // When
            attribute.OnActionExecuting(context);

            // Then
            context.Result.Should().BeNotFoundResult();
        }

        [Test]
        public void
            ValidateAllowedDlsSubApplicationAttribute_does_not_set_not_found_result_for_matching_application()
        {
            // Given
            var context = new ActionExecutingContext(
                new ActionContext(
                    new DefaultHttpContext().WithMockUser(true, 101),
                    new RouteData(new RouteValueDictionary()),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new HomeController(A.Fake<IConfiguration>()).WithDefaultContext()
                    .WithMockUser(true, 101)
            );
            context.ActionArguments.Add("dlsSubApplication", DlsSubApplication.LearningPortal);

            var attribute =
                new ValidateAllowedDlsSubApplicationAttribute(new[] { nameof(DlsSubApplication.LearningPortal) });

            // When
            attribute.OnActionExecuting(context);

            // Then
            context.Result.Should().BeNull();
        }

        [Test]
        public void
            ValidateAllowedDlsSubApplicationAttribute_does_not_set_not_found_result_if_no_application_list_set()
        {
            // Given
            var context = new ActionExecutingContext(
                new ActionContext(
                    new DefaultHttpContext().WithMockUser(true, 101),
                    new RouteData(new RouteValueDictionary()),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new HomeController(A.Fake<IConfiguration>()).WithDefaultContext()
                    .WithMockUser(true, 101)
            );
            context.ActionArguments.Add("dlsSubApplication", DlsSubApplication.LearningPortal);

            var attribute =
                new ValidateAllowedDlsSubApplicationAttribute(new string[] { });

            // When
            attribute.OnActionExecuting(context);

            // Then
            context.Result.Should().BeNull();
        }
    }
}
