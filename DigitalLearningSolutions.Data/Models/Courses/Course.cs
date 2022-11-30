namespace DigitalLearningSolutions.Data.Models.Courses
{
    public class Course : CourseNameInfo
    {
        public int CustomisationId { get; set; }
        public int CentreId { get; set; }
        public int ApplicationId { get; set; }
        public bool Active { get; set; }
        public string? Status { get; set; }
        public string CourseNameWithInactiveFlag => !Active ? "Inactive - " + CourseName : CourseName;

        public override bool Equals(object? obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Course c = (Course)obj;
            return CustomisationId == c.CustomisationId;
        }

        public override int GetHashCode()
        {
            return CustomisationId;
        }
    }
}
