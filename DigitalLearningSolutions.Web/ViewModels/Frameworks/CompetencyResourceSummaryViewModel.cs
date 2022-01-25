using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class CompetencyResourceSummaryViewModel : BaseSignpostingViewModel
    {
        private string _Link;
        private string _Catalog;
        private int _ReferenceId;
        public int ReferenceId
        {
            get
            {
                return Resource?.References?.FirstOrDefault()?.RefId ?? _ReferenceId;
            }
            set
            {
                _ReferenceId = value;
            }
        }

        public string ResourceName => Resource?.Title ?? String.Empty;
        public string ResourceType => Resource?.ResourceType ?? String.Empty;
        public string Description => Resource?.Description ?? String.Empty;
        public string Link
        {
            get
            {
                return Resource?.References?.FirstOrDefault()?.Link ?? _Link;
            }
            set
            {
                _Link = value;
            }
        }
        public string Catalogue
        {
            get
            {
                return Resource?.References?.FirstOrDefault()?.Catalogue?.Name ?? _Catalog;
            }
            set
            {
                _Catalog = value;
            }
        }
        public string SelectedCatalogue { get; set; }
        public decimal? Rating { get; set; }
        public string NameOfCompetency { get; set; }
        public string SearchText { get; set; }
        public ResourceMetadata Resource { get; set; }
        public CompetencyResourceSummaryViewModel()
        {

        }
        public CompetencyResourceSummaryViewModel(ResourceMetadata resource)
        {
            Resource = resource;
        }

        public CompetencyResourceSummaryViewModel(int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId) : base(frameworkId, frameworkCompetencyId, frameworkCompetencyGroupId)
        {
        }
    }
}
