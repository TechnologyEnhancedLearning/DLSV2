namespace DigitalLearningSolutions.Data.Models.Courses
{
    public class CurrentCourse : CurrentLearningItem
    {
        public int SupervisorAdminId { get; set; }
        public int GroupCustomisationId { get; set; }
        public int EnrollmentMethodID { get; set; }
        public bool PLLocked { get; set; }
    }
}
