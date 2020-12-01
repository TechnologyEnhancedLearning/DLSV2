namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models.CourseContent;

    internal class CourseSectionHelper
    {
        public static CourseSection CreateDefaultCourseSection(
            string sectionName = "SectionName",
            bool hasLearning = true,
            double percentComplete = 15.0
        )
        {
            return new CourseSection(
                sectionName,
                hasLearning,
                percentComplete
                );
        }
    }
}
