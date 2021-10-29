namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.CourseSetup
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using NUnit.Framework;
    using System.Collections.Generic;

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
        public void Edit_Course_Options_page_opens_up_with_course_options()
        {
            // Given
            const int customisationId = 1;
            A.CallTo(
                () => courseService.VerifyAdminUserCanAccessCourse(
                    customisationId,
                    A<int>._,
                    A<int>._
                )
            ).Returns(true);

            A.CallTo(
                () => courseService.GetCourseOptionsForAdminCategoryId(
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
            var courseOptions = new CourseOptions()
            {
                Active = true,
                DiagObjSelect = true,
                HideInLearnerPortal = true,
                SelfRegister = true
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

        [Test]
        public void Edit_Course_Options_redirects_to_not_found_page_when_admin_user_cannot_access_course()
        {
            // Given
            const int centreId = 101;
            var context = new ActionExecutingContext(
                new ActionContext(
                    new DefaultHttpContext(),
                    new RouteData(new RouteValueDictionary() { { "customisationId", "1" }}),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new ManageCourseController(A.Fake<ICourseService>()).WithDefaultContext().
                    WithMockUser(true, centreId: centreId)
            );

            // When
            new VerifyAdminUserCanAccessCourse(A.Fake<ICourseService>()).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNotFoundResult();
        }
    }
}
