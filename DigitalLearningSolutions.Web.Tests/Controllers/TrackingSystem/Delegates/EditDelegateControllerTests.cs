namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
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
        private IJobGroupsService jobGroupsService = null!;
        private PromptsService promptsService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void SetUp()
        {
            promptsService = A.Fake<PromptsService>();
            jobGroupsService = A.Fake<IJobGroupsService>();
            userService = A.Fake<IUserService>();

            controller = new EditDelegateController(userService, jobGroupsService, promptsService)
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
            var delegateEntity = UserTestHelper.GetDefaultDelegateEntity(DelegateId, centreId: 4);
            A.CallTo(() => userService.GetDelegateById(DelegateId)).Returns(delegateEntity);

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
            var delegateEntity = UserTestHelper.GetDefaultDelegateEntity(
                DelegateId,
                userCentreDetailsId: 1,
                centreSpecificEmail: centreSpecificEmail
            );
            A.CallTo(() => userService.GetDelegateById(DelegateId)).Returns(delegateEntity);

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
            var delegateEntity = UserTestHelper.GetDefaultDelegateEntity(DelegateId, centreSpecificEmail: null);
            A.CallTo(() => userService.GetDelegateById(DelegateId)).Returns(delegateEntity);

            // When
            var result = controller.Index(DelegateId);

            // Then
            result.As<ViewResult>().Model.As<EditDelegateViewModel>().CentreSpecificEmail.Should()
                .Be(delegateEntity.UserAccount.PrimaryEmail);
        }

        [Test]
        public void Index_post_returns_view_with_model_error_with_duplicate_email()
        {
            // Given
            const string email = "centre@email.com";
            var delegateEntity = UserTestHelper.GetDefaultDelegateEntity(
                DelegateId,
                userCentreDetailsId: 1,
                centreSpecificEmail: "test@email.com"
            );
            var formData = new EditDelegateFormData
            {
                JobGroupId = 1,
                CentreSpecificEmail = email,
                HasProfessionalRegistrationNumber = false,
            };

            A.CallTo(() => userService.GetDelegateById(DelegateId)).Returns(delegateEntity);
            A.CallTo(
                () => userService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                    email,
                    delegateEntity.DelegateAccount.CentreId,
                    delegateEntity.UserAccount.Id
                )
            ).Returns(true);

            // When
            var result = controller.Index(formData, DelegateId);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => userService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                        email,
                        delegateEntity.DelegateAccount.CentreId,
                        delegateEntity.UserAccount.Id
                    )
                ).MustHaveHappenedOnceExactly();

                result.As<ViewResult>().Model.Should().BeOfType<EditDelegateViewModel>();
                AssertModelStateErrorIsExpected(
                    result,
                    "This email is already in use by another user at the centre"
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
                CentreSpecificEmail = "email@test.com"
            };

            // When
            var result = controller.Index(formData, DelegateId);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                        () => userService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                            A<string>._,
                            A<int>._,
                            A<int>._
                        )
                    )
                    .MustNotHaveHappened();

                result.As<ViewResult>().Model.Should().BeOfType<EditDelegateViewModel>();
                AssertModelStateErrorIsExpected(
                    result,
                    "Invalid professional registration number format. Accepted formats are: 1–2 letters followed by 6 digits (e.g., AB123456)" +
                    " 4–8 digits only " + "Optional ‘P’ followed by 5–6 digits ‘C’ or ‘P’ followed by 6 digits " +
                    "Optional letter followed by 5–6 digits ‘L’ followed by 4–6 digits 2 digits, hyphen, then 4–5 alphanumeric characters"

                );
                A.CallTo(() => userService.GetDelegateById(A<int>._)).MustNotHaveHappened();
            }
        }

        [Test]
        public void Index_post_calls_userServices_and_redirects_with_no_validation_errors()
        {
            // Given
            const string centreSpecificEmail = "centre@email.com";
            var delegateEntity = UserTestHelper.GetDefaultDelegateEntity(
                DelegateId,
                userCentreDetailsId: 1,
                centreSpecificEmail: "email@test.com"
            );
            var formData = new EditDelegateFormData
            {
                JobGroupId = 1,
                HasProfessionalRegistrationNumber = false,
                CentreSpecificEmail = centreSpecificEmail,
            };

            A.CallTo(() => userService.GetDelegateById(DelegateId)).Returns(delegateEntity);
            A.CallTo(
                () => userService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                    centreSpecificEmail,
                    delegateEntity.DelegateAccount.CentreId,
                    delegateEntity.UserAccount.Id
                )
            ).Returns(false);

            // When
            var result = controller.Index(formData, DelegateId);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => userService.GetDelegateById(DelegateId)).MustHaveHappenedOnceExactly();
                A.CallTo(
                    () => userService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                        centreSpecificEmail,
                        delegateEntity.DelegateAccount.CentreId,
                        delegateEntity.UserAccount.Id
                    )
                ).MustHaveHappenedOnceExactly();
                A.CallTo(
                    () => userService.UpdateUserDetailsAndCentreSpecificDetails(
                        A<EditAccountDetailsData>._,
                        A<DelegateDetailsData>._,
                        centreSpecificEmail,
                        delegateEntity.DelegateAccount.CentreId,
                        false,
                        true,
                        false
                    )
                ).MustHaveHappened();
                result.Should().BeRedirectToActionResult().WithControllerName("ViewDelegate").WithActionName("Index");
            }
        }

        [Test]
        public void Index_post_does_not_if_check_email_is_in_use_if_email_is_unchanged()
        {
            // Given
            const string centreSpecificEmail = "centre@email.com";
            var delegateEntity = UserTestHelper.GetDefaultDelegateEntity(
                DelegateId,
                userCentreDetailsId: 1,
                centreSpecificEmail: centreSpecificEmail
            );
            var formData = new EditDelegateFormData
            {
                JobGroupId = 1,
                HasProfessionalRegistrationNumber = false,
                CentreSpecificEmail = centreSpecificEmail,
            };

            A.CallTo(() => userService.GetDelegateById(DelegateId)).Returns(delegateEntity);

            // When
            var result = controller.Index(formData, DelegateId);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => userService.GetDelegateById(DelegateId)).MustHaveHappenedOnceExactly();
                A.CallTo(
                    () => userService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                        A<string>._,
                        A<int>._,
                        A<int>._
                    )
                ).MustNotHaveHappened();
                A.CallTo(
                    () => userService.UpdateUserDetailsAndCentreSpecificDetails(
                        A<EditAccountDetailsData>._,
                        A<DelegateDetailsData>._,
                        centreSpecificEmail,
                        delegateEntity.DelegateAccount.CentreId,
                        false,
                        false,
                        false
                    )
                ).MustHaveHappened();
                result.Should().BeRedirectToActionResult().WithControllerName("ViewDelegate").WithActionName("Index");
            }
        }

        [Test]
        public void Index_post_saves_centre_specific_email_as_null_if_same_as_primary_email_and_centre_email_is_null()
        {
            // Given
            const string primaryEmail = "primary@email";
            var delegateEntity = UserTestHelper.GetDefaultDelegateEntity(
                DelegateId,
                primaryEmail: primaryEmail,
                userCentreDetailsId: 1,
                centreSpecificEmail: null
            );
            A.CallTo(() => userService.GetDelegateById(DelegateId)).Returns(delegateEntity);

            var formData = new EditDelegateFormData
            {
                JobGroupId = 1,
                HasProfessionalRegistrationNumber = false,
                CentreSpecificEmail = primaryEmail,
            };

            // When
            var result = controller.Index(formData, DelegateId);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => userService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                        A<string>._,
                        A<int>._,
                        A<int>._
                    )
                ).MustNotHaveHappened();

                A.CallTo(
                    () => userService.UpdateUserDetailsAndCentreSpecificDetails(
                        A<EditAccountDetailsData>._,
                        A<DelegateDetailsData>._,
                        null,
                        delegateEntity.DelegateAccount.CentreId,
                        false,
                        false,
                        false
                    )
                ).MustHaveHappened();
                result.Should().BeRedirectToActionResult().WithControllerName("ViewDelegate").WithActionName("Index");
            }
        }

        [Test]
        public void
            Index_post_saves_centre_specific_email_as_null_if_same_as_primary_email_and_user_has_no_centre_details()
        {
            // Given
            const string primaryEmail = "primary@email";
            var delegateEntity = UserTestHelper.GetDefaultDelegateEntity(
                DelegateId,
                primaryEmail: primaryEmail,
                userCentreDetailsId: null,
                centreSpecificEmail: null
            );
            A.CallTo(() => userService.GetDelegateById(DelegateId)).Returns(delegateEntity);

            var formData = new EditDelegateFormData
            {
                JobGroupId = 1,
                HasProfessionalRegistrationNumber = false,
                CentreSpecificEmail = primaryEmail,
            };

            // When
            var result = controller.Index(formData, DelegateId);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => userService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                        A<string>._,
                        A<int>._,
                        A<int>._
                    )
                ).MustNotHaveHappened();

                A.CallTo(
                    () => userService.UpdateUserDetailsAndCentreSpecificDetails(
                        A<EditAccountDetailsData>._,
                        A<DelegateDetailsData>._,
                        null,
                        delegateEntity.DelegateAccount.CentreId,
                        false,
                        false,
                        false
                    )
                ).MustHaveHappened();
                result.Should().BeRedirectToActionResult().WithControllerName("ViewDelegate").WithActionName("Index");
            }
        }

        [Test]
        public void
            Index_post_saves_centre_specific_email_as_given_value_if_it_is_the_same_as_primary_email_and_centre_specific_email_already_exists()
        {
            // Given
            const string newCentreSpecificEmail = "primary@email";
            var delegateEntity = UserTestHelper.GetDefaultDelegateEntity(
                DelegateId,
                primaryEmail: newCentreSpecificEmail,
                userCentreDetailsId: 1,
                centreSpecificEmail: "old@centre.com"
            );

            var formData = new EditDelegateFormData
            {
                JobGroupId = 1,
                HasProfessionalRegistrationNumber = false,
                CentreSpecificEmail = newCentreSpecificEmail,
            };

            A.CallTo(() => userService.GetDelegateById(DelegateId)).Returns(delegateEntity);

            A.CallTo(
                () => userService.CentreSpecificEmailIsInUseAtCentreByOtherUser(
                    newCentreSpecificEmail,
                    delegateEntity.DelegateAccount.CentreId,
                    delegateEntity.UserAccount.Id
                )
            ).Returns(false);

            // When
            var result = controller.Index(formData, DelegateId);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => userService.UpdateUserDetailsAndCentreSpecificDetails(
                        A<EditAccountDetailsData>._,
                        A<DelegateDetailsData>._,
                        newCentreSpecificEmail,
                        delegateEntity.DelegateAccount.CentreId,
                        false,
                        true,
                        false
                    )
                ).MustHaveHappened();
                result.Should().BeRedirectToActionResult().WithControllerName("ViewDelegate").WithActionName("Index");
            }
        }

        private static void AssertModelStateErrorIsExpected(IActionResult result, string expectedErrorMessage)
        {
            var errorMessage = result.As<ViewResult>().ViewData.ModelState.Select(x => x.Value!.Errors)
                .Where(y => y.Count > 0).ToList().First().First().ErrorMessage;
            errorMessage.Should().BeEquivalentTo(expectedErrorMessage);
        }
    }
}
