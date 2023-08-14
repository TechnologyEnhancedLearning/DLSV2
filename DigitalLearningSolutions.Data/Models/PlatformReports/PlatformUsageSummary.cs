namespace DigitalLearningSolutions.Data.Models.PlatformReports
{
    public class PlatformUsageSummary
    {
        public int ActiveCentres { get; set; }
        public int LearnerLogins { get; set; }
        public int Learners { get; set; }
        public int CourseLearningTime { get; set; }
        public int CourseEnrolments { get; set; }
        public int CourseCompletions { get; set; }
        public int IndependentSelfAssessmentEnrolments { get; set; }
        public int IndependentSelfAssessmentCompletions { get; set; }
        public int SupervisedSelfAssessmentEnrolments { get; set; }
        public int SupervisedSelfAssessmentCompletions { get; set; }
    }
}
