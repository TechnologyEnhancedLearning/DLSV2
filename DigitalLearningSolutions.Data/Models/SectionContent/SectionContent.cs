namespace DigitalLearningSolutions.Data.Models.SectionContent
{
    public class SectionContent
    {
        public string SectionName { get; }
        public string TimeMins { get; }
        public string AverageSectionTime { get; }
        public bool HasLearning { get; }
        public double PercentComplete { get; }

        public SectionContent(
            string sectionName,
            string timeMins,
            string averageSectionTime,
            bool hasLearning,
            double percentComplete
        )
        {
            SectionName = sectionName;
            TimeMins = timeMins;
            AverageSectionTime = averageSectionTime;
            HasLearning = hasLearning;
            PercentComplete = percentComplete;
        }
    }
}
