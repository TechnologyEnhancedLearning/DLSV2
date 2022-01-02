namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mime;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;
    using FluentAssertions.AspNetCore.Mvc;

    public class CourseSetupControllerTests
    {
        private readonly List<Category> categories = new List<Category>
        {
            new Category { CategoryName = "Category 1" },
            new Category { CategoryName = "Category 2" }
        };

        private readonly List<CourseStatistics> courses = new List<CourseStatistics>
        {
            new CourseStatistics
            {
                ApplicationName = "Course",
                CustomisationName = "Customisation",
                Active = true,
                CourseTopic = "Topic 1",
                CategoryName = "Category 1",
                HideInLearnerPortal = true,
                DelegateCount = 1,
                CompletedCount = 1
            }
        };

        private readonly List<Topic> topics = new List<Topic>
        {
            new Topic { CourseTopic = "Topic 1" },
            new Topic { CourseTopic = "Topic 2" }
        };

        private readonly List<ApplicationDetails> applicationOptions = new List<ApplicationDetails>
        { new ApplicationDetails
            {
                ApplicationId = 1,
                ApplicationName = "Test Name",
                CategoryName = "Test Category Name",
                CourseTopicId = 1,
                CourseTopic = "Topic",
                PLAssess = true,
                DiagAssess = true,
            },
        };

        private CourseSetupController controller = null!;
        private ICourseCategoriesDataService courseCategoryDataService = null!;
        private ICourseService courseService = null!;
        private ITutorialService tutorialService = null!;
        private ISectionService sectionService = null!;
        private ICourseTopicsDataService courseTopicsDataService = null!;
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;

        [SetUp]
        public void Setup()
        {
            courseCategoryDataService = A.Fake<ICourseCategoriesDataService>();
            courseTopicsDataService = A.Fake<ICourseTopicsDataService>();
            courseService = A.Fake<ICourseService>();
            tutorialService = A.Fake<ITutorialService>();
            sectionService = A.Fake<ISectionService>();

            A.CallTo(() => courseService.GetCentreSpecificCourseStatistics(A<int>._, A<int>._)).Returns(courses);
            A.CallTo(() => courseCategoryDataService.GetCategoriesForCentreAndCentrallyManagedCourses(A<int>._))
                .Returns(categories);
            A.CallTo(() => courseTopicsDataService.GetCourseTopicsAvailableAtCentre(A<int>._)).Returns(topics);

            A.CallTo(
                () => courseService.GetApplicationOptionsAlphabeticalListForCentre(A<int>._, A<int>._)
            ).Returns(applicationOptions);

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            const string cookieName = "CourseFilter";
            const string cookieValue = "Status|Active|false";

            controller = new CourseSetupController(courseService, courseCategoryDataService, courseTopicsDataService, tutorialService, sectionService)
                .WithMockHttpContext(httpRequest, cookieName, cookieValue, httpResponse)
                .WithMockUser(true)
                .WithMockTempData();
        }

        [Test]
        public void Index_with_no_query_parameters_uses_cookie_value_for_filterBy()
        {
            // When
            var result = controller.Index();

            // Then
            result.As<ViewResult>().Model.As<CourseSetupViewModel>().FilterBy.Should()
                .Be("Status|Active|false");
        }

        [Test]
        public void Index_with_query_parameters_uses_query_parameter_value_for_filterBy()
        {
            // Given
            const string filterBy = "Status|HideInLearnerPortal|true";
            A.CallTo(() => httpRequest.Query.ContainsKey("filterBy")).Returns(true);

            // When
            var result = controller.Index(filterBy: filterBy);

            // Then
            result.As<ViewResult>().Model.As<CourseSetupViewModel>().FilterBy.Should()
                .Be(filterBy);
        }

        [Test]
        public void Index_with_CLEAR_filterBy_query_parameter_removes_cookie()
        {
            // Given
            const string filterBy = "CLEAR";

            // When
            var result = controller.Index(filterBy: filterBy);

            // Then
            A.CallTo(() => httpResponse.Cookies.Delete("CourseFilter")).MustHaveHappened();
            result.As<ViewResult>().Model.As<CourseSetupViewModel>().FilterBy.Should()
                .BeNull();
        }

        [Test]
        public void Index_with_null_filterBy_and_new_filter_query_parameter_adds_new_cookie_value()
        {
            // Given
            const string? filterBy = null;
            const string newFilterValue = "Status|HideInLearnerPortal|true";
            A.CallTo(() => httpRequest.Query.ContainsKey("filterBy")).Returns(true);

            // When
            var result = controller.Index(filterBy: filterBy, filterValue: newFilterValue);

            // Then
            A.CallTo(() => httpResponse.Cookies.Append("CourseFilter", newFilterValue, A<CookieOptions>._))
                .MustHaveHappened();
            result.As<ViewResult>().Model.As<CourseSetupViewModel>().FilterBy.Should()
                .Be(newFilterValue);
        }

        [Test]
        public void Index_with_CLEAR_filterBy_and_new_filter_query_parameter_sets_new_cookie_value()
        {
            // Given
            const string filterBy = "CLEAR";
            const string newFilterValue = "Status|HideInLearnerPortal|true";

            // When
            var result = controller.Index(filterBy: filterBy, filterValue: newFilterValue);

            // Then
            A.CallTo(() => httpResponse.Cookies.Append("CourseFilter", newFilterValue, A<CookieOptions>._))
                .MustHaveHappened();
            result.As<ViewResult>().Model.As<CourseSetupViewModel>().FilterBy.Should()
                .Be(newFilterValue);
        }

        [Test]
        public void Index_with_no_filtering_should_default_to_Active_courses()
        {
            // Given
            var controllerWithNoCookies = new CourseSetupController(
                    courseService,
                    courseCategoryDataService,
                    courseTopicsDataService,
                    tutorialService,
                    sectionService
                )
                .WithDefaultContext()
                .WithMockUser(true);

            // When
            var result = controllerWithNoCookies.Index();

            // Then
            result.As<ViewResult>().Model.As<CourseSetupViewModel>().FilterBy.Should()
                .Be("Status|Active|true");
        }

        [Test]
        public void AddCourseNew_sets_new_temp_data()
        {
            // When
            var result = controller.AddCourseNew();

            // Then
            controller.TempData.Peek<AddNewCentreCourseData>().Should().NotBeNull();
            result.Should().BeRedirectToActionResult().WithActionName("SelectCourse");
        }

        /*[Test]
        public void SelectCourse_post_updates_temp_data_and_redirects()
        {
            var application = new ApplicationDetails
            {
                ApplicationId = 1,
                ApplicationName = "Test Name",
                CategoryName = "Test Category Name",
                CourseTopicId = 1,
                CourseTopic = "Topic",
                PLAssess = true,
                DiagAssess = true,
            };

            var expectedModel = new SelectCourseViewModel { Application = application };
            var formData = new SelectCourseFormData { ApplicationId = application.ApplicationId };
            var initialTempData = new AddNewCentreCourseData();
            controller.TempData.Set(initialTempData);

            // When
            var result = controller.SelectCourse(formData);

            // Then
            controller.TempData.Peek<AddNewCentreCourseData>()!.SelectCourseModel.Should()
                .BeEquivalentTo(expectedModel);
            result.As<ViewResult>().Model.Should().BeOfType<SetCourseDetailsViewModel>();
        }
        */

        [Test]
        public void SelectCourse_does_not_redirect_with_invalid_model()
        {
            var model = new SelectCourseViewModel { ApplicationId = 1 };
            controller.ModelState.AddModelError("ApplicationId", "Select a course");

            // When
            var result = controller.SelectCourse(model);

            // Then
            result.Should().BeViewResult().ModelAs<SelectCourseViewModel>();
            controller.ModelState["ApplicationId"].Errors[0].ErrorMessage.Should()
                .BeEquivalentTo("Select a course");
        }

        /*[Test]
        public void
            SaveCourseDetails_correctly_adds_model_error_if_customisation_name_is_not_unique()
        {
            // Given
            var formData = GetEditCourseDetailsFormData();

            A.CallTo(
                () => courseService.DoesCourseNameExistAtCentre(
                    "Name",
                    101,
                    1,
                    1
                )
            ).Returns(true);

            // When
            var result = controller.SetCourseDetails(1, formData);

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
            var formData = GetEditCourseDetailsFormData(customisationName: "");

            A.CallTo(
                () => courseService.DoesCourseNameExistAtCentre(
                    "",
                    101,
                    1,
                    1
                )
            ).Returns(true);

            // When
            var result = controller.SetCourseDetails(1, formData);

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
                .BeEquivalentTo("A course with no add on already exists");
        }

        [Test]
        public void
            SaveCourseDetails_clears_values_of_conditional_inputs_if_corresponding_checkboxes_or_radios_are_unchecked()
        {
            // Given
            var formData = GetEditCourseDetailsFormData(
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
            var result = controller.SetCourseDetails(1, formData);

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
        }*/

        private static void AssertModelStateErrorIsExpected(IActionResult result, string expectedErrorMessage)
        {
            var errorMessage = result.As<ViewResult>().ViewData.ModelState.Select(x => x.Value.Errors)
                .Where(y => y.Count > 0).ToList().First().First().ErrorMessage;
            errorMessage.Should().BeEquivalentTo(expectedErrorMessage);
        }

        private static EditCourseDetailsFormData GetEditCourseDetailsFormData(
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
            return new EditCourseDetailsFormData
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
        }
    }
}
