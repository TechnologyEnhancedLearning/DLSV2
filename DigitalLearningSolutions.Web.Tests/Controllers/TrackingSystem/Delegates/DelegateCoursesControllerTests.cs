namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateCourses;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public class DelegateCoursesControllerTests
    {
        private readonly IEnumerable<ApplicationDetails> applicationOptions =
            Builder<ApplicationDetails>.CreateListOfSize(1).Build();

        private readonly CentreCourseDetails details = Builder<CentreCourseDetails>.CreateNew()
            .With(
                x => x.Courses = new List<CourseStatisticsWithAdminFieldResponseCounts>
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
                }
            )
            .And(x => x.Categories = new List<string> { "Category 1", "Category 2" })
            .And(x => x.Topics = new List<string> { "Topic 1", "Topic 2" })
            .Build();

        private DelegateCoursesController controller = null!;
        private DelegateCoursesController controllerWithCookies = null!;
        private ICourseService courseService = null!;
        private ICourseDelegatesDownloadFileService courseDelegatesDownloadFileService = null!;
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;

        [SetUp]
        public void Setup()
        {
            courseService = A.Fake<ICourseService>();
            courseDelegatesDownloadFileService = A.Fake<ICourseDelegatesDownloadFileService>();

            A.CallTo(() => courseService.GetDelegateCourseDetails(A<int>._, A<int?>._)).Returns(details);
            A.CallTo(
                () => courseService.GetApplicationOptionsAlphabeticalListForCentre(A<int>._, A<int?>._, A<int?>._)
            ).Returns(applicationOptions);

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();

            controller = new DelegateCoursesController(courseService, courseDelegatesDownloadFileService)
                .WithDefaultContext()
                .WithMockUser(true, 101)
                .WithMockTempData();

            const string cookieName = "DelegateCoursesFilter";
            const string cookieValue = "Status|Active|false";

            controllerWithCookies = new DelegateCoursesController(courseService, courseDelegatesDownloadFileService)
                .WithMockHttpContext(httpRequest, cookieName, cookieValue, httpResponse)
                .WithMockUser(true, 101)
                .WithMockTempData();
        }

        [Test]
        public void Index_with_no_query_parameters_uses_cookie_value_for_existingFilterString()
        {
            // When
            var result = controllerWithCookies.Index();

            // Then
            result.As<ViewResult>().Model.As<DelegateCoursesViewModel>().ExistingFilterString.Should()
                .Be("Status|Active|false");
        }

        [Test]
        public void Index_with_query_parameters_uses_query_parameter_value_for_existingFilterString()
        {
            // Given
            const string existingFilterString = "Status|HideInLearnerPortal|true";

            A.CallTo(() => httpRequest.Query.ContainsKey("existingFilterString")).Returns(true);

            // When
            var result = controllerWithCookies.Index(existingFilterString: existingFilterString);

            // Then
            result.As<ViewResult>().Model.As<DelegateCoursesViewModel>().ExistingFilterString.Should()
                .Be(existingFilterString);
        }

        [Test]
        public void Index_with_CLEAR_existingFilterString_query_parameter_removes_cookie()
        {
            // Given
            const string existingFilterString = "CLEAR";

            // When
            var result = controllerWithCookies.Index(existingFilterString: existingFilterString);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => httpResponse.Cookies.Delete("DelegateCoursesFilter")).MustHaveHappened();
                result.As<ViewResult>().Model.As<DelegateCoursesViewModel>().ExistingFilterString.Should()
                    .BeNull();
            }
        }

        [Test]
        public void Index_with_null_existingFilterString_and_new_filter_query_parameter_adds_new_cookie_value()
        {
            // Given
            const string? existingFilterString = null;
            const string newFilterValue = "Status|HideInLearnerPortal|true";

            A.CallTo(() => httpRequest.Query.ContainsKey("existingFilterString")).Returns(true);

            // When
            var result = controllerWithCookies.Index(existingFilterString: existingFilterString, newFilterToAdd: newFilterValue);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => httpResponse.Cookies.Append("DelegateCoursesFilter", newFilterValue, A<CookieOptions>._))
                    .MustHaveHappened();
                result.As<ViewResult>().Model.As<DelegateCoursesViewModel>().ExistingFilterString.Should()
                    .Be(newFilterValue);
            }
        }

        [Test]
        public void Index_with_CLEAR_existingFilterString_and_new_filter_query_parameter_sets_new_cookie_value()
        {
            // Given
            const string existingFilterString = "CLEAR";
            const string newFilterValue = "Status|HideInLearnerPortal|true";

            // When
            var result = controllerWithCookies.Index(existingFilterString: existingFilterString, newFilterToAdd: newFilterValue);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => httpResponse.Cookies.Append("DelegateCoursesFilter", newFilterValue, A<CookieOptions>._))
                    .MustHaveHappened();
                result.As<ViewResult>().Model.As<DelegateCoursesViewModel>().ExistingFilterString.Should()
                    .Be(newFilterValue);
            }
        }

        [Test]
        public void Index_with_no_filtering_should_default_to_Active_courses()
        {
            // When
            var result = controller.Index();

            // Then
            result.As<ViewResult>().Model.As<DelegateCoursesViewModel>().ExistingFilterString.Should()
                .Be("Status|Active|true");
        }
    }
}
