namespace DigitalLearningSolutions.Data.Models.CourseContent
{
    public class CourseSection
    {
        public string Title { get; }
        public bool HasLearning { get; }
        public double PercentComplete { get; }

        public CourseSection(string sectionName, bool hasLearning, double percentComplete)
        {
            Title = sectionName;
            HasLearning = hasLearning;
            PercentComplete = percentComplete;
        }
    }
}
