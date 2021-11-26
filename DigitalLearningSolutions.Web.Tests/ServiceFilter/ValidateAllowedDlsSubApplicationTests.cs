namespace DigitalLearningSolutions.Web.Tests.ServiceFilter
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.FeatureManagement;
    using NUnit.Framework;

    public class ValidateAllowedDlsSubApplicationTests
    {
        private ActionExecutingContext context = null!;
        private IFeatureManager featureManager = null!;

        [SetUp]
        public void SetUp()
        {
            featureManager = A.Fake<IFeatureManager>();
            A.CallTo(() => featureManager.IsEnabledAsync(FeatureFlags.RefactoredTrackingSystem))
                .Returns(true);
            A.CallTo(() => featureManager.IsEnabledAsync(FeatureFlags.RefactoredSuperAdminInterface))
                .Returns(true);

            SetUpContextWithActionParameter();
        }

        [Test]
        public void
            ValidateAllowedDlsSubApplication_sets_not_found_result_if_accessing_wrong_application()
        {
            // Given
            GivenUserIsTryingToAccess(DlsSubApplication.TrackingSystem);

            var attribute =
                new ValidateAllowedDlsSubApplication(
                    featureManager,
                    new[] { nameof(DlsSubApplication.LearningPortal) }
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
            GivenUserIsTryingToAccess(DlsSubApplication.LearningPortal);

            var attribute =
                new ValidateAllowedDlsSubApplication(
                    featureManager,
                    new[] { nameof(DlsSubApplication.LearningPortal) }
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
            GivenUserIsTryingToAccess(DlsSubApplication.LearningPortal);

            var attribute =
                new ValidateAllowedDlsSubApplication(featureManager);

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
            GivenUserIsTryingToAccess(Enumeration.FromName<DlsSubApplication>(application));

            A.CallTo(() => featureManager.IsEnabledAsync(featureFlag))
                .Returns(false);

            var attribute =
                new ValidateAllowedDlsSubApplication(featureManager);

            // When
            attribute.OnActionExecuting(context);

            // Then
            context.Result.Should().BeNotFoundResult();
        }

        private void SetUpContextWithActionParameter()
        {
            context = ContextHelper
                .GetDefaultActionExecutingContext(
                    new NotificationPreferencesController(A.Fake<INotificationPreferencesService>())
                )
                .WithMockUser(true, 101);
            context.ActionDescriptor.Parameters = new[]
            {
                new ParameterDescriptor
                {
                    BindingInfo = new BindingInfo(),
                    Name = "dlsSubApplication",
                    ParameterType = typeof(DlsSubApplication),
                },
            };
        }

        private void GivenUserIsTryingToAccess(DlsSubApplication application)
        {
            context.ActionArguments.Add("dlsSubApplication", application);
        }
    }
}
