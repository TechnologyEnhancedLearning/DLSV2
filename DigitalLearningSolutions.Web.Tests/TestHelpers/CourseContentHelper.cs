namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models.CourseContent;

    internal class CourseContentHelper
    {
        public static CourseContent CreateDefaultCourseContent(
            int customisationId = 1,
            string customisationName = "Customisation",
            string applicationName = "Application"
        )
        {
            return new CourseContent
            {
                Id = customisationId,
                CustomisationName = customisationName,
                ApplicationName = applicationName
            };
        }
    }
}
