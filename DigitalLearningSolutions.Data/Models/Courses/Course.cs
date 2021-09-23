namespace DigitalLearningSolutions.Data.Models.Courses
{
    public class Course : CourseNameInfo
    {
        public int CustomisationId { get; set; }
        public int CentreId { get; set; }
        public int ApplicationId { get; set; }
        public bool Active { get; set; }

        public string CourseNameWithInactiveFlag => !Active ? "Inactive - " + CourseName : CourseName;
    }
}
