namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class CourseDelegatesControllerTests
    {
        private const int UserCentreId = 3;
        private const int selfAssessmentId = 3;
        private ActivityDelegatesController controller = null!;
        private ICourseDelegatesDownloadFileService courseDelegatesDownloadFileService = null!;
        private ICourseDelegatesService courseDelegatesService = null!;
        private IPaginateService paginateService = null!;
        private IConfiguration configuration = null!;
        private ISelfAssessmentService selfAssessmentDelegatesService = null!;
        private ICourseService courseService = null!;
        private IDelegateActivityDownloadFileService delegateActivityDownloadFileService = null!;
        private IUserService userService = null!;
        private ICourseAdminFieldsService courseAdminFieldsService = null!;

        [SetUp]
        public void SetUp()
        {
            courseDelegatesService = A.Fake<ICourseDelegatesService>();
            courseDelegatesDownloadFileService = A.Fake<ICourseDelegatesDownloadFileService>();
            paginateService = A.Fake<IPaginateService>();
            configuration = A.Fake<IConfiguration>();
            selfAssessmentDelegatesService = A.Fake<ISelfAssessmentService>();
            courseService = A.Fake<ICourseService>();
            userService = A.Fake<IUserService>();
            courseAdminFieldsService = A.Fake<ICourseAdminFieldsService>();

            controller = new ActivityDelegatesController(
                    courseDelegatesService,
                    courseDelegatesDownloadFileService,
                    paginateService,
                    configuration,
                    selfAssessmentDelegatesService,
                    courseService,
                    delegateActivityDownloadFileService,
                    userService,
                    courseAdminFieldsService
                )
                .WithDefaultContext()
                 .WithMockHttpContextSession()
                .WithMockUser(true, UserCentreId)
                .WithMockTempData()
                .WithMockServices();
        }

        [Test]
        public void Index_shows_index_page_when_no_customisationId_supplied()
        {
            // Given
            var course = new Course { CustomisationId = 1, Active = true };

            A.CallTo(() => courseDelegatesService.GetCoursesAndCourseDelegatesPerPageForCentre("", 0, 10, "SearchableName", "Ascending",
                1, UserCentreId, null, true, null, null, null, null, null, null))
                .Returns((new CourseDelegatesData(
                        1,
                        new List<Course> { course },
                        new List<CourseDelegate>(),
                        new List<CourseAdminField>()
                    ), 1)
                );

            // When
            var result = controller.Index();

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Index_shows_index_page_when_no_selfAssessmentId_supplied()
        {
            // Given
            var selfAssessmentDelegate = new SelfAssessmentDelegate(6, "Lname");

            A.CallTo(() => selfAssessmentDelegatesService.GetSelfAssessmentDelegatesPerPage("", 0, 10, "SearchableName", "Ascending",
                6, UserCentreId, null, null, null, null, null))
                .Returns((new SelfAssessmentDelegatesData(
                        new List<SelfAssessmentDelegate> { selfAssessmentDelegate }
                    ), 1)
                );

            // When
            var result = controller.Index();

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void CourseDelegates_Index_returns_Not_Found_when_service_returns_null()
        {
            // Given
            var course = new Course { CustomisationId = 1, Active = true };
            A.CallTo(() => courseDelegatesService.GetCoursesAndCourseDelegatesPerPageForCentre("", 0, 10, "SearchableName", "Ascending",
                2, UserCentreId, null, true, null, null, null, null, null, null))
                .Returns((new CourseDelegatesData(
                        2,
                        new List<Course> { course },
                        new List<CourseDelegate>(),
                        new List<CourseAdminField>()
                    ), 0)
                );

            // When
            var result = controller.Index(2, selfAssessmentId);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void SelfAssessmentDelegates_Index_returns_ViewResult_with_EmptyDelegates_when_service_returns_null()
        {
            // Given
            var selfAssessmentDelegate = new SelfAssessmentDelegate(6, "Lname");

            A.CallTo(() => selfAssessmentDelegatesService.GetSelfAssessmentDelegatesPerPage("", 0, 10, "SearchableName", "Ascending",
                10, UserCentreId, null, null, null, null, null))
                .Returns((new SelfAssessmentDelegatesData(
                        new List<SelfAssessmentDelegate> { selfAssessmentDelegate }
                    ), 0)
                );

            // When
            var result = controller.Index(null, 10);

            // Then
            result.Should().BeRedirectToActionResult();
        }

        [Test]
        public void Index_should_default_to_Active_filter()
        {
            // Given
            const int customisationId = 2;
            var searchString = string.Empty;
            var sortBy = "SearchableName";
            var sortDirection = "Ascending";
            var course = new Course { CustomisationId = customisationId, Active = true };
            var courseDelegate = Builder<CourseDelegate>
                .CreateListOfSize(2)
                .TheFirst(1)
                .With(c => c.IsDelegateActive = false)
                .TheLast(1)
                .With(c => c.IsDelegateActive = true)
                .Build();
            A.CallTo(
                    () => courseDelegatesService.GetCoursesAndCourseDelegatesPerPageForCentre("", 0, 10, "SearchableName", "Ascending",
                customisationId, UserCentreId, null, true, null, null, null, null, null, null)

                )
                .Returns((
                    new CourseDelegatesData(
                        customisationId,
                        new List<Course> { course },
                        courseDelegate,
                        new List<CourseAdminField>()
                    ), 1)
                );

            var httpRequest = A.Fake<HttpRequest>();
            var httpResponse = A.Fake<HttpResponse>();
            const string cookieName = "CourseDelegatesFilter";
            const string cookieValue = "AccountStatus|IsDelegateActive|true";

            var courseDelegatesController = new ActivityDelegatesController(
                    courseDelegatesService,
                    courseDelegatesDownloadFileService,
                    paginateService,
                    configuration,
                    selfAssessmentDelegatesService,
                    courseService,
                    delegateActivityDownloadFileService,
                    userService,
                    courseAdminFieldsService
                )
                .WithMockHttpContext(httpRequest, cookieName, cookieValue, httpResponse)
                .WithMockUser(true, UserCentreId)
                .WithMockTempData();

            A.CallTo(() => httpRequest.Cookies).Returns(A.Fake<IRequestCookieCollection>());
            SearchSortFilterAndPaginateTestHelper
                .GivenACallToPaginateServiceReturnsResult<CourseDelegate>(
                    paginateService, courseDelegate.Count, searchString, sortBy, sortDirection
                );

            // When
            var result = courseDelegatesController.Index(customisationId, selfAssessmentId);

            // Then
            using (new AssertionScope())
            {
                result.As<ViewResult>().Model.As<CourseDelegatesViewModel>().CourseDetails!.ExistingFilterString.Should()
                    .Be("AccountStatus|IsDelegateActive|true");

                A.CallTo(
                        () => courseDelegatesService.GetCoursesAndCourseDelegatesPerPageForCentre("", 0, 10, "SearchableName", "Ascending",
                customisationId, UserCentreId, null, true, null, null, null, null, null, null)
                    )
                    .MustHaveHappened();
            }
        }
    }
}
