namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class CourseTopicsServiceTests
    {
        private ICourseTopicsDataService courseTopicsDataService = null!;
        private CourseTopicsService courseTopicsService = null!;

        [SetUp]
        public void Setup()
        {
            courseTopicsDataService = A.Fake<ICourseTopicsDataService>();
            courseTopicsService = new CourseTopicsService(
                courseTopicsDataService
            );
        }

        [Test]
        public void GetActiveTopicsAvailableAtCentre_calls_data_service()
        {
            // Given
            const int centreId = 1;
            var topic = new Topic { CourseTopicID = 1, CourseTopic = "Topic", Active = true };
            var topics = new List<Topic> { topic };

            A.CallTo(() => courseTopicsDataService.GetCourseTopicsAvailableAtCentre(1)).Returns(topics);

            // When
            var result = courseTopicsService.GetActiveTopicsAvailableAtCentre(centreId);

            // Then
            using (new AssertionScope())
            {
                A.CallTo(() => courseTopicsDataService.GetCourseTopicsAvailableAtCentre(centreId))
                    .MustHaveHappenedOnceExactly();
                result.Should().BeEquivalentTo(topics);
            }
        }

        [Test]
        public void GetActiveTopicsAvailableAtCentre_returns_only_active_topics()
        {
            // Given
            const int centreId = 1;
            var topicOne = new Topic { CourseTopicID = 1, CourseTopic = "Topic", Active = true };
            var topicTwo = new Topic { CourseTopicID = 2, CourseTopic = "Topic 2", Active = false };
            var topics = new List<Topic> { topicOne, topicTwo };

            A.CallTo(() => courseTopicsDataService.GetCourseTopicsAvailableAtCentre(1)).Returns(topics);

            // When
            var result = courseTopicsService.GetActiveTopicsAvailableAtCentre(centreId)
                .ToList();

            // Then
            result.Should().ContainSingle(t => t.Active == true);
        }
    }
}
