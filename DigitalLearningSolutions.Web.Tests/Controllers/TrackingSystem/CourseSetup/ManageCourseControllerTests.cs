namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.CourseSetup
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    internal class ManageCourseControllerTests
    {
        private readonly ICourseDataService courseDataService = A.Fake<ICourseDataService>();
        private ManageCourseController controller = null!;

        [SetUp]
        public void Setup()
        {
            controller = new ManageCourseController(courseDataService)
                .WithDefaultContext()
                .WithMockUser(true, 101);
        }

        [Test]
        public void Index_returns_NotFound_when_no_appropriate_course_found()
        {
            // Given
            A.CallTo(() => courseDataService.GetCourseDetailsForAdminCategoryId(A<int>._, A<int>._, A<int>._))
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
            result.Should().BeNotFoundResult();
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
