namespace DigitalLearningSolutions.Data.Models.CourseContent
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Courses;

    public class CourseContent
    {
        public int Id { get; }
        public string Title { get; }
        public string AverageDuration { get; }
        public string CentreName { get; }
        public string? BannerText { get; }
        public List<CourseSection> Sections { get; } = new List<CourseSection>();

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
