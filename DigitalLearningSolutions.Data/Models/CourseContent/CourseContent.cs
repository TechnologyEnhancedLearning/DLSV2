namespace DigitalLearningSolutions.Data.Models.CourseContent
{
    public class CourseContent
    {
        public int Id { get; private set; }
        public string Title { get; private set; }
        public string AverageDuration { get; private set; }
        public string CentreName { get; private set; }
        public string? BannerText { get; private set; }

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
