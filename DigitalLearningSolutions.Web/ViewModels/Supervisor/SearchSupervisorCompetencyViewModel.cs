using DigitalLearningSolutions.Data.Enums;
using DigitalLearningSolutions.Data.Models.Frameworks;
using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
using DigitalLearningSolutions.Web.Extensions;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    public class SearchSupervisorCompetencyViewModel
    {
        public int SupervisorDelegateId { get; set; }
        public int CandidateAssessmentId { get; set; }
        public int SelfAssessmentId { get; set; }
        public int? CompetencyGroupId { get; set; }
        public bool IsSupervisorResultsReviewed { get; set; }
        public bool IncludeRequirementsFilters { get; set; }
        public string SearchText { get; set; }
        public int SelectedFilter { get; set; }
        public int Page { get; set; }
        public List<FilterModel> Filters { get; set; }
        public List<AppliedFilterViewModel> AppliedFilters { get; set; }
        public List<CompetencyFlag> CompetencyFlags { get; set; }
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
                    { "CandidateAssessmentId", CandidateAssessmentId.ToString() },
                    { "SearchText", SearchText }
                };
                return new CurrentFiltersViewModel(AppliedFilters, SearchText, route);
            }
        }
        public SearchSupervisorCompetencyViewModel Initialise(List<AppliedFilterViewModel> appliedFilters, List<CompetencyFlag> competencyFlags, bool isSupervisorResultsReviewed, bool includeRequirementsFilters)
        {
            var allFilters = Enum.GetValues(typeof(SelfAssessmentCompetencyFilter)).Cast<SelfAssessmentCompetencyFilter>();
            var filterOptions = (from f in allFilters
                                 let includeRejectedWhenSupervisorReviewed = f != SelfAssessmentCompetencyFilter.ConfirmationRejected || isSupervisorResultsReviewed
                                 where SupervisorCompetencyFilterHelper.IsResponseStatusFilter((int)f) && includeRejectedWhenSupervisorReviewed
                                 select f).ToList();
            if (includeRequirementsFilters)
            {
                if (AnyQuestionMeetingRequirements) filterOptions.Add(SelfAssessmentCompetencyFilter.MeetingRequirements);
                if (AnyQuestionNotMeetingRequirements) filterOptions.Add(SelfAssessmentCompetencyFilter.NotMeetingRequirements);
                if (AnyQuestionPartiallyMeetingRequirements) filterOptions.Add(SelfAssessmentCompetencyFilter.PartiallyMeetingRequirements);
            }

            var dropdownFilterOptions = filterOptions.Select(
                f => new FilterOptionModel(f.GetDescription(isSupervisorResultsReviewed),
                ((int)f).ToString(),
                FilterStatus.Default)).ToList();

            if (competencyFlags?.Count() > 0)
            {
                var competencyFlagOptions = competencyFlags.DistinctBy(f => f.FlagId, null)
                    .Select(c =>
                        new FilterOptionModel(
                                $"{c.FlagGroup}: {c.FlagName}",
                                c.FlagId.ToString(),
                                FilterStatus.Default));
                dropdownFilterOptions.AddRange(competencyFlagOptions);
            }

            Filters = new List<FilterModel>()
            {
                new FilterModel(
                    filterProperty: FilterBy,
                    filterName: FilterBy,
                    filterOptions: dropdownFilterOptions)
            };
            IsSupervisorResultsReviewed = isSupervisorResultsReviewed;
            AppliedFilters = appliedFilters ?? new List<AppliedFilterViewModel>();
            return this;
        }

        public SearchSupervisorCompetencyViewModel(int supervisorDelegateId, string searchText, int candidateAssessmentId,  bool isSupervisorResultsReviewed, bool includeRequirementsFilters, List<AppliedFilterViewModel> appliedFilters, List<CompetencyFlag> competencyFlags = null)
        {
            FilterBy = nameof(SelectedFilter);
            SearchText = searchText ?? string.Empty;
            CandidateAssessmentId = candidateAssessmentId;
            IncludeRequirementsFilters = includeRequirementsFilters;
            SupervisorDelegateId = supervisorDelegateId;
            Initialise(appliedFilters, competencyFlags, isSupervisorResultsReviewed, includeRequirementsFilters);
        }

        public SearchSupervisorCompetencyViewModel()
        {
            Filters = new List<FilterModel>();
            AppliedFilters = new List<AppliedFilterViewModel>();
        }
    }
}
