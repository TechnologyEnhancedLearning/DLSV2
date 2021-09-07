namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    internal class CourseContentControllerTests
    {
        private readonly ICourseDataService courseDataService = A.Fake<ICourseDataService>();
        private readonly ISectionService sectionService = A.Fake<ISectionService>();
        private CourseContentController controller = null!;

        [SetUp]
        public void Setup()
        {
            controller = new CourseContentController(courseDataService, sectionService)
                .WithDefaultContext()
                .WithMockUser(true, 101);
        }

        [Test]
        public void Index_returns_NotFound_when_no_appropriate_course_found()
        {
            // Given
            A.CallTo(() => courseDataService.GetCourseDetailsForAdminCategoryId(A<int>._, A<int>._, A<int>._)).Returns(null);

            // When
            var result = controller.Index(1);

            // Then
            result.Should().BeNotFoundResult();
        }

        [Test]
        public void Index_returns_Index_page_when_appropriate_course_found()
        {
            // Given
            A.CallTo(() => courseDataService.GetCourseDetailsForAdminCategoryId(A<int>._, A<int>._, A<int>._))
                .Returns(CourseDetailsTestHelper.GetDefaultCourseDetails());
            A.CallTo(() => sectionService.GetSectionsAndTutorialsForCustomisation(A<int>._, A<int>._))
                .Returns(new List<Section>());

            // When
            var result = controller.Index(1);

            // Then
            result.Should().BeViewResult().WithDefaultViewName().ModelAs<CourseContentViewModel>();
        }
    }
}
