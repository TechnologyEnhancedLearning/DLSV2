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
        private readonly ICourseDataService courseDataService = A.Fake<ICourseDataService>();
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
        public void Index_returns_NotFound_when_no_appropriate_course_found()
        {
            // Given
            A.CallTo(() => courseService.GetCourseDetailsForAdminCategoryId(A<int>._, A<int>._, A<int>._))
                .Returns(null);

            // When
            var result = controller.Index(1);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Index_returns_ManageCourse_page_when_appropriate_course_found()
        {
            // Given
            A.CallTo(() => courseDataService.GetCourseDetailsForAdminCategoryId(A<int>._, A<int>._, A<int>._))
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
        public void SaveLearningPathwayDefaults_shows_validation_errors_if_number_input_contains_non_integers()
        {
            // Given
            var model = new EditLearningPathwayDefaultsViewModel(2, "9.0", "asdfg", false, false);

            // When
            var result = controller.SaveLearningPathwayDefaults(2, model);

            // Then
            A.CallTo(
                () => courseService.UpdateLearningPathwayDefaultsForCourse(
                    2,
                    A<int>._,
                    A<int>._,
                    false,
                    false
                )
            ).MustNotHaveHappened();
            result.Should().BeViewResult().ModelAs<EditLearningPathwayDefaultsViewModel>();
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [Test]
        public void SaveLearningPathwayDefaults_shows_validation_error_if_CompleteWithinMonths_input_is_less_than_zero()
        {
            // Given
            var model = new EditLearningPathwayDefaultsViewModel(1, "-1", "12", false, false);

            // When
            var result = controller.SaveLearningPathwayDefaults(1, model);

            // Then
            A.CallTo(
                () => courseService.UpdateLearningPathwayDefaultsForCourse(
                    1,
                    -1,
                    12,
                    false,
                    false
                )
            ).MustNotHaveHappened();
            result.Should().BeViewResult().ModelAs<EditLearningPathwayDefaultsViewModel>();
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [Test]
        public void
            SaveLearningPathwayDefaults_shows_validation_error_if_CompleteWithinMonths_input_is_greater_than_48()
        {
            // Given
            var model = new EditLearningPathwayDefaultsViewModel(1, "49", "12", false, false);

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
        public void SaveLearningPathwayDefaults_shows_validation_error_if_ValidityMonths_input_is_less_than_zero()
        {
            // Given
            var model = new EditLearningPathwayDefaultsViewModel(1, "12", "-1", false, false);

            // When
            var result = controller.SaveLearningPathwayDefaults(1, model);

            // Then
            A.CallTo(
                () => courseService.UpdateLearningPathwayDefaultsForCourse(
                    1,
                    12,
                    -1,
                    false,
                    false
                )
            ).MustNotHaveHappened();
            result.Should().BeViewResult().ModelAs<EditLearningPathwayDefaultsViewModel>();
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [Test]
        public void SaveLearningPathwayDefaults_shows_validation_errors_if_ValidityMonths_input_is_greater_than_48()
        {
            // Given
            var model = new EditLearningPathwayDefaultsViewModel(1, "12", "49", false, false);

            // When
            var result = controller.SaveLearningPathwayDefaults(1, model);

            // Then
            A.CallTo(
                () => courseService.UpdateLearningPathwayDefaultsForCourse(
                    1,
                    12,
                    49,
                    false,
                    false
                )
            ).MustNotHaveHappened();
            result.Should().BeViewResult().ModelAs<EditLearningPathwayDefaultsViewModel>();
            Assert.IsFalse(controller.ModelState.IsValid);
        }
    }
}
