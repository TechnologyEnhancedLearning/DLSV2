namespace DigitalLearningSolutions.Data.Models.CourseContent
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;

    public class CourseContent
    {
        public int Id { get; private set; }
        public string Title { get; private set; }
        public string AverageDuration { get; private set; }
        public string CentreName { get; private set; }
        public string? BannerText { get; private set; }
        public List<CourseSection> Sections { get; private set; } = new List<CourseSection>();

        public CourseContent(
            int id,
            string applicationName,
            string customisationName,
            string averageDuration,
            string centreName,
            string? bannerText
        )
        {
            Id = id;
            Title = $"{applicationName} - {customisationName}";
            AverageDuration = averageDuration;
            CentreName = centreName;
            BannerText = bannerText;
        }
    }
}
