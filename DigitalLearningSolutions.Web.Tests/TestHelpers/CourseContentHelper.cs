namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using System;
    using DigitalLearningSolutions.Data.Models.CourseContent;

    internal class CourseContentHelper
    {
        public static CourseContent CreateDefaultCourseContent(
            int customisationId = 1,
            string customisationName = "Customisation",
            string applicationName = "Application",
            string averageDuration = "Duration",
            string centreName = "Centre",
            string? bannerText = "Banner",
            bool includeCertification = false,
            DateTime? completed = null
        )
        {
            return new CourseContent(
                customisationId,
                applicationName,
                customisationName,
                averageDuration,
                centreName,
                bannerText,
                includeCertification,
                completed
            );
        }
    }
}
