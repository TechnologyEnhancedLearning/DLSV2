namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.CourseContent;

    public class SectionCardViewModel
    {
        public string Title { get; }
        public string PercentComplete { get; private set; }
        private bool HasLearning;
        

        public SectionCardViewModel(CourseSection section)
        {
            Title = section.Title;
            HasLearning = section.HasLearning;
            PercentComplete = HasLearning ? $"{section.PercentComplete}% Complete" : "";
        }
    }
}
