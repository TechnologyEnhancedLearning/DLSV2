namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.SectionContent;

    public class SectionContentViewModel
    {
        public string CourseTitle { get; }
        public string SectionName { get; }
        public string PercentComplete { get; }
        public string TimeInformation { get; }
        public int CustomisationId { get; }
        public IEnumerable<TutorialCardViewModel> Tutorials { get; }

        public SectionContentViewModel(SectionContent sectionContent, int customisationId)
        {
            CourseTitle = sectionContent.CourseTitle;
            SectionName = sectionContent.SectionName;
            PercentComplete = sectionContent.HasLearning ? $"{GetPercentComplete(sectionContent):f0}% Complete" : "";
            TimeInformation = $"{sectionContent.SectionTime}m (average time {sectionContent.AverageSectionTime}m)";
            CustomisationId = customisationId;
            Tutorials = sectionContent.Tutorials.Select(tutorial => new TutorialCardViewModel(tutorial));
        }

        public double GetPercentComplete(SectionContent sectionContent)
        {
            var totalStatus = sectionContent.Tutorials.Sum(tutorial => tutorial.TutorialStatus);
            return sectionContent.Tutorials.Count == 0 || !sectionContent.HasLearning
                ? 0
                : (totalStatus * 100) / (sectionContent.Tutorials.Count * 2);
            
        }
    }
}
