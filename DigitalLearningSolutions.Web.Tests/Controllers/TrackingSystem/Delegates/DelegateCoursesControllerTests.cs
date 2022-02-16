namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateCourses;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public class DelegateCoursesControllerTests
    {
        private readonly List<ApplicationDetails> applicationOptions = new List<ApplicationDetails>
        {
            new ApplicationDetails
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

        private readonly List<CourseStatisticsWithAdminFieldResponseCounts> courses = new List<CourseStatisticsWithAdminFieldResponseCounts>
        {
            new CourseStatisticsWithAdminFieldResponseCounts
            {
                ApplicationName = "Course",
                CustomisationName = "Customisation",
                Active = true,
                CourseTopic = "Topic 1",
                CategoryName = "Category 1",
                HideInLearnerPortal = true,
                DelegateCount = 1,
                CompletedCount = 1,
            },
        };

        private readonly List<Category> categories = new List<Category>
        {
            new Category { CategoryName = "Category 1" },
            new Category { CategoryName = "Category 2" },
        };

        private readonly List<Topic> topics = new List<Topic>
        {
            new Topic { CourseTopic = "Topic 1" },
            new Topic { CourseTopic = "Topic 2" },
        };

        private DelegateCoursesController controller = null!;
        private DelegateCoursesController controllerWithCookies = null!;
        private ICourseCategoriesDataService courseCategoryDataService = null!;
        private ICourseService courseService = null!;
        private ICourseTopicsDataService courseTopicsDataService = null!;
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;

        [SetUp]
        public void Setup()
        {
            courseCategoryDataService = A.Fake<ICourseCategoriesDataService>();
            courseTopicsDataService = A.Fake<ICourseTopicsDataService>();
            courseService = A.Fake<ICourseService>();

            A.CallTo(() => courseService.GetCentreSpecificCourseStatisticsWithAdminFieldResponseCounts(A<int>._, A<int>._)).Returns(courses);
            A.CallTo(() => courseCategoryDataService.GetCategoriesForCentreAndCentrallyManagedCourses(A<int>._))
                .Returns(categories);
            A.CallTo(() => courseTopicsDataService.GetCourseTopicsAvailableAtCentre(A<int>._)).Returns(topics);
            A.CallTo(
                () => courseService.GetApplicationOptionsAlphabeticalListForCentre(A<int>._, A<int?>._, A<int?>._)
            ).Returns(applicationOptions);

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();

            controller = new DelegateCoursesController(
                courseService,
                courseCategoryDataService,
                courseTopicsDataService
            )
            .WithDefaultContext()
            .WithMockUser(true, 101)
            .WithMockTempData();

            const string cookieName = "DelegateCoursesFilter";
            const string cookieValue = "Status|Active|false";

            controllerWithCookies = new DelegateCoursesController(
                courseService,
                courseCategoryDataService,
                courseTopicsDataService
            )
            .WithMockHttpContext(httpRequest, cookieName, cookieValue, httpResponse)
            .WithMockUser(true, 101)
            .WithMockTempData();
        }

        [Test]
        public void Index_with_no_query_parameters_uses_cookie_value_for_filterBy()
        {
            // When
            var result = controllerWithCookies.Index();

            // Then
            result.As<ViewResult>().Model.As<DelegateCoursesViewModel>().FilterBy.Should()
                .Be("Status|Active|false");
        }

        [Test]
        public void Index_with_query_parameters_uses_query_parameter_value_for_filterBy()
        {
            // Given
            const string filterBy = "Status|HideInLearnerPortal|true";

            A.CallTo(() => httpRequest.Query.ContainsKey("filterBy")).Returns(true);

            // When
            var result = controllerWithCookies.Index(filterBy: filterBy);

            // Then
            result.As<ViewResult>().Model.As<DelegateCoursesViewModel>().FilterBy.Should()
                .Be(filterBy);
        }

        [Test]
        public void Index_with_CLEAR_filterBy_query_parameter_removes_cookie()
        {
            // Given
            const string filterBy = "CLEAR";

            // When
            var result = controllerWithCookies.Index(filterBy: filterBy);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => httpResponse.Cookies.Delete("DelegateCoursesFilter")).MustHaveHappened();
                result.As<ViewResult>().Model.As<DelegateCoursesViewModel>().FilterBy.Should()
                    .BeNull();
            }
        }

        [Test]
        public void Index_with_null_filterBy_and_new_filter_query_parameter_adds_new_cookie_value()
        {
            // Given
            const string? filterBy = null;
            const string newFilterValue = "Status|HideInLearnerPortal|true";

            A.CallTo(() => httpRequest.Query.ContainsKey("filterBy")).Returns(true);

            // When
            var result = controllerWithCookies.Index(filterBy: filterBy, filterValue: newFilterValue);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => httpResponse.Cookies.Append("DelegateCoursesFilter", newFilterValue, A<CookieOptions>._))
                    .MustHaveHappened();
                result.As<ViewResult>().Model.As<DelegateCoursesViewModel>().FilterBy.Should()
                    .Be(newFilterValue);
            }
        }

        [Test]
        public void Index_with_CLEAR_filterBy_and_new_filter_query_parameter_sets_new_cookie_value()
        {
            // Given
            const string filterBy = "CLEAR";
            const string newFilterValue = "Status|HideInLearnerPortal|true";

            // When
            var result = controllerWithCookies.Index(filterBy: filterBy, filterValue: newFilterValue);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => httpResponse.Cookies.Append("DelegateCoursesFilter", newFilterValue, A<CookieOptions>._))
                    .MustHaveHappened();
                result.As<ViewResult>().Model.As<DelegateCoursesViewModel>().FilterBy.Should()
                    .Be(newFilterValue);
            }
        }

        [Test]
        public void Index_with_no_filtering_should_default_to_Active_courses()
        {
            // When
            var result = controller.Index();

            // Then
            result.As<ViewResult>().Model.As<DelegateCoursesViewModel>().FilterBy.Should()
                .Be("Status|Active|true");
        }
    }
}
