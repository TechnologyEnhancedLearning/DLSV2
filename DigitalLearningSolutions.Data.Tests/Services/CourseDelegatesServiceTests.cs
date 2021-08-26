namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class CourseDelegatesServiceTests
    {
        private ICourseDataService courseDataService = null!;
        private ICourseDelegatesDataService courseDelegatesDataService = null!;
        private ICourseDelegatesService courseDelegatesService = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void Setup()
        {
            courseDataService = A.Fake<ICourseDataService>();
            courseDelegatesDataService = A.Fake<ICourseDelegatesDataService>();
            userDataService = A.Fake<IUserDataService>();

            courseDelegatesService = new CourseDelegatesService(
                courseDataService,
                userDataService,
                courseDelegatesDataService
            );
        }

        [Test]
        public void GetCoursesAndCourseDelegatesForCentre_expected_values()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            A.CallTo(() => userDataService.GetAdminUserById(adminUser.Id)).Returns(adminUser);
            A.CallTo(() => courseDataService.GetCoursesAtCentreForCategoryId(adminUser.CentreId, adminUser.CategoryId))
                .Returns(new List<Course> { new Course { CustomisationId = 1 } });
            A.CallTo(() => courseDelegatesDataService.GetDelegatesOnCourse(A<int>._, A<int>._))
                .Returns(new List<CourseDelegate> { new CourseDelegate() });

            // When
            var result = courseDelegatesService.GetCoursesAndCourseDelegatesForCentre(
                adminUser.CentreId,
                adminUser.Id,
                null
            );

            // Then
            using (new AssertionScope())
            {
                result.Courses.Should().HaveCount(1);
                result.Delegates.Should().HaveCount(1);
                result.CustomisationId.Should().Be(1);
            }
        }

        [Test]
        public void GetCoursesAndCourseDelegatesForCentre_contains_empty_lists_with_no_courses_in_category()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            A.CallTo(() => userDataService.GetAdminUserById(A<int>._)).Returns(adminUser);
            A.CallTo(() => courseDataService.GetCoursesAtCentreForCategoryId(2, 7)).Returns(new List<Course>());

            // When
            var result = courseDelegatesService.GetCoursesAndCourseDelegatesForCentre(2, 7, null);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => courseDelegatesDataService.GetDelegatesOnCourse(A<int>._, A<int>._))
                    .MustNotHaveHappened();
                result.Courses.Should().BeEmpty();
                result.Delegates.Should().BeEmpty();
                result.CustomisationId.Should().BeNull();
            }
        }
    }
}
