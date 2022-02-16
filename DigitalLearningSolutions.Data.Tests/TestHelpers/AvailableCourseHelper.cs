namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models.Courses;

    public static class AvailableCourseHelper
    {
        public static AvailableCourse CreateDefaultAvailableCourse(
            int customisationId = 1,
            string courseName = "Course 1",
            bool hasDiagnostic = true,
            bool hasLearning = true,
            bool isAssessed = true,
            string brand = "Brand 1",
            string? category = "Category 1",
            string? topic = "Topic 1",
            int delegateStatus = 0
        )
        {
            return new AvailableCourse
            {
                Id = customisationId,
                Name = courseName,
                HasDiagnostic = hasDiagnostic,
                HasLearning = hasLearning,
                IsAssessed = isAssessed,
                Brand = brand,
                Category = category,
                Topic = topic,
                DelegateStatus = delegateStatus
            };
        }
    }
}
