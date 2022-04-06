namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DateHelper = Helpers.DateHelper;

    public class SearchableCourseDelegateViewModel : BaseFilterableViewModel
    {
        public SearchableCourseDelegateViewModel(
            CourseDelegate courseDelegate,
            IList<DelegateCourseAdminField> adminFields,
            IEnumerable<CourseAdminField> adminFieldsWithOptions
        )
        {
            DelegateId = courseDelegate.DelegateId;
            CandidateNumber = courseDelegate.CandidateNumber;
            ProfessionalRegistrationNumber = PrnStringHelper.GetPrnDisplayString(
                courseDelegate.HasBeenPromptedForPrn,
                courseDelegate.ProfessionalRegistrationNumber
            );
            TitleName = courseDelegate.FullNameForSearchingSorting;
            Email = courseDelegate.EmailAddress;
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
            DelegateCourseAdminFields = adminFields;
            AdminFieldFilters =
                CourseDelegateViewModelFilterOptions.GetAdminFieldFilters(adminFields, adminFieldsWithOptions);
        }

        public int DelegateId { get; set; }
        public string CandidateNumber { get; set; }
        public string TitleName { get; set; }
        public string? Email { get; set; }
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
        public IEnumerable<DelegateCourseAdminField> DelegateCourseAdminFields { get; set; }
        public Dictionary<int, string> AdminFieldFilters { get; set; }
    }
}
