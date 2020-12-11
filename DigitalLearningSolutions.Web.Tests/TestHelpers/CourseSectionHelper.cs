namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models.CourseContent;

    internal class CourseSectionHelper
    {
        public static CourseSection CreateDefaultCourseSection(
            int id = 1,
            string sectionName = "SectionName",
            bool hasLearning = true,
            double percentComplete = 15.0,
            int timeMins = 1,
            int averageSectionTime = 2
        )
        {
            return new CourseSection(
                sectionName,
                id,
                hasLearning,
                percentComplete,
                timeMins,
                averageSectionTime
            );
        }
    }
}
