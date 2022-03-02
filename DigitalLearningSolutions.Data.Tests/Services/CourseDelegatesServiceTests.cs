namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class CourseDelegatesServiceTests
    {
        private ICourseAdminFieldsService courseAdminFieldsService = null!;
        private ICourseDataService courseDataService = null!;
        private ICourseDelegatesDataService courseDelegatesDataService = null!;
        private ICourseDelegatesService courseDelegatesService = null!;

        [SetUp]
        public void Setup()
        {
            courseAdminFieldsService = A.Fake<ICourseAdminFieldsService>();
            courseDataService = A.Fake<ICourseDataService>();
            courseDelegatesDataService = A.Fake<ICourseDelegatesDataService>();

            courseDelegatesService = new CourseDelegatesService(
                courseAdminFieldsService,
                courseDataService,
                courseDelegatesDataService
            );
        }

        [Test]
        public void GetCoursesAndCourseDelegatesForCentre_populates_course_delegates_data()
        {
            // Given
            const int centreId = 2;
            const int categoryId = 1;
            const int customisationId = 1;
            A.CallTo(() => courseDataService.GetCoursesAvailableToCentreByCategory(centreId, categoryId))
                .Returns(
                    new List<CourseAssessmentDetails>
                        { new CourseAssessmentDetails { CustomisationId = customisationId } }
                );
            A.CallTo(() => courseDelegatesDataService.GetDelegatesOnCourse(A<int>._, A<int>._))
                .Returns(new List<CourseDelegate> { new CourseDelegate() });
            A.CallTo(() => courseAdminFieldsService.GetCustomPromptsForCourse(A<int>._))
                .Returns(
                    new CourseAdminFields(
                        customisationId,
                        new List<CustomPrompt> { new CustomPrompt(1, "prompt", null, true) }
                    )
                );

            // When
            var result = courseDelegatesService.GetCoursesAndCourseDelegatesForCentre(
                centreId,
                categoryId,
                null
            );

            // Then
            using (new AssertionScope())
            {
                result.Courses.Should().HaveCount(1);
                result.Delegates.Should().HaveCount(1);
                result.CustomisationId.Should().Be(customisationId);
                result.CourseAdminFields.Should().HaveCount(1);
            }
        }

        [Test]
        public void GetCoursesAndCourseDelegatesForCentre_contains_empty_lists_with_no_courses_in_category()
        {
            // Given
            A.CallTo(() => courseDataService.GetCoursesAvailableToCentreByCategory(2, 7))
                .Returns(new List<CourseAssessmentDetails>());

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
                result.CourseAdminFields.Should().BeEmpty();
            }
        }

        [Test]
        public void GetCoursesAndCourseDelegatesForCentre_uses_passed_in_customisation_id()
        {
            // Given
            const int customisationId = 1;
            const int centreId = 2;
            const int categoryId = 1;
            A.CallTo(() => courseDataService.GetCoursesAvailableToCentreByCategory(centreId, categoryId))
                .Returns(new List<CourseAssessmentDetails> { new CourseAssessmentDetails { CustomisationId = 1 } });
            A.CallTo(() => courseDelegatesDataService.GetDelegatesOnCourse(A<int>._, A<int>._))
                .Returns(new List<CourseDelegate> { new CourseDelegate() });
            A.CallTo(() => courseAdminFieldsService.GetCustomPromptsForCourse(A<int>._))
                .Returns(
                    new CourseAdminFields(
                        customisationId,
                        new List<CustomPrompt> { new CustomPrompt(1, "prompt", null, true) }
                    )
                );

            // When
            var result = courseDelegatesService.GetCoursesAndCourseDelegatesForCentre(
                centreId,
                categoryId,
                customisationId
            );

            // Then
            A.CallTo(() => courseDelegatesDataService.GetDelegatesOnCourse(customisationId, centreId))
                .MustHaveHappened();
            A.CallTo(() => courseAdminFieldsService.GetCustomPromptsForCourse(customisationId))
                .MustHaveHappenedOnceExactly();
            result.Should().NotBeNull();
        }

        [Test]
        public void
            GetCoursesAndCourseDelegatesForCentre_throws_exception_when_passed_in_customisation_id_does_not_have_accessible_course()
        {
            // Given
            const int customisationId = 2;
            const int centreId = 2;
            const int categoryId = 1;
            A.CallTo(() => courseDataService.GetCoursesAvailableToCentreByCategory(centreId, categoryId))
                .Returns(new List<CourseAssessmentDetails> { new CourseAssessmentDetails { CustomisationId = 1 } });

            // Then
            Assert.Throws<CourseAccessDeniedException>(
                () => courseDelegatesService.GetCoursesAndCourseDelegatesForCentre(
                    centreId,
                    categoryId,
                    customisationId
                )
            );
            A.CallTo(() => courseDelegatesDataService.GetDelegatesOnCourse(A<int>._, A<int>._))
                .MustNotHaveHappened();
            A.CallTo(() => courseAdminFieldsService.GetCustomPromptsForCourse(A<int>._))
                .MustNotHaveHappened();
        }
    }
}
