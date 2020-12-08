namespace DigitalLearningSolutions.Data.Models.SectionContent
{
    public class SectionContent
    {
        public string SectionName { get; }
        public int TimeMins { get; }
        public int AverageSectionTime { get; }
        public bool HasLearning { get; }
        public double PercentComplete { get; }

        public SectionContent(
            string sectionName,
            int timeMins,
            int averageSectionTime,
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
