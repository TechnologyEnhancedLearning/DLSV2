namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging.Abstractions;
    using NUnit.Framework;

    public class PromoteToAdminControllerTests
    {
        private ICentreContractAdminUsageService centreContractAdminUsageService = null!;
        private PromoteToAdminController controller = null!;
        private ICourseCategoriesDataService courseCategoriesDataService = null!;
        private IRegistrationService registrationService = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void Setup()
        {
            userDataService = A.Fake<IUserDataService>();
            centreContractAdminUsageService = A.Fake<ICentreContractAdminUsageService>();
            courseCategoriesDataService = A.Fake<ICourseCategoriesDataService>();
            registrationService = A.Fake<IRegistrationService>();
            controller = new PromoteToAdminController(
                    userDataService,
                    courseCategoriesDataService,
                    centreContractAdminUsageService,
                    registrationService,
                    new NullLogger<PromoteToAdminController>()
                )
                .WithDefaultContext();
        }

        [Test]
        public void Summary_post_registers_delegate_with_expected_values()
        {
            // Given
            const int delegateId = 1;
            var formData = new AdminRolesFormData
            {
                IsCentreAdmin = true,
                IsSupervisor = false,
                IsTrainer = false,
                IsContentCreator = false,
                ContentManagementRole = ContentManagementRole.NoContentManagementRole,
                LearningCategory = 0
            };
            A.CallTo(() => registrationService.PromoteDelegateToAdmin(A<AdminRoles>._, A<int>._, A<int>._))
                .DoesNothing();

            // When
            var result = controller.Index(formData, delegateId);

            // Then
            A.CallTo(
                    () =>
                        registrationService.PromoteDelegateToAdmin(
                            A<AdminRoles>.That.Matches(
                                a =>
                                    a.IsCentreAdmin &&
                                    !a.IsContentManager
                            ),
                            null,
                            delegateId
                        )
                )
                .MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithControllerName("ViewDelegate").WithActionName("Index");
        }

        [Test]
        public void Summary_post_returns_500_error_with_unexpected_register_error()
        {
            // Given
            var formData = new AdminRolesFormData
            {
                IsCentreAdmin = true,
                IsSupervisor = false,
                IsTrainer = false,
                IsContentCreator = false,
                ContentManagementRole = ContentManagementRole.NoContentManagementRole,
                LearningCategory = 0
            };
            A.CallTo(() => registrationService.PromoteDelegateToAdmin(A<AdminRoles>._, A<int?>._, A<int>._))
                .Throws(new AdminCreationFailedException(AdminCreationError.UnexpectedError));

            // When
            var result = controller.Index(formData, 1);

            // Then
            result.Should().BeStatusCodeResult().WithStatusCode(500);
        }

        [Test]
        public void Summary_post_returns_redirect_to_index_with_email_in_use_register_error()
        {
            // Given
            var formData = new AdminRolesFormData
            {
                IsCentreAdmin = true,
                IsSupervisor = false,
                IsTrainer = false,
                IsContentCreator = false,
                ContentManagementRole = ContentManagementRole.NoContentManagementRole,
                LearningCategory = 0
            };
            A.CallTo(() => registrationService.PromoteDelegateToAdmin(A<AdminRoles>._, A<int?>._, A<int>._))
                .Throws(new AdminCreationFailedException(AdminCreationError.EmailAlreadyInUse));

            // When
            var result = controller.Index(formData, 1);

            // Then
            result.Should().BeViewResult().WithViewName("EmailInUse");
        }
    }
}
