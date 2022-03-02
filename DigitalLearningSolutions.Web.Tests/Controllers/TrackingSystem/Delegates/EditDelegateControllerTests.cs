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
        private CentreRegistrationPromptHelper centreRegistrationPromptHelper = null!;
        private EditDelegateController controller = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;
        private IUserService userService = null!;

        [SetUp]
        public void SetUp()
        {
            centreRegistrationPromptHelper = A.Fake<CentreRegistrationPromptHelper>();
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            userService = A.Fake<IUserService>();

            controller = new EditDelegateController(userService, jobGroupsDataService, centreRegistrationPromptHelper)
                .WithDefaultContext()
                .WithMockUser(true);
        }

        [Test]
        public void Index_returns_not_found_with_null_delegate()
        {
            // Given
            A.CallTo(() => userService.GetUsersById(null, DelegateId)).Returns((null, null));

            // When
            var result = controller.Index(DelegateId);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Index_returns_not_found_with_delegate_at_different_centre()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(centreId: 4);
            A.CallTo(() => userService.GetUsersById(null, DelegateId)).Returns((null, delegateUser));

            // When
            var result = controller.Index(DelegateId);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Index_post_returns_view_with_model_error_with_duplicate_email()
        {
            // Given
            const string email = "test@email.com";
            var formData = new EditDelegateFormData
            {
                JobGroupId = 1,
                Email = email,
                HasProfessionalRegistrationNumber = false,
            };
            A.CallTo(() => userService.NewEmailAddressIsValid(email, null, DelegateId, A<int>._)).Returns(false);
            A.CallTo(() => userService.NewAliasIsValid(A<string>._, DelegateId, A<int>._)).Returns(true);

            // When
            var result = controller.Index(formData, DelegateId);

            // Then
            using (new AssertionScope())
            {
                result.As<ViewResult>().Model.Should().BeOfType<EditDelegateViewModel>();
                AssertModelStateErrorIsExpected(
                    result,
                    "A user with this email address is already registered at this centre"
                );
            }
        }

        [Test]
        public void Index_post_returns_view_with_model_error_with_duplicate_alias()
        {
            // Given
            const string alias = "alias";
            var formData = new EditDelegateFormData
            {
                JobGroupId = 1,
                AliasId = alias,
            };
            A.CallTo(() => userService.NewEmailAddressIsValid(A<string>._, null, DelegateId, A<int>._)).Returns(true);
            A.CallTo(() => userService.NewAliasIsValid(alias, DelegateId, A<int>._)).Returns(false);

            // When
            var result = controller.Index(formData, DelegateId);

            // Then
            using (new AssertionScope())
            {
                result.As<ViewResult>().Model.Should().BeOfType<EditDelegateViewModel>();
                AssertModelStateErrorIsExpected(
                    result,
                    "A user with this alias ID is already registered at this centre"
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
            A.CallTo(() => userService.NewEmailAddressIsValid(A<string>._, null, DelegateId, A<int>._)).Returns(true);
            A.CallTo(() => userService.NewAliasIsValid(A<string>._, DelegateId, A<int>._)).Returns(true);

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
            }
        }

        [Test]
        public void Index_post_calls_userService_and_redirects_with_no_validation_errors()
        {
            // Given
            var formData = new EditDelegateFormData
            {
                JobGroupId = 1,
                HasProfessionalRegistrationNumber = false,
            };
            A.CallTo(() => userService.NewEmailAddressIsValid(A<string>._, null, DelegateId, A<int>._)).Returns(true);
            A.CallTo(() => userService.NewAliasIsValid(A<string>._, DelegateId, A<int>._)).Returns(true);

            // When
            var result = controller.Index(formData, DelegateId);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                    () => userService.UpdateUserAccountDetailsViaDelegateAccount(
                        A<EditDelegateDetailsData>._,
                        A<CentreAnswersData>._
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
