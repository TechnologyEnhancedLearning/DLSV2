﻿namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using DigitalLearningSolutions.Data.Models.Courses;

    public class CurrentCourseViewModel : CurrentLearningItemViewModel
    {
        public bool UserIsSupervisor { get; }
        public bool IsEnrolledWithGroup { get; }
        public bool IsLocked { get; }
        public bool SelfEnrolled { get; }

        public CurrentCourseViewModel(CurrentCourse course) : base(course)
        {
            UserIsSupervisor = course.SupervisorAdminId != 0;
            IsEnrolledWithGroup = course.GroupCustomisationId != 0;
            SelfEnrolled = course.EnrollmentMethodID == 1;
            IsLocked = course.PLLocked;
        }
    }
}
