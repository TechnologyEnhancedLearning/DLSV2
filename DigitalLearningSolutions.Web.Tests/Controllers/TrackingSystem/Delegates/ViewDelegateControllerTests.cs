namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.ViewDelegate;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    internal class ViewDelegateControllerTests
    {
        private ICourseDataService courseDataService = null!;
        private ICourseService courseService = null!;
        private IUserDataService userDataService = null!;
        private ViewDelegateController viewDelegateController = null!;
        private IConfiguration config = null!;

        [SetUp]
        public void SetUp()
        {
            var centreCustomPromptsService = A.Fake<ICentreRegistrationPromptsService>();
            var centreCustomPromptsHelper = new PromptsService(centreCustomPromptsService);
            var passwordResetService = A.Fake<IPasswordResetService>();

            userDataService = A.Fake<IUserDataService>();
            courseService = A.Fake<ICourseService>();
            courseDataService = A.Fake<ICourseDataService>();
            config = A.Fake<IConfiguration>();

            viewDelegateController = new ViewDelegateController(
                    userDataService,
                    centreCustomPromptsHelper,
                    courseService,
                    passwordResetService,
                    courseDataService,
                    config
                )
                .WithDefaultContext()
                .WithMockUser(true);
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
        public void Removal_confirmation_page_displays_for_valid_delegate_and_course()
        {
            // Given
            A.CallTo(() => userDataService.GetDelegateUserCardById(1))
                .Returns(new DelegateUserCard { CentreId = 2, Id = 1 });

            A.CallTo(() => courseDataService.GetCourseNameAndApplication(1))
                .Returns(new CourseNameInfo());

            A.CallTo(() => courseService.DelegateHasCurrentProgress(1, 1))
                .Returns(true);

            // When
            var result = viewDelegateController.ConfirmRemoveFromCourse(
                1,
                1,
                DelegateAccessRoute.ViewDelegate
            );

            // Then
            result.Should().BeViewResult();
        }

        [Test]
        public void Removal_confirmation_page_returns_not_found_result_for_delegate_with_no_active_progress()
        {
            // Given
            A.CallTo(() => userDataService.GetDelegateUserCardById(1))
                .Returns(new DelegateUserCard { CentreId = 2, Id = 1 });

            A.CallTo(() => courseDataService.GetCourseNameAndApplication(1))
                .Returns(new CourseNameInfo());

            A.CallTo(() => courseService.DelegateHasCurrentProgress(1, 1))
                .Returns(false);

            // When
            var result = viewDelegateController.ConfirmRemoveFromCourse(
                1,
                1,
                DelegateAccessRoute.ViewDelegate
            );

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Delegate_removal_for_delegate_with_no_active_progress_returns_not_found_result()
        {
            // Given
            A.CallTo(() => userDataService.GetDelegateUserCardById(1))
                .Returns(new DelegateUserCard { CentreId = 2, Id = 1 });

            A.CallTo(() => courseDataService.GetCourseNameAndApplication(1))
                .Returns(new CourseNameInfo());

            A.CallTo(() => courseService.DelegateHasCurrentProgress(1, 1))
                .Returns(false);

            var model = new RemoveFromCourseViewModel { Confirm = true };

            // When
            var result = viewDelegateController.ExecuteRemoveFromCourse(
                1,
                1,
                model,
                DelegateAccessRoute.ViewDelegate
            );

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Delegate_removal_for_valid_delegate_returns_redirect_to_action()
        {
            // Given
            A.CallTo(() => userDataService.GetDelegateUserCardById(1))
                .Returns(new DelegateUserCard { CentreId = 2, Id = 1 });

            A.CallTo(() => courseDataService.GetCourseNameAndApplication(1))
                .Returns(new CourseNameInfo());

            A.CallTo(() => courseService.DelegateHasCurrentProgress(1, 1))
                .Returns(true);

            var model = new RemoveFromCourseViewModel
            {
                Confirm = true,
            };

            // When
            var result = viewDelegateController.ExecuteRemoveFromCourse(
                1,
                1,
                model,
                DelegateAccessRoute.ViewDelegate
            );

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
                .Returns(new DelegateUserCard() { CentreId = 1 });

            // When
            var result = viewDelegateController.ReactivateDelegate(2);

            // Then
            result.Should().BeNotFoundResult();
        }
    }
}
