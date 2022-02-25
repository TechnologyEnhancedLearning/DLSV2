namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.ViewDelegate;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    internal class ViewDelegateControllerTests
    {
        private ICourseDataService courseDataService = null!;
        private ICourseService courseService = null!;
        private IUserDataService userDataService = null!;
        private ViewDelegateController viewDelegateController = null!;

        [SetUp]
        public void SetUp()
        {
            var centreCustomPromptsService = A.Fake<ICentreRegistrationPromptsService>();
            var centreCustomPromptsHelper = new CentreRegistrationPromptHelper(centreCustomPromptsService);
            userDataService = A.Fake<IUserDataService>();
            courseService = A.Fake<ICourseService>();
            var passwordResetService = A.Fake<IPasswordResetService>();
            courseDataService = A.Fake<ICourseDataService>();

            viewDelegateController = new ViewDelegateController(
                    userDataService,
                    centreCustomPromptsHelper,
                    courseService,
                    passwordResetService,
                    courseDataService
                )
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
        public void Removal_confirmation_page_displays_for_valid_delegate_and_course()
        {
            // given
            A.CallTo(() => userDataService.GetDelegateUserCardById(1))
                .Returns(new DelegateUserCard { CentreId = 2, Id = 1 });
            A.CallTo(() => courseDataService.GetCourseNameAndApplication(1))
                .Returns(new CourseNameInfo());
            A.CallTo(
                    () => courseService.DelegateHasCurrentProgress(
                        1,
                        1
                    )
                )
                .Returns(true);

            // when
            var result = viewDelegateController.ConfirmRemoveFromCourse(1, 1);

            // then
            result.Should().BeViewResult();
        }

        [Test]
        public void Removal_confirmation_page_returns_not_found_result_for_delegate_with_no_active_progress()
        {
            // given
            A.CallTo(() => userDataService.GetDelegateUserCardById(1))
                .Returns(new DelegateUserCard { CentreId = 2, Id = 1 });
            A.CallTo(() => courseDataService.GetCourseNameAndApplication(1))
                .Returns(new CourseNameInfo());
            A.CallTo(
                    () => courseService.DelegateHasCurrentProgress(
                        1,
                        1
                    )
                )
                .Returns(false);

            // when
            var result = viewDelegateController.ConfirmRemoveFromCourse(1, 1);

            // then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Delegate_removal_for_delegate_with_no_active_progress_returns_not_found_result()
        {
            // given
            A.CallTo(() => userDataService.GetDelegateUserCardById(1))
                .Returns(new DelegateUserCard { CentreId = 2, Id = 1 });
            A.CallTo(() => courseDataService.GetCourseNameAndApplication(1))
                .Returns(new CourseNameInfo());
            A.CallTo(
                    () => courseService.DelegateHasCurrentProgress(
                        1,
                        1
                    )
                )
                .Returns(false);
            var model = new RemoveFromCourseViewModel { Confirm = true };

            // when
            var result = viewDelegateController.ExecuteRemoveFromCourse(1, 1, model);

            // then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Delegate_removal_for_valid_delegate_returns_redirect_to_action()
        {
            // given
            A.CallTo(() => userDataService.GetDelegateUserCardById(1))
                .Returns(new DelegateUserCard { CentreId = 2, Id = 1 });
            A.CallTo(() => courseDataService.GetCourseNameAndApplication(1))
                .Returns(new CourseNameInfo());
            A.CallTo(
                    () => courseService.DelegateHasCurrentProgress(
                        1,
                        1
                    )
                )
                .Returns(true);
            var model = new RemoveFromCourseViewModel { Confirm = true };

            // when
            var result = viewDelegateController.ExecuteRemoveFromCourse(1, 1, model);

            // then
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
            //Given
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
            A.CallTo(() => userDataService.GetDelegateUserCardById(10)).Returns(new DelegateUserCard() { CentreId = 1});

            // When
            var result = viewDelegateController.ReactivateDelegate(2);

            // Then
            result.Should().BeNotFoundResult();
        }
    }
}
