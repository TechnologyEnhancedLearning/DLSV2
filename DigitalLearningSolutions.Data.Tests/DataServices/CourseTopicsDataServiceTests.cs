namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class CourseTopicsDataServiceTests
    {
        private CourseTopicsDataService courseTopicsDataService = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            courseTopicsDataService = new CourseTopicsDataService(connection);
        }

        [Test]
        public void GetTopicsAvailableAtCentre_should_return_expected_items()
        {
            // Given
            var expectedTopics = new List<Topic>
            {
                new Topic { CourseTopic = "Digital Skills", CourseTopicID = 2, Active = true},
                new Topic { CourseTopic = "Excel", CourseTopicID = 5, Active = true},
                new Topic { CourseTopic = "Microsoft Office", CourseTopicID = 3, Active = true},
                new Topic { CourseTopic = "OneNote", CourseTopicID = 10, Active = true},
                new Topic { CourseTopic = "Outlook", CourseTopicID = 7, Active = true},
                new Topic { CourseTopic = "PowerPoint", CourseTopicID = 6, Active = true},
                new Topic { CourseTopic = "SharePoint", CourseTopicID = 9, Active = true},
                new Topic { CourseTopic = "Social Media", CourseTopicID = 8, Active = true},
                new Topic { CourseTopic = "Undefined", CourseTopicID = 1, Active = true},
                new Topic { CourseTopic = "Word", CourseTopicID = 4, Active = true}
            };

            // When
            var result = courseTopicsDataService.GetCourseTopicsAvailableAtCentre(101).ToList();

            // Then
            result.Should().BeEquivalentTo(expectedTopics);
        }
    }
}
