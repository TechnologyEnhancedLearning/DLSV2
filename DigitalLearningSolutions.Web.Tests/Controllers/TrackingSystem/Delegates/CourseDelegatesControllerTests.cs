namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class CourseDelegatesControllerTests
    {
        private CourseDelegatesController controller = null!;
        private ICourseDelegatesService courseDelegatesService = null!;
        private ICourseService courseService = null!;

        [SetUp]
        public void SetUp()
        {
            courseDelegatesService = A.Fake<ICourseDelegatesService>();
            courseService = A.Fake<ICourseService>();
            controller = new CourseDelegatesController(courseDelegatesService, courseService)
                .WithDefaultContext()
                .WithMockUser(true);
        }

        [Test]
        public void DelegateProgress_returns_NotFound_when_requested_record_does_not_exist()
        {
            // Given
            A.CallTo(() => courseService.GetDelegateCourseProgress(A<int>._, A<int>._)).Returns(null);

            // When
            var result = controller.DelegateProgress(1);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void DelegateProgress_returns_NotFound_when_requested_record_delegate_is_at_different_centre()
        {
            // Given
            var delegateCourseInfo = new DelegateCourseInfo
            {
                DelegateCentreId = 1
            };
            A.CallTo(() => courseService.GetDelegateCourseProgress(A<int>._, A<int>._))
                .Returns(
                    new DelegateCourseDetails(
                        delegateCourseInfo,
                        new List<CustomPromptWithAnswer>(),
                        new AttemptStats(0, 0)
                    )
                );

            // When
            var result = controller.DelegateProgress(1);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void
            DelegateProgress_returns_NotFound_when_requested_record_course_is_at_different_centre_and_not_an_all_centre_course()
        {
            // Given
            var delegateCourseInfo = new DelegateCourseInfo
            {
                DelegateCentreId = 2,
                CustomisationCentreId = 1,
                AllCentresCourse = false
            };
            A.CallTo(() => courseService.GetDelegateCourseProgress(A<int>._, A<int>._))
                .Returns(
                    new DelegateCourseDetails(
                        delegateCourseInfo,
                        new List<CustomPromptWithAnswer>(),
                        new AttemptStats(0, 0)
                    )
                );

            // When
            var result = controller.DelegateProgress(1);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void DelegateProgress_returns_NotFound_when_requested_record_course_category_does_not_match()
        {
            // Given
            var delegateCourseInfo = new DelegateCourseInfo
            {
                DelegateCentreId = 2,
                CustomisationCentreId = 2,
                AllCentresCourse = false,
                CourseCategoryId = 3
            };
            A.CallTo(() => courseService.GetDelegateCourseProgress(A<int>._, A<int>._))
                .Returns(
                    new DelegateCourseDetails(
                        delegateCourseInfo,
                        new List<CustomPromptWithAnswer>(),
                        new AttemptStats(0, 0)
                    )
                );

            // When
            var result = controller.WithMockUser(true, adminCategoryId: 1).DelegateProgress(1);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void DelegateProgress_returns_DelegateProgress_page_when_requested_record_is_valid_at_centre()
        {
            // Given
            var delegateCourseInfo = new DelegateCourseInfo
            {
                DelegateCentreId = 2,
                CustomisationCentreId = 2,
                AllCentresCourse = false,
                CourseCategoryId = 1
            };
            A.CallTo(() => courseService.GetDelegateCourseProgress(A<int>._, A<int>._))
                .Returns(
                    new DelegateCourseDetails(
                        delegateCourseInfo,
                        new List<CustomPromptWithAnswer>(),
                        new AttemptStats(0, 0)
                    )
                );

            // When
            var result = controller.WithMockUser(true, adminCategoryId: 1).DelegateProgress(1);

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<DelegateProgressViewModel>();
        }

        [Test]
        public void
            DelegateProgress_returns_DelegateProgress_page_when_requested_record_is_valid_and_course_is_all_centre_course()
        {
            // Given
            var delegateCourseInfo = new DelegateCourseInfo
            {
                DelegateCentreId = 2,
                CustomisationCentreId = 3,
                AllCentresCourse = true,
                CourseCategoryId = 1
            };
            A.CallTo(() => courseService.GetDelegateCourseProgress(A<int>._, A<int>._))
                .Returns(
                    new DelegateCourseDetails(
                        delegateCourseInfo,
                        new List<CustomPromptWithAnswer>(),
                        new AttemptStats(0, 0)
                    )
                );

            // When
            var result = controller.WithMockUser(true, adminCategoryId: 1).DelegateProgress(1);

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<DelegateProgressViewModel>();
        }
    }
}
