namespace DigitalLearningSolutions.Data.Models.Courses
{
    public class CourseNameInfo
    {
        public string CustomisationName { get; set; } = null!;
        public string ApplicationName { get; set; } = null!;

        public string CourseName => string.IsNullOrWhiteSpace(CustomisationName)
            ? ApplicationName
            : ApplicationName + " - " + CustomisationName;
    }
}
