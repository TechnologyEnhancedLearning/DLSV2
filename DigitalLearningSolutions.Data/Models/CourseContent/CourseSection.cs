namespace DigitalLearningSolutions.Data.Models.CourseContent
{
    public class CourseSection
    {
        public string Title { get; }
        public int Id { get; }
        public bool HasLearning { get; }
        public double PercentComplete { get; }
        public int TimeMins { get; }
        public int AverageSectionTime { get; }

        public CourseSection(string sectionName,
            int id,
            bool hasLearning,
            double percentComplete,
            int timeMins,
            int averageSectionTime
        )
        {
            Title = sectionName;
            Id = id;
            HasLearning = hasLearning;
            PercentComplete = percentComplete;
            TimeMins = timeMins;
            AverageSectionTime = averageSectionTime;
        }
    }
}
