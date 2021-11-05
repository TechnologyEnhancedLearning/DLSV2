namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

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

        private CourseSetupController controller = null!;
        private ICourseCategoriesDataService courseCategoryDataService = null!;
        private ICourseService courseService = null!;
        private ITutorialService tutorialService = null!;
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

            A.CallTo(() => courseService.GetCentreSpecificCourseStatistics(A<int>._, A<int>._)).Returns(courses);
            A.CallTo(() => courseCategoryDataService.GetCategoriesForCentreAndCentrallyManagedCourses(A<int>._))
                .Returns(categories);
            A.CallTo(() => courseTopicsDataService.GetCourseTopicsAvailableAtCentre(A<int>._)).Returns(topics);

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();
            const string cookieName = "CourseFilter";
            const string cookieValue = "Status|Active|false";

            controller = new CourseSetupController(courseService, courseCategoryDataService, courseTopicsDataService, tutorialService)
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
                    tutorialService
                )
                .WithDefaultContext()
                .WithMockUser(true);

            // When
            var result = controllerWithNoCookies.Index();

            // Then
            result.As<ViewResult>().Model.As<CourseSetupViewModel>().FilterBy.Should()
                .Be("Status|Active|true");
        }
    }
}
