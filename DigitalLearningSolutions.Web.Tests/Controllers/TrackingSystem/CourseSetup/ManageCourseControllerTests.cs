namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using NUnit.Framework;

    internal class ManageCourseControllerTests
    {
        private ManageCourseController controller = null!;
        private ICourseService courseService = null!;

        [SetUp]
        public void Setup()
        {
            courseService = A.Fake<ICourseService>();

            controller = new ManageCourseController(courseService)
                .WithDefaultContext()
                .WithMockUser(true, 101)
                .WithMockTempData();
        }

        [Test]
        public void Index_returns_ManageCourse_page_when_appropriate_course_found()
        {
            // Given
            A.CallTo(() => courseService.GetCourseDetailsFilteredByCategory(A<int>._, A<int>._, A<int>._))
                .Returns(new CourseDetails());

            // When
            var result = controller.Index(1);

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<ManageCourseViewModel>();
        }

        [Test]
        public void AddAdminField_post_updates_temp_data_and_redirects()
        {
            // Given
            var initialEditViewModel = new EditLearningPathwayDefaultsViewModel(1, "0", "0", false, false);
            var initialTempData = new EditLearningPathwayDefaultsData(initialEditViewModel);
            var inputViewModel = new EditLearningPathwayDefaultsViewModel(1, "12", "3", true, true);
            controller.TempData.Set(initialTempData);

            // When
            var result = controller.EditLearningPathwayDefaults(1, inputViewModel);

            // Then
            using (new AssertionScope())
            {
                controller.TempData.Peek<EditLearningPathwayDefaultsData>()!.LearningPathwayDefaultsModel.Should()
                    .BeEquivalentTo(inputViewModel);
                result.Should().BeRedirectToActionResult().WithActionName("EditAutoRefreshOptions");
            }
        }

        [Test]
        public void SaveLearningPathwayDefaults_save_calls_correct_method()
        {
            // Given
            var model = new EditLearningPathwayDefaultsViewModel(1, "6", "12", false, false);

            A.CallTo(
                () => courseService.UpdateLearningPathwayDefaultsForCourse(
                    A<int>._,
                    A<int>._,
                    A<int>._,
                    A<bool>._,
                    A<bool>._,
                    A<int>._,
                    A<int>._,
                    A<bool>._
                )
            ).DoesNothing();

            // When
            var result = controller.EditLearningPathwayDefaults(1, model);

            // Then
            A.CallTo(
                () => courseService.UpdateLearningPathwayDefaultsForCourse(
                    1,
                    6,
                    12,
                    false,
                    false,
                    0,
                    0,
                    false
                )
            ).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void SaveLearningPathwayDefaults_saves_if_number_input_is_null()
        {
            // Given
            var model = new EditLearningPathwayDefaultsViewModel(1, null, null, false, false);

            // When
            var result = controller.EditLearningPathwayDefaults(1, model);

            // Then
            A.CallTo(
                () => courseService.UpdateLearningPathwayDefaultsForCourse(
                    1,
                    0,
                    0,
                    false,
                    false,
                    0,
                    0,
                    false
                )
            ).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void
            SaveLearningPathwayDefaults_does_not_call_service_with_invalid_model()
        {
            // Given
            var model = new EditLearningPathwayDefaultsViewModel(1, "49", "12", false, false);
            controller.ModelState.AddModelError("CompleteWithinMonths", "Enter a whole number from 0 to 48");

            // When
            var result = controller.EditLearningPathwayDefaults(1, model);

            // Then
            A.CallTo(
                () => courseService.UpdateLearningPathwayDefaultsForCourse(
                    A<int>._,
                    A<int>._,
                    A<int>._,
                    A<bool>._,
                    A<bool>._,
                    A<int>._,
                    A<int>._,
                    A<bool>._
                )
            ).MustNotHaveHappened();
            result.Should().BeViewResult().ModelAs<EditLearningPathwayDefaultsViewModel>();
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Test]
        public void SaveAutoRefreshOptions_save_calls_correct_method()
        {
            // Given
            var learningPathwayModel = new EditLearningPathwayDefaultsViewModel(1, "6", "12", false, false);
            var autoRefreshModel = GetEditAutoRefreshOptionsViewModel();

            var learningPathwayData = new EditLearningPathwayDefaultsData(learningPathwayModel);
            controller.TempData.Set(learningPathwayData);

            var courseOptions = new List<(int, string)> { (1, "courseName") };
            A.CallTo(() => courseService.GetCourseOptionsAlphabeticalListForCentre(A<int>._, A<int>._))
                .Returns(courseOptions);

            A.CallTo(
                () => courseService.UpdateLearningPathwayDefaultsForCourse(
                    A<int>._,
                    A<int>._,
                    A<int>._,
                    A<bool>._,
                    A<bool>._,
                    A<int>._,
                    A<int>._,
                    A<bool>._
                )
            ).DoesNothing();

            // When
            var result = controller.EditAutoRefreshOptions(1, autoRefreshModel);

            // Then
            A.CallTo(
                () => courseService.UpdateLearningPathwayDefaultsForCourse(
                    1,
                    6,
                    12,
                    false,
                    false,
                    1,
                    12,
                    true
                )
            ).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void SaveAutoRefreshOptions_saves_if_number_input_is_null()
        {
            // Given
            var learningPathwayModel = new EditLearningPathwayDefaultsViewModel(1, "6", "12", false, false);
            var autoRefreshModel = GetEditAutoRefreshOptionsViewModel(autoRefreshMonths: null);

            var learningPathwayData = new EditLearningPathwayDefaultsData(learningPathwayModel);
            controller.TempData.Set(learningPathwayData);

            var courseOptions = new List<(int, string)> { (1, "courseName") };
            A.CallTo(() => courseService.GetCourseOptionsAlphabeticalListForCentre(A<int>._, A<int>._))
                .Returns(courseOptions);

            A.CallTo(
                () => courseService.UpdateLearningPathwayDefaultsForCourse(
                    A<int>._,
                    A<int>._,
                    A<int>._,
                    A<bool>._,
                    A<bool>._,
                    A<int>._,
                    A<int>._,
                    A<bool>._
                )
            ).DoesNothing();

            // When
            var result = controller.EditAutoRefreshOptions(1, autoRefreshModel);

            // Then
            A.CallTo(
                () => courseService.UpdateLearningPathwayDefaultsForCourse(
                    1,
                    6,
                    12,
                    false,
                    false,
                    1,
                    0,
                    true
                )
            ).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void SaveAutoRefreshOptions_does_not_call_service_with_invalid_model()
        {
            // Given
            var learningPathwayModel = new EditLearningPathwayDefaultsViewModel(1, "6", "12", false, false);
            var autoRefreshModel = GetEditAutoRefreshOptionsViewModel(autoRefreshMonths: 13);
            controller.ModelState.AddModelError("AutoRefreshMonths", "Enter a whole number from 0 to 12");

            var learningPathwayData = new EditLearningPathwayDefaultsData(learningPathwayModel);
            controller.TempData.Set(learningPathwayData);

            var courseOptions = new List<(int, string)> { (1, "courseName") };
            A.CallTo(() => courseService.GetCourseOptionsAlphabeticalListForCentre(A<int>._, A<int>._))
                .Returns(courseOptions);

            A.CallTo(
                () => courseService.UpdateLearningPathwayDefaultsForCourse(
                    A<int>._,
                    A<int>._,
                    A<int>._,
                    A<bool>._,
                    A<bool>._,
                    A<int>._,
                    A<int>._,
                    A<bool>._
                )
            ).DoesNothing();
            controller.ModelState.AddModelError("RefreshToCustomisationId", "Select a course");

            // When
            var result = controller.EditAutoRefreshOptions(1, autoRefreshModel);

            // Then
            A.CallTo(
                () => courseService.UpdateLearningPathwayDefaultsForCourse(
                    A<int>._,
                    A<int>._,
                    A<int>._,
                    A<bool>._,
                    A<bool>._,
                    A<int>._,
                    A<int>._,
                    A<bool>._
                )
            ).MustNotHaveHappened();
            result.Should().BeViewResult().ModelAs<EditAutoRefreshOptionsViewModel>();
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Test]
        public void SaveCourseDetails_calls_correct_service_method_with_valid_inputs()
        {
            // Given
            var model = GetEditCourseDetailsViewModel();

            A.CallTo(
                () => courseService.UpdateCourseDetails(
                    1,
                    "Name",
                    "Password",
                    "hello@test.com",
                    false,
                    75,
                    90
                )
            ).DoesNothing();

            // When
            var result = controller.SaveCourseDetails(1, model);

            // Then
            A.CallTo(
                () => courseService.UpdateCourseDetails(
                    1,
                    "Name",
                    "Password",
                    "hello@test.com",
                    false,
                    90,
                    75
                )
            ).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void
            SaveCourseDetails_does_not_call_service_with_invalid_model()
        {
            // Given
            var model = GetEditCourseDetailsViewModel();
            controller.ModelState.AddModelError("Email", "Email address must not contain any whitespace characters");

            // When
            var result = controller.SaveCourseDetails(1, model);

            // Then
            A.CallTo(
                () => courseService.UpdateCourseDetails(
                    A<int>._,
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<bool>._,
                    A<int>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
            result.Should().BeViewResult().ModelAs<EditCourseDetailsViewModel>();
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Test]
        public void
            SaveCourseDetails_correctly_adds_model_error_if_customisation_name_is_not_unique()
        {
            // Given
            var model = GetEditCourseDetailsViewModel();

            A.CallTo(
                () => courseService.DoesCourseNameExistAtCentre(
                    "Name",
                    101,
                    1,
                    1
                )
            ).Returns(true);

            // When
            var result = controller.SaveCourseDetails(1, model);

            // Then
            A.CallTo(
                () => courseService.UpdateCourseDetails(
                    A<int>._,
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<bool>._,
                    A<int>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
            result.Should().BeViewResult().ModelAs<EditCourseDetailsViewModel>();
            controller.ModelState["CustomisationName"].Errors[0].ErrorMessage.Should()
                .BeEquivalentTo("Course name must be unique, including any additions");
        }

        [Test]
        public void
            SaveCourseDetails_correctly_adds_model_error_if_application_already_exists_with_blank_customisation_name()
        {
            // Given
            var model = GetEditCourseDetailsViewModel(customisationName: string.Empty);

            A.CallTo(
                () => courseService.DoesCourseNameExistAtCentre(
                    "",
                    101,
                    1,
                    1
                )
            ).Returns(true);

            // When
            var result = controller.SaveCourseDetails(1, model);

            // Then
            A.CallTo(
                () => courseService.UpdateCourseDetails(
                    A<int>._,
                    A<string>._,
                    A<string>._,
                    A<string>._,
                    A<bool>._,
                    A<int>._,
                    A<int>._
                )
            ).MustNotHaveHappened();
            result.Should().BeViewResult().ModelAs<EditCourseDetailsViewModel>();
            controller.ModelState["CustomisationName"].Errors[0].ErrorMessage.Should()
                .BeEquivalentTo("A course with no add-on already exists");
        }

        [Test]
        public void
            SaveCourseDetails_clears_values_of_conditional_inputs_if_corresponding_checkboxes_or_radios_are_unchecked()
        {
            // Given
            var model = GetEditCourseDetailsViewModel(
                passwordProtected: false,
                receiveNotificationEmails: false,
                isAssessed: true
            );

            A.CallTo(
                () => courseService.DoesCourseNameExistAtCentre(
                    "Name",
                    101,
                    1,
                    1
                )
            ).Returns(false);

            // When
            var result = controller.SaveCourseDetails(1, model);

            // Then
            A.CallTo(
                () => courseService.UpdateCourseDetails(
                    1,
                    "Name",
                    null!,
                    null!,
                    true,
                    0,
                    0
                )
            ).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("Index");
        }

        [Test]
        public void Edit_Course_Options_page_opens_up_with_course_options()
        {
            // Given
            const int customisationId = 1;
            A.CallTo(
                () => courseService.VerifyAdminUserCanManageCourse(
                    customisationId,
                    A<int>._,
                    A<int>._
                )
            ).Returns(true);

            A.CallTo(
                () => courseService.GetCourseOptionsFilteredByCategory(
                    customisationId,
                    A<int>._,
                    A<int>._
                )
            ).Returns(new CourseOptions());

            // When
            var result = controller.EditCourseOptions(customisationId);

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<EditCourseOptionsViewModel>();
        }

        [Test]
        public void Edit_Course_Options_page_redirects_to_Index_when_course_details_are_updated()
        {
            // Given
            const int customisationId = 1;
            var courseOptions = new CourseOptions
            {
                Active = true,
                DiagObjSelect = true,
                HideInLearnerPortal = true,
                SelfRegister = true,
            };

            A.CallTo(
                () => courseService.UpdateCourseOptions(
                    A<CourseOptions>._,
                    customisationId
                )
            ).DoesNothing();

            var editCourseOptionsViewModel = new EditCourseOptionsViewModel(courseOptions, customisationId);

            // When
            var result = controller.EditCourseOptions(customisationId, editCourseOptionsViewModel);

            // Then
            A.CallTo(
                () => courseService.UpdateCourseOptions(
                    A<CourseOptions>._,
                    customisationId
                )
            ).MustHaveHappened();

            result.Should().BeRedirectToActionResult().WithControllerName("ManageCourse").WithActionName("Index");
        }

        private static EditAutoRefreshOptionsViewModel GetEditAutoRefreshOptionsViewModel(
            int customisationId = 1,
            int refreshToCustomisationId = 1,
            int? autoRefreshMonths = 12,
            bool applyLpDefaultsToSelfEnrol = true
        )
        {
            var formData = new EditAutoRefreshOptionsFormData
            {
                RefreshToCustomisationId = refreshToCustomisationId,
                AutoRefreshMonths = autoRefreshMonths == null ? null : autoRefreshMonths.ToString(),
                ApplyLpDefaultsToSelfEnrol = applyLpDefaultsToSelfEnrol,
            };
            var courseOptions = new List<SelectListItem>();

            return new EditAutoRefreshOptionsViewModel(
                formData,
                customisationId,
                courseOptions
            );
        }

        private static EditCourseDetailsViewModel GetEditCourseDetailsViewModel(
            int customisationId = 1,
            int applicationId = 1,
            string customisationName = "Name",
            bool passwordProtected = true,
            string password = "Password",
            bool receiveNotificationEmails = true,
            string notificationEmails = "hello@test.com",
            bool postLearningAssessment = true,
            bool isAssessed = false,
            bool diagAssess = true,
            string? tutCompletionThreshold = "90",
            string? diagCompletionThreshold = "75"
        )
        {
            var formData = new EditCourseDetailsFormData
            {
                ApplicationId = applicationId,
                CustomisationName = customisationName,
                PasswordProtected = passwordProtected,
                Password = password,
                ReceiveNotificationEmails = receiveNotificationEmails,
                NotificationEmails = notificationEmails,
                PostLearningAssessment = postLearningAssessment,
                IsAssessed = isAssessed,
                DiagAssess = diagAssess,
                TutCompletionThreshold = tutCompletionThreshold,
                DiagCompletionThreshold = diagCompletionThreshold,
            };

            return new EditCourseDetailsViewModel(
                formData,
                customisationId
            );
        }
    }
}
