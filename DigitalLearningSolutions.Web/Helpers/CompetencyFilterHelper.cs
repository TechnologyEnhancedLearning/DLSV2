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
        public static IEnumerable<Competency> FilterCompetencies(IEnumerable<Competency> competencies, IEnumerable<Data.Models.Frameworks.CompetencyFlag> competencyFlags, SearchSelfAssessmentOverviewViewModel search)
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

        public static void ApplyResponseStatusFilters(ref IEnumerable<Competency> competencies, IEnumerable<int> filters, string searchText = "")
        {
            var appliedResponseStatusFilters = filters.Where(IsResponseStatusFilter).ToList();

            if (!appliedResponseStatusFilters.Any() && string.IsNullOrWhiteSpace(searchText))
            {
                return;
            }

            // Break search text into words
            var wordsInSearchText = searchText?
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                ?? Array.Empty<string>();

            bool MatchesSearch(Competency c) =>
                wordsInSearchText.Length == 0
                || wordsInSearchText.All(w =>
                       (c.CompetencyGroup?.Contains(w, StringComparison.CurrentCultureIgnoreCase) ?? false)
                    || (c.Description?.Contains(w, StringComparison.CurrentCultureIgnoreCase) ?? false)
                    || (c.Name?.Contains(w, StringComparison.CurrentCultureIgnoreCase) ?? false));

            // Define reusable filter checks
            var filterChecks = new Dictionary<SelfAssessmentCompetencyFilter, Func<Competency, bool>>
            {
                [SelfAssessmentCompetencyFilter.RequiresSelfAssessment] = c => c.AssessmentQuestions.Any(q => q.ResultId == null),
                [SelfAssessmentCompetencyFilter.SelfAssessed] = c => c.AssessmentQuestions.Any(q => q.ResultId != null && q.Requested == null && q.SignedOff == null),
                [SelfAssessmentCompetencyFilter.ConfirmationRequested] = c => c.AssessmentQuestions.Any(q => q.Verified == null && q.Requested != null),
                [SelfAssessmentCompetencyFilter.ConfirmationRejected] = c => c.AssessmentQuestions.Any(q => q.Verified.HasValue && q.SignedOff != true),
                [SelfAssessmentCompetencyFilter.Verified] = c => c.AssessmentQuestions.Any(q => q.Verified.HasValue && q.SignedOff == true),
                [SelfAssessmentCompetencyFilter.AwaitingConfirmation] = c => c.AssessmentQuestions.Any(q => q.Verified == null && q.Requested != null && q.UserIsVerifier == true),
                [SelfAssessmentCompetencyFilter.PendingConfirmation] = c => c.AssessmentQuestions.Any(q => q.ResultId != null && q.Verified == null && q.Requested != null && q.UserIsVerifier == false),
                [SelfAssessmentCompetencyFilter.Optional] = c => c.Optional
            };

            // Require ALL applied filters to match
            bool MatchesFilters(Competency c) =>
                !appliedResponseStatusFilters.Any()
                || appliedResponseStatusFilters.All(f => filterChecks[(SelfAssessmentCompetencyFilter)f](c));

            // Final filtering
            competencies = competencies.Where(c => MatchesSearch(c) && MatchesFilters(c));
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

        private static void ApplyCompetencyGroupFilters(ref IEnumerable<Competency> competencies, SearchSelfAssessmentOverviewViewModel search)
        {
            var filteredCompetencies = competencies;
            var appliedCompetencyGroupFilters = search.AppliedFilters?.Select(f => int.Parse(f.FilterValue)).Where(f => IsCompetencyFlagFilter(f)) ?? Enumerable.Empty<int>();
            if (appliedCompetencyGroupFilters.Any())
            {
                filteredCompetencies = competencies.Where(c => c.CompetencyFlags.Any(f => appliedCompetencyGroupFilters.Contains(f.FlagId)));
            }
            competencies = filteredCompetencies;
        }

        private static void UpdateRequirementsFilterDropdownOptionsVisibility(SearchSelfAssessmentOverviewViewModel search, IEnumerable<Competency> competencies)
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
                (int)SelfAssessmentCompetencyFilter.Optional,
                (int)SelfAssessmentCompetencyFilter.RequiresSelfAssessment,
                (int)SelfAssessmentCompetencyFilter.SelfAssessed,
                (int)SelfAssessmentCompetencyFilter.Verified,
                (int)SelfAssessmentCompetencyFilter.ConfirmationRequested,
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
