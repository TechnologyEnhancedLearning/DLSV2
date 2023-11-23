namespace DigitalLearningSolutions.Data.Models.CourseContent
{
    public class CourseSection
    {
        public string Title { get; }
        public int Id { get; }
        public bool HasLearning { get; }
        public double PercentComplete { get; }
        public bool PostLearningAssessmentPassed { get; }

        public CourseSection(
            string sectionName,
            int id,
            bool hasLearning,
            double percentComplete,
            int pLPasses
        )
        {
            Title = sectionName;
            Id = id;
            HasLearning = hasLearning;
            PercentComplete = percentComplete;
            PostLearningAssessmentPassed = pLPasses >= 1;
        }
    }
}
