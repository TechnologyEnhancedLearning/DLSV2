using System;
using System.Collections.Generic;
using System.Linq;
using DigitalLearningSolutions.Data.Enums;
using DigitalLearningSolutions.Data.Models.SelfAssessments;
using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments;

namespace DigitalLearningSolutions.Web.Helpers
{
    public class CompetencyFilterHelper
    {
        public static IEnumerable<Competency> ApplyResponseStatusFilters(IEnumerable<Competency> competencies, IEnumerable<int> filters, string searchText = "")
        {
            var filteredCompetencies = competencies;
            var appliedResponseStatusFilters = filters.Where(f => IsResponseStatusFilter(f));
            if (appliedResponseStatusFilters.Any() || searchText.Length > 0)
            {
                var wordsInSearchText = searchText.Split().Where(w => w != string.Empty);
                filters = appliedResponseStatusFilters;
                filteredCompetencies = from c in competencies
                    let searchTextMatchesGroup = wordsInSearchText.Any(w => c.CompetencyGroup?.Contains(w, StringComparison.CurrentCultureIgnoreCase) ?? false)
                    let searchTextMatchesCompetencyDescription = wordsInSearchText.Any(w => c.Description?.Contains(w, StringComparison.CurrentCultureIgnoreCase) ?? false)
                    let searchTextMatchesCompetencyName = wordsInSearchText.Any(w => c.Name?.Contains(w, StringComparison.CurrentCultureIgnoreCase) ?? false)
                    let responseStatusFilterMatchesAnyQuestion =
                       (filters.Contains((int)SelfAssessmentCompetencyFilter.RequiresSelfAssessment) && c.AssessmentQuestions.Any(q => q.ResultId == null))
                    || (filters.Contains((int)SelfAssessmentCompetencyFilter.SelfAssessed) && c.AssessmentQuestions.Any(q => q.Verified == null && q.ResultId != null))
                    || (filters.Contains((int)SelfAssessmentCompetencyFilter.ConfirmationRequested) && c.AssessmentQuestions.Any(q => q.Verified == null && q.Requested != null))
                    || (filters.Contains((int)SelfAssessmentCompetencyFilter.Verified) && c.AssessmentQuestions.Any(q => q.Verified.HasValue))
                    where (wordsInSearchText.Count() == 0 || searchTextMatchesGroup || searchTextMatchesCompetencyDescription || searchTextMatchesCompetencyName)
                        && (!appliedResponseStatusFilters.Any() || responseStatusFilterMatchesAnyQuestion)
                    select c;
            }
            return filteredCompetencies;
        }

        public static IEnumerable<Competency> ApplyRequirementsFilters(IEnumerable<Competency> competencies, IEnumerable<int> filters)
        {
            var filteredCompetencies = competencies;
            var appliedRequirementsFilters = filters.Where(f => IsRequirementsFilter(f));
            if (appliedRequirementsFilters.Any())
            {
                filters = appliedRequirementsFilters;
                filteredCompetencies = from c in competencies
                    let requirementsFilterMatchesAnyQuestion =
                           (filters.Contains((int)SelfAssessmentCompetencyFilter.MeetingRequirements) && c.AssessmentQuestions.Any(q => q.ResultRAG == 3))
                        || (filters.Contains((int)SelfAssessmentCompetencyFilter.PartiallyMeetingRequirements) && c.AssessmentQuestions.Any(q => q.ResultRAG == 2))
                        || (filters.Contains((int)SelfAssessmentCompetencyFilter.NotMeetingRequirements) && c.AssessmentQuestions.Any(q => q.ResultRAG == 1))
                    where requirementsFilterMatchesAnyQuestion
                    select c;
            }
            return filteredCompetencies;
        }

        public static IEnumerable<Competency> ApplyCompetencyGroupFilters(IEnumerable<Competency> competencies, SearchSelfAssessmentOvervieviewViewModel search)
        {
            var filteredCompetencies = competencies;
            var appliedCompetencyGroupFilters = search.AppliedFilters?.Select(f => int.Parse(f.FilterValue)).Where(f => IsCompetencyGroupFilter(f)) ?? Enumerable.Empty<int>();
            if (appliedCompetencyGroupFilters.Any())
            {
                foreach(var competency in filteredCompetencies)
                {
                    competency.CompetencyFlags = search.CompetencyFlags.Where(f => f.CompetencyId == competency.Id);
                }
                filteredCompetencies = competencies.Where(c => c.CompetencyFlags.Any(f => appliedCompetencyGroupFilters.Contains(f.FlagId)));
            }
            return filteredCompetencies;
        }

        public static void UpdateRequirementsFilterDropdownOptionsVisibility(SearchSelfAssessmentOvervieviewViewModel search, IEnumerable<Competency> competencies)
        {
            var filteredQuestions = competencies.SelectMany(c => c.AssessmentQuestions);
            if (search != null)
            {
                search.AnyQuestionMeetingRequirements = filteredQuestions.Any(q => q.ResultRAG == 3);
                search.AnyQuestionPartiallyMeetingRequirements = filteredQuestions.Any(q => q.ResultRAG == 2);
                search.AnyQuestionNotMeetingRequirements = filteredQuestions.Any(q => q.ResultRAG == 1);
            }
        }

        public static bool IsRequirementsFilter(int filter)
        {
            var requirementFilters = new int[]
            {
                (int)SelfAssessmentCompetencyFilter.MeetingRequirements,
                (int)SelfAssessmentCompetencyFilter.PartiallyMeetingRequirements,
                (int)SelfAssessmentCompetencyFilter.NotMeetingRequirements

            };
            return requirementFilters.Contains(filter);
        }

        public static bool IsResponseStatusFilter(int filter)
        {
            var responseStatusFilters = new int[]
            {
                (int)SelfAssessmentCompetencyFilter.RequiresSelfAssessment,
                (int)SelfAssessmentCompetencyFilter.SelfAssessed,
                (int)SelfAssessmentCompetencyFilter.Verified,
                (int)SelfAssessmentCompetencyFilter.ConfirmationRequested
            };
            return responseStatusFilters.Contains(filter);
        }

        public static bool IsCompetencyGroupFilter(string filter)
        {
            int filterAsInteger;
            return int.TryParse(filter, out filterAsInteger) && IsCompetencyGroupFilter(filterAsInteger);
        }

        public static bool IsCompetencyGroupFilter(int filter)
        {
            return filter > 0;
        }
    }
}
