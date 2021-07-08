namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class CourseServiceTests
    {
        private const int CentreId = 2;
        private const int AdminCategoryId = 0;
        private ICourseDataService courseDataService = null!;
        private CourseService courseService = null!;

        [SetUp]
        public void Setup()
        {
            courseDataService = A.Fake<ICourseDataService>();
            A.CallTo(() => courseDataService.GetCourseStatisticsAtCentreForCategoryId(CentreId, AdminCategoryId))
                .Returns(GetSampleCourses());

            courseService = new CourseService(courseDataService);
        }

        [Test]
        public void GetTopCourseStatistics_should_return_correctly_ordered_active_course_statistics()
        {
            using (new TransactionScope())
            {
                // Given
                var expectedIdOrder = new List<int> { 3, 1 };

                // When
                var resultIdOrder = courseService.GetTopCourseStatistics(CentreId, AdminCategoryId)
                    .Select(r => r.CustomisationId).ToList();

                // Then
                Assert.That(resultIdOrder.SequenceEqual(expectedIdOrder));
            }
        }

        [Test]
        public void GetCentreSpecificCourseStatistics_should_only_return_course_statistics_for_centre()
        {
            using (new TransactionScope())
            {
                // Given
                var expectedIdOrder = new List<int> { 1, 2 };

                // When
                var resultIdOrder = courseService.GetCentreSpecificCourseStatistics(CentreId, AdminCategoryId)
                    .Select(r => r.CustomisationId).ToList();

                // Then
                resultIdOrder.Should().BeEquivalentTo(expectedIdOrder);
            }
        }

        private IEnumerable<CourseStatistics> GetSampleCourses()
        {
            return new List<CourseStatistics>
            {
                new CourseStatistics
                {
                    CustomisationId = 1,
                    CentreId = CentreId,
                    Active = true,
                    DelegateCount = 100,
                    CompletedCount = 41
                },
                new CourseStatistics
                {
                    CustomisationId = 2,
                    CentreId = CentreId,
                    Active = false,
                    DelegateCount = 50,
                    CompletedCount = 30
                },
                new CourseStatistics
                {
                    CustomisationId = 3,
                    CentreId = CentreId + 1,
                    Active = true,
                    DelegateCount = 500,
                    CompletedCount = 99
                }
            };
        }
    }
}
