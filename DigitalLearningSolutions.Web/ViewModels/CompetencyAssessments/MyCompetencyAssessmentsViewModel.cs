namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class MyCompetencyAssessmentsViewModel : BaseCompetencyAssessmentsPageViewModel
    {
        public readonly IEnumerable<CompetencyAssessment> CompetencyAssessments;
        public readonly bool IsWorkforceManager;

        public override SelectList CompetencyAssessmentSortByOptions { get; } = new SelectList(new[]
        {
            CompetencyAssessmentSortByOptionTexts.CompetencyAssessmentName,
            CompetencyAssessmentSortByOptionTexts.CompetencyAssessmentOwner,
            CompetencyAssessmentSortByOptionTexts.CompetencyAssessmentCreatedDate,
            CompetencyAssessmentSortByOptionTexts.CompetencyAssessmentPublishStatus
        });

        public MyCompetencyAssessmentsViewModel(
            IEnumerable<CompetencyAssessment> competencyAssessments, // Renamed parameter to avoid shadowing
            string? searchString,
            string sortBy,
            string sortDirection,
            int page,
            bool isWorkforceManager
        ) : base(searchString, sortBy, sortDirection, page)
        {
            var sortedItems = SortingHelper.SortCompetencyAssessmentItems(
                competencyAssessments,
                sortBy,
                sortDirection
            );
            var filteredItems = SearchHelper.FilterCompetencyAssessments(sortedItems, SearchString, 60, false).ToList();
            MatchingSearchResults = filteredItems.Count;
            SetTotalPages();
            CompetencyAssessments = PaginateItems(filteredItems).Cast<CompetencyAssessment>(); // Assign to the field
            IsWorkforceManager = isWorkforceManager;
        }
    }
}
