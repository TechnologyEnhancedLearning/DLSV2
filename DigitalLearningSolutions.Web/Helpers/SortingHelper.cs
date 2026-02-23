namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
    using DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments;

    /// <summary>
    /// This is the older version of the SortingHelper. For future search/sort implementations we should
    /// be using the generic version <see cref="GenericSortingHelper"/> and implementing BaseSearchableItem
    /// on the entity to be sorted
    /// </summary>
    public static class SortingHelper
    {
        public static IEnumerable<CompetencyAssessment> SortCompetencyAssessmentItems(
            IEnumerable<CompetencyAssessment> competencyAssessments,
            string sortBy,
            string sortDirection
        )
        {
            return sortBy switch
            {
                CompetencyAssessmentSortByOptionTexts.CompetencyAssessmentName => sortDirection == BaseCompetencyAssessmentsPageViewModel.DescendingText
                    ? competencyAssessments.OrderByDescending(competencyAssessment => competencyAssessment.CompetencyAssessmentName)
                    : competencyAssessments.OrderBy(competencyAssessment => competencyAssessment.CompetencyAssessmentName),
                CompetencyAssessmentSortByOptionTexts.CompetencyAssessmentOwner => sortDirection == BaseCompetencyAssessmentsPageViewModel.DescendingText
                    ? competencyAssessments.OrderByDescending(competencyAssessment => competencyAssessment.Owner)
                    : competencyAssessments.OrderBy(competencyAssessment => competencyAssessment.Owner),
                CompetencyAssessmentSortByOptionTexts.CompetencyAssessmentCreatedDate => sortDirection == BaseCompetencyAssessmentsPageViewModel.DescendingText
                    ? competencyAssessments.OrderByDescending(competencyAssessment => competencyAssessment.CreatedDate)
                    : competencyAssessments.OrderBy(competencyAssessment => competencyAssessment.CreatedDate),
                CompetencyAssessmentSortByOptionTexts.CompetencyAssessmentPublishStatus => sortDirection == BaseCompetencyAssessmentsPageViewModel.DescendingText
                    ? competencyAssessments.OrderByDescending(competencyAssessment => competencyAssessment.PublishStatusID)
                    : competencyAssessments.OrderBy(competencyAssessment => competencyAssessment.PublishStatusID),
                CompetencyAssessmentSortByOptionTexts.CompetencyAssessmentBrand => sortDirection == BaseCompetencyAssessmentsPageViewModel.DescendingText
                    ? competencyAssessments.OrderByDescending(competencyAssessment => competencyAssessment.Brand)
                    : competencyAssessments.OrderBy(competencyAssessment => competencyAssessment.Brand),
                CompetencyAssessmentSortByOptionTexts.CompetencyAssessmentNationalCompetencyAssessment => sortDirection == BaseCompetencyAssessmentsPageViewModel.DescendingText
                ? competencyAssessments.OrderByDescending(competencyAssessment => competencyAssessment.NRPRole)
                : competencyAssessments.OrderBy(competencyAssessment => competencyAssessment.NRPRole),
                CompetencyAssessmentSortByOptionTexts.CompetencyAssessmentNationalRoleGroup => sortDirection == BaseCompetencyAssessmentsPageViewModel.DescendingText
                ? competencyAssessments.OrderByDescending(competencyAssessment => competencyAssessment.NRPProfessionalGroup)
                : competencyAssessments.OrderBy(competencyAssessment => competencyAssessment.NRPProfessionalGroup),
                _ => competencyAssessments
            };
        }
    }
}
