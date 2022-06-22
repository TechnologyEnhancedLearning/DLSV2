using System;
using System.Collections.Generic;
using System.Linq;
using DigitalLearningSolutions.Data.Enums;
using DigitalLearningSolutions.Web.Models.Enums;
using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
using DigitalLearningSolutions.Web.Extensions;

namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class SearchSelfAssessmentOvervieviewViewModel
    {
        public int SelfAssessmentId { get; set; }
        public int? CompetencyGroupId { get; set; }
        public bool IsSupervisorResultsReviewed { get; set; }
        public string Vocabulary { get; set; }
        public string SearchText { get; set; }
        public SelfAssessmentCompetencyFilter? ResponseStatus { get; set; }
        public int Page { get; set; }
        public List<FilterModel> Filters { get; set; }
        public List<AppliedFilterViewModel> AppliedFilters { get; set; }
        public bool AnyQuestionMeetingRequirements { get; set; }
        public bool AnyQuestionPartiallyMeetingRequirements { get; set; }
        public bool AnyQuestionNotMeetingRequirements { get; set; }
        public string FilterBy { get; set; }

        [Obsolete]
        public CurrentFiltersViewModel CurrentFilters
        {
            get
            {
                var route = new Dictionary<string, string>()
                {
                    { "SelfAssessmentId", SelfAssessmentId.ToString() },
                    { "Vocabulary", Vocabulary },
                    { "SearchText", SearchText }
                };
                return new CurrentFiltersViewModel(AppliedFilters, SearchText, route);
            }
        }

        public SearchSelfAssessmentOvervieviewViewModel Initialise(List<AppliedFilterViewModel> appliedFilters)
        {
            var allFilters = Enum.GetValues(typeof(SelfAssessmentCompetencyFilter)).Cast<SelfAssessmentCompetencyFilter>();
            var filterOptions = allFilters.Where(f => f.IsResponseStatusFilter()).ToList();
            if (AnyQuestionMeetingRequirements) filterOptions.Add(SelfAssessmentCompetencyFilter.MeetingRequirements);
            if (AnyQuestionNotMeetingRequirements) filterOptions.Add(SelfAssessmentCompetencyFilter.NotMeetingRequirements);
            if (AnyQuestionPartiallyMeetingRequirements) filterOptions.Add(SelfAssessmentCompetencyFilter.PartiallyMeetingRequirements);

            var dropdownFilterOptions = filterOptions.Select(
                f => new FilterOptionModel(f.GetDescription(IsSupervisorResultsReviewed),
                f.ToString(),
                FilterStatus.Default));

            Filters = new List<FilterModel>()
            {
                new FilterModel(
                        filterProperty: FilterBy,
                        filterName: FilterBy,
                        filterOptions: dropdownFilterOptions)
            };
            AppliedFilters = appliedFilters ?? new List<AppliedFilterViewModel>();
            return this;
        }

        public SearchSelfAssessmentOvervieviewViewModel(string searchText, int selfAssessmentId, string vocabulary, List<AppliedFilterViewModel> appliedFilters)
        {
            FilterBy = nameof(ResponseStatus);
            SearchText = searchText ?? string.Empty;
            SelfAssessmentId = selfAssessmentId;   
            Vocabulary = vocabulary;
            Initialise(appliedFilters);
        }

        public SearchSelfAssessmentOvervieviewViewModel()
        {
            Filters = new List<FilterModel>();
            AppliedFilters = new List<AppliedFilterViewModel>();
        }
    }
}
