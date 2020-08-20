namespace DigitalLearningSolutions.Data.Models
{
    using System;

    public class CurrentCourse : BaseCourse
    {
        public DateTime? CompleteByDate { get; set; }
        public int SupervisorAdminId { get; set; }
        public int GroupCustomisationId { get; set; }
        public int EnrollmentMethodID { get; set; }
        public bool PLLocked { get; set; }
    }
}
