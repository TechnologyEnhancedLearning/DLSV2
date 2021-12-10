namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class CourseDelegatesDataServiceTests
    {
        private ICourseDelegatesDataService courseDelegatesDataService = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            courseDelegatesDataService = new CourseDelegatesDataService(connection);
        }

        [Test]
        public void GetDelegatesOnCourse_returns_expected_values()
        {
            // Given
            var expectedFirstRecord = new CourseDelegate
            {
                Active = true,
                CandidateNumber = "PC97",
                CompleteByDate = null,
                DelegateId = 32926,
                EmailAddress = "erpock.hs@5bntu",
                Enrolled = new DateTime(2012, 07, 02, 13, 30, 37, 807),
                FirstName = "xxxxx",
                LastName = "xxxx",
                LastUpdated = new DateTime(2012, 07, 31, 10, 18, 39, 993),
                Locked = false,
                ProgressId = 18395,
                RemovedDate = null,
                Completed = null,
                AllAttempts = 0,
                AttemptsPassed = 0
            };

            // When
            var result = courseDelegatesDataService.GetDelegatesOnCourse(1, 2).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Should().HaveCount(3);
                result.First().Should().BeEquivalentTo(expectedFirstRecord);
            }
        }
    }
}
