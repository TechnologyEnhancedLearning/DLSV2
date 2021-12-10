using DigitalLearningSolutions.Data.Models.LearningHubApiClient;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class CompetencyResourceSignpostingViewModel
    {
        public const int ItemsPerPage = 10;
        public string NameOfCompetency { get; set; }
        public int FrameworkId { get; set; }
        public int? FrameworkCompetencyId { get; set; }
        public int? FrameworkCompetencyGroupId { get; set; }
        public List<SignpostingCardViewModel> CompetencyResourceLinks { get; set; }
        public IEnumerable<SearchableCompetencyViewModel> Delegates { get; set; }
        public string SearchText { get; set; }
        public int Page { get; set; }
        public bool LearningHubApiError { get; set; }
        public int TotalPages
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

        public CompetencyResourceSignpostingViewModel(int frameworkId, int? frameworkCompetencyId, int? frameworkCompetencyGroupId)
        {
            FrameworkId = frameworkId;
            FrameworkCompetencyId = frameworkCompetencyId;
            FrameworkCompetencyGroupId = frameworkCompetencyGroupId;
            Page = 1;
        }

        public CompetencyResourceSignpostingViewModel()
        {

        }
        
        public void Empty()
        {
            SearchResult = new ResourceSearchResult()
            {
                Results = new List<ResourceMetadata>() { }
            };
            this.Page = 1;
        }
    }
}
