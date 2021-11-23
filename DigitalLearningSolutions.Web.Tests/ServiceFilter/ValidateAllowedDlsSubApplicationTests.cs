namespace DigitalLearningSolutions.Web.Tests.ServiceFilter
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
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
    using Microsoft.FeatureManagement;
    using NUnit.Framework;

    public class ValidateAllowedDlsSubApplicationTests
    {
        private IFeatureManager featureManager = null!;

        [SetUp]
        public void SetUp()
        {
            featureManager = A.Fake<IFeatureManager>();
            A.CallTo(() => featureManager.IsEnabledAsync(FeatureFlags.RefactoredTrackingSystem))
                .Returns(true);
            A.CallTo(() => featureManager.IsEnabledAsync(FeatureFlags.RefactoredSuperAdminInterface))
                .Returns(true);
        }

        [Test]
        public void
            ValidateAllowedDlsSubApplication_sets_not_found_result_if_accessing_wrong_application()
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
                new ValidateAllowedDlsSubApplication(
                    new[] { nameof(DlsSubApplication.LearningPortal) },
                    featureManager
                );

            // When
            attribute.OnActionExecuting(context);

            // Then
            context.Result.Should().BeNotFoundResult();
        }

        [Test]
        public void
            ValidateAllowedDlsSubApplication_does_not_set_not_found_result_for_matching_application()
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
                new ValidateAllowedDlsSubApplication(
                    new[] { nameof(DlsSubApplication.LearningPortal) },
                    featureManager
                );

            // When
            attribute.OnActionExecuting(context);

            // Then
            context.Result.Should().BeNull();
        }

        [Test]
        public void
            ValidateAllowedDlsSubApplication_does_not_set_not_found_result_if_no_application_list_set()
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
                new ValidateAllowedDlsSubApplication(new string[] { }, featureManager);

            // When
            attribute.OnActionExecuting(context);

            // Then
            context.Result.Should().BeNull();
        }

        [Test]
        [TestCase(nameof(DlsSubApplication.TrackingSystem), FeatureFlags.RefactoredTrackingSystem)]
        [TestCase(nameof(DlsSubApplication.SuperAdmin), FeatureFlags.RefactoredSuperAdminInterface)]
        public void
            ValidateAllowedDlsSubApplication_sets_not_found_result_if_relevant_feature_flag_off(
                string application,
                string featureFlag
            )
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
            context.ActionArguments.Add("dlsSubApplication", Enumeration.FromName<DlsSubApplication>(application));
            A.CallTo(() => featureManager.IsEnabledAsync(featureFlag))
                .Returns(false);

            var attribute =
                new ValidateAllowedDlsSubApplication(new string[] { }, featureManager);

            // When
            attribute.OnActionExecuting(context);

            // Then
            context.Result.Should().BeNotFoundResult();
        }
    }
}
