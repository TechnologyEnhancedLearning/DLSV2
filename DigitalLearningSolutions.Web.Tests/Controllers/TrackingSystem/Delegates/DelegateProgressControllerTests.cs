namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class DelegateProgressControllerTests
    {
        private DelegateProgressController controller = null!;
        private ICourseService courseService = null!;

        [SetUp]
        public void SetUp()
        {
            courseService = A.Fake<ICourseService>();
            controller = new DelegateProgressController(courseService)
                .WithDefaultContext()
                .WithMockUser(true);
        }

        [Test]
        public void Index_returns_NotFound_when_requested_record_does_not_exist()
        {
            // Given
            A.CallTo(() => courseService.GetDelegateCourseProgress(A<int>._, A<int>._)).Returns(null);

            // When
            var result = controller.Index(1, DelegateProgressAccessRoute.ViewDelegate);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Index_returns_NotFound_when_requested_record_delegate_is_at_different_centre()
        {
            // Given
            var delegateCourseInfo = new DelegateCourseInfo
            {
                DelegateCentreId = 1,
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
            var result = controller.Index(1, DelegateProgressAccessRoute.ViewDelegate);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void
            Index_returns_NotFound_when_requested_record_course_is_at_different_centre_and_not_an_all_centre_course()
        {
            // Given
            var delegateCourseInfo = new DelegateCourseInfo
            {
                DelegateCentreId = 2,
                CustomisationCentreId = 1,
                AllCentresCourse = false,
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
            var result = controller.Index(1, DelegateProgressAccessRoute.ViewDelegate);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Index_returns_NotFound_when_requested_record_course_category_does_not_match()
        {
            // Given
            var delegateCourseInfo = new DelegateCourseInfo
            {
                DelegateCentreId = 2,
                CustomisationCentreId = 2,
                AllCentresCourse = false,
                CourseCategoryId = 3,
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
            var result = controller.WithMockUser(true, adminCategoryId: 1)
                .Index(1, DelegateProgressAccessRoute.ViewDelegate);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Index_returns_DelegateProgress_page_when_requested_record_is_valid_at_centre()
        {
            // Given
            var delegateCourseInfo = new DelegateCourseInfo
            {
                DelegateCentreId = 2,
                CustomisationCentreId = 2,
                AllCentresCourse = false,
                CourseCategoryId = 1,
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
            var result = controller.WithMockUser(true, adminCategoryId: 1)
                .Index(1, DelegateProgressAccessRoute.ViewDelegate);

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<DelegateProgressViewModel>();
        }

        [Test]
        public void
            Index_returns_DelegateProgress_page_when_requested_record_is_valid_and_course_is_all_centre_course()
        {
            // Given
            var delegateCourseInfo = new DelegateCourseInfo
            {
                DelegateCentreId = 2,
                CustomisationCentreId = 3,
                AllCentresCourse = true,
                CourseCategoryId = 1,
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
            var result = controller.WithMockUser(true, adminCategoryId: 1)
                .Index(1, DelegateProgressAccessRoute.ViewDelegate);

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<DelegateProgressViewModel>();
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void Index_returns_DelegateProgress_page_when_AccessedVia_route_parameters_are_valid(int accessedViaId)
        {
            // Given
            var accessedVia = Enumeration.FromId<DelegateProgressAccessRoute>(accessedViaId);
            var delegateCourseInfo = new DelegateCourseInfo
            {
                DelegateCentreId = 2,
                CustomisationCentreId = 2,
                AllCentresCourse = false,
                CourseCategoryId = 1,
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
            var result = controller.WithMockUser(true, adminCategoryId: 1).Index(1, accessedVia);

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<DelegateProgressViewModel>();
        }
    }
}
