using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Models.LearningResources;
using DigitalLearningSolutions.Data.Models.SelfAssessments;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.Services
{
    public interface ICompetencyLearningResourcesService
    {
        IEnumerable<int> GetCompetencyIdsLinkedToResource(int learningResourceReferenceId);

        IEnumerable<CompetencyLearningResource> GetActiveCompetencyLearningResourcesByCompetencyId(int competencyId);

        IEnumerable<CompetencyResourceAssessmentQuestionParameter> GetCompetencyResourceAssessmentQuestionParameters(IEnumerable<int> competencyLearningResourceIds);
        int AddCompetencyLearningResource(int resourceRefID, string originalResourceName, string description, string resourceType, string link, string catalogue, decimal rating, int competencyID, int adminId);
        IEnumerable<CompetencyLearningResource> GetActiveCompetencyLearningResourcesByCompetencyIdAndReferenceId(int competencyId, int referenceId);
    }
    public class CompetencyLearningResourcesService : ICompetencyLearningResourcesService
    {
        private readonly ICompetencyLearningResourcesDataService competencyLearningResourcesDataService;
        public CompetencyLearningResourcesService(ICompetencyLearningResourcesDataService competencyLearningResourcesDataService)
        {
            this.competencyLearningResourcesDataService = competencyLearningResourcesDataService;
        }
        public int AddCompetencyLearningResource(int resourceRefID, string originalResourceName, string description, string resourceType, string link, string catalogue, decimal rating, int competencyID, int adminId)
        {
            return competencyLearningResourcesDataService.AddCompetencyLearningResource(resourceRefID, originalResourceName, description, resourceType, link, catalogue, rating, competencyID, adminId);
        }

        public IEnumerable<CompetencyLearningResource> GetActiveCompetencyLearningResourcesByCompetencyId(int competencyId)
        {
            return competencyLearningResourcesDataService.GetActiveCompetencyLearningResourcesByCompetencyId(competencyId);
        }

        public IEnumerable<int> GetCompetencyIdsLinkedToResource(int learningResourceReferenceId)
        {
            return competencyLearningResourcesDataService.GetCompetencyIdsLinkedToResource(learningResourceReferenceId);
        }

        public IEnumerable<CompetencyResourceAssessmentQuestionParameter> GetCompetencyResourceAssessmentQuestionParameters(IEnumerable<int> competencyLearningResourceIds)
        {
            return competencyLearningResourcesDataService.GetCompetencyResourceAssessmentQuestionParameters(competencyLearningResourceIds);
        }
        public IEnumerable<CompetencyLearningResource> GetActiveCompetencyLearningResourcesByCompetencyIdAndReferenceId(int competencyId, int referenceId)
        {
            return competencyLearningResourcesDataService.GetActiveCompetencyLearningResourcesByCompetencyIdAndReferenceId(competencyId, referenceId);
        }
    }
}
