namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NUnit.Framework;

    public class CourseDelegatesControllerTests
    {
        private const int UserCentreId = 3;
        private CourseDelegatesController controller = null!;
        private ICourseDelegatesDownloadFileService courseDelegatesDownloadFileService = null!;
        private ICourseDelegatesService courseDelegatesService = null!;

        [SetUp]
        public void SetUp()
        {
            courseDelegatesService = A.Fake<ICourseDelegatesService>();
            courseDelegatesDownloadFileService = A.Fake<ICourseDelegatesDownloadFileService>();

            controller = new CourseDelegatesController(courseDelegatesService, courseDelegatesDownloadFileService)
                .WithDefaultContext()
                .WithMockUser(true, UserCentreId);
        }

        [Test]
        public void Index_shows_index_page_when_no_customisationId_supplied()
        {
            // Given
            var course = new Course { CustomisationId = 1, Active = true };
            A.CallTo(() => courseDelegatesService.GetCoursesAndCourseDelegatesForCentre(UserCentreId, null, null))
                .Returns(new CourseDelegatesData(1, new List<Course> { course }, new List<CourseDelegate>()));

            // When
            var result = controller.Index();

            // Then
            result.Should().BeViewResult().WithDefaultViewName();
        }

        [Test]
        public void Index_returns_Not_Found_when_service_returns_null()
        {
            // Given
            A.CallTo(() => courseDelegatesService.GetCoursesAndCourseDelegatesForCentre(UserCentreId, null, 2))
                .Throws<CourseNotFoundException>();

            // When
            var result = controller.Index(2);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Index_should_default_to_Active_filter_and_return_active_course_delegates()
        {
            // Given
            const int customisationId = 2;
            var course = new Course { CustomisationId = customisationId, Active = true };
            var courseDelegate = Builder<CourseDelegate>
                .CreateListOfSize(2)
                .TheFirst(1)
                .With(c => c.Active = false)
                .TheLast(1)
                .With(c => c.Active = true)
                .Build();
            A.CallTo(
                    () => courseDelegatesService.GetCoursesAndCourseDelegatesForCentre(
                        UserCentreId,
                        null,
                        customisationId
                    )
                )
                .Returns(new CourseDelegatesData(customisationId, new List<Course> { course }, courseDelegate));

            var httpRequest = A.Fake<HttpRequest>();
            var httpResponse = A.Fake<HttpResponse>();
            const string cookieName = "CourseDelegatesFilter";
            const string cookieValue = "AccountStatus|Active|true";

            var courseDelegatesController = new CourseDelegatesController(
                    courseDelegatesService,
                    courseDelegatesDownloadFileService
                )
                .WithMockHttpContext(httpRequest, cookieName, cookieValue, httpResponse)
                .WithMockUser(true, UserCentreId)
                .WithMockTempData();

            A.CallTo(() => httpRequest.Cookies).Returns(A.Fake<IRequestCookieCollection>());

            // When
            var result = courseDelegatesController.Index(customisationId);

            // Then
            using (new AssertionScope())
            {
                result.As<ViewResult>().Model.As<CourseDelegatesViewModel>().CourseDetails!.FilterBy.Should()
                    .Be("AccountStatus|Active|true");
                result.As<ViewResult>().Model.As<CourseDelegatesViewModel>().CourseDetails!.Delegates.Should()
                    .HaveCount(1);

                A.CallTo(
                        () => courseDelegatesService.GetCoursesAndCourseDelegatesForCentre(
                            UserCentreId,
                            null,
                            customisationId
                        )
                    )
                    .MustHaveHappened();
            }
        }

        [Test]
        public void AllCourseDelegates_gets_courses_for_user_details_only()
        {
            // Given
            A.CallTo(() => courseDelegatesService.GetCourseDelegatesForCentre(2, UserCentreId))
                .Returns(new List<CourseDelegate>());

            // When
            controller.AllCourseDelegates(2);

            // Then
            A.CallTo(() => courseDelegatesService.GetCourseDelegatesForCentre(2, UserCentreId))
                .MustHaveHappened();
        }
    }
}
