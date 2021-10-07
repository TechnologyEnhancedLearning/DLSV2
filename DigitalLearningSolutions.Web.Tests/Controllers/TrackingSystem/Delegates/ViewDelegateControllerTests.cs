﻿namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    class ViewDelegateControllerTests
    {
        private ViewDelegateController viewDelegateController = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void SetUp()
        {

            var centreCustomPromptsService = A.Fake<ICentreCustomPromptsService>();
            var centreCustomPromptsHelper = new CentreCustomPromptHelper(centreCustomPromptsService);
            userDataService = A.Fake<IUserDataService>();
            var courseService = A.Fake<ICourseService>();
            var passwordResetService = A.Fake<IPasswordResetService>();

            viewDelegateController = new ViewDelegateController(userDataService, centreCustomPromptsHelper, courseService, passwordResetService)
                .WithDefaultContext()
                .WithMockUser(true);

        }


        [Test]
        public void Deactivating_delegate_returns_redirect()
        {
            // given
            A.CallTo(() => userDataService.GetDelegateUserCardById(1))
                .Returns(new DelegateUserCard { CentreId = 2, Id = 1 });

            // when
            var result = viewDelegateController.DeactivateDelegate(1);

            // then
            result.Should().BeRedirectToActionResult();
        }

        [Test]
        public void Deactivating_nonexistent_delegate_returns_not_found_result()
        {
            // when
            var result = viewDelegateController.DeactivateDelegate(-1);

            // then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Deactivating_delegate_on_wrong_centre_returns_not_found_result()
        {

            // when
            var result = viewDelegateController.DeactivateDelegate(2);

            // then
            result.Should().BeNotFoundResult();
        }


        [Test]
        public void Reactivating_delegate_returns_redirect()
        {
            // Given
            A.CallTo(() => userDataService.GetDelegateUserCardById(1))
                .Returns(new DelegateUserCard { CentreId = 2, Id = 1, Active = false});

            // When
            var result = viewDelegateController.ReactivateDelegate(1);

            // Then
            result.Should().BeRedirectToActionResult();
        }

        [Test]
        public void Reactivate_nonexistent_delegate_returns_not_found_result()
        {
            // When
            var result = viewDelegateController.ReactivateDelegate(10);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Reactivate_delegate_on_wrong_centre_returns_not_found_result()
        {

            // When
            var result = viewDelegateController.ReactivateDelegate(2);

            // Then
            result.Should().BeNotFoundResult();
        }
    }
}
