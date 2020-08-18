namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using System;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.ControllerHelpers;
    using Microsoft.Extensions.Configuration;

    public class CurrentCourseViewModel : BaseCourseViewModel
    {
        public DateTime LastAccessedDate { get; }
        public DateTime? CompleteByDate { get; }
        public bool UserIsSupervisor { get; }
        public bool IsEnrolledWithGroup { get; }
        public bool IsLocked { get; }
        public bool SelfEnrolled { get; }
        public DateValidator.ValidationResult? CompleteByValidationResult { get; set; }

        public CurrentCourseViewModel(CurrentCourse course, IConfiguration config) : base(course, config)
        {
            LastAccessedDate = course.LastAccessed;
            CompleteByDate = course.CompleteByDate;
            UserIsSupervisor = course.SupervisorAdminId != 0;
            IsEnrolledWithGroup = course.GroupCustomisationId != 0;
            SelfEnrolled = course.EnrollmentMethodID == 1;
            IsLocked = course.PLLocked;
        }

        public string DateStyle()
        {
            if (CompleteByDate < DateTime.Today)
            {
                return "overdue";
            }

            if (CompleteByDate < (DateTime.Today + TimeSpan.FromDays(30)))
            {
                return "due-soon";
            }

            return "";
        }

        public string DueByDescription()
        {
            return DateStyle() switch
            {
                "overdue" => "Course overdue; ",
                "due-soon" => "Course due soon; ",
                _ => ""
            };
        }
    }
}
