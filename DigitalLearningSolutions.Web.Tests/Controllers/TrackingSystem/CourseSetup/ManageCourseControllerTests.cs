namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.CourseSetup
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    internal class ManageCourseControllerTests
    {
        private readonly ICourseService courseService = A.Fake<ICourseService>();
        private ManageCourseController controller = null!;

        [SetUp]
        public void Setup()
        {
            controller = new ManageCourseController(courseService)
                .WithDefaultContext()
                .WithMockUser(true, 101);
        }

        [Test]
        public void Index_returns_ManageCourse_page_when_appropriate_course_found()
        {
            // Given
            A.CallTo(() => courseService.GetCourseDetailsForAdminCategoryId(A<int>._, A<int>._, A<int>._))
                .Returns(new CourseDetails());

            // When
            var result = controller.Index(1);

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<ManageCourseViewModel>();
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
                    A<bool>._
                )
            ).DoesNothing();

            // When
            var result = controller.SaveLearningPathwayDefaults(1, model);

            // Then
            A.CallTo(
                () => courseService.UpdateLearningPathwayDefaultsForCourse(
                    1,
                    6,
                    12,
                    false,
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
            var result = controller.SaveLearningPathwayDefaults(1, model);

            // Then
            A.CallTo(
                () => courseService.UpdateLearningPathwayDefaultsForCourse(
                    1,
                    0,
                    0,
                    false,
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
            var result = controller.SaveLearningPathwayDefaults(1, model);

            // Then
            A.CallTo(
                () => courseService.UpdateLearningPathwayDefaultsForCourse(
                    1,
                    49,
                    12,
                    false,
                    false
                )
            ).MustNotHaveHappened();
            result.Should().BeViewResult().ModelAs<EditLearningPathwayDefaultsViewModel>();
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [Test]
        public void Edit_Course_Options_with_course_details_page_opens()
        {
            // Given
            A.CallTo(
                () => courseDataService.GetCourseDetailsForAdminCategoryId(
                    A<int>._,
                    A<int>._,
                    A<int>._
                )
            ).Returns(new CourseDetails());

            // When
            var result = controller.EditCourseOptions(1);

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<EditCourseOptionsViewModel>();
        }

        [Test]
        public void Edit_Course_Options_post_returns_not_found_when_course_details_are_not_updated()
        {
            // Given
            var courseOptions = new CourseOptions();
            var editCourseOptionsViewModel = new EditCourseOptionsViewModel(courseOptions, 1);
            A.CallTo(
                () => courseDataService.TryUpdateCourseOptions(
                    A<CourseOptions>._,
                    A<int>._,
                    A<int>._,
                    A<int>._
                )
            ).Returns(false);

            // When
            var result = controller.EditCourseOptions(editCourseOptionsViewModel);

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("ManageCourse").WithActionName("Index");
        }

        [Test]
        public void Edit_Course_Options_page_when_course_details_are_successfully_updated()
        {
            // Given
            var courseOptions = new CourseOptions();
            var editCourseOptionsViewModel = new EditCourseOptionsViewModel(courseOptions, 1);
            A.CallTo(
                () => courseDataService.TryUpdateCourseOptions(
                    A<CourseOptions>._,
                    A<int>._,
                    A<int>._,
                    A<int>._
                )
            ).Returns(true);

            // When
            var result = controller.EditCourseOptions(editCourseOptionsViewModel);

            // Then
            result.Should().BeRedirectToActionResult().WithControllerName("ManageCourse").WithActionName("Index");
        }
    }
}
