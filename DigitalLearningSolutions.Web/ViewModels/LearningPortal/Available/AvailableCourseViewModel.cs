namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available
{
    using DigitalLearningSolutions.Data.Models.Courses;

    public class AvailableCourseViewModel : BaseLearningItemViewModel
    {
        public readonly string Brand;
        public readonly string? Category;
        public readonly string? Topic;
        public readonly DelegateStatus DelegateStatus;
        public readonly string? EnrolButtonText;
        public readonly string? EnrolButtonAriaLabel;

        public AvailableCourseViewModel(AvailableCourse course) : base(course)
        {
            Brand = course.Brand;
            Category = course.Category;
            Topic = course.Topic;
            DelegateStatus = (DelegateStatus)course.DelegateStatus;
            EnrolButtonText = GetEnrolButtonText(DelegateStatus);
            EnrolButtonAriaLabel = EnrolButtonText == null ? null : $"{EnrolButtonText} on activity";
        }

        private static string? GetEnrolButtonText(DelegateStatus delegateStatus) =>
            delegateStatus switch
            {
                DelegateStatus.NotEnrolled => "Enrol",
                DelegateStatus.Removed => "Enrol",
                DelegateStatus.Expired => "Re-enrol",
                _ => null
            };
    }

    public enum DelegateStatus
    {
        NotEnrolled = 0,
        Expired = 1,
        Completed = 2,
        Current = 3,
        Removed = 4
    }
}
