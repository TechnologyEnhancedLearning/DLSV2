namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.CourseContent;

    public class SectionCardViewModel
    {
        public readonly string Title;
        public readonly bool HasLearning;
        public readonly double PercentComplete;

        public SectionCardViewModel(CourseSection section)
        {
            Title = section.Title;
            HasLearning = section.HasLearning;
            PercentComplete = section.PercentComplete;
        }

        public string GetPercentComplete()
        {
            return HasLearning ? $"{PercentComplete}% Complete" : "";
        }
    }
}
