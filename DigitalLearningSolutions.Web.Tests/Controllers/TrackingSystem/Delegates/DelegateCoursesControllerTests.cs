﻿namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions.AspNetCore.Mvc;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Http;
    using NUnit.Framework;

    public class DelegateCoursesControllerTests
    {
        private const string CookieName = "DelegateCoursesFilter";

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



        private DelegateCoursesController controllerWithCookies = null!;
        private ICourseDelegatesDownloadFileService courseDelegatesDownloadFileService = null!;
        private ICourseService courseService = null!;
        private HttpRequest httpRequest = null!;
        private HttpResponse httpResponse = null!;
        private IPaginateService paginateService = null!;
        private IActivityService activityService = null!;
        private ICourseCategoriesService courseCategoriesService = null!;
        private ICourseTopicsService courseTopicsService = null!;

        [SetUp]
        public void Setup()
        {
            courseService = A.Fake<ICourseService>();
            courseDelegatesDownloadFileService = A.Fake<ICourseDelegatesDownloadFileService>();
            activityService = A.Fake<IActivityService>();
            paginateService = A.Fake<IPaginateService>();
            courseCategoriesService = A.Fake<ICourseCategoriesService>();
            courseTopicsService = A.Fake<ICourseTopicsService>();
            A.CallTo(() => activityService.GetCourseCategoryNameForActivityFilter(A<int>._))
              .Returns("All");
            A.CallTo(() => courseService.GetCentreCourseDetailsWithAllCentreCourses(A<int>._, A<int?>._, A<string?>._, A<string?>._, A<string?>._, A<string>._))
                .Returns(details);
            A.CallTo(() => courseService.GetApplicationOptionsAlphabeticalListForCentre(A<int>._, A<int?>._, A<int?>._))
                .Returns(applicationOptions);

            httpRequest = A.Fake<HttpRequest>();
            httpResponse = A.Fake<HttpResponse>();

            const string cookieValue = "Status|Active|false";

            controllerWithCookies = new DelegateCoursesController(
                    courseService,
                    courseDelegatesDownloadFileService,
                    paginateService,
                    activityService,
                    courseCategoriesService,
                    courseTopicsService
                )
                .WithMockHttpContext(httpRequest, CookieName, cookieValue, httpResponse)
                .WithMockUser(true, 101)
                .WithMockTempData();
        }

        [Test]
        public void Index_calls_expected_methods_and_returns_view()
        {
            // When
            var result = controllerWithCookies.Index();

            // Then
            using (new AssertionScope())
            {
                var delegateCourses = Builder<CourseStatisticsWithAdminFieldResponseCounts>.CreateListOfSize(5).Build();
                A.CallTo(() => courseService.GetDelegateCourses(string.Empty, 1, 1, true, true, string.Empty, string.Empty, string.Empty, string.Empty)).Returns(delegateCourses);


                var delegateSelfAssessments = Builder<DelegateAssessmentStatistics>.CreateListOfSize(5).Build();
                A.CallTo(() => courseService.GetDelegateAssessments(A<string>._, A<int>._, A<string>._, A<string>._, A<int>._)).Returns(delegateSelfAssessments);
                A.CallTo(
                    () => paginateService.Paginate(A<IEnumerable<CourseStatistics>>._,
                         A<int>._,
                        A<PaginationOptions>._, A<FilterOptions>._, A<string>._, A<string>._, A<string>._
                    )
                ).MustHaveHappened();

                A.CallTo(
                        () => httpResponse.Cookies.Append(
                            CookieName,
                            A<string>._,
                            A<CookieOptions>._
                        )
                    )
                    .MustHaveHappened();
                result.Should().BeViewResult().WithDefaultViewName();
            }
        }
    }
}
