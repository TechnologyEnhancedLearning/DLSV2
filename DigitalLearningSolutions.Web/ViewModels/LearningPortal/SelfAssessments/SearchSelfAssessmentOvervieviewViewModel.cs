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
        public string Vocabulary { get; set; }
        public string SearchText { get; set; }
        public SelfAssessmentCompetencyFilter? ResponseStatus { get; set; }
        public int Page { get; set; }
        public List<FilterModel> Filters { get; set; }
        public List<AppliedFilterViewModel> AppliedFilters { get; set; }
        public string FilterBy { get; set; }

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

        public SearchSelfAssessmentOvervieviewViewModel(string searchText, int selfAssessmentId, string vocabulary, List<AppliedFilterViewModel> appliedFilters)
        {
            FilterBy = nameof(ResponseStatus);
            SearchText = searchText ?? string.Empty;
            SelfAssessmentId = selfAssessmentId;
            Vocabulary = vocabulary;
            Filters = new List<FilterModel>()
            {
                new FilterModel(
                        filterProperty: FilterBy,
                        filterName: FilterBy,
                        filterOptions: Enum.GetValues(typeof(SelfAssessmentCompetencyFilter))
                            .Cast<SelfAssessmentCompetencyFilter>()
                            .Select(f => new FilterOptionModel(f.GetDescription(), f.ToString(), FilterStatus.Default)))
            };
            AppliedFilters = appliedFilters ?? new List<AppliedFilterViewModel>();
        }
        public SearchSelfAssessmentOvervieviewViewModel()
        {
            Filters = new List<FilterModel>();
            AppliedFilters = new List<AppliedFilterViewModel>();
        }
    }
}
