using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class CompetencyResourceSummaryViewModel
    {
        public int ReferenceId { get; set; }

        public string ResourceName => Resource?.Title ?? String.Empty;
        public string ResourceType => Resource?.ResourceType ?? String.Empty;
        public string Description => Resource?.Description ?? String.Empty;
        public string Catalogue { get; set; }
        public string NameOfCompetency { get; set; }
        public int FrameworkId { get; set; }
        public int? FrameworkCompetencyId { get; set; }
        public int? FrameworkCompetencyGroupId { get; set; }

        public string SearchText { get; set; }

        public ResourceMetadata Resource { get; set; }

        public CompetencyResourceSummaryViewModel()
        {

        }
        public CompetencyResourceSummaryViewModel(ResourceMetadata resource)
        {
            Resource = resource;
        }

        public CompetencyResourceSummaryViewModel(int frameworkId, int frameworkCompetencyId, int frameworkCompetencyGroupId)
        {
            FrameworkId = frameworkId;
            FrameworkCompetencyId = frameworkCompetencyId;
            FrameworkCompetencyGroupId = frameworkCompetencyGroupId;
        }
    }
}
