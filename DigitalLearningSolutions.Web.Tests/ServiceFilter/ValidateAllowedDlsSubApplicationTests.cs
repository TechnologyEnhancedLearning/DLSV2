namespace DigitalLearningSolutions.Web.Tests.ServiceFilter
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
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
        }

        [Test]
        public void
            ValidateAllowedDlsSubApplication_sets_login_result_when_user_is_unauthenticated()
        {
            // Given
            context = ContextHelper
                .GetDefaultActionExecutingContext(
                    new NotificationPreferencesController(A.Fake<INotificationPreferencesService>())
                )
                .WithMockUser(false, 101);
            context.ActionDescriptor.Parameters = new[]
            {
                new ParameterDescriptor
                {
                    BindingInfo = new BindingInfo(),
                    Name = "dlsSubApplication",
                    ParameterType = typeof(DlsSubApplication),
                },
            };
            GivenUserIsTryingToAccess(DlsSubApplication.TrackingSystem);

            var attribute =
                new ValidateAllowedDlsSubApplication(
                    featureManager,
                    new[] { nameof(DlsSubApplication.LearningPortal) }
                );

            // When
            attribute.OnActionExecuting(context);

            // Then
            context.Result.Should().BeRedirectToActionResult().WithControllerName("Login").WithActionName("Index");
        }

        [Test]
        public void
            ValidateAllowedDlsSubApplication_add_dlsSubApplication_action_parameters_when_user_is_is_redirected_to_learning_portal_and_no_others_exist()
        {
            // Given
            const string actionName = "EditDetails";
            const string controllerName = "MyAccount";
            var controller = new MyAccountController(
                A.Fake<ICentreRegistrationPromptsService>(),
                A.Fake<IUserService>(),
                A.Fake<IUserDataService>(),
                A.Fake<IImageResizeService>(),
                A.Fake<IJobGroupsDataService>(),
                A.Fake<IEmailVerificationService>(),
                A.Fake<PromptsService>(),
                A.Fake<ILogger<MyAccountController>>(),
                A.Fake<IConfiguration>()
            )
            {
                ControllerContext = new ControllerContext
                {
                    ActionDescriptor = new ControllerActionDescriptor
                    {
                        ActionName = actionName,
                        ControllerName = controllerName,
                    },
                },
            };

            context = ContextHelper
                .GetDefaultActionExecutingContext(
                    controller
                )
                .WithMockUser(true, 101, adminId: null);
            context.ActionDescriptor = new ActionDescriptor
            {
                Parameters = new[]
                {
                    new ParameterDescriptor
                    {
                        BindingInfo = new BindingInfo(),
                        Name = "dlsSubApplication",
                        ParameterType = typeof(DlsSubApplication),
                    },
                },
            };
            GivenUserIsTryingToAccess(DlsSubApplication.Main);

            var attribute =
                new ValidateAllowedDlsSubApplication(
                    featureManager,
                    new[] { nameof(DlsSubApplication.Main) }
                );

            // When
            attribute.OnActionExecuting(context);

            // Then
            context.Result.Should().BeRedirectToActionResult().WithControllerName(controllerName)
                .WithActionName(actionName)
                .WithRouteValue("dlsSubApplication", DlsSubApplication.LearningPortal);
        }

        [Test]
        public void
            ValidateAllowedDlsSubApplication_preserves_other_action_parameters_when_user_is_is_redirected_to_learning_portal()
        {
            // Given
            const string actionName = "EditDetails";
            const string controllerName = "MyAccount";
            var controller = new MyAccountController(
                A.Fake<ICentreRegistrationPromptsService>(),
                A.Fake<IUserService>(),
                A.Fake<IUserDataService>(),
                A.Fake<IImageResizeService>(),
                A.Fake<IJobGroupsDataService>(),
                A.Fake<IEmailVerificationService>(),
                A.Fake<PromptsService>(),
                A.Fake<ILogger<MyAccountController>>(),
                A.Fake<IConfiguration>()
            )
            {
                ControllerContext = new ControllerContext
                {
                    ActionDescriptor = new ControllerActionDescriptor
                    {
                        ActionName = actionName,
                        ControllerName = controllerName,
                    },
                },
            };

            context = ContextHelper
                .GetDefaultActionExecutingContext(
                    controller
                )
                .WithMockUser(true, 101, adminId: null);
            context.ActionDescriptor = new ActionDescriptor
            {
                Parameters = new[]
                {
                    new ParameterDescriptor
                    {
                        BindingInfo = new BindingInfo(),
                        Name = "dlsSubApplication",
                        ParameterType = typeof(DlsSubApplication),
                    },
                },
            };
            context.ActionArguments.Add("parameterToPreserve", true);
            GivenUserIsTryingToAccess(DlsSubApplication.Main);

            var attribute =
                new ValidateAllowedDlsSubApplication(
                    featureManager,
                    new[] { nameof(DlsSubApplication.Main) }
                );

            // When
            attribute.OnActionExecuting(context);

            // Then
            context.Result.Should().BeRedirectToActionResult().WithControllerName(controllerName)
                .WithActionName(actionName)
                .WithRouteValue("parameterToPreserve", true)
                .WithRouteValue("dlsSubApplication", DlsSubApplication.LearningPortal);
        }

        [Test]
        public void
            ValidateAllowedDlsSubApplication_sets_not_found_result_if_accessing_wrong_application()
        {
            // Given
            SetUpContextWithActionParameter();
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
            ValidateAllowedDlsSubApplication_does_not_set_result_if_no_application_parameter_found()
        {
            // Given
            SetUpContextWithActionParameter();
            GivenUserIsTryingToAccess(DlsSubApplication.TrackingSystem);
            context.ActionDescriptor.Parameters = new ParameterDescriptor[] { };

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
            ValidateAllowedDlsSubApplication_sets_not_found_result_if_application_is_null()
        {
            // Given
            SetUpContextWithActionParameter();
            GivenUserIsTryingToAccess(null);

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
            SetUpContextWithActionParameter();
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
            SetUpContextWithActionParameter();
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
            SetUpContextWithActionParameter();
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

        private void GivenUserIsTryingToAccess(DlsSubApplication? application)
        {
            context.ActionArguments.Add("dlsSubApplication", application);
        }
    }
}
