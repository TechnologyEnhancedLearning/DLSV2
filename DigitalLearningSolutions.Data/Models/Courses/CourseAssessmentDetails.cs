namespace DigitalLearningSolutions.Data.Models.Courses
{
    public class CourseAssessmentDetails : Course
    {
        public bool IsAssessed { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CourseTopic { get; set; } = string.Empty;
        public bool HasLearning { get; set; }
        public bool HasDiagnostic { get; set; }
    }
}
