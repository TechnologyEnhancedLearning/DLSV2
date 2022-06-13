namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EditDelegate;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public class EditDelegateControllerTests
    {
        private const int DelegateId = 1;
        private EditDelegateController controller = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private PromptsService promptsService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void SetUp()
        {
            promptsService = A.Fake<PromptsService>();
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            userService = A.Fake<IUserService>();

            controller = new EditDelegateController(userService, jobGroupsDataService, promptsService)
                .WithDefaultContext()
                .WithMockUser(true);
        }

        [Test]
        public void Index_returns_not_found_with_null_delegate()
        {
            // Given
            A.CallTo(() => userService.GetDelegateById(DelegateId)).Returns(null);

            // When
            var result = controller.Index(DelegateId);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Index_returns_not_found_with_delegate_at_different_centre()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegate(DelegateId, centreId: 4);
            A.CallTo(() => userService.GetDelegateById(DelegateId)).Returns(delegateUser);

            // When
            var result = controller.Index(DelegateId);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Index_shows_centre_specific_email_if_not_null()
        {
            // Given
            const string centreSpecificEmail = "centre@email.com";
            var delegateUser = UserTestHelper.GetDefaultDelegate(
                DelegateId,
                userCentreDetailsId: 1,
                centreSpecificEmail: centreSpecificEmail,
                centreSpecificEmailVerified: false
            );
            A.CallTo(() => userService.GetDelegateById(DelegateId)).Returns(delegateUser);

            // When
            var result = controller.Index(DelegateId);

            // Then
            result.As<ViewResult>().Model.As<EditDelegateViewModel>().CentreSpecificEmail.Should()
                .Be(centreSpecificEmail);
        }

        [Test]
        public void Index_shows_primary_email_if_centre_specific_email_is_null()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegate(DelegateId, centreSpecificEmail: null);
            A.CallTo(() => userService.GetDelegateById(DelegateId)).Returns(delegateUser);

            // When
            var result = controller.Index(DelegateId);

            // Then
            result.As<ViewResult>().Model.As<EditDelegateViewModel>().CentreSpecificEmail.Should()
                .Be(delegateUser.UserAccount.PrimaryEmail);
        }

        [Test]
        public void Index_post_returns_view_with_model_error_with_duplicate_email()
        {
            // Given
            const string email = "test@email.com";
            var delegateUser = UserTestHelper.GetDefaultDelegate(DelegateId);
            var formData = new EditDelegateFormData
            {
                JobGroupId = 1,
                Email = email,
                HasProfessionalRegistrationNumber = false,
            };

            A.CallTo(() => userService.GetDelegateById(DelegateId)).Returns(delegateUser);
            A.CallTo(() => userService.NewEmailAddressIsValid(email, delegateUser.UserAccount.Id)).Returns(false);

            // When
            var result = controller.Index(formData, DelegateId);

            // Then
            using (new AssertionScope())
            {
                result.As<ViewResult>().Model.Should().BeOfType<EditDelegateViewModel>();
                AssertModelStateErrorIsExpected(
                    result,
                    "This email is already in use"
                );
            }
        }

        [Test]
        public void Index_post_returns_view_with_model_error_with_invalid_prn()
        {
            // Given
            var formData = new EditDelegateFormData
            {
                JobGroupId = 1,
                HasProfessionalRegistrationNumber = true,
                ProfessionalRegistrationNumber = "!&^£%&*^!%£",
            };

            A.CallTo(() => userService.NewEmailAddressIsValid(A<string>._, 2)).Returns(true);

            // When
            var result = controller.Index(formData, DelegateId);

            // Then
            using (new AssertionScope())
            {
                result.As<ViewResult>().Model.Should().BeOfType<EditDelegateViewModel>();
                AssertModelStateErrorIsExpected(
                    result,
                    "Invalid professional registration number format - Only alphanumeric characters (a-z, A-Z and 0-9) and hyphens (-) allowed"
                );
                A.CallTo(() => userService.GetDelegateById(A<int>._)).MustNotHaveHappened();
            }
        }

        [Test]
        public void Index_post_calls_userServices_and_redirects_with_no_validation_errors()
        {
            // Given
            const string centreSpecificEmail = "centre@email.com";
            var delegateUser = UserTestHelper.GetDefaultDelegate(
                DelegateId,
                userCentreDetailsId: 1,
                centreSpecificEmail: centreSpecificEmail,
                centreSpecificEmailVerified: false
            );
            var formData = new EditDelegateFormData
            {
                JobGroupId = 1,
                HasProfessionalRegistrationNumber = false,
                CentreSpecificEmail = centreSpecificEmail,
            };

            A.CallTo(() => userService.GetDelegateById(DelegateId)).Returns(delegateUser);
            A.CallTo(() => userService.NewEmailAddressIsValid(centreSpecificEmail, delegateUser.UserAccount.Id))
                .Returns(true);

            // When
            var result = controller.Index(formData, DelegateId);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => userService.GetDelegateById(DelegateId)).MustHaveHappenedOnceExactly();
                A.CallTo(() => userService.NewEmailAddressIsValid(centreSpecificEmail, delegateUser.UserAccount.Id))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(
                    () => userService.UpdateUserDetailsAndCentreSpecificDetails(
                        A<EditAccountDetailsData>._,
                        A<DelegateDetailsData>._,
                        centreSpecificEmail,
                        delegateUser.DelegateAccount.CentreId
                    )
                ).MustHaveHappened();
                result.Should().BeRedirectToActionResult().WithControllerName("ViewDelegate").WithActionName("Index");
            }
        }

        [Test]
        public void Index_post_saves_centre_specific_email_as_null_if_same_as_primary_email()
        {
            // Given
            const string primaryEmail = "primary@email";
            var delegateUser = UserTestHelper.GetDefaultDelegate(
                DelegateId,
                primaryEmail: primaryEmail,
                centreSpecificEmail: "random@email"
            );
            A.CallTo(() => userService.GetDelegateById(DelegateId)).Returns(delegateUser);

            var formData = new EditDelegateFormData
            {
                JobGroupId = 1,
                HasProfessionalRegistrationNumber = false,
                CentreSpecificEmail = primaryEmail,
            };
            A.CallTo(() => userService.NewEmailAddressIsValid(A<string>._, A<int>._)).Returns(true);

            // When
            var result = controller.Index(formData, DelegateId);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => userService.UpdateUserDetailsAndCentreSpecificDetails(
                        A<EditAccountDetailsData>._,
                        A<DelegateDetailsData>._,
                        null,
                        delegateUser.DelegateAccount.CentreId
                    )
                ).MustHaveHappened();
                result.Should().BeRedirectToActionResult().WithControllerName("ViewDelegate").WithActionName("Index");
            }
        }

        private static void AssertModelStateErrorIsExpected(IActionResult result, string expectedErrorMessage)
        {
            var errorMessage = result.As<ViewResult>().ViewData.ModelState.Select(x => x.Value.Errors)
                .Where(y => y.Count > 0).ToList().First().First().ErrorMessage;
            errorMessage.Should().BeEquivalentTo(expectedErrorMessage);
        }
    }
}
