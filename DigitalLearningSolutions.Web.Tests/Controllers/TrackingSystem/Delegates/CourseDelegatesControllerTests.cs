namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates;
    using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
    using FakeItEasy;
    using FluentAssertions.AspNetCore.Mvc;
    using NUnit.Framework;

    public class CourseDelegatesControllerTests
    {
        private CourseDelegatesController controller = null!;
        private ICourseDelegatesService courseDelegatesService = null!;

        [SetUp]
        public void SetUp()
        {
            courseDelegatesService = A.Fake<ICourseDelegatesService>();

            controller = new CourseDelegatesController(courseDelegatesService).WithDefaultContext().WithMockUser(true);
        }

        [Test]
        public void Index_shows_index_page_when_no_customisationId_supplied()
        {
            // Given
            var course = new Course { CustomisationId = 1, Active = true };
            A.CallTo(() => courseDelegatesService.GetCoursesAndCourseDelegatesForCentre(2, null, null))
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
            A.CallTo(() => courseDelegatesService.GetCoursesAndCourseDelegatesForCentre(2, null, 2))
                .Returns(null);

            // When
            var result = controller.Index(2);

            // Then
            result.Should().BeNotFoundResult();
        }
    }
}
