namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.CourseContent;

    public class SectionCardViewModel
    {
        public string Title { get; }
        public string PercentComplete { get; }

        public SectionCardViewModel(CourseSection section)
        {
            Title = section.Title;
            PercentComplete = section.HasLearning ? $"{section.PercentComplete}% Complete" : "";
        }
    }
}
