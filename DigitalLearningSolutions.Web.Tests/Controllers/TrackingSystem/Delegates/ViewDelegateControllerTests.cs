namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.ViewDelegate;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    internal class ViewDelegateControllerTests
    {
        private IConfiguration config = null!;
        private ICourseService courseService = null!;
        private IUserDataService userDataService = null!;
        private IUserService userService = null!;
        private ViewDelegateController viewDelegateController = null!;

        [SetUp]
        public void SetUp()
        {
            var centreCustomPromptsService = A.Fake<ICentreRegistrationPromptsService>();
            var centreCustomPromptsHelper = new PromptsService(centreCustomPromptsService);
            var passwordResetService = A.Fake<IPasswordResetService>();

            userService = A.Fake<IUserService>();
            userDataService = A.Fake<IUserDataService>();
            courseService = A.Fake<ICourseService>();
            config = A.Fake<IConfiguration>();

            viewDelegateController = new ViewDelegateController(
                    userDataService,
                    userService,
                    centreCustomPromptsHelper,
                    courseService,
                    passwordResetService,
                    config
                )
                .WithDefaultContext()
                .WithMockUser(true);
        }

        [Test]
        public void Index_shows_centre_specific_email_if_not_null()
        {
            // Given
            const int delegateId = 1;
            const string centreSpecificEmail = "centre@email.com";
            var delegateEntity = UserTestHelper.GetDefaultDelegateEntity(
                userCentreDetailsId: 1,
                centreSpecificEmail: centreSpecificEmail
            );
            A.CallTo(() => userService.GetDelegateById(delegateId)).Returns(delegateEntity);

            // When
            var result = viewDelegateController.Index(delegateId);

            // Then
            result.As<ViewResult>().Model.As<ViewDelegateViewModel>().DelegateInfo.Email.Should()
                .Be(centreSpecificEmail);
        }

        [Test]
        public void Index_shows_primary_email_if_centre_specific_email_is_null()
        {
            // Given
            const int delegateId = 1;
            var delegateEntity = UserTestHelper.GetDefaultDelegateEntity(delegateId, centreSpecificEmail: null);
            A.CallTo(() => userService.GetDelegateById(delegateId)).Returns(delegateEntity);

            // When
            var result = viewDelegateController.Index(delegateId);

            // Then
            result.As<ViewResult>().Model.As<ViewDelegateViewModel>().DelegateInfo.Email.Should()
                .Be(delegateEntity.UserAccount.PrimaryEmail);
        }

        [Test]
        public void Index_returns_not_found_result_if_no_delegate_found_with_given_id()
        {
            // Given
            const int delegateId = 1;
            A.CallTo(() => userService.GetDelegateById(delegateId)).Returns(null);

            // When
            var result = viewDelegateController.Index(delegateId);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Deactivating_delegate_returns_redirect()
        {
            // Given
            A.CallTo(() => userDataService.GetDelegateUserCardById(1))
                .Returns(new DelegateUserCard { CentreId = 2, Id = 1 });

            // When
            var result = viewDelegateController.DeactivateDelegate(1);

            // Then
            result.Should().BeRedirectToActionResult();
        }

        [Test]
        public void Reactivating_delegate_redirects_to_index_page()
        {
            // Given
            A.CallTo(() => userDataService.GetDelegateUserCardById(1))
                .Returns(new DelegateUserCard { CentreId = 2, Id = 1, Active = false });

            A.CallTo(() => userDataService.ActivateDelegateUser(1)).DoesNothing();

            // When
            var result = viewDelegateController.ReactivateDelegate(1);

            // Then
            A.CallTo(() => userDataService.ActivateDelegateUser(1)).MustHaveHappened();
            result.Should().BeRedirectToActionResult();
        }

        [Test]
        public void ReactivateDelegate_nonexistent_delegate_returns_not_found_result()
        {
            // Given
            A.CallTo(() => userDataService.GetDelegateUserCardById(10)).Returns(null);

            // When
            var result = viewDelegateController.ReactivateDelegate(10);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void ReactivateDelegate_delegate_on_wrong_centre_returns_not_found_result()
        {
            //Given
            A.CallTo(() => userDataService.GetDelegateUserCardById(10))
                .Returns(new DelegateUserCard { CentreId = 1 });

            // When
            var result = viewDelegateController.ReactivateDelegate(2);

            // Then
            result.Should().BeNotFoundResult();
        }
    }
}
