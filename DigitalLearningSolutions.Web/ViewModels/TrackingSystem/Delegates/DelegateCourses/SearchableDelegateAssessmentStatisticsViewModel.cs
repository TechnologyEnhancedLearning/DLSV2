namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateCourses
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;

    public class SearchableDelegateAssessmentStatisticsViewModel : SearchableDelegateCourseStatisticsViewModel
    {
        public SearchableDelegateAssessmentStatisticsViewModel(DelegateAssessmentStatistics delegateAssessmentStatistics) : base (delegateAssessmentStatistics)
        {
            Name = delegateAssessmentStatistics.Name;
            Category = delegateAssessmentStatistics.Category;
            Tags = FilterableTagHelper.GetCurrentStatusTagsForDelegateAssessment(delegateAssessmentStatistics);
            Supervised = delegateAssessmentStatistics.Supervised;
            DelegateCount = delegateAssessmentStatistics.DelegateCount;
            SubmittedSignedOffCount = delegateAssessmentStatistics.SubmittedSignedOffCount;
        }

        public string Name { get; set; }
        public string Category { get; set; }
        public bool Supervised { get; set; }
        public int DelegateCount { get; set; }
        public int SubmittedSignedOffCount { get; set; }
    }
}
