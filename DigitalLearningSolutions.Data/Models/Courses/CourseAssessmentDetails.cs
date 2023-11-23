namespace DigitalLearningSolutions.Data.Models.Courses
{
    public class CourseAssessmentDetails : Course
    {
        public bool IsAssessed { get; set; }
        public string CategoryName { get; set; }
        public string CourseTopic { get; set; }
        public bool HasLearning { get; set; }
        public bool HasDiagnostic { get; set; }
    }
}
