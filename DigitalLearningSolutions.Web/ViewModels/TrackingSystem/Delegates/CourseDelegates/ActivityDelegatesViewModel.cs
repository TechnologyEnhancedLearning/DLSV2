using DigitalLearningSolutions.Data.Models.CourseDelegates;
using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
using DigitalLearningSolutions.Data.Models.SelfAssessments;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    public class ActivityDelegatesViewModel : CourseDelegatesViewModel
    {
        public ActivityDelegatesViewModel(CourseDelegatesData courseDelegatesData, SearchSortFilterPaginationResult<CourseDelegate> result, IEnumerable<FilterModel> availableFilters, string customisationIdQueryParameterName, string applicationName, bool isCourseDelegate) : base(courseDelegatesData, result, availableFilters, customisationIdQueryParameterName)
        {
            IsCourseDelegate = isCourseDelegate;
            ActivityName = applicationName;
        }
        public ActivityDelegatesViewModel(SelfAssessmentDelegatesData selfAssessmentDelegatesData, SearchSortFilterPaginationResult<SelfAssessmentDelegate> result, IEnumerable<FilterModel> availableFilters, string customisationIdQueryParameterName, int? selfAssessmentId, string selfAssessmentName, bool isCourseDelegate, bool unSupervised)
        {
            SelfAssessmentId = selfAssessmentId;
            IsCourseDelegate = isCourseDelegate;
            ActivityName = selfAssessmentName;
            Unsupervised = unSupervised;
            DelegatesDetails = selfAssessmentId != null
                ? new SelectedDelegateDetailsViewModel(
                    result,
                    availableFilters,
                    selfAssessmentDelegatesData,
                    new Dictionary<string, string>
                        { { customisationIdQueryParameterName, selfAssessmentId.ToString() } },
                    Unsupervised
                )
                : null;
        }

        public int? SelfAssessmentId { get; set; }
        public string? ActivityName { get; set; }
        public bool IsCourseDelegate { get; set; }
        public bool Unsupervised { get; set; }
        public SelectedDelegateDetailsViewModel? DelegatesDetails { get; set; }
    }
}
