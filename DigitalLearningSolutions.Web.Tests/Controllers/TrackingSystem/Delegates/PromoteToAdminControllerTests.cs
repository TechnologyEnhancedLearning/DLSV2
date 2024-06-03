﻿namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using FakeItEasy;
    using FakeItEasy.Configuration;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging.Abstractions;
    using NUnit.Framework;
    using System.Threading.Tasks;

    public class PromoteToAdminControllerTests
    {
        private ICentreContractAdminUsageService centreContractAdminUsageService = null!;
        private PromoteToAdminController controller = null!;
        private ICourseCategoriesService courseCategoriesService = null!;
        private IRegistrationService registrationService = null!;
        private IUserService userService = null!;
        private IEmailGenerationService emailGenerationService = null!;
        private IEmailService emailService = null!;

        [SetUp]
        public void Setup()
        {
            centreContractAdminUsageService = A.Fake<ICentreContractAdminUsageService>();
            courseCategoriesService = A.Fake<ICourseCategoriesService>();
            registrationService = A.Fake<IRegistrationService>();
            userService = A.Fake<IUserService>();
            emailGenerationService = A.Fake<IEmailGenerationService>();
            emailService = A.Fake<IEmailService>();

            controller = new PromoteToAdminController(
                    courseCategoriesService,
                    centreContractAdminUsageService,
                    registrationService,
                    new NullLogger<PromoteToAdminController>(),
                    userService,
                    emailGenerationService,
                    emailService
                )
                .WithDefaultContext();
            controller = controller.WithMockTempData();
        }

        [Test]
        public void Summary_post_registers_delegate_with_expected_values()
        {
            // Given
            const int delegateId = 159;
            var formData = new AdminRolesFormData
            {
                IsCentreAdmin = true,
                IsSupervisor = false,
                IsTrainer = false,
                IsContentCreator = false,
                ContentManagementRole = ContentManagementRole.NoContentManagementRole,
                LearningCategory = 0,
                UserId = 2,
                CentreId = 101,
            };
            A.CallTo(() => registrationService.PromoteDelegateToAdmin(A<AdminRoles>._, A<int>._, A<int>._, A<int>._, false))
                .DoesNothing();

            DelegateEntity delegateEntity = A.Fake<DelegateEntity>();
            delegateEntity.UserAccount.FirstName = "TestUserName";
            delegateEntity.UserAccount.PrimaryEmail = "test@example.com";

            A.CallTo(() => userService.GetDelegateById(delegateId)).Returns(delegateEntity);

            AdminUser returnedAdminUser = new AdminUser()
            {
                FirstName = "AdminUserFirstName",
                LastName = "AdminUserLastName",
                EmailAddress = "adminuser@example.com"
            };

            DelegateUser returnedDelegateUser = new DelegateUser() { };

            A.CallTo(() => userService.GetAdminUserByAdminId(A<int?>._)).Returns(returnedAdminUser);
            A.CallTo(() => userService.GetDelegateUserByDelegateUserIdAndCentreId(A<int?>._, A<int?>._)).Returns(returnedDelegateUser);

            var emailBody = A.Fake<MimeKit.BodyBuilder>();

            Email adminRolesEmail = new Email(string.Empty, emailBody, string.Empty, string.Empty);

            A.CallTo(() => emailGenerationService.GenerateDelegateAdminRolesNotificationEmail(
                            A<string>._, A<string>._, A<string>._, A<string>._,
                            A<bool>._, A<bool>._, A<bool>._, A<bool>._,
                            A<bool>._, A<bool>._, A<bool>._, A<bool>._,
                            A<string>._, A<string>._
                )).Returns(adminRolesEmail);

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
                            formData.UserId,
                            formData.CentreId,
                            false
                        )
                )
                .MustHaveHappened();

            A.CallTo(() => emailService.SendEmail(A<Email>._)).MustHaveHappened();

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
            A.CallTo(() => registrationService.PromoteDelegateToAdmin(A<AdminRoles>._, A<int?>._, A<int>._, A<int>._, A<bool>._))
                .Throws(new AdminCreationFailedException());

            // When
            var result = controller.Index(formData, 1);

            // Then
            result.Should().BeStatusCodeResult().WithStatusCode(500);
        }

        //[Test]
        //public void Summary_post_returns_redirect_to_index_with_email_in_use_register_error()
        //{
        //    // Given
        //    var formData = new AdminRolesFormData
        //    {
        //        IsCentreAdmin = true,
        //        IsSupervisor = false,
        //        IsTrainer = false,
        //        IsContentCreator = false,
        //        ContentManagementRole = ContentManagementRole.NoContentManagementRole,
        //        LearningCategory = 0
        //    };
        //    A.CallTo(() => registrationService.PromoteDelegateToAdmin(A<AdminRoles>._, A<int>._, A<int>._, A<int>._))
        //        .Throws(new AdminCreationFailedException(AdminCreationError.EmailAlreadyInUse));

        //    // When
        //    var result = controller.Index(formData, 1);

        //    // Then
        //    result.Should().BeViewResult().WithViewName("EmailInUse");
        //}
    }
}
