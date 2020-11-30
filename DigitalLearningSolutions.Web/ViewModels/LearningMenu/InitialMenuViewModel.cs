namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CourseContent;

    public class InitialMenuViewModel
    {
        public int Id { get; }
        public string Title { get; }
        public string AverageDuration { get; }
        public string CentreName { get; }
        public string? BannerText { get; }
        public IEnumerable<SectionCardViewModel> Sections { get; }

        public InitialMenuViewModel(CourseContent courseContent)
        {
            Id = courseContent.Id;
            Title = courseContent.Title;
            AverageDuration = courseContent.AverageDuration;
            CentreName = courseContent.CentreName;
            BannerText = courseContent.BannerText;
            Sections = courseContent.Sections.Select(section => new SectionCardViewModel(section));
        }
    }
}
