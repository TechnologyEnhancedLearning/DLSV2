namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using DigitalLearningSolutions.Data.Models.SectionContent;

    public class SectionContentViewModel
    {
        public string CourseTitle { get; }
        public string SectionName { get; }
        public string PercentComplete { get; }
        public string TimeInformation { get; }
        public int CustomisationId { get; }

        public SectionContentViewModel(SectionContent sectionContent, int customisationId)
        {
            CourseTitle = sectionContent.CourseTitle;
            SectionName = sectionContent.SectionName;
            PercentComplete = sectionContent.HasLearning ? $"{sectionContent.PercentComplete:f0}% Complete" : "";
            TimeInformation = $"{sectionContent.TimeMins}m (average time {sectionContent.AverageSectionTime}m)";
            CustomisationId = customisationId;
        }
    }
}
