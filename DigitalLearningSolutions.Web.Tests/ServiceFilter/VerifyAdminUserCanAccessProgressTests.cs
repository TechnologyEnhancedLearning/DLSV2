namespace DigitalLearningSolutions.Web.Tests.ServiceFilter
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    public class VerifyAdminUserCanAccessProgressTests
    {
        private readonly ICourseService courseService = A.Fake<ICourseService>();
        private ActionExecutingContext context = null!;

        [SetUp]
        public void Setup()
        {
            var homeController = new HomeController(A.Fake<IConfiguration>()).WithDefaultContext().WithMockTempData()
                .WithMockUser(true, 101, adminCategoryId: 1);
            context = new ActionExecutingContext(
                new ActionContext(
                    new DefaultHttpContext(),
                    new RouteData(new RouteValueDictionary()),
                    new ActionDescriptor()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                homeController
            );
            context.RouteData.Values["progressId"] = 2;
        }

        [Test]
        public void Returns_NotFound_if_service_returns_null()
        {
            // Given
            A.CallTo(() => courseService.GetDelegateCourseProgress(2, 101))
                .Returns(null);

            // When
            new VerifyAdminUserCanAccessProgress(courseService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNotFoundResult();
        }

        [Test]
        public void Returns_NotFound_if_delegate_is_at_different_centre()
        {
            // Given
            var delegateCourseInfo = new DelegateCourseInfo
            {
                DelegateCentreId = 1,
                CustomisationCentreId = 101,
                AllCentresCourse = false,
                CourseCategoryId = 1,
            };
            A.CallTo(() => courseService.GetDelegateCourseProgress(2, 101))
                .Returns(
                    new DelegateCourseDetails(
                        delegateCourseInfo,
                        new List<CoursePromptWithAnswer>(),
                        new AttemptStats(0, 0)
                    )
                );

            // When
            new VerifyAdminUserCanAccessProgress(courseService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNotFoundResult();
        }

        [Test]
        public void Returns_NotFound_if_course_is_at_different_centre_and_not_all_centre_course()
        {
            // Given
            var delegateCourseInfo = new DelegateCourseInfo
            {
                DelegateCentreId = 101,
                CustomisationCentreId = 1,
                AllCentresCourse = false,
                CourseCategoryId = 1,
            };
            A.CallTo(() => courseService.GetDelegateCourseProgress(2, 101))
                .Returns(
                    new DelegateCourseDetails(
                        delegateCourseInfo,
                        new List<CoursePromptWithAnswer>(),
                        new AttemptStats(0, 0)
                    )
                );

            // When
            new VerifyAdminUserCanAccessProgress(courseService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNotFoundResult();
        }

        [Test]
        public void Returns_NotFound_if_course_category_does_not_match()
        {
            // Given
            var delegateCourseInfo = new DelegateCourseInfo
            {
                DelegateCentreId = 101,
                CustomisationCentreId = 101,
                AllCentresCourse = false,
                CourseCategoryId = 3,
            };
            A.CallTo(() => courseService.GetDelegateCourseProgress(2, 101))
                .Returns(
                    new DelegateCourseDetails(
                        delegateCourseInfo,
                        new List<CoursePromptWithAnswer>(),
                        new AttemptStats(0, 0)
                    )
                );

            // When
            new VerifyAdminUserCanAccessProgress(courseService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNotFoundResult();
        }

        [Test]
        public void Does_not_return_NotFound_if_record_is_valid_at_centre()
        {
            // Given
            var delegateCourseInfo = new DelegateCourseInfo
            {
                DelegateCentreId = 101,
                CustomisationCentreId = 101,
                AllCentresCourse = false,
                CourseCategoryId = 1,
            };
            A.CallTo(() => courseService.GetDelegateCourseProgress(2, 101))
                .Returns(
                    new DelegateCourseDetails(
                        delegateCourseInfo,
                        new List<CoursePromptWithAnswer>(),
                        new AttemptStats(0, 0)
                    )
                );

            // When
            new VerifyAdminUserCanAccessProgress(courseService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNull();
        }

        [Test]
        public void Does_not_return_NotFound_if_record_is_valid_and_course_is_all_centres_course()
        {
            // Given
            var delegateCourseInfo = new DelegateCourseInfo
            {
                DelegateCentreId = 101,
                CustomisationCentreId = 3,
                AllCentresCourse = true,
                CourseCategoryId = 1,
            };
            A.CallTo(() => courseService.GetDelegateCourseProgress(2, 101))
                .Returns(
                    new DelegateCourseDetails(
                        delegateCourseInfo,
                        new List<CoursePromptWithAnswer>(),
                        new AttemptStats(0, 0)
                    )
                );

            // When
            new VerifyAdminUserCanAccessProgress(courseService).OnActionExecuting(context);

            // Then
            context.Result.Should().BeNull();
        }
    }
}
