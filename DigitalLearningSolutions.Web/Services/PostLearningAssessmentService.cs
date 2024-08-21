using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Models.PostLearningAssessment;

namespace DigitalLearningSolutions.Web.Services
{
    public interface IPostLearningAssessmentService
    {
        PostLearningAssessment? GetPostLearningAssessment(int customisationId, int candidateId, int sectionId);
        PostLearningContent? GetPostLearningContent(int customisationId, int sectionId);
    }
    public class PostLearningAssessmentService : IPostLearningAssessmentService
    {
        private readonly IPostLearningAssessmentDataService postLearningAssessmentDataService;
        public PostLearningAssessmentService(IPostLearningAssessmentDataService postLearningAssessmentDataService)
        {
            this.postLearningAssessmentDataService = postLearningAssessmentDataService;
        }
        public PostLearningAssessment? GetPostLearningAssessment(int customisationId, int candidateId, int sectionId)
        {
            return postLearningAssessmentDataService.GetPostLearningAssessment(customisationId, candidateId, sectionId);
        }

        public PostLearningContent? GetPostLearningContent(int customisationId, int sectionId)
        {
            return postLearningAssessmentDataService.GetPostLearningContent(customisationId, sectionId);
        }
    }
}
