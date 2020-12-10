namespace DigitalLearningSolutions.Data.Models.SectionContent
{
    public class SectionContent
    {
        public string CourseTitle { get; }
        public string SectionName { get; }
        public int? TimeMins { get; }
        public int AverageSectionTime { get; }
        public bool HasLearning { get; }
        public double PercentComplete { get; }

        public SectionContent(
            string customisationName,
            string applicationName,
            string sectionName,
            int? timeMins,
            int averageSectionTime,
            bool hasLearning,
            double percentComplete
        )
        {
            CourseTitle = $"{applicationName} - {customisationName}";
            SectionName = sectionName;
            TimeMins = timeMins;
            AverageSectionTime = averageSectionTime;
            HasLearning = hasLearning;
            PercentComplete = percentComplete;
        }
    }
}
