using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class CompetencyResourceSummaryViewModel : BaseSignpostingViewModel
    {
        public int ReferenceId { get; set; }

        public string ResourceName => Resource?.Title ?? String.Empty;
        public string ResourceType => Resource?.ResourceType ?? String.Empty;
        public string Description => Resource?.Description ?? String.Empty;
        public string Catalogue { get; set; }
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
