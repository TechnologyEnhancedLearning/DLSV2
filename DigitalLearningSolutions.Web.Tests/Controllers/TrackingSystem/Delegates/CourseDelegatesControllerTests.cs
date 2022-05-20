namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
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
        private ICourseAdminFieldsService courseAdminFieldsService = null!;
        private ICourseDelegatesDownloadFileService courseDelegatesDownloadFileService = null!;
        private ICourseDelegatesService courseDelegatesService = null!;
        private ISearchSortFilterPaginateService searchSortFilterPaginateService = null!;

        [SetUp]
        public void SetUp()
        {
            courseAdminFieldsService = A.Fake<ICourseAdminFieldsService>();
            courseDelegatesService = A.Fake<ICourseDelegatesService>();
            courseDelegatesDownloadFileService = A.Fake<ICourseDelegatesDownloadFileService>();
            searchSortFilterPaginateService = A.Fake<ISearchSortFilterPaginateService>();

            controller = new CourseDelegatesController(
                    courseAdminFieldsService,
                    courseDelegatesService,
                    courseDelegatesDownloadFileService,
                    searchSortFilterPaginateService
                )
                .WithDefaultContext()
                .WithMockUser(true, UserCentreId);
        }

        [Test]
        public void Index_shows_index_page_when_no_customisationId_supplied()
        {
            // Given
            var course = new Course { CustomisationId = 1, Active = true };
            A.CallTo(() => courseDelegatesService.GetCoursesAndCourseDelegatesForCentre(UserCentreId, null, null))
                .Returns(
                    new CourseDelegatesData(
                        1,
                        new List<Course> { course },
                        new List<CourseDelegate>(),
                        new List<CourseAdminField>()
                    )
                );

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
                .Throws<CourseAccessDeniedException>();

            // When
            var result = controller.Index(2);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Index_should_default_to_Active_filter()
        {
            // Given
            const int customisationId = 2;
            var course = new Course { CustomisationId = customisationId, Active = true };
            var courseDelegate = Builder<CourseDelegate>
                .CreateListOfSize(2)
                .TheFirst(1)
                .With(c => c.IsDelegateActive = false)
                .TheLast(1)
                .With(c => c.IsDelegateActive = true)
                .Build();
            A.CallTo(
                    () => courseDelegatesService.GetCoursesAndCourseDelegatesForCentre(
                        UserCentreId,
                        null,
                        customisationId
                    )
                )
                .Returns(
                    new CourseDelegatesData(
                        customisationId,
                        new List<Course> { course },
                        courseDelegate,
                        new List<CourseAdminField>()
                    )
                );

            var httpRequest = A.Fake<HttpRequest>();
            var httpResponse = A.Fake<HttpResponse>();
            const string cookieName = "CourseDelegatesFilter";
            const string cookieValue = "AccountStatus|IsDelegateActive|true";

            var courseDelegatesController = new CourseDelegatesController(
                    courseAdminFieldsService,
                    courseDelegatesService,
                    courseDelegatesDownloadFileService,
                    searchSortFilterPaginateService
                )
                .WithMockHttpContext(httpRequest, cookieName, cookieValue, httpResponse)
                .WithMockUser(true, UserCentreId)
                .WithMockTempData();

            A.CallTo(() => httpRequest.Cookies).Returns(A.Fake<IRequestCookieCollection>());
            SearchSortFilterAndPaginateTestHelper
                .GivenACallToSearchSortFilterPaginateServiceReturnsResult<CourseDelegate>(
                    searchSortFilterPaginateService
                );

            // When
            var result = courseDelegatesController.Index(customisationId);

            // Then
            using (new AssertionScope())
            {
                result.As<ViewResult>().Model.As<CourseDelegatesViewModel>().CourseDetails!.ExistingFilterString.Should()
                    .Be("AccountStatus|IsDelegateActive|true");

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
