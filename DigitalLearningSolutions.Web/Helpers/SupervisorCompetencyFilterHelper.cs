using DigitalLearningSolutions.Data.Enums;
using DigitalLearningSolutions.Data.Models.SelfAssessments;
using DigitalLearningSolutions.Web.ViewModels.Supervisor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.Helpers
{
    public class SupervisorCompetencyFilterHelper
    {
        public static IEnumerable<Competency> FilterCompetencies(IEnumerable<Competency> competencies, IEnumerable<Data.Models.Frameworks.CompetencyFlag> competencyFlags, SearchSupervisorCompetencyViewModel search)
        {
            var filteredCompetencies = competencies;
            if (search != null)
            {
                var searchText = search.SearchText?.Trim() ?? string.Empty;
                var filters = search.AppliedFilters?.Select(f => int.Parse(f.FilterValue)) ?? Enumerable.Empty<int>();
                search.CompetencyFlags = competencyFlags.ToList();
                ApplyResponseStatusFilters(ref filteredCompetencies, filters, searchText);
                UpdateRequirementsFilterDropdownOptionsVisibility(search, filteredCompetencies);
                ApplyRequirementsFilters(ref filteredCompetencies, filters);

                foreach (var competency in filteredCompetencies)
                    competency.CompetencyFlags = search.CompetencyFlags.Where(f => f.CompetencyId == competency.Id);

                ApplyCompetencyGroupFilters(ref filteredCompetencies, search);
            }
            return filteredCompetencies;
        }

        private static void ApplyResponseStatusFilters(ref IEnumerable<Competency> competencies, IEnumerable<int> filters, string searchText = "")
        {
            var filteredCompetencies = competencies;
            var appliedResponseStatusFilters = filters.Where(f => IsResponseStatusFilter(f));
            if (appliedResponseStatusFilters.Any() || searchText.Length > 0)
            {
                var wordsInSearchText = searchText.Split().Where(w => w != string.Empty);
                filters = appliedResponseStatusFilters;
                filteredCompetencies = from c in competencies
                                       let searchTextMatchesGroup = wordsInSearchText.All(w => c.CompetencyGroup?.Contains(w, StringComparison.CurrentCultureIgnoreCase) ?? false)
                                       let searchTextMatchesCompetencyDescription = wordsInSearchText.All(w => c.Description?.Contains(w, StringComparison.CurrentCultureIgnoreCase) ?? false)
                                       let searchTextMatchesCompetencyName = wordsInSearchText.All(w => c.Name?.Contains(w, StringComparison.CurrentCultureIgnoreCase) ?? false)
                                       let responseStatusFilterMatchesAnyQuestion =
                                          (filters.Contains((int)SelfAssessmentCompetencyFilter.RequiresSelfAssessment) && c.AssessmentQuestions.Any(q => q.ResultId == null))
                                       || (filters.Contains((int)SelfAssessmentCompetencyFilter.SelfAssessed) && c.AssessmentQuestions.Any(q => q.ResultId != null && q.Requested == null && q.SignedOff == null))
                                       || (filters.Contains((int)SelfAssessmentCompetencyFilter.PendingConfirmation) && c.AssessmentQuestions.Any(q => q.ResultId != null &&  q.Verified == null && q.Requested != null&&q.UserIsVerifier==false))
                                       || (filters.Contains((int)SelfAssessmentCompetencyFilter.ConfirmationRejected) && c.AssessmentQuestions.Any(q => q.Verified.HasValue && q.SignedOff != true))
                                       || (filters.Contains((int)SelfAssessmentCompetencyFilter.AwaitingConfirmation) && c.AssessmentQuestions.Any(q => q.Verified == null && q.Requested != null && q.UserIsVerifier == true))
                                       || (filters.Contains((int)SelfAssessmentCompetencyFilter.Verified) && c.AssessmentQuestions.Any(q => q.Verified.HasValue && q.SignedOff == true))
                                       where (wordsInSearchText.Count() == 0 || searchTextMatchesGroup || searchTextMatchesCompetencyDescription || searchTextMatchesCompetencyName)
                                           && (!appliedResponseStatusFilters.Any() || responseStatusFilterMatchesAnyQuestion)
                                       select c;
            }
            competencies = filteredCompetencies;
        }

        private static void ApplyRequirementsFilters(ref IEnumerable<Competency> competencies, IEnumerable<int> filters)
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
            competencies = filteredCompetencies;
        }

        private static void ApplyCompetencyGroupFilters(ref IEnumerable<Competency> competencies, SearchSupervisorCompetencyViewModel search)
        {
            var filteredCompetencies = competencies;
            var appliedCompetencyGroupFilters = search.AppliedFilters?.Select(f => int.Parse(f.FilterValue)).Where(f => IsCompetencyFlagFilter(f)) ?? Enumerable.Empty<int>();
            if (appliedCompetencyGroupFilters.Any())
            {
                filteredCompetencies = competencies.Where(c => c.CompetencyFlags.Any(f => appliedCompetencyGroupFilters.Contains(f.FlagId)));
            }
            competencies = filteredCompetencies;
        }

        private static void UpdateRequirementsFilterDropdownOptionsVisibility(SearchSupervisorCompetencyViewModel search, IEnumerable<Competency> competencies)
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
                (int)SelfAssessmentCompetencyFilter.PendingConfirmation,
                (int)SelfAssessmentCompetencyFilter.AwaitingConfirmation,
                (int)SelfAssessmentCompetencyFilter.ConfirmationRejected
            };
            return responseStatusFilters.Contains(filter);
        }

        public static bool IsCompetencyFlagFilter(int filter)
        {
            return filter > 0;
        }
    }
}
