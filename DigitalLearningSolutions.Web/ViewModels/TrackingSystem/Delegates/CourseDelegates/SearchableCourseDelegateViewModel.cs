namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SearchableCourseDelegateViewModel : BaseFilterableViewModel
    {
        public SearchableCourseDelegateViewModel(CourseDelegate courseDelegate)
        {
            DelegateId = courseDelegate.DelegateId;
            CandidateNumber = courseDelegate.CandidateNumber;
            ProfessionalRegistrationNumber = courseDelegate.ProfessionalRegistrationNumber;
            TitleName = DisplayStringHelper.GetNameWithEmailForDisplay(courseDelegate.FullNameForSearchingSorting, courseDelegate.EmailAddress);
            Active = courseDelegate.Active;
            ProgressId = courseDelegate.ProgressId;
            Locked = courseDelegate.Locked;
            LastUpdated = courseDelegate.LastUpdated.ToString(DateHelper.StandardDateAndTimeFormat);
            Enrolled = courseDelegate.Enrolled.ToString(DateHelper.StandardDateAndTimeFormat);
            CompleteBy = courseDelegate.CompleteByDate?.ToString(DateHelper.StandardDateAndTimeFormat);
            Completed = courseDelegate.Completed?.ToString(DateHelper.StandardDateAndTimeFormat);
            RemovedDate = courseDelegate.RemovedDate?.ToString(DateHelper.StandardDateAndTimeFormat);
            CustomisationId = courseDelegate.CustomisationId;
            PassRate = courseDelegate.PassRate;
            Tags = FilterableTagHelper.GetCurrentTagsForCourseDelegate(courseDelegate);
        }

        public int DelegateId { get; set; }
        public string CandidateNumber { get; set; }
        public string TitleName { get; set; }
        public bool Active { get; set; }
        public int ProgressId { get; set; }
        public bool Locked { get; set; }
        public string LastUpdated { get; set; }
        public string Enrolled { get; set; }
        public string? CompleteBy { get; set; }
        public string? Completed { get; set; }
        public string? RemovedDate { get; set; }
        public double PassRate { get; set; }
        public int CustomisationId { get; set; }
        public string? ProfessionalRegistrationNumber { get; set; }
    }
}
