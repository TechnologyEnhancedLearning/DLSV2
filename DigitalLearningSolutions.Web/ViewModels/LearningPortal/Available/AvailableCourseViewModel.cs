namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using Microsoft.Extensions.Configuration;

    public class AvailableCourseViewModel : BaseCourseViewModel
    {
        public readonly string Brand;
        public readonly string? Category;
        public readonly string? Topic;
        public readonly DelegateStatus DelegateStatus;
        public AvailableCourseViewModel(AvailableCourse course, IConfiguration config) : base(course, config)
        {
            Brand = course.Brand;
            Category = GetValidOrNull(course.Category);
            Topic = GetValidOrNull(course.Topic);
            DelegateStatus = (DelegateStatus)course.DelegateStatus;
        }

        private static string? GetValidOrNull(string toValidate)
        {
            return toValidate.ToLower() == "undefined" ? null : toValidate;
        }
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
