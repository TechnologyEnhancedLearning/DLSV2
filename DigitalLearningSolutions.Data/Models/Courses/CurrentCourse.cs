namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System;

    public class CurrentCourse : StartedCourse
    {
        public DateTime? CompleteByDate { get; set; }
        public int SupervisorAdminId { get; set; }
        public int GroupCustomisationId { get; set; }
        public int EnrollmentMethodID { get; set; }
        public bool PLLocked { get; set; }
    }
}
