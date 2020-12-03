namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.CourseContent;

    public class SectionCardViewModel
    {
        public string Title { get; }
        public int SectionId { get; }
        public string PercentComplete { get; }

        public SectionCardViewModel(CourseSection section)
        {
            Title = section.Title;
            SectionId = section.Id;
            PercentComplete = section.HasLearning ? $"{section.PercentComplete:f0}% Complete" : "";
        }
    }
}
