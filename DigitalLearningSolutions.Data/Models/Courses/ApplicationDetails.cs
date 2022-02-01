namespace DigitalLearningSolutions.Data.Models.Courses
{
    public class ApplicationDetails
    {
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public string CategoryName { get; set; }
        public int CourseTopicId { get; set; }
        public string CourseTopic { get; set; }
        public bool PLAssess { get; set; }
        public bool DiagAssess { get; set; }
    }
}
