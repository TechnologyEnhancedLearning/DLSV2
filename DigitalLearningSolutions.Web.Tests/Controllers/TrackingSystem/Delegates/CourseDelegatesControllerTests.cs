namespace DigitalLearningSolutions.Web.Tests.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Exceptions;
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
        private const int UserCentreId = 3;
        private CourseDelegatesController controller = null!;
        private ICourseDelegatesService courseDelegatesService = null!;

        [SetUp]
        public void SetUp()
        {
            courseDelegatesService = A.Fake<ICourseDelegatesService>();

            controller = new CourseDelegatesController(courseDelegatesService).WithDefaultContext()
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
