namespace DigitalLearningSolutions.Data.Models.Courses
{
    public class CourseNameInfo
    {
        public string CourseName{ get; set; } = null!;
        public string ApplicationName { get; set; } = null!;

        public string CompositeName => string.IsNullOrWhiteSpace(CourseName)
            ? ApplicationName
            : ApplicationName + " - " + CourseName;
    }
}
