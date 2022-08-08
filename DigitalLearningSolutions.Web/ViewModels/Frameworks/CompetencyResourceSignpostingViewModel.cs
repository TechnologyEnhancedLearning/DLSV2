using System;
using System.Collections.Generic;
using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class CompetencyResourceSignpostingViewModel : BaseSignpostingViewModel
    {
        public const int ItemsPerPage = 10;
        public string NameOfCompetency { get; set; }
        public string Title { get; set; }
        public List<SignpostingCardViewModel> CompetencyResourceLinks { get; set; }
        public IEnumerable<SearchableCompetencyViewModel> Delegates { get; set; }
        public List<Catalogue> Catalogues { get; set; }
        public int? CatalogueId { get; set; }
        public string SearchText { get; set; }
        public override int Page { get; set; }
        public bool LearningHubApiError { get; set; }
        public override int TotalPages
        {
            get
            {
                return (int)Math.Ceiling((SearchResult?.TotalNumResources ?? 0) / (double)ItemsPerPage);
            }
        }
        public int TotalNumResources
        {
            get
            {
                return SearchResult?.TotalNumResources ?? 0;
            }
        }
        public ResourceSearchResult SearchResult { get; set; }

        public static explicit operator CompetencyResourceSignpostingViewModel(CompetencyResourceSummaryViewModel model)
        {
            return new CompetencyResourceSignpostingViewModel(model.FrameworkId, model.FrameworkCompetencyId, model.FrameworkCompetencyGroupId);
        }

        public CompetencyResourceSignpostingViewModel(int frameworkId, int? frameworkCompetencyId, int? frameworkCompetencyGroupId) : base(frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId, 1, ItemsPerPage)
        {
            FrameworkId = frameworkId;
            FrameworkCompetencyId = frameworkCompetencyId;
            FrameworkCompetencyGroupId = frameworkCompetencyGroupId;
            Page = 1;
        }

        public CompetencyResourceSignpostingViewModel()
        {

        }
    }
}
