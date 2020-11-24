namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.CourseContent;

    public class InitialMenuViewModel
    {
        public string Title { get; }
        public string AverageDuration { get; }
        public string CentreName { get; }
        public string? BannerText { get; }

        public InitialMenuViewModel(CourseContent courseContent)
        {
            Title = courseContent.Title;
            AverageDuration = courseContent.AverageDuration;
            CentreName = courseContent.CentreName;
            BannerText = courseContent.BannerText;
        }
    }
}
