using System;
using System.Collections.Generic;
using System.Linq;
using DigitalLearningSolutions.Data.Enums;
using DigitalLearningSolutions.Web.Models.Enums;
using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
using DigitalLearningSolutions.Web.Extensions;

namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    public class SearchSelfAssessmentOvervieviewViewModel
    {
        public int SelfAssessmentId { get; set; }
        public string Vocabulary { get; set; }
        public string SearchText { get; set; }
        public SelfAssessmentResponseStatus? ResponseStatus { get; set; }
        public int Page { get; set; }
        public List<FilterViewModel> Filters { get; set; }
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
            SearchText = searchText;
            SelfAssessmentId = selfAssessmentId;
            Vocabulary = vocabulary;
            Filters = new List<FilterViewModel>()
            {
                new FilterViewModel(
                    filterProperty: FilterBy,
                    filterName: FilterBy,
                    filterOptions: new List<FilterOptionViewModel>
                    {
                        new FilterOptionViewModel(SelfAssessmentResponseStatus.NotYetResponded.GetDescription(), SelfAssessmentResponseStatus.NotYetResponded.ToString(), FilterStatus.Default),
                        new FilterOptionViewModel(SelfAssessmentResponseStatus.SelfAssessed.GetDescription(), SelfAssessmentResponseStatus.SelfAssessed.ToString(), FilterStatus.Warning),
                        new FilterOptionViewModel(SelfAssessmentResponseStatus.Verified.GetDescription(), SelfAssessmentResponseStatus.Verified.ToString(), FilterStatus.Success)
                    })

            };
            AppliedFilters = appliedFilters;
        }
        public SearchSelfAssessmentOvervieviewViewModel()
        {
            Filters = new List<FilterViewModel>();
            AppliedFilters = new List<AppliedFilterViewModel>();
        }
    }
}
